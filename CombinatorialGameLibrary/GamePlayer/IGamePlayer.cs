using System.Threading;
using System.Threading.Tasks;
using CombinatorialGameLibrary.GameManagement;

namespace CombinatorialGameLibrary.GamePlayer {
    public interface IGamePlayer {
        Task<int> RequestMove(MoveRequest request, CancellationToken token);
    }
}
