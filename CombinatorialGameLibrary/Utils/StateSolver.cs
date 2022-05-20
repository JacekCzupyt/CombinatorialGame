using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GamePlayer;

namespace CombinatorialGameLibrary.Utils {
    public static class StateSolver {
        public static int SolveState(int n, int k) {
            if (n - n / 2 < k)
                return 0;

            var game = new SimpleGameController(n, k);

            var minmax = new MinMaxAiPlayer().MinMax(game);

            return minmax.Item2;
        }
    }
}
