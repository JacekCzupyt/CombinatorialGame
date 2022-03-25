using System.Reflection.Metadata;

namespace CombinatorialGameLibrary {
    public interface IGamePlayer {
        void RequestMove(MoveRequest request);
    }
}
