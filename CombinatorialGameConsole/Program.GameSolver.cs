using System;
using System.Collections.Generic;
using CombinatorialGameLibrary.Utils;

namespace CombinatorialGameConsole {
    partial class Program {
        private static void SolveGames(int maxN, int maxK) {
            var res = new int[maxN, maxK];

            for (int k = 2; k <= maxK; k++) {
                res[0, k - 1] = 0;
            }

            for (int n = 1; n <= maxN; n++) {
                res[n - 1, 0] = -1;
            }

            for (int n = 2; n <= maxN; n++) {
                for (int k = 2; k <= maxK; k++) {
                    res[n - 1, k - 1] = StateSolver.SolveState(n, k);
                }
            }

            var dispDict = new Dictionary<int, (ConsoleColor, string)>() {
                { 0, (default, "0") },
                { 1, (ConsoleColor.Cyan, "1") },
                { -1, (ConsoleColor.Red, "2") }
            };

            for (int n = 0; n < maxN; n++) {
                for (int k = 0; k < maxK; k++) {
                    var disp = dispDict[res[n, k]];
                    Console.ForegroundColor = disp.Item1;
                    Console.Write(disp.Item2 + " ");
                }
                Console.Write("\n");
            }
        }
    }
}
