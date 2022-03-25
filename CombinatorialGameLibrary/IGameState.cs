using System;
using System.Collections.Generic;

namespace CombinatorialGameLibrary {
    public interface IGameState : ICloneable {
        int N { get; }
        int K { get; }
        int ActivePlayer { get; }
        IReadOnlyList<int> GameList { get; }
        VictoryState EndGameState { get; }
        IReadOnlyList<int> History { get; }
    }

    public interface IGameController : IGameState {
        VictoryState MakeMove(int m);
        
        void UndoMove();
    }
}
