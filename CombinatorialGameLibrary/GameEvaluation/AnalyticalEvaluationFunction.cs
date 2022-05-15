using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CombinatorialGameLibrary.GameController;
using CombinatorialGameLibrary.GameManagement;
using CombinatorialGameLibrary.GameState;
using System.Collections.Generic;

namespace CombinatorialGameLibrary.GameEvaluation {
    public class AnalyticalEvaluationFunction {
        public double lastPositionEvaluation { get; }
        public double alpha { get; set; }
        public double beta { get; set; }

        public double gamma { get; }

        public AnalyticalEvaluationFunction(double alpha, double beta) {
            lastPositionEvaluation = 0;
            this.alpha = alpha;
            this.beta = beta;
            this.gamma = 1;
        }

        public double EvaluatePosition(IGameState state) {
            double res = 0;
            int number_of_ones = 0;
            int number_of_minusones = 0;

            // (number_of_ones, number_of_minusones) = CountOnesAndMinusOnes(state.GameList, state.K);

            for (int m = 1; m <= (state.N - 1) / (state.K - 1); m++) {
                for (int i = 0; i < m; i++) {
                    int start = i;
                    int end = i + m * (state.K - 1);
                    if (end >= state.N) {
                        break;
                    }
                    (number_of_ones, number_of_minusones) = CountOnesAndMinusOnes(state.GameList, i, m, state.K);
                    while (end < state.N) {
                        if (number_of_ones != 0 || number_of_minusones != 0) {
                            if (number_of_ones == 0) {
                                if (number_of_minusones == state.K) {
                                    return 1_000_000;
                                }
                                res += this.beta * Math.Pow(number_of_minusones, this.gamma);
                            }
                            if (number_of_minusones == 0) {
                                if (number_of_ones == state.K) {
                                    return -1_000_000;
                                }
                                res -= this.alpha * Math.Pow(number_of_ones, this.gamma);
                            }
                        }
                        if (state.GameList[start] != 0) {
                            if (state.GameList[start] == 1) {
                                number_of_ones--;
                            }
                            if (state.GameList[start] == -1) {
                                number_of_minusones--;
                            }
                        }
                        start += m;
                        end += m;
                        if (end >= state.N) {
                            break;
                        }
                        if (state.GameList[end] != 0) {
                            if (state.GameList[end] == 1) {
                                number_of_ones++;
                            }
                            if (state.GameList[end] == -1) {
                                number_of_minusones++;
                            }
                        }
                    }
                }
            }
            return res;
        }
        public (int, int) CountOnesAndMinusOnes(IReadOnlyList<int> gameList, int start, int step, int length) {
            int number_of_ones = 0;
            int number_of_minusones = 0;
            for (int i = start; i <= start + step * (length - 1); i += step) {
                if (gameList[i] == 1)
                    number_of_ones++;
                else if (gameList[i] == -1)
                    number_of_minusones++;
            }
            // Debug.WriteLine(
            //     $"Start: {start}, Step: {step}, End: {start + (length - 1) * step}, 1: {number_of_ones}, -1: {number_of_minusones}"
            // );
            return (number_of_ones, number_of_minusones);
        }
        public int EvaluateSequence(IReadOnlyList<int> gameList, int start, int step, int length) {
            int sum = 0;
            int count = 0;
            // Counts all -1s and 1s and their sum
            for (int i = start; i <= start + length * step; i += step) {
                if (gameList[i] != 0) {
                    count++;
                    sum += gameList[i];
                }
            }
            // Console.WriteLine($"Start: {start}, Step: {step}, End: {start + length * step}, Count: {count}, Sum: {sum}");
            // If sum is different than count, there are both -1s and 1s in the sequence
            if (Math.Abs(sum) != count) {
                return 0;
            }
            if (count == length + 1) {
                if (sum > 0)
                    return 1_000_000;
                else
                    return -1_000_000;
            }
            return sum;
        }
    }
}
