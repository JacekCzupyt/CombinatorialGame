using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CombinatorialGameLibrary;
using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GameManagement;
using CombinatorialGameLibrary.GamePlayer;
using CombinatorialGameLibrary.GameState;
using CombinatorialGameLibrary.Utils;

namespace CombinatorialGameConsole {
    class Program {
        static async Task Main(string[] args) {
            SolveGames(11, 6);
        }

        static void SolveGames(int maxN, int maxK) {
            var res = new int[maxN-1, maxK-1];
            
            for (int n = 2; n <= maxN; n++) {
                for (int k = 2; k <= maxK; k++) {
                    res[n-2, k-2] = StateSolver.SolveState(n, k);
                }
            }

            var dispDict = new Dictionary<int, (ConsoleColor, string)>() {
                { 0, (default, "0") },
                { 1, (ConsoleColor.Cyan, "1") },
                { -1, (ConsoleColor.Red, "2") }
            };

            for (int n = 0; n < maxN-1; n++) {
                for (int k = 0; k < maxK-1; k++) {
                    var disp = dispDict[res[n, k]];
                    Console.ForegroundColor = disp.Item1;
                    Console.Write(disp.Item2 + " ");
                }
                Console.Write("\n");
            }
        }

        static async Task PlayGame() {
            var player1 = new ConsolePlayer("Player1");
            var player2 = new MinMaxAiPlayer();
            
            var gameManager = new SimpleGameController(10, 3);

            var game = new GameManager(player1, player2, gameManager, true);

            game.MoveComplete += (player, _) => {
                Console.Clear();
                DisplayGame(game.GameState);
                if (player == 1)
                    Console.ReadKey();
                if(game.GamePaused)
                    game.ResumeGame();
            };
            
            DisplayGame(game.GameState);

            var res = await game.PlayGame();
            
            Console.WriteLine($"Player {res.Winner} has won!");
        }

        static void DisplayGame(IGameState state) {
            bool checkVictory = state.EndGameState.GameEnded && state.EndGameState.Winner != 0;
            Dictionary<int, ConsoleColor> colorSet = new() {
                { 0, default },
                { 1, ConsoleColor.Cyan },
                { -1, ConsoleColor.Red }
            };

            int? jmp = state.EndGameState.SequenceJump, st = state.EndGameState.SequenceStart;

            for (int i = 0; i < state.N; i++) {
                if (checkVictory && i % jmp == st % jmp && (i - st) / jmp >= 0 && (i - st) / jmp < state.K)
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                else {
                    Console.BackgroundColor = default;
                }

                Console.ForegroundColor = colorSet[state.GameList[i]];

                Console.Write(i + 1);

                Console.BackgroundColor = default;
                Console.ForegroundColor = default;

                Console.Write(" ");
            }

            Console.Write("\n");
        }
    }
}
