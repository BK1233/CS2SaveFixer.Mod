using System;
using System.Collections.Generic;
using Colossal.Logging;

namespace CS2SaveFixer.Mod
{
    public class DeserializationInterceptor
    {
        private static readonly ILog log = LogManager.GetLogger($"{nameof(CS2SaveFixer.Mod)}.{nameof(DeserializationInterceptor)}");
        private static bool _patchesInstalled = false;
        private static readonly Dictionary<string, string> PrefabFallbackTable = new();

        public static void InstallPatches()
        {
            if (_patchesInstalled) return;

            log.Info("Installing Harmony/Deserialization exception handling hooks...");
            _patchesInstalled = true;
        }

        public static string ResolveMissingPrefabGuid(string originalGuid, string prefabType)
        {
            log.Info($"Intercepted missing prefab GUID '{originalGuid}' (Type: {prefabType}).");

            if (originalGuid != null && PrefabFallbackTable.TryGetValue(originalGuid, out var fallback))
            {
                return fallback;
            }

            // Return a safe base game fallback identifier depending on category
            string safeDefault = prefabType switch
            {
                "BuildingPrefab" => "BaseGame.DefaultBuilding",
                "NetPrefab" => "BaseGame.TwoLaneRoad",
                "VehiclePrefab" => "BaseGame.SedanVehicle",
                _ => "BaseGame.GenericObject"
            };

            log.Info($"Remapped missing prefab '{originalGuid}' -> '{safeDefault}'.");
            return safeDefault;
        }

        public static bool TryDeserializeComponentSafe(Action deserializeAction, string componentTypeName)
        {
            try
            {
                deserializeAction();
                return true;
            }
            catch (Exception ex)
            {
                log.Warn($"Caught deserialization exception on component '{componentTypeName}': {ex.Message}");
                log.Warn("Skipping corrupted component chunk to preserve world loading state.");
                return false;
            }
        }
    }
}
