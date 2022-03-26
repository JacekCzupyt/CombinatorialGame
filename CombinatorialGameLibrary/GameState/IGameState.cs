using System;
using System.Collections.Generic;

namespace CombinatorialGameLibrary.GameState {
    public interface IGameState : ICloneable {
        int N { get; }
        int K { get; }
        int ActivePlayer { get; }
        IReadOnlyList<int> GameList { get; }
        VictoryState EndGameState { get; }
        IReadOnlyList<int> History { get; }
    }
}
