# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build

빌드 스크립트 사용 (권장):
```
build.bat
```

직접 컴파일 (`csc.exe` 경로는 `build.bat` 참조):
```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:exe /out:NonlinearSolver.exe /r:System.dll /r:Microsoft.CSharp.dll /r:System.Core.dll /utf8output /codepage:65001 ResultModels.cs FunctionEvaluator.cs NumericalMethods.cs Program.cs
```

## Run

대화형 실행:
```
NonlinearSolver.exe
```

파이프로 입력 파일 사용 (비대화형 테스트):
```
NonlinearSolver.exe < test_input.txt
```

## Architecture

단일 네임스페이스 `NonlinearSolver`의 4개 파일로 구성됩니다.

**데이터 흐름:**
`Program.cs` (사용자 입력) → `FunctionEvaluator` (수식 컴파일) → `NumericalMethods` (수치해석) → `SolverResult` (결과 저장) → `Program.cs` (결과 출력)

**핵심 설계 포인트:**

- **`FunctionEvaluator`**: 사용자가 입력한 C# 수식 문자열(`"Math.Pow(x,3) - x - 2"`)을 `CSharpCodeProvider`로 런타임에 동적 컴파일하여 `MethodInfo`로 호출. 수치 미분은 중앙 차분법(`h = 1e-7`)으로 내부 구현.
- **`NumericalMethods`**: 이분법, 뉴턴-랩슨법, 할선법 세 가지 정적 메서드. 각 메서드는 `SolverResult`에 `List<IterationStep>`을 축적하며 계산 과정을 기록.
- **`ResultModels`**: `IterationStep`(반복 1단계 데이터)과 `SolverResult`(전체 결과 + Steps 리스트)만 담는 순수 DTO.
- **`Program`**: 콘솔 UI 루프. 입력은 `Enter` 시 기본값이 적용되므로 `test_input.txt`처럼 빈 줄을 넣어 기본값을 사용 가능.

## Target Framework

- .NET Framework 4.6.1 (`net461`), C# 7.3
- `Microsoft.CSharp` 참조 필수 (동적 컴파일용)
- `dotnet build`/`dotnet run` 불가 — `csc.exe` 직접 컴파일 또는 `build.bat` 사용
