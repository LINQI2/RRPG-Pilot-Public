import fs from 'node:fs/promises';
import {FileBlob,SpreadsheetFile} from '@oai/artifact-tool';
const repo='D:/AI_Agent_Lab/rrpg_pilot/repo';
const apply=process.argv.includes('--apply');
const terrain=process.argv.includes('--terrain');
const facing=process.argv.includes('--person-facing');
const output=`${repo}/Development/ArtLibrary/07_Process/2026-11-06/Evidence/outputs/${facing?'person-facing':terrain?'terrain-binding':'tavern-binding'}`;
if(apply) await fs.mkdir(output,{recursive:true});
for (const [file,sheetName,names] of [['RRPG_VisualAssets.xlsx','VisualAsset',['00_导航','VisualAsset']],['RRPG_VillagePresentation.xlsx','VillagePresentation',['VillagePresentation','BuildingPresentationBinding']]]) {
 if(terrain&&sheetName!=='VisualAsset')continue;
 const w=await SpreadsheetFile.importXlsx(await FileBlob.load(`${repo}/Development/Config/${file}`));
 const s=w.worksheets.getItem(sheetName);
 if(!apply){console.log(JSON.stringify({file,rows:s.getRange('A1:F40').values}));continue;}
 const before=Object.fromEntries(names.map(n=>[n,{values:w.worksheets.getItem(n).getRange('A1:P80').values,formulas:w.worksheets.getItem(n).getRange('A1:P80').formulas}]));
 if(facing){
  if(sheetName==='VisualAsset'){
   if(s.getRange('B26').values[0][0])throw Error('Expected unused visual row 26');
   s.getRange('A26:D26').copyFrom(s.getRange('A25:D25'),'all');
   s.getRange('A26:D26').values=[['B CANDIDATE / PERSON FACING','va_village_r5_person_ne','Village/ArtUpgrade01/person_world_neutral_ne_r1.png','sprite_point_100']];
  }else{
   if(s.getRange('B27').values[0][0])throw Error('Expected unused presentation row 27');
   s.getRange('A27:C27').copyFrom(s.getRange('A26:C26'),'all');
   s.getRange('A27:C27').values=[['B CANDIDATE / PERSON FACING','village.person.r2.ne','va_village_r5_person_ne']];
  }
 }else if(terrain){
  if(s.getRange('B24:B25').values.some(r=>r[0]))throw Error('Expected unused terrain rows 24/25');
  s.getRange('A24:D25').copyFrom(s.getRange('A23:D23'),'all');
  s.getRange('A24:D25').values=[['B CANDIDATE / TERRAIN','va_village_r5_grass','Village/ArtUpgrade01/grass_surface_r1.png','sprite_point_100'],['B CANDIDATE / TERRAIN','va_village_r5_earth','Village/ArtUpgrade01/earth_surface_r1.png','sprite_point_100']];
 }else if(sheetName==='VisualAsset'){
  if(s.getRange('B23').values[0][0]) throw Error('Expected unused row 23');
  s.getRange('A23:D23').copyFrom(s.getRange('A18:D18'),'all');
  s.getRange('A23:D23').values=[['B CANDIDATE / ART ACCEPTANCE PENDING','va_village_r5_tavern','Village/ArtUpgrade01/tavern.png','sprite_point_100']];
 }else{
  if(s.getRange('B26').values[0][0]!=='village.place.r2.fixed.Tavern'||s.getRange('C26').values[0][0]!=='va_village_r2_workshop') throw Error('Unexpected tavern binding');
  s.getRange('C26').values=[['va_village_r5_tavern']];
 }
 w.recalculate();
 const check=await w.inspect({kind:'table',range:`${sheetName}!A19:D27`,include:'values,formulas',tableMaxRows:9,tableMaxCols:4});console.log(check.ndjson);
 // Windows native renderer exited with code 1 before export in both sandboxed and
 // unrestricted execution. Keep export/semantic verification independent of it.
 if(process.argv.includes('--render')){
  const preview=await w.render({sheetName,range:sheetName==='VisualAsset'?'A21:D23':'A24:C26',scale:1,format:'png'});
  await fs.writeFile(`${output}/${sheetName}.png`,new Uint8Array(await preview.arrayBuffer()));
 }
 const xlsx=await SpreadsheetFile.exportXlsx(w);await xlsx.save(`${output}/${file}`);
 const reread=await SpreadsheetFile.importXlsx(await FileBlob.load(`${output}/${file}`));
 let checked=0;
 for(const n of names){const after=reread.worksheets.getItem(n).getRange('A1:P80');for(const kind of ['values','formulas'])for(let r=0;r<80;r++)for(let c=0;c<16;c++){
  const affected=facing ? n==='VisualAsset'&&r===25&&c<4||n==='VillagePresentation'&&r===26&&c<3 : terrain ? n==='VisualAsset'&&(r===23||r===24)&&c<4 : n==='VisualAsset'&&r===22&&c<4||n==='VillagePresentation'&&r===25&&c===2;
  if(affected)continue;
  if(JSON.stringify(before[n][kind][r]?.[c]??null)!==JSON.stringify(after[kind][r]?.[c]??null))throw Error(`Unrelated ${kind} changed: ${n} r${r+1} c${c+1}`);checked++;
 }}
 console.log(JSON.stringify({file,unrelatedCellsChecked:checked,saved:`${output}/${file}`}));
}
