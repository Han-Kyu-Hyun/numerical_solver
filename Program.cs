using System;
using System.Collections.Generic;
using System.Globalization;

namespace NonlinearSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            try { Console.OutputEncoding = System.Text.Encoding.UTF8; } catch { }

            while (true)
            {
                TryClear();
                ShowBanner();

                // 1단계: 함수 입력
                FunctionEvaluator func = GetFunction();
                if (func == null) break;

                // 2단계: 공통 파라미터
                Console.WriteLine();
                PrintSection("공통 파라미터 설정");
                double tol     = GetDouble("허용 오차 (Enter = 1e-6)    : ", 1e-6);
                int    maxIter = GetInt   ("최대 반복 횟수 (Enter = 100) : ", 100);

                // 3단계: 각 방법별 초기값 입력
                PrintSection("이분법 초기 구간 [a, b] 설정");
                Console.WriteLine("  ※ f(a) 와 f(b) 의 부호가 반드시 달라야 합니다.");
                double bisA = GetDouble("  구간 시작점 a : ", -10.0);
                double bisB = GetDouble("  구간 끝점   b : ",  10.0);

                PrintSection("뉴턴-랩슨법 초기값 설정");
                double nrX0 = GetDouble("  초기값 x0 : ", 1.0);

                PrintSection("할선법 초기값 설정");
                double secX0 = GetDouble("  초기값 x0 : ", 0.0);
                double secX1 = GetDouble("  초기값 x1 : ", 1.0);

                // 4단계: 세 가지 방법으로 계산
                Console.WriteLine();
                PrintInfo("계산 중...");
                Console.WriteLine();

                SolverResult bisResult = NumericalMethods.Bisection    (func, bisA, bisB, tol, maxIter);
                SolverResult nrResult  = NumericalMethods.NewtonRaphson(func, nrX0,       tol, maxIter);
                SolverResult secResult = NumericalMethods.Secant       (func, secX0, secX1, tol, maxIter);

                var results = new List<SolverResult> { bisResult, nrResult, secResult };

                // 5단계: 결과 출력
                foreach (SolverResult r in results)
                    PrintMethodResult(r);

                PrintComparisonTable(results);

                // 6단계: 반복 여부 확인
                Console.Write("\n  다른 함수를 계산하시겠습니까? (y = 예 / 그 외 = 종료): ");
                string rawAnswer = Console.ReadLine();
                string answer    = (rawAnswer != null) ? rawAnswer.Trim().ToLower() : "";
                if (answer != "y") break;
            }

            Console.WriteLine("\n  프로그램을 종료합니다.");
            try { Console.ReadKey(); } catch { }
        }

        // ────────────────────────────────────────────────────────────────
        //  입력 헬퍼 메서드
        // ────────────────────────────────────────────────────────────────

        static FunctionEvaluator GetFunction()
        {
            while (true)
            {
                Console.Write("  f(x) = ");
                string rawInput = Console.ReadLine();
                string input    = (rawInput != null) ? rawInput.Trim() : "";

                if (input.Length == 0 || input.ToLower() == "exit")
                    return null;

                try
                {
                    var    func    = new FunctionEvaluator(input);
                    double testVal = func.Evaluate(0.0);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  [OK] 함수 등록 완료.  f(0) = " + testVal.ToString("G8"));
                    Console.ResetColor();
                    return func;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("  [오류] " + ex.Message);
                    Console.ResetColor();
                    Console.WriteLine("  수식을 다시 입력해 주세요. (exit 입력 시 종료)");
                }
            }
        }

        static double GetDouble(string prompt, double defaultValue)
        {
            Console.Write("  " + prompt);
            string rawLine = Console.ReadLine();
            string input   = (rawLine != null) ? rawLine.Trim() : "";
            if (input.Length == 0) return defaultValue;
            double v;
            return double.TryParse(input,
                NumberStyles.Float, CultureInfo.InvariantCulture, out v) ? v : defaultValue;
        }

        static int GetInt(string prompt, int defaultValue)
        {
            Console.Write("  " + prompt);
            string rawLine = Console.ReadLine();
            string input   = (rawLine != null) ? rawLine.Trim() : "";
            if (input.Length == 0) return defaultValue;
            int v;
            return int.TryParse(input, out v) ? v : defaultValue;
        }

        static void TryClear()
        {
            try { Console.Clear(); } catch { }
        }

        // ────────────────────────────────────────────────────────────────
        //  출력 헬퍼 메서드
        // ────────────────────────────────────────────────────────────────

        static void ShowBanner()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  +===========================================================+");
            Console.WriteLine("  |       비선형 1변수 방정식 수치해석 풀이 프로그램          |");
            Console.WriteLine("  |   Nonlinear Equation Numerical Analysis Solver             |");
            Console.WriteLine("  +===========================================================+");
            Console.ResetColor();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  [ 수식 입력 방법 ]  C# Math 클래스 문법을 사용합니다.");
            Console.ResetColor();
            Console.WriteLine("  +--------------------------------------------+");
            Console.WriteLine("  | 원하는 함수       |  입력 예시              |");
            Console.WriteLine("  +-------------------+-------------------------+");
            Console.WriteLine("  | x^2 - 4           |  Math.Pow(x,2) - 4      |");
            Console.WriteLine("  | x^3 - x - 2       |  Math.Pow(x,3) - x - 2  |");
            Console.WriteLine("  | sin(x) - x/2      |  Math.Sin(x) - x/2.0    |");
            Console.WriteLine("  | e^x - 3x          |  Math.Exp(x) - 3*x      |");
            Console.WriteLine("  | cos(x) - x        |  Math.Cos(x) - x        |");
            Console.WriteLine("  | ln(x) - 1         |  Math.Log(x) - 1        |");
            Console.WriteLine("  +--------------------------------------------+");
            Console.WriteLine("  (종료하려면 'exit' 입력)");
            Console.WriteLine();
        }

        static void PrintSection(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("  --- " + title + " ---");
            Console.ResetColor();
        }

        static void PrintInfo(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  " + msg);
            Console.ResetColor();
        }

        static void PrintMethodResult(SolverResult r)
        {
            string border = new string('-', Math.Max(0, 50 - r.MethodName.Length));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  +-- " + r.MethodName + " " + border);
            Console.ResetColor();

            if (!string.IsNullOrEmpty(r.ErrorMessage))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  | [실패] " + r.ErrorMessage);
                Console.ResetColor();
                Console.WriteLine("  +" + new string('-', 55));
                return;
            }

            // 반복 테이블 헤더
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  |");
            Console.WriteLine(
                "  |  " +
                PadLeft("반복",   5) + "  " +
                PadLeft("x (근 추정값)", 22) + "  " +
                PadLeft("f(x)", 18) + "  " +
                PadLeft("오차", 16));
            Console.WriteLine(
                "  |  " +
                new string('-', 5) + "  " +
                new string('-', 22) + "  " +
                new string('-', 18) + "  " +
                new string('-', 16));
            Console.ResetColor();

            // 반복 과정 (최대 15행)
            int showCount = Math.Min(r.Steps.Count, 15);
            for (int i = 0; i < showCount; i++)
            {
                IterationStep s = r.Steps[i];
                Console.WriteLine(
                    "  |  " +
                    PadLeft(s.Iteration.ToString(),   5) + "  " +
                    PadLeft(s.X.ToString("G14"),     22) + "  " +
                    PadLeft(s.FX.ToString("G10"),    18) + "  " +
                    PadLeft(s.Error.ToString("G8"),  16));
            }
            if (r.Steps.Count > 15)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("  |  ... (이하 " + (r.Steps.Count - 15) + "개 행 생략)");
                Console.ResetColor();
            }

            Console.WriteLine("  |");
            if (r.Converged)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  |  [수렴 성공]");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  |  [미수렴] 최대 반복 횟수 초과");
            }
            Console.ResetColor();

            Console.WriteLine("  |  근 (Root)   : " + r.Root.ToString("G15"));
            Console.WriteLine("  |  f(근) 값   : " + r.FunctionValue.ToString("G10") + "   <- 0에 가까울수록 정확");
            Console.WriteLine("  |  반복 횟수  : " + r.Iterations + " 회");
            Console.WriteLine("  |  계산 시간  : " + r.ElapsedMilliseconds + " ms");
            Console.WriteLine("  +" + new string('-', 55));
        }

        static void PrintComparisonTable(List<SolverResult> results)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  +====================================================+");
            Console.WriteLine("  |              [ 최종 결과 비교 ]                    |");
            Console.WriteLine("  +====================================================+");
            Console.ResetColor();

            string sep = "  " + new string('-', 70);
            Console.WriteLine();
            Console.WriteLine(
                "  " +
                PadRight("방법", 28) +
                PadLeft("근 (Root)", 20) +
                PadLeft("f(근)", 14) +
                PadLeft("반복", 7) +
                PadLeft("수렴", 5));
            Console.WriteLine(sep);

            foreach (SolverResult r in results)
            {
                bool   hasError = !string.IsNullOrEmpty(r.ErrorMessage);
                string rootStr  = hasError ? "계산 불가" : r.Root.ToString("G12");
                string fvalStr  = hasError ? "-"         : r.FunctionValue.ToString("G6");
                string convStr  = r.Converged ? "[OK]" : "[--]";

                Console.ForegroundColor = r.Converged ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(
                    "  " +
                    PadRight(r.MethodName, 28) +
                    PadLeft(rootStr,  20) +
                    PadLeft(fvalStr,  14) +
                    PadLeft(r.Iterations.ToString(), 7) +
                    PadLeft(convStr,   5));
                Console.ResetColor();
            }
            Console.WriteLine(sep);

            // 수렴 방법들 결과 일치 여부 확인
            var converged = new List<SolverResult>();
            foreach (SolverResult r in results)
                if (r.Converged) converged.Add(r);

            if (converged.Count >= 2)
            {
                double refRoot = converged[0].Root;
                bool   allMatch = true;
                foreach (SolverResult r in converged)
                    if (Math.Abs(r.Root - refRoot) >= 1e-6) allMatch = false;

                Console.WriteLine();
                if (allMatch)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("  * 수렴한 방법들의 결과가 서로 일치합니다. (오차 < 1e-6)");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("  * 주의: 수렴한 방법들의 결과에 차이가 있습니다.");
                    Console.WriteLine("    (방정식에 여러 근이 있거나 초기값 설정을 확인하세요)");
                }
                Console.ResetColor();
            }
        }

        // 문자열 정렬 헬퍼 (C# 5 호환)
        static string PadLeft(string s, int width)
        {
            if (s == null) s = "";
            return s.PadLeft(width);
        }

        static string PadRight(string s, int width)
        {
            if (s == null) s = "";
            return s.PadRight(width);
        }
    }
}
