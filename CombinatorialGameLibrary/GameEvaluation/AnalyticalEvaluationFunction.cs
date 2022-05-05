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
        public double lastPositionEvaluation
        {get;}
        public double alpha
        {get; set;}
        public double beta
        {get; set;}

        public double gamma
        {get;}

        public AnalyticalEvaluationFunction(double alpha, double beta) {
            lastPositionEvaluation = 0;
            this.alpha = alpha;
            this.beta = beta;
            this.gamma = 1;
        }

        public double EvaluatePosition(IGameState state) {
            double res = 0;
            for (int i = 0; i < state.N; i++) {
                for (int m = 1; m <= (state.N - i + 1)/state.K; m++) {
                    int e = EvaluateSequence(state.GameList, i, m, state.K);
                    // Check if sequence has the same sign as the active player
                    if(e * state.ActivePlayer > 0)
                        res -= this.alpha * state.ActivePlayer *  Math.Pow(Math.Abs(e), this.gamma);
                    else
                        res += this.beta * state.ActivePlayer * Math.Pow(Math.Abs(e), this.gamma);
            }
            
        }
        return 0;
        
        }
        public int EvaluateSequence(IReadOnlyList<int> gameList ,int start, int step, int length) {
            int sum = 0;
            int count = 0;
            // Counts all -1s and 1s and their sum
            for (int i = start; i < start + length * step; i++) {
                if (gameList[i] != 0) {
                    count++;
                    sum+=gameList[i];
                }

            }
            // If sum is different than count, there are both -1s and 1s in the sequence
            if (Math.Abs(sum) != count){
                    return 0;
            }
            // FIXME: This is not the best way to do this, use int.max?
            if (count == length) {
                if (sum > 0)
                    return 1_000_000;
                else
                    return -1_000_000;
                
            }
            return sum;

        }
    }
}
