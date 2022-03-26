using System.Threading.Tasks;

namespace CombinatorialGameLibrary.GameManagement {
    public interface IGamePlayer {
        Task<int> RequestMove(MoveRequest request);
    }
}
