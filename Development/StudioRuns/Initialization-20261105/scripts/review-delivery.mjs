import fs from 'node:fs';
import path from 'node:path';
import crypto from 'node:crypto';
import {execFileSync} from 'node:child_process';
const root=String.raw`D:\AI_Agent_Lab\rrpg_pilot\repo`;
const run=path.join(root,'Development/StudioRuns/Initialization-20261105');
const targets=['Docs/项目状态快照.md','Docs/开发验收交接.md','Docs/项目日志.md','Docs/文档地图.md','Docs/DevelopmentSpecs/README.md'];
for(const name of targets){
 const file=path.join(root,name), original=fs.readFileSync(file,'utf8').replace(/^\uFEFF/,'');
 if(!original.startsWith('> 2026-11-05 Initial World '))throw Error('Unexpected prefix: '+name);
 const rest=original.slice(original.indexOf('\n')+1);
 const link=(name===targets[4]?'../../':'../')+'Development/StudioRuns/Initialization-20261105/technical-review.md';
 const entry='> 2026-11-05 Initial World 可审查交付：A档Kitchen/House/Person白模、配置接线及人物查表/等比显示已实现。Compiler 280/280、初始化Edit 24/24、DSH产品Play 2/2；主开发最终独立V4+视觉3/3（0 skip），修改后validate PASS。存档/场景字节一致，136 guard均restored。**字体保护异常未解决：原dirty字体增加533B，无接手时完整副本，未从Git覆盖；整包仍Partial / Product Acceptance Pending / 未Commit未Push**。Farm TECH PROOF与通用Place neutral保留为技术表现限制。[技术复核与证据]('+link+')。这是本轮附记，不生成最终Handoff，下方历史结论保留。';
 fs.writeFileSync(file,entry+'\n'+rest,'utf8');
 const old=fs.readFileSync(path.join(run,'before-doc-sync',name),'utf8').replace(/^\uFEFF/,'');
 if(!fs.readFileSync(file,'utf8').endsWith(old))throw Error('Historical body mismatch: '+name);
}
const baseline=JSON.parse(fs.readFileSync(String.raw`D:\AI_Agent_Lab\audit-results\rrpg-initialworld-20261105\preexisting-files.json`,'utf8').replace(/^\uFEFF/,''));
const known=new Map(baseline.map(r=>[r.path,r]));
const status=execFileSync('git',['-c','core.quotepath=false','status','--porcelain=v1','-uall'],{cwd:root,encoding:'utf8'});
const rows=[];
for(const line of status.split(/\r?\n/).filter(Boolean)){
 const name=line.slice(3), file=path.join(root,name);
 if(name==='Development/StudioRuns/Initialization-20261105/delivery-files.json')continue;
 if(!fs.existsSync(file)||!fs.statSync(file).isFile())continue;
 const data=fs.readFileSync(file),sha=crypto.createHash('sha256').update(data).digest('hex'),old=known.get(name)?.sha256??null;
 if(sha!==old)rows.push({path:name,status:line.slice(0,2),bytes:data.length,sha256:sha,baselineSha256:old});
}
fs.writeFileSync(path.join(run,'delivery-files.json'),JSON.stringify(rows,null,2));
console.log('Five Current entries updated; historical bodies retained. Manifest entries='+rows.length);
