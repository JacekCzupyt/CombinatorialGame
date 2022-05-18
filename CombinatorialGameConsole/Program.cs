using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CombinatorialGameLibrary;
using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GameManagement;
using CombinatorialGameLibrary.GamePlayer;
using CombinatorialGameLibrary.GameState;
using CombinatorialGameLibrary.Utils;
using CombinatorialGameLibrary.GameEvaluation;

namespace CombinatorialGameConsole {
    partial class Program {
        private static AbstractMinMaxPlayer player = new AnalyticalGamePlayer(null, maxTime: 5);
        static void Main(string[] args) {
            var gameState = new SimpleGameState(30, 8);
            // await player.RequestMove(new MoveRequest(gameState, 1), CancellationToken.None);
        }
    }
}
