# 비선형 방정식 수치해석 풀이 프로그램

비선형 1변수 방정식 `f(x) = 0`의 근을 세 가지 수치해석 방법으로 동시에 구하고 결과를 비교하는 콘솔 애플리케이션입니다.

## 지원 알고리즘

| 방법 | 수렴 속도 | 초기값 조건 |
|------|-----------|-------------|
| **이분법 (Bisection)** | 선형 수렴 | 구간 [a, b] — f(a)·f(b) < 0 필수 |
| **뉴턴-랩슨법 (Newton-Raphson)** | 2차 수렴 (매우 빠름) | 초기값 x₀ 1개 |
| **할선법 (Secant)** | 초선형 수렴 | 초기값 x₀, x₁ 2개 |

## 요구 사항

- Windows OS
- .NET Framework 4.6.1 이상
- `csc.exe` (`C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`)

## 빌드

```bat
build.bat
```

직접 컴파일:

```
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe ^
  /target:exe /out:NonlinearSolver.exe ^
  /r:System.dll /r:Microsoft.CSharp.dll /r:System.Core.dll ^
  /utf8output /codepage:65001 ^
  ResultModels.cs FunctionEvaluator.cs NumericalMethods.cs Program.cs
```

## 실행

대화형 모드:

```
NonlinearSolver.exe
```

입력 파일로 비대화형 실행:

```
NonlinearSolver.exe < test_input.txt
```

## 수식 입력 방법

C# `Math` 클래스 문법을 사용합니다.

| 수학 표기 | 입력 예시 |
|-----------|-----------|
| x³ - x - 2 | `Math.Pow(x,3) - x - 2` |
| x² - 4 | `Math.Pow(x,2) - 4` |
| sin(x) - x/2 | `Math.Sin(x) - x/2.0` |
| eˣ - 3x | `Math.Exp(x) - 3*x` |
| cos(x) - x | `Math.Cos(x) - x` |
| ln(x) - 1 | `Math.Log(x) - 1` |

## 사용 예시

```
f(x) = Math.Pow(x,3) - x - 2

허용 오차 (Enter = 1e-6)    :        ← Enter로 기본값 사용
최대 반복 횟수 (Enter = 100) :        ← Enter로 기본값 사용

이분법 구간 시작점 a : -10
이분법 구간 끝점   b : 10
뉴턴-랩슨법 초기값 x0 : 1
할선법 초기값 x0 : 0
할선법 초기값 x1 : 1
```

출력 예시:
```
+-- 이분법 (Bisection) ---------------------------------
|   반복     x (근 추정값)             f(x)          오차
|  -----  ----------------------  ------------------  ----------------
|      1       0.0               -2.0              10
|     ...
|  [수렴 성공]
|  근 (Root)  : 1.52137970680458
|  f(근) 값  : -2.66E-14
|  반복 횟수 : 21 회

+====================================================+
|              [ 최종 결과 비교 ]                    |
+====================================================+
  방법                            근 (Root)      f(근)   반복  수렴
  이분법 (Bisection)         1.52137970680    -2.66e-14    21  [OK]
  뉴턴-랩슨법 (Newton-Raphson) 1.52137970680    1.33e-15     5  [OK]
  할선법 (Secant)            1.52137970680    0.00e+00     6  [OK]
```

## 아키텍처

```
Program.cs          ← 콘솔 UI, 입력/출력 루프
FunctionEvaluator.cs ← C# 수식 동적 컴파일 (CSharpCodeProvider)
NumericalMethods.cs  ← 이분법 / 뉴턴-랩슨법 / 할선법 구현
ResultModels.cs      ← DTO: IterationStep, SolverResult
```

**데이터 흐름:**

```
Program.cs → FunctionEvaluator → NumericalMethods → SolverResult → Program.cs
 (사용자 입력)   (수식 컴파일)       (수치해석)         (결과 저장)    (결과 출력)
```

- `FunctionEvaluator`: 수식 문자열을 런타임에 동적 컴파일. 수치 미분은 중앙 차분법(`h = 1e-7`) 사용.
- `NumericalMethods`: 세 알고리즘 모두 `List<IterationStep>`에 반복 과정을 기록.
- `ResultModels`: 순수 DTO — 로직 없음.
- `Program`: `Enter` 입력 시 기본값 적용 → `test_input.txt`의 빈 줄로 자동화 가능.
