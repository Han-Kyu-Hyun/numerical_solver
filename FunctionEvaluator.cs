using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;

namespace NonlinearSolver
{
    /// <summary>
    /// 사용자가 입력한 수식 문자열을 실제 계산 가능한 함수로 변환하는 클래스.
    /// 예) "Math.Pow(x,3) - x - 2" 입력 → f(x) = x³ - x - 2 로 동작
    /// 내부적으로 C# 코드를 즉석에서 컴파일하여 실행합니다.
    /// </summary>
    public class FunctionEvaluator
    {
        private MethodInfo _evalMethod;
        private string     _expression;

        public string Expression { get { return _expression; } }

        public FunctionEvaluator(string expression)
        {
            _expression = expression;
            Compile(expression);
        }

        /// <summary>수식 문자열을 C# 코드로 감싸서 컴파일</summary>
        private void Compile(string expression)
        {
            // 사용자 수식을 하나의 C# 메서드 안에 집어넣어 컴파일
            string code =
                "using System;\n" +
                "namespace NonlinearSolver.Dynamic\n" +
                "{\n" +
                "    public static class DynamicFunction\n" +
                "    {\n" +
                "        public static double Evaluate(double x)\n" +
                "        {\n" +
                "            return (double)(" + expression + ");\n" +
                "        }\n" +
                "    }\n" +
                "}";

            var provider   = new CSharpCodeProvider();
            var parameters = new CompilerParameters
            {
                GenerateInMemory   = true,   // 파일 대신 메모리에 컴파일
                GenerateExecutable = false   // .dll 형태로 생성
            };
            parameters.ReferencedAssemblies.Add("System.dll");

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.HasErrors)
            {
                var sb = new System.Text.StringBuilder();
                foreach (CompilerError err in results.Errors)
                    sb.AppendLine("  [" + err.ErrorNumber + "] " + err.ErrorText);
                throw new ArgumentException("함수 파싱 실패:\n" + sb.ToString());
            }

            // 컴파일 성공 시 메서드 참조를 저장
            Type type = results.CompiledAssembly.GetType("NonlinearSolver.Dynamic.DynamicFunction");
            _evalMethod = type.GetMethod("Evaluate");
        }

        /// <summary>f(x) 계산</summary>
        public double Evaluate(double x)
        {
            try
            {
                return (double)_evalMethod.Invoke(null, new object[] { x });
            }
            catch (TargetInvocationException ex)
            {
                throw new InvalidOperationException(
                    "f(" + x.ToString("G6") + ") 계산 중 오류: " +
                    (ex.InnerException != null ? ex.InnerException.Message : ex.Message));
            }
        }

        /// <summary>
        /// 수치 미분으로 도함수 근사 (중앙 차분법)
        /// f'(x) ≈ [f(x+h) - f(x-h)] / 2h
        /// Newton-Raphson 에서 사용됩니다.
        /// </summary>
        public double Derivative(double x, double h = 1e-7)
        {
            return (Evaluate(x + h) - Evaluate(x - h)) / (2.0 * h);
        }
    }
}
