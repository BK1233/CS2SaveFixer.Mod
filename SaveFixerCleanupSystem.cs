using System;
using System.Collections.Generic;
using System.Linq;

namespace CS2SaveFixer.Mod
{
    public class SimulatedEntity
    {
        public int Id { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public bool HasValidPrefab { get; set; } = true;
        public bool IsOrphaned { get; set; } = false;
    }

    public class SaveFixerCleanupSystem
    {
        private static readonly Colossal.Logging.ILog log = Colossal.Logging.LogManager.GetLogger($"{nameof(CS2SaveFixer.Mod)}.{nameof(SaveFixerCleanupSystem)}");

        public static void Initialize()
        {
            Console.WriteLine("[CS2SaveFixer] SaveFixerCleanupSystem registered for ECS world updates.");
            log.Info("SaveFixerCleanupSystem registered for ECS world updates.");
        }

        public static int RunSanitizePass(List<SimulatedEntity> entities)
        {
            int fixedCount = 0;
            int writeIndex = 0;

            for (int readIndex = 0; readIndex < entities.Count; readIndex++)
            {
                var entity = entities[readIndex];

                // 1. Sanitize float coordinates (NaN / Inf)
                if (float.IsNaN(entity.PositionX) || float.IsInfinity(entity.PositionX) ||
                    float.IsNaN(entity.PositionY) || float.IsInfinity(entity.PositionY) ||
                    float.IsNaN(entity.PositionZ) || float.IsInfinity(entity.PositionZ))
                {
                    entity.PositionX = 0f;
                    entity.PositionY = 0f;
                    entity.PositionZ = 0f;
                    fixedCount++;
                    log.Info($"Sanitized invalid position vector on Entity ID {entity.Id}.");
                }

                // 2. Remove invalid/orphaned simulation entities
                if (entity.IsOrphaned || !entity.HasValidPrefab)
                {
                    fixedCount++;
                    log.Info($"Purged orphaned entity ID {entity.Id}.");
                }
                else
                {
                    if (writeIndex != readIndex)
                    {
                        entities[writeIndex] = entity;
                    }
                    writeIndex++;
                }
            }

            if (writeIndex < entities.Count)
            {
                entities.RemoveRange(writeIndex, entities.Count - writeIndex);
            }

            log.Info($"Sanitize pass complete. Fixed/cleaned {fixedCount} entity issues.");
            return fixedCount;
        }
    }
}
