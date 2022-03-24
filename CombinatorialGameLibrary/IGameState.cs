using System;
using System.Collections.Generic;

namespace CombinatorialGameLibrary {
    public interface IGameState : ICloneable {
        int N { get; }
        int K { get; }
        int ActivePlayer { get; }
        List<int> GameList { get; }
        VictoryState State { get; }
    }

    public interface IChangeableGameState : IGameState {
        VictoryState MakeMove(int m);
    }
}
