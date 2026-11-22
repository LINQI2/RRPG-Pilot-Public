# postcommit 原始响应（未改写）

## 0. git rev-parse HEAD（原文）

```
21004a26e716c6bcdae10b8ada2f329de27e4e81
```

分支 `ai/rrpg-r3b-population-pilot`；`git status` staged=0 / dirty=134；
最近三条提交：`21004a2 feat(v17): preserve accepted management and initial world dependency closure`、
`96db828 docs(v17): record final dependency verification and verified compiler closeout`、
`fe11c40 feat(v17): preserve verified Initial World compiler and authoring chain locally`。

## 1. 门禁 begin

```json
{"record":"records/20261106T061529Z-v17-postcommit-01-begin.json","growthAdopted":false,"command":"begin","result":"OK","sha256":"7b5717b976bac0d90dce70ba65b4dda0995bf78e8b3fd317ea9918058109c016"}
```

## 2. 请求 ScriptCompilation（唯一一次）

```json
{"result":{"content":[{"type":"text","text":"System.String: RELOAD_REQUESTED compiling=True"}],"isError":false}}
```

## 3. 域重载后状态回读

```
project=D:\AI_Agent_Lab\rrpg_pilot\repo\UnityProject\RRPG1.0game
version=2022.3.62t9 product=RRPG_AI_Pilot
playing=False compiling=False updating=False
scriptCompilationFailed=False
console err=0 warn=523 log=2
assemblies=313 ManagementView@Assembly-CSharp REDACTED_EMAIL
prefab transforms=81 missingScripts=0
guid 4a7dfebd -> Assets/Scripts/Village/ProductHome/ManagementView.cs | ManagementView
guid f37df54f -> Assets/Scripts/Village/ProductHome/ManagementRowView.cs | ManagementRowView
guid f9418c72 -> Assets/Art/Fonts/SourceHanSansCN/RRPG_SourceHanSansCN_Regular_TMP.asset | .asset
```

## 4. 生成根与配置镜像（只读）

```
rootLoaded=True
root.contentSignature=78f532b6b58d916d67df6300e26bb0db95fc40ffc02c887c162aa8f7a06168ae
rootType=RRPG.StaticConfig.RRPGStaticConfigRoot
mirrorFiles=44
mirror InitialWorld.json sha=5af02464bbf359c6 bytes=1569
mirror InitialPopulation.json sha=ae0fe5456266ff66 bytes=1377
mirror InitialBuilding.json sha=32abb64be5c5fbe7 bytes=2209
api ComputeSignature(List`1,BuildOutcome) isStatic=True pub=False
api IsUpToDate(String,Int32,PresentationInput,Payload,Payload,Payload,Payload,List`1,Payload,Payload) isStatic=True pub=False
```

## 5. 门禁 end

```json
{"record":"records/20261106T061717Z-v17-postcommit-01-end.json","command":"end","result":"PASS","sha256":"7b5717b976bac0d90dce70ba65b4dda0995bf78e8b3fd317ea9918058109c016"}
```
