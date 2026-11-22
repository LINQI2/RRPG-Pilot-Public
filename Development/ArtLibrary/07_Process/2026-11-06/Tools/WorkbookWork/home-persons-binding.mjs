import fs from 'node:fs/promises';
import {FileBlob,SpreadsheetFile} from '@oai/artifact-tool';
const repo='D:/AI_Agent_Lab/rrpg_pilot/repo';
const base=repo+'/Development/ArtLibrary';
const output=base+'/07_Process/2026-11-06/Evidence/outputs/home-persons';
const people=JSON.parse(await fs.readFile(new URL('./home-persons-input.json',import.meta.url),'utf8'));
const apply=process.argv.includes('--apply');
await fs.mkdir(output,{recursive:true});
for(const [file,sheet,start,cols] of [['RRPG_VisualAssets.xlsx','VisualAsset',27,4],['RRPG_VillagePresentation.xlsx','VillagePresentation',28,3]]){
 const w=await SpreadsheetFile.importXlsx(await FileBlob.load(repo+'/Development/Config/'+file));
 const s=w.worksheets.getItem(sheet);
 if(!apply){
  console.log(JSON.stringify({file,rows:s.getRange('A20:D42').values}));
  if(process.argv.includes('--render'))try{const img=await w.render({sheetName:sheet,range:sheet==='VisualAsset'?'A23:D26':'A24:C27',scale:1,format:'png'});await fs.writeFile(output+'/'+sheet+'-before.png',new Uint8Array(await img.arrayBuffer()));console.log('Render saved');}catch(e){console.log('Render unavailable: '+e.message);}
  continue;
 }
 const end=start+11;
 if(s.getRange('B'+start+':B'+end).values.some(r=>r[0]))throw Error('Expected unused target rows '+file);
 const rows=people.flatMap(p=>['se','ne'].map(dir=>sheet==='VisualAsset'
  ? ['B PERSON FIRST PASS','va_village_person_'+p.key+'_'+dir,'Village/ArtUpgrade01/Persons/'+(dir==='se'?p.front:p.back)+'.png','sprite_point_100']
  : ['B PERSON FIRST PASS','village.person.art01.'+p.key+'.'+dir,'va_village_person_'+p.key+'_'+dir]));
 for(let i=0;i<12;i++)s.getRange('A'+(start+i)+':'+String.fromCharCode(64+cols)+(start+i)).copyFrom(s.getRange('A'+(start-1)+':'+String.fromCharCode(64+cols)+(start-1)),'all');
 s.getRange('A'+start+':'+String.fromCharCode(64+cols)+end).values=rows;
 w.recalculate();
 console.log((await w.inspect({kind:'table',range:sheet+'!A'+start+':'+String.fromCharCode(64+cols)+end,include:'values,formulas',tableMaxRows:12,tableMaxCols:4})).ndjson);
 await (await SpreadsheetFile.exportXlsx(w)).save(output+'/'+file);
 const reread=await SpreadsheetFile.importXlsx(await FileBlob.load(output+'/'+file));
 if(JSON.stringify(reread.worksheets.getItem(sheet).getRange('A'+start+':'+String.fromCharCode(64+cols)+end).values)!==JSON.stringify(rows))throw Error('Export values mismatch');
}

