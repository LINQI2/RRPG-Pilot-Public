"""Lossless package assembly for Artifact Tool's missing native-format preservation.

Cell values are authored by Artifact Tool. Transfer only the explicitly changed
cells into the original package, retaining its styles, views and every other part.
No openpyxl/pandas authoring; no Unity serialized assets are touched.
"""
import copy
import hashlib
import html
import io
import json
import re
import zipfile
from pathlib import Path
import xml.etree.ElementTree as ET

REPO = Path(__file__).resolve().parents[5]
LIB = REPO / 'Development/ArtLibrary'
NS = 'http://schemas.openxmlformats.org/spreadsheetml/2006/main'
Q = lambda n: '{' + NS + '}' + n

def strings(z):
    if 'xl/sharedStrings.xml' not in z.namelist():
        return []
    return [''.join(e.itertext()) for e in ET.fromstring(z.read('xl/sharedStrings.xml')).findall(Q('si'))]

def values(z, name):
    ss = strings(z)
    result = {}
    for c in ET.fromstring(z.read(name)).iter(Q('c')):
        formula = c.find(Q('f'))
        if formula is not None:
            result[c.attrib['r']] = ('formula', formula.text)
            continue
        v = c.find(Q('v'))
        if c.get('t') == 'inlineStr':
            val = ''.join(t.text or '' for t in c.iter(Q('t')))
        elif c.get('t') == 's' and v is not None:
            val = ss[int(v.text)]
        elif v is not None:
            val = v.text
        else:
            continue
        if val != '':
            result[c.attrib['r']] = ('value', val)
    return result

def col_number(address):
    n = 0
    for c in re.match(r'[A-Z]+', address)[0]:
        n = n * 26 + ord(c) - 64
    return n

reports = []
for filename, sheet, targets in [
    ('RRPG_VisualAssets.xlsx', 'xl/worksheets/sheet2.xml', [f'{c}{r}' for r in (23, 24, 25, 26) for c in 'ABCD']),
    ('RRPG_VillagePresentation.xlsx', 'xl/worksheets/sheet1.xml', ['C26', 'A27', 'B27', 'C27']),
]:
    original = LIB / '08_ExistingProject/BeforeHomeUpgrade-2026-11-06' / filename
    current = REPO / 'Development/Config' / filename
    with zipfile.ZipFile(original) as old, zipfile.ZipFile(current) as authored:
        raw = old.read(sheet)
        namespaces = list(ET.iterparse(io.BytesIO(raw), events=['start-ns']))
        for _, (prefix, uri) in namespaces:
            if not re.fullmatch(r'ns\d+', prefix):
                ET.register_namespace(prefix, uri)
        root = ET.fromstring(raw)
        original_root = copy.deepcopy(root)
        data = root.find(Q('sheetData'))
        original_rows = {r.attrib['r'] for r in data}
        authored_values = values(authored, sheet)
        for address in targets:
            kind, text = authored_values[address]
            assert kind == 'value', 'Target cells must be string values'
            row_id = re.search(r'\d+', address)[0]
            row = next((r for r in data if r.get('r') == row_id), None)
            if row is None:
                template = next(r for r in data if r.get('r') == '18')
                row = ET.Element(Q('row'), dict(template.attrib, r=row_id))
                data.append(row)
            cell = next((c for c in row if c.get('r') == address), None)
            if cell is None:
                template_address = re.match('[A-Z]+', address)[0] + '18'
                template_cell = next((c for r in data for c in r if c.get('r') == template_address), None)
                attrs = dict(template_cell.attrib) if template_cell is not None else {}
                cell = ET.SubElement(row, Q('c'), dict(attrs, r=address))
            for child in list(cell):
                cell.remove(child)
            cell.set('t', 'inlineStr')
            ET.SubElement(ET.SubElement(cell, Q('is')), Q('t')).text = text
            row[:] = sorted(row, key=lambda c: col_number(c.get('r')))
        data[:] = sorted(data, key=lambda r: int(r.get('r')))
        dim = root.find(Q('dimension'))
        bounds = dim.get('ref').split(':')
        end = bounds[-1]
        last_row = max(int(re.search(r'\d+', address)[0]) for address in targets)
        if int(re.search(r'\d+', end)[0]) < last_row:
            dim.set('ref', bounds[0] + ':' + re.match('[A-Z]+', end)[0] + str(last_row))
        def protected_tree(tree):
            tree = copy.deepcopy(tree)
            tree.find(Q('dimension')).attrib = dict(original_root.find(Q('dimension')).attrib)
            sd = tree.find(Q('sheetData'))
            for row in list(sd):
                if row.get('r') not in original_rows:
                    sd.remove(row)
                    continue
                for cell in list(row):
                    if cell.get('r') in targets:
                        row.remove(cell)
            return ET.tostring(tree)
        assert protected_tree(root) == protected_tree(original_root), 'Unrelated worksheet content changed'
        encoded = ET.tostring(root, encoding='utf-8', xml_declaration=True)
        start_tag = re.search(rb'<(?:[A-Za-z0-9_]+:)?worksheet\b[^>]*>', encoded)
        additions = b''
        # ElementTree omits unused namespace declarations. mc:Ignorable still
        # references their prefixes, so keep the original declarations in scope.
        for _, (prefix, uri) in namespaces:
            declaration = ('xmlns:' + prefix + '=') if prefix else 'xmlns='
            if declaration.encode() not in start_tag[0]:
                additions += (' ' + declaration + '"' + html.escape(uri, quote=True) + '"').encode()
        encoded = encoded[:start_tag.end()-1] + additions + encoded[start_tag.end()-1:]
        out = io.BytesIO()
        with zipfile.ZipFile(out, 'w') as result:
            for info in old.infolist():
                result.writestr(copy.copy(info), encoded if info.filename == sheet else old.read(info.filename))
        out.seek(0)
        with zipfile.ZipFile(out) as result:
            for name in old.namelist():
                if name != sheet:
                    assert old.read(name) == result.read(name), 'Protected package part changed: ' + name
            for name in old.namelist():
                if re.fullmatch(r'xl/worksheets/sheet\d+.xml', name):
                    assert values(result, name) == values(authored, name), 'Authored cell values/formulas disagree: ' + name
        payload = out.getvalue()
    current.write_bytes(payload)
    reports.append({'file': filename, 'targetCells': targets, 'allOtherPartsByteIdenticalToOriginal': True,
                    'protectedWorksheetContentUnchanged': True, 'allValuesAndFormulasMatchArtifactOutput': True,
                    'sha256': hashlib.sha256(payload).hexdigest()})

report_path = LIB / '07_Process/2026-11-06/Evidence/workbook-package-preservation.json'
report_path.write_text(json.dumps(reports, ensure_ascii=False, indent=2), encoding='utf-8')
print(json.dumps(reports, ensure_ascii=False))
