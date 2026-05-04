using System;
using System.Diagnostics;

namespace NonlinearSolver
{
    /// <summary>
    /// 비선형 방정식을 푸는 세 가지 수치해석 방법을 제공합니다.
    /// 모든 메서드는 f(x) = 0 의 해(근)를 찾습니다.
    /// </summary>
    public static class NumericalMethods
    {
        // ═══════════════════════════════════════════════════════════════
        // 1. 이분법 (Bisection Method)
        //
        // 원리: f(a) * f(b) < 0 이면 [a, b] 구간에 반드시 근이 존재.
        //       중간점 c = (a+b)/2 를 구하고, 부호가 다른 쪽 절반을
        //       반복적으로 좁혀나가서 근을 찾습니다.
        //
        // 장점: 반드시 수렴 (구간 설정만 올바르면)
        // 단점: 수렴 속도가 느림 (매 반복마다 구간이 절반으로)
        // ═══════════════════════════════════════════════════════════════
        public static SolverResult Bisection(
            FunctionEvaluator func,
            double a, double b,
            double tolerance, int maxIterations)
        {
            var result = new SolverResult { MethodName = "이분법 (Bisection)" };
            var sw     = Stopwatch.StartNew();

            double fa = func.Evaluate(a);
            double fb = func.Evaluate(b);

            // 구간 조건 확인: f(a)와 f(b)의 부호가 달라야 근이 존재
            if (fa * fb > 0)
            {
                result.ErrorMessage =
                    "f(" + a.ToString("G4") + ")=" + fa.ToString("G4") +
                    " 와 f(" + b.ToString("G4") + ")=" + fb.ToString("G4") +
                    " 의 부호가 같습니다.\n" +
                    "  → 이 구간에 근이 없거나 짝수 개의 근이 있습니다. 구간을 다시 설정하세요.";
                sw.Stop();
                return result;
            }

            double c     = a;
            double error = double.MaxValue;

            for (int i = 1; i <= maxIterations; i++)
            {
                double prevC = c;
                c = (a + b) / 2.0;           // 구간의 중간점
                double fc = func.Evaluate(c);
                error = Math.Abs(c - prevC);  // 이전 추정값과의 차이

                result.Steps.Add(new IterationStep
                {
                    Iteration = i,
                    X         = c,
                    FX        = fc,
                    Error     = error
                });

                // 수렴 조건: f(c) ≈ 0 이거나 구간이 충분히 좁아짐
                if (Math.Abs(fc) < tolerance || error < tolerance)
                {
                    result.Converged = true;
                    break;
                }

                // 근이 있는 절반 선택
                if (fa * fc < 0) { b = c; fb = fc; }
                else             { a = c; fa = fc; }
            }

            sw.Stop();
            result.Root                = c;
            result.FunctionValue       = func.Evaluate(c);
            result.Iterations          = result.Steps.Count;
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }


        // ═══════════════════════════════════════════════════════════════
        // 2. 뉴턴-랩슨법 (Newton-Raphson Method)
        //
        // 원리: 현재 점에서 접선을 그어 x절편을 다음 추정값으로 사용.
        //       x_{n+1} = x_n - f(x_n) / f'(x_n)
        //       도함수는 수치 미분(중앙 차분법)으로 계산합니다.
        //
        // 장점: 수렴 속도 매우 빠름 (2차 수렴)
        // 단점: 초기값 설정이 나쁘면 발산 가능, f'(x)≈0 이면 실패
        // ═══════════════════════════════════════════════════════════════
        public static SolverResult NewtonRaphson(
            FunctionEvaluator func,
            double x0,
            double tolerance, int maxIterations)
        {
            var result = new SolverResult { MethodName = "뉴턴-랩슨법 (Newton-Raphson)" };
            var sw     = Stopwatch.StartNew();

            double x = x0;

            for (int i = 1; i <= maxIterations; i++)
            {
                double fx  = func.Evaluate(x);
                double fpx = func.Derivative(x);   // 수치 미분으로 f'(x) 계산

                // 도함수가 0에 너무 가까우면 나눗셈 불가
                if (Math.Abs(fpx) < 1e-12)
                {
                    result.ErrorMessage =
                        "반복 " + i + "에서 도함수 f'(" + x.ToString("G6") +
                        ") ≈ 0 으로 계산 불가.\n" +
                        "  → 초기값을 다른 값으로 변경해 보세요.";
                    break;
                }

                double xNew  = x - fx / fpx;        // 접선의 x절편 = 다음 추정값
                double error = Math.Abs(xNew - x);

                result.Steps.Add(new IterationStep
                {
                    Iteration = i,
                    X         = xNew,
                    FX        = func.Evaluate(xNew),
                    Error     = error
                });

                x = xNew;

                if (error < tolerance || Math.Abs(fx) < tolerance)
                {
                    result.Converged = true;
                    break;
                }
            }

            sw.Stop();
            result.Root                = x;
            result.FunctionValue       = func.Evaluate(x);
            result.Iterations          = result.Steps.Count;
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }


        // ═══════════════════════════════════════════════════════════════
        // 3. 할선법 (Secant Method)
        //
        // 원리: 두 점 (x0, f(x0)), (x1, f(x1))을 지나는 직선(할선)의
        //       x절편을 다음 추정값으로 사용.
        //       x_{n+1} = x_n - f(x_n) * (x_n - x_{n-1}) / (f(x_n) - f(x_{n-1}))
        //
        // 장점: 도함수 불필요, Newton-Raphson보다 안정적
        // 단점: 두 초기값 필요, Newton-Raphson보다 수렴 속도 약간 느림
        // ═══════════════════════════════════════════════════════════════
        public static SolverResult Secant(
            FunctionEvaluator func,
            double x0, double x1,
            double tolerance, int maxIterations)
        {
            var result = new SolverResult { MethodName = "할선법 (Secant)" };
            var sw     = Stopwatch.StartNew();

            double xPrev = x0;
            double xCurr = x1;

            for (int i = 1; i <= maxIterations; i++)
            {
                double fPrev = func.Evaluate(xPrev);
                double fCurr = func.Evaluate(xCurr);
                double denom = fCurr - fPrev;   // 두 함수값의 차이

                // 두 함수값이 같으면 직선의 기울기가 0 → 나눗셈 불가
                if (Math.Abs(denom) < 1e-12)
                {
                    result.ErrorMessage =
                        "반복 " + i + "에서 f(x0)≈f(x1) 로 직선을 그을 수 없습니다.\n" +
                        "  → 두 초기값을 더 멀리 설정해 보세요.";
                    break;
                }

                // 할선과 x축의 교점
                double xNew  = xCurr - fCurr * (xCurr - xPrev) / denom;
                double error = Math.Abs(xNew - xCurr);

                result.Steps.Add(new IterationStep
                {
                    Iteration = i,
                    X         = xNew,
                    FX        = func.Evaluate(xNew),
                    Error     = error
                });

                xPrev = xCurr;
                xCurr = xNew;

                if (error < tolerance || Math.Abs(fCurr) < tolerance)
                {
                    result.Converged = true;
                    break;
                }
            }

            sw.Stop();
            result.Root                = xCurr;
            result.FunctionValue       = func.Evaluate(xCurr);
            result.Iterations          = result.Steps.Count;
            result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
            return result;
        }
    }
}
