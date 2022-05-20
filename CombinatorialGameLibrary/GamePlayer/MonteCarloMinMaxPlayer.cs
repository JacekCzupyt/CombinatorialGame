using CombinatorialGameLibrary.GameEvaluation;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameLibrary.GamePlayer {
    public class MonteCarloMinMaxPlayer : AbstractMinMaxPlayer {
        private MonteCarloEvaluationFunction evaluator;
        
        public MonteCarloMinMaxPlayer(int? maxDepth = 1, int? monteCarloCount = 1000, float? monteCarloTime = null, float? dynamicTime = 1f) : base(maxDepth, null) {
            evaluator = new MonteCarloEvaluationFunction(monteCarloCount, monteCarloTime, dynamicTime);
        }
        protected override float EvaluateMove(IGameState state) {
            return evaluator.EvaluatePosition(state, _token);
        }
    }
}
