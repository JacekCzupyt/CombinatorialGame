using System;
using System.Collections.Generic;
using CombinatorialGameLibrary;

namespace CombinatorialGameConsole {
    class Program {
        static void Main(string[] args) {
            var game = new SimpleGameController(10, 4);
            while (!game.EndGameState.GameEnded) {
                DisplayGame(game);
                int val = int.Parse(Console.ReadLine());
                game.MakeMove(val-1);
                Console.Clear();
            }

            DisplayGame(game);
            Console.WriteLine($"Player {game.EndGameState.Winner} has won!");
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
