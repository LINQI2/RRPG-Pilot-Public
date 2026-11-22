"""Transfer Artifact Tool-authored appended cells into original XLSX packages, preserving all existing parts."""
import copy, html, io, json, re, zipfile
import xml.etree.ElementTree as E
from pathlib import Path
repo=Path(__file__).resolve().parents[5]
lib=repo/'Development/ArtLibrary'
backup=lib/'08_ExistingProject/BeforeHomePersons-20261108'
authored=lib/'07_Process/2026-11-06/Evidence/outputs/home-persons'
ns='http://schemas.openxmlformats.org/spreadsheetml/2006/main'
q=lambda n:'{'+ns+'}'+n
def vals(z,sheet):
 ss=[] if 'xl/sharedStrings.xml' not in z.namelist() else [''.join(e.itertext()) for e in E.fromstring(z.read('xl/sharedStrings.xml')).findall(q('si'))]
 out={}
 for c in E.fromstring(z.read(sheet)).iter(q('c')):
  f=c.find(q('f'));v=c.find(q('v'))
  if f is not None: out[c.get('r')]=('f',f.text);continue
  value=''.join(t.text or '' for t in c.iter(q('t'))) if c.get('t')=='inlineStr' else ss[int(v.text)] if c.get('t')=='s' and v is not None else v.text if v is not None else ''
  if value!='':out[c.get('r')]=('v',value)
 return out
reports=[]
for file,sheet,start,cols in [('RRPG_VisualAssets.xlsx','xl/worksheets/sheet2.xml',27,'ABCD'),('RRPG_VillagePresentation.xlsx','xl/worksheets/sheet1.xml',28,'ABC')]:
 with zipfile.ZipFile(backup/file) as old,zipfile.ZipFile(authored/file) as new:
  raw=old.read(sheet);namespaces=list(E.iterparse(io.BytesIO(raw),events=['start-ns']))
  for _,(prefix,uri) in namespaces:
   if not re.fullmatch(r'ns\d+',prefix):E.register_namespace(prefix,uri)
  root=E.fromstring(raw);sd=root.find(q('sheetData'));template=next(r for r in sd if r.get('r')==str(start-1))
  before=vals(old,sheet);after=vals(new,sheet);targets={c+str(r) for r in range(start,start+12) for c in cols}
  assert not targets.intersection(before),'New cells already populated'
  assert {k:v for k,v in after.items() if k not in targets}==before,'Artifact output changed unrelated cells'
  for r in range(start,start+12):
   assert not any(x.get('r')==str(r) for x in sd),'Target row already exists'
   row=E.SubElement(sd,q('row'),dict(template.attrib,r=str(r)))
   for col in cols:
    cell_template=next((x for x in template if x.get('r')==col+str(start-1)),None)
    attrs=dict(cell_template.attrib) if cell_template is not None else {}
    cell=E.SubElement(row,q('c'),dict(attrs,r=col+str(r),t='inlineStr'))
    kind,value=after[col+str(r)];assert kind=='v'
    E.SubElement(E.SubElement(cell,q('is')),q('t')).text=value
  dim=root.find(q('dimension'));end=dim.get('ref').split(':')[-1]
  if int(re.search(r'\d+',end)[0])<start+11:dim.set('ref',dim.get('ref').split(':')[0]+':'+re.match('[A-Z]+',end)[0]+str(start+11))
  # Removing our appended rows must reproduce the complete original worksheet tree.
  check=copy.deepcopy(root)
  for row in list(check.find(q('sheetData'))):
   if start<=int(row.get('r'))<start+12:check.find(q('sheetData')).remove(row)
  check.find(q('dimension')).attrib=dict(E.fromstring(raw).find(q('dimension')).attrib)
  assert E.tostring(check)==E.tostring(E.fromstring(raw)),'Protected worksheet content changed'
  encoded=E.tostring(root,encoding='utf-8',xml_declaration=True)
  tag=re.search(rb'<(?:[A-Za-z0-9_]+:)?worksheet\b[^>]*>',encoded)
  additions=b''
  for _,(prefix,uri) in namespaces:
   decl='xmlns:'+prefix+'=' if prefix else 'xmlns='
   if decl.encode() not in tag[0]:additions+=(' '+decl+'"'+html.escape(uri,quote=True)+'"').encode()
  encoded=encoded[:tag.end()-1]+additions+encoded[tag.end()-1:]
  output=io.BytesIO()
  with zipfile.ZipFile(output,'w') as z:
   for info in old.infolist():z.writestr(copy.copy(info),encoded if info.filename==sheet else old.read(info.filename))
  with zipfile.ZipFile(io.BytesIO(output.getvalue())) as z:
   for name in old.namelist():
    if name!=sheet:assert z.read(name)==old.read(name),'Unrelated package part changed'
    if re.fullmatch(r'xl/worksheets/sheet\d+.xml',name):assert vals(z,name)==vals(new,name),'Authored values disagree'
  (repo/'Development/Config'/file).write_bytes(output.getvalue())
  reports.append(dict(file=file,appendedRows=[start,start+11],allOtherPackagePartsByteIdentical=True,protectedWorksheetTreeUnchanged=True,allValuesMatchArtifactOutput=True))
(lib/'07_Process/2026-11-06/Evidence/home-persons-workbook-preservation.json').write_text(json.dumps(reports,indent=2),encoding='utf-8')
print(json.dumps(reports))

