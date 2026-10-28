using System;
using Game;
using Game.Modding;

namespace CS2SaveFixer.Mod
{
    public class SaveFixerMod : Game.Modding.IMod
    {
        public static SaveFixerMod Instance { get; private set; } = null!;
        public bool IsFallbackPrefabResolverEnabled { get; set; } = true;
        public bool IsAutomaticCleanupSystemEnabled { get; set; } = true;

        private static readonly Colossal.Logging.ILog log = Colossal.Logging.LogManager.GetLogger($"{nameof(CS2SaveFixer.Mod)}.{nameof(SaveFixerMod)}");

        public void OnLoad(UpdateSystem updateSystem)
        {
            Instance = this;
            log.Info("[CS2SaveFixer] CS2 Save Fixer Runtime Mod loaded successfully.");
            InitializeSystems();
        }

        public void OnDispose()
        {
            log.Info("[CS2SaveFixer] CS2 Save Fixer Runtime Mod unloaded.");
        }

        private void InitializeSystems()
        {
            DeserializationInterceptor.InstallPatches();
            SaveFixerCleanupSystem.Initialize();
        }
    }
}
