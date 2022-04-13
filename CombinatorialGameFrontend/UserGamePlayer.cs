using System;
using System.Threading.Tasks;
using CombinatorialGameLibrary.GameManagement;
using CombinatorialGameLibrary.GamePlayer;

namespace CombinatorialGameFrontend {
    public class UserGamePlayer : IGamePlayer {
        private UserGamePlayer() {}
        
        public static IGamePlayer Instance { get; } = new UserGamePlayer();

        public Task<int> RequestMove(MoveRequest request) {
            Request = request;
            moveResult = new TaskCompletionSource<int>();
            NotifyMove?.Invoke();
            return moveResult.Task;
        }

        public MoveRequest Request { get; private set; }
        private TaskCompletionSource<int> moveResult;

        public event Action NotifyMove;

        public void MakeMove(int move) {
            if (moveResult?.Task.Status == TaskStatus.Running)
                moveResult.SetResult(move);
            else
                throw new InvalidOperationException("No move has been requested");
        }
    }
}
