using System.Collections.Generic;

namespace NonlinearSolver
{
    /// <summary>반복 계산 한 단계의 결과를 저장하는 클래스</summary>
    public class IterationStep
    {
        public int    Iteration { get; set; }   // 반복 횟수 (1, 2, 3...)
        public double X         { get; set; }   // 현재 근의 추정값
        public double FX        { get; set; }   // f(x) 값
        public double Error     { get; set; }   // 이전 단계와의 차이 (오차)
    }

    /// <summary>수치해석 방법 전체 결과를 담는 클래스</summary>
    public class SolverResult
    {
        public string              MethodName        { get; set; }  // 방법 이름
        public double              Root              { get; set; }  // 계산된 근
        public double              FunctionValue     { get; set; }  // f(근) 값 (0에 가까울수록 정확)
        public int                 Iterations        { get; set; }  // 총 반복 횟수
        public bool                Converged         { get; set; }  // 수렴 성공 여부
        public string              ErrorMessage      { get; set; }  // 오류 메시지 (없으면 null)
        public long                ElapsedMilliseconds { get; set; } // 계산 소요 시간(ms)
        public List<IterationStep> Steps             { get; set; }  // 반복 과정 상세

        public SolverResult()
        {
            Steps = new List<IterationStep>();
        }
    }
}
