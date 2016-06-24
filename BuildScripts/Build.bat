IF /i "%1" == "NET40" (SET FrameworkVersion=v4.0)
IF /i "%1" == "NET40" (SET BuildConfigKey=NET40-Client)
IF /i "%1" == "NET40" (SET BuildConfiguration=NET40-Release)

IF /i "%1" == "NET40CP" (SET FrameworkVersion=v4.0)
IF /i "%1" == "NET40CP" (SET BuildConfigKey=NET40-Client)
IF /i "%1" == "NET40CP" (SET BuildConfiguration=NET40CP-Release)

IF /i "%1" == "NET35" (SET FrameworkVersion=v3.5)
IF /i "%1" == "NET35" (SET BuildConfigKey=NET35)
IF /i "%1" == "NET35" (SET BuildConfiguration=NET35-Release)

IF /i "%1" == "NET45" (SET FrameworkVersion=v4.5)
IF /i "%1" == "NET45" (SET BuildConfigKey=NET45)
IF /i "%1" == "NET45" (SET BuildConfiguration=NET45-Release)

SET MSBUILD_EXE="C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe"

::SET FrameworkVersion=v3.5
::SET BuildConfigKey=NET35
::SET BuildConfiguration=NET35-Release

@echo on
%MSBUILD_EXE% /m "../MethodInvocationLogger.Castle/MethodInvocationLogger.Castle.csproj" /p:Platform="AnyCPU" /p:BuildConfigKey=%BuildConfigKey% /p:TargetFrameworkVersion=%FrameworkVersion% /ToolsVersion:14.0  /property:Configuration=%BuildConfiguration% /t:Rebuild
%MSBUILD_EXE% /m "../MethodInvocationLogger/MethodInvocationLogger.csproj" /p:Platform="AnyCPU" /p:BuildConfigKey=%BuildConfigKey% /p:TargetFrameworkVersion=%FrameworkVersion% /ToolsVersion:14.0  /property:Configuration=%BuildConfiguration% /t:Rebuild
@echo off