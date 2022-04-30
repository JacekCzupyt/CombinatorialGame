using System;
using System.Threading;
using System.Threading.Tasks;
using CombinatorialGameLibrary.GameManagement;
using CombinatorialGameLibrary.GamePlayer;

namespace CombinatorialGameFrontend {
    public class UserGamePlayer : IGamePlayer {
        private UserGamePlayer() {}
        
        public static UserGamePlayer Instance { get; } = new UserGamePlayer();

        public Task<int> RequestMove(MoveRequest request, CancellationToken token) {
            Request = request;
            moveResult = new TaskCompletionSource<int>();
            NotifyMove?.Invoke(request);
            moveRequested = true;
            token.Register(OnMoveCanceled);
            return moveResult.Task;
        }

        public MoveRequest Request { get; private set; }
        private TaskCompletionSource<int> moveResult;
        private bool moveRequested;

        public event Action<MoveRequest> NotifyMove;

        public void MakeMove(int move) {
            if (moveRequested) {
                moveResult.SetResult(move);
                moveRequested = false;
            }
            else
                throw new InvalidOperationException("No move has been requested");
        }

        private void OnMoveCanceled() {
            moveRequested = false;
            moveResult.TrySetCanceled();
        }
    }
}
