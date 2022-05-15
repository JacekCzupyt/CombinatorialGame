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
        static AnalyticalEvaluationFunction evaluator = new AnalyticalEvaluationFunction(1.0, 1.0);

        private static AbstractMinMaxPlayer player = new AnalyticalGamePlayer(null, maxTime: 5);
        static async Task Main(string[] args) {
            var gameState = new SimpleGameState(100, 4);
            await player.RequestMove(new MoveRequest(gameState, 1), CancellationToken.None);
            // Console.WriteLine($"Evaluation of gamestate: {evaluator.EvaluatePosition(gameState)}");
        }
    }
}
