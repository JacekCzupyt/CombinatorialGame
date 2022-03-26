using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameLibrary.GameController {
    public interface IGameController : IGameState {
        VictoryState MakeMove(int m);
        
        void UndoMove();
    }
}
