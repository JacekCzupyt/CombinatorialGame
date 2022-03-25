namespace CombinatorialGameLibrary {
    public interface IGameController : IGameState {
        VictoryState MakeMove(int m);
        
        void UndoMove();
    }
}
