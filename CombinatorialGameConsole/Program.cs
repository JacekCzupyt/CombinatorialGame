using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        static void Main(string[] args) {
            var gameList = new int[] { 1, -1, 0, 0, 1 };
            var gameState = new SimpleGameState(5, 3, gameList, -1);
            Console.WriteLine($"Evaluation of gamestate: {evaluator.EvaluatePosition(gameState)}");
        }

        static float MockEvaluationFunction(IGameState gameState) {
            return -gameState.GameList.Sum();
        }
    }
}
