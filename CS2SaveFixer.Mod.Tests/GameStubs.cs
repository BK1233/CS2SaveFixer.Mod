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

namespace Colossal.Logging
{
    public interface ILog
    {
        void Info(string msg);
        void Warn(string msg);
        void Error(string msg);
    }

    public static class LogManager
    {
        public static ILog GetLogger(string name)
        {
            return new DummyLog();
        }

        private class DummyLog : ILog
        {
            public void Info(string msg) { }
            public void Warn(string msg) { }
            public void Error(string msg) { }
        }
    }
}
