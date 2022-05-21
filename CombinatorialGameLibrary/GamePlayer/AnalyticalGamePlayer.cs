using CombinatorialGameLibrary.GameEvaluation;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameLibrary.GamePlayer {
    public class AnalyticalGamePlayer : AbstractMinMaxPlayer {
        private AnalyticalEvaluationFunction evaluation;

        public AnalyticalGamePlayer(int? maxDepth = 6, float? maxTime = 4, float alpha = 1, float beta = 1) : base(maxDepth, maxTime) {
            evaluation = new AnalyticalEvaluationFunction(alpha, beta);
        }

        protected override float EvaluateMove(IGameState state) {
            return (float)evaluation.EvaluatePosition(state);
        }
    }
}
