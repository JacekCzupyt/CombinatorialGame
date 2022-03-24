using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CombinatorialGameLibrary {
    public class SimpleGameState : IGameState {
        public int N { get; }
        public int K { get; }
        public List<int> GameList { get; }
        
        public SimpleGameState(int n, int k) {
            N = n;
            K = k;
            
            GameList = new List<int>(N);
            for (int i = 0; i < N; i++)
                GameList[i] = 0;
        }

        public int? MakeMove(int m, int c) {
            m--;
            if (m < 0 || m >= N)
                throw new ArgumentException("m not int range");
            if (GameList[m] != 0)
                throw new ArgumentException("m is already colored");
            if (c != -1 && c != 1)
                throw new ArgumentException("c is not a valid color");

            GameList[m] = c;
            var res = CheckVictory(m);
            if (res is not null)
                return res.Value.Item3;
            if (GameList.All(x => x != 0))
                return 0;
            return null;
        }
        
        private (int, int, int)? CheckVictory(int m) {
            int c = GameList[m];
            if (c == 0)
                return null;

            // For each jump value
            for (int j = 1; j <= (N-1)/(K-1); j++) {
                int len = 1;
                // Look ahead
                for (int h = 1; m + j * h < N; h++) {
                    int pos = m + j * h;
                    if (GameList[pos] != c)
                        break;
                    len++;
                    if (len >= K)
                        return (m, j, -c);
                }
                // Look behind
                for (int h = 1; m - j * h >= 0; h++) {
                    int pos = m - j * h;
                    if (GameList[pos] != c)
                        break;
                    len++;
                    if (len >= K)
                        return (pos, j, -c);
                }
            }
            return null;
        }
        
        public object Clone() {
            return new SimpleGameState(N, K, GameList);
        }
        
        private SimpleGameState(int n, int k, List<int> gameList) {
            N = n;
            K = k;
            GameList = gameList;
        }
    }
}
