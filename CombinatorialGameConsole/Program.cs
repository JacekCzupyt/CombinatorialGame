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

namespace CombinatorialGameConsole {
    partial class Program {
        static void Main(string[] args) {
            var gameList = new int[] { 1, 0, 0, -1, 0, 0, 1, 0, 0, 0 };
            var gameState = new SimpleGameState(10, 4, gameList, -1);
            Console.WriteLine($"Evaluation of gamestate: {MockEvaluationFunction(gameState)}");
        }

        static float MockEvaluationFunction(IGameState gameState) {
            return -gameState.GameList.Sum();
        }
    }
}
