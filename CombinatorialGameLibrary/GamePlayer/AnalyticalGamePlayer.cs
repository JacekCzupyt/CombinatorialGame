using CombinatorialGameLibrary.GameEvaluation;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameLibrary.GamePlayer {
    public class AnalyticalGamePlayer : AbstractMinMaxPlayer {
        private AnalyticalEvaluationFunction evaluation;

        public AnalyticalGamePlayer(int depth, float alpha = 1, float beta = 1) : base(depth) {
            evaluation = new AnalyticalEvaluationFunction(alpha, beta);
        }

        protected override float EvaluateMove(IGameState state) {
            return (float)evaluation.EvaluatePosition(state);
        }
    }
}
