using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CombinatorialGameLibrary;
using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GameManagement;
using CombinatorialGameLibrary.GamePlayer;
using CombinatorialGameLibrary.GameState;

namespace CombinatorialGameConsole {
    class Program {
        static async Task Main(string[] args) {
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
