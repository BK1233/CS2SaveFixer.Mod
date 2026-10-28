namespace Game
{
    public class UpdateSystem
    {
    }
}

namespace Game.Modding
{
    public interface IMod
    {
        void OnLoad(UpdateSystem updateSystem);
        void OnDispose();
    }
}
