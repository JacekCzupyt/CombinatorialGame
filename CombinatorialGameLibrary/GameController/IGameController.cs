using CombinatorialGameLibrary.GameState;
using System.Collections.Generic;

namespace CombinatorialGameLibrary.GameController {
    public interface IGameController : IGameState {
        VictoryState MakeMove(int m);

        VictoryState MakeRandomMove();

        List<int> getAvailableIdxs();
        void UndoMove();

        void Restart();
    }
}
