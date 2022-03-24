using System;
using System.Collections.Generic;

namespace CombinatorialGameLibrary {
    public interface IGameState : ICloneable {
        int N { get; }
        int K { get; }
        List<int> GameList { get; }
        int? MakeMove(int m, int c);
    }
}
