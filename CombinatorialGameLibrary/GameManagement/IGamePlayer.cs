using System.Threading.Tasks;

namespace CombinatorialGameLibrary {
    public interface IGamePlayer {
        Task<int> RequestMove(MoveRequest request);
    }
}
