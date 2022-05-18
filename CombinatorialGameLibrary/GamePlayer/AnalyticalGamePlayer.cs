using CombinatorialGameLibrary.GameEvaluation;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameLibrary.GamePlayer {
    public class AnalyticalGamePlayer : AbstractMinMaxPlayer {
        private AnalyticalEvaluationFunction evaluation;

        public AnalyticalGamePlayer(int? maxDepth = 5, float? maxTime = 5, float alpha = 1, float beta = 1) : base(maxDepth, maxTime) {
            evaluation = new AnalyticalEvaluationFunction(alpha, beta);
        }

        protected override float EvaluateMove(IGameState state) {
            return (float)evaluation.EvaluatePosition(state);
        }
    }
}
