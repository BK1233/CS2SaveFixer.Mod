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

        public void OnLoad(UpdateSystem updateSystem)
        {
            Instance = this;
            Console.WriteLine("[CS2SaveFixer] CS2 Save Fixer Runtime Mod loaded successfully.");
            InitializeSystems();
        }

        public void OnDispose()
        {
            Console.WriteLine("[CS2SaveFixer] CS2 Save Fixer Runtime Mod unloaded.");
        }

        private void InitializeSystems()
        {
            DeserializationInterceptor.InstallPatches();
            SaveFixerCleanupSystem.Initialize();
        }
    }
}
