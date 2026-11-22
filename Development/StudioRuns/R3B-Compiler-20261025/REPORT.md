# R3-B Compiler 真实维护验证

2026-10-25，北京时间16:20:45—16:20:55运行宿主测试/校验。授权工程仅D:/AI_Agent_Lab/rrpg_pilot/repo；未触及正式RRPG、未运行Unity、未改代码/断言/权限。执行者为维护端普通用户宿主，不冒称DSH已获得Shell能力。

- 本机.NET SDK 10.0.204；锁定依赖、空离线源加现有缓存恢复成功，无下载新包。
- dotnet build Release --no-restore退出0，0警告0错误。
- 首次受限环境vstest未能连接testhost，运行中止，保留restricted-test-attempt.log；不是业务测试失败。
- 普通用户宿主dotnet test Release --no-build --no-restore退出0，原始TRX total=264 executed=264 passed=264 failed=0 notExecuted=0 aborted=0。
- rrpg-config.dll validate --root 指定Pilot退出0，日志明确Mirror未写入；未执行配置build、Unity Builder、Git提交或编辑器启停。
- 运行前后494项已有dirty/untracked文件逐项SHA256未变（changedProtected=[]），包含当前工作现场；只生成Compiler bin/obj、测试临时夹具和本证据目录。临时目录限定Studio私有runtime，不清用户临时目录。

TRX SHA256：587d9dd1c1dd2c990fafb1a1b1c2188985759b8314bef4b704574dc4d506b0f5
宿主结果SHA256：2690af3eff291dd72c8b3516811b931ea6cf05139c3346c5244149b4f8500ee8

复核入口：本目录compiler.trx、tests.log、build.log、validate.log、host-result.json。完整保护基线保存在Studio私有runtime/compiler-gate-20261025/before.json。维护仅补齐本门禁，不代表R3-B整包通过，后续文档一致性及剩余体验核验由原主开发继续。
