using System;
using System.Threading.Tasks;
using CombinatorialGameLibrary.GameManagement;
using CombinatorialGameLibrary.GamePlayer;

namespace CombinatorialGameConsole {
    public class ConsolePlayer : IGamePlayer {
        public string PlayerName { get; }
        
        public ConsolePlayer(string playerName) {
            PlayerName = playerName;
        }
        public Task<int> RequestMove(MoveRequest request) {
            return Task.Run(() => GetPlayerInput(request));
        }

        private int GetPlayerInput(MoveRequest request) {
            while (true) {
                if(request.Error is not null)
                    Console.WriteLine($"Invalid move: {request.Error.Message}");
                
                Console.WriteLine($"Input {PlayerName} move:");
                
                try {
                    return int.Parse(Console.ReadLine() ?? string.Empty) - 1;
                }
                catch (FormatException e) {
                    request.Error = e;
                }
            }
        }
    }
}
