using CombinatorialGameLibrary.GamePlayer;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameConsole {
    partial class Program {
        private static AbstractMinMaxPlayer player = new AnalyticalGamePlayer(null, maxTime: 5);
        static void Main(string[] args) {
            var gameState = new SimpleGameState(30, 8);
            // await player.RequestMove(new MoveRequest(gameState, 1), CancellationToken.None);
        }
    }
}
