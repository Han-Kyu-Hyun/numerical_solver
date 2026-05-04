@echo off
chcp 65001 > nul
echo.
echo  [빌드] 비선형 방정식 수치해석 프로그램 컴파일 중...
echo.

set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe
set REFS=/r:System.dll /r:Microsoft.CSharp.dll /r:System.Core.dll

%CSC% /target:exe /out:NonlinearSolver.exe /utf8output /codepage:65001 %REFS% ^
    ResultModels.cs ^
    FunctionEvaluator.cs ^
    NumericalMethods.cs ^
    Program.cs

if %ERRORLEVEL% == 0 (
    echo.
    echo  [OK] 빌드 성공! NonlinearSolver.exe 가 생성되었습니다.
    echo.
    echo  실행하려면: NonlinearSolver.exe
) else (
    echo.
    echo  [오류] 빌드 실패. 위의 오류 메시지를 확인하세요.
)

pause
