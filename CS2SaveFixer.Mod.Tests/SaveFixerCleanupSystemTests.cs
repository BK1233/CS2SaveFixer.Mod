using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace CS2SaveFixer.Mod.Tests
{
    public class SaveFixerCleanupSystemTests
    {
        [Fact]
        public void Initialize_ShouldNotThrowException()
        {
            // Act & Assert
            var exception = Record.Exception(() => SaveFixerCleanupSystem.Initialize());
            Assert.Null(exception);
        }

        [Fact]
        public void Initialize_ShouldPrintExpectedLogMessageToConsole()
        {
            // Arrange
            var originalOut = Console.Out;
            using var stringWriter = new StringWriter();
            try
            {
                Console.SetOut(stringWriter);

                // Act
                SaveFixerCleanupSystem.Initialize();

                // Assert
                string output = stringWriter.ToString();
                Assert.Contains("[CS2SaveFixer] SaveFixerCleanupSystem registered for ECS world updates.", output);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Fact]
        public void RunSanitizePass_WithValidEntities_ShouldNotModifyEntitiesAndReturnZero()
        {
            // Arrange
            var entities = new List<SimulatedEntity>
            {
                new SimulatedEntity { Id = 1, PositionX = 10f, PositionY = 20f, PositionZ = 30f, HasValidPrefab = true, IsOrphaned = false },
                new SimulatedEntity { Id = 2, PositionX = -5f, PositionY = 0f, PositionZ = 15f, HasValidPrefab = true, IsOrphaned = false }
            };

            // Act
            int fixedCount = SaveFixerCleanupSystem.RunSanitizePass(entities);

            // Assert
            Assert.Equal(0, fixedCount);
            Assert.Equal(2, entities.Count);
            Assert.Equal(10f, entities[0].PositionX);
            Assert.Equal(-5f, entities[1].PositionX);
        }

        [Fact]
        public void RunSanitizePass_WithNaNOrInfinityPositions_ShouldSanitizeToZeroAndReturnFixedCount()
        {
            // Arrange
            var entities = new List<SimulatedEntity>
            {
                new SimulatedEntity { Id = 1, PositionX = float.NaN, PositionY = 10f, PositionZ = 20f, HasValidPrefab = true, IsOrphaned = false },
                new SimulatedEntity { Id = 2, PositionX = 0f, PositionY = float.PositiveInfinity, PositionZ = 0f, HasValidPrefab = true, IsOrphaned = false }
            };

            // Act
            int fixedCount = SaveFixerCleanupSystem.RunSanitizePass(entities);

            // Assert
            Assert.Equal(2, fixedCount);
            Assert.Equal(2, entities.Count);
            Assert.Equal(0f, entities[0].PositionX);
            Assert.Equal(0f, entities[0].PositionY);
            Assert.Equal(0f, entities[0].PositionZ);
            Assert.Equal(0f, entities[1].PositionY);
        }

        [Fact]
        public void RunSanitizePass_WithOrphanedOrInvalidPrefabEntities_ShouldRemoveEntitiesAndReturnFixedCount()
        {
            // Arrange
            var entities = new List<SimulatedEntity>
            {
                new SimulatedEntity { Id = 1, PositionX = 1f, PositionY = 1f, PositionZ = 1f, HasValidPrefab = true, IsOrphaned = true },
                new SimulatedEntity { Id = 2, PositionX = 2f, PositionY = 2f, PositionZ = 2f, HasValidPrefab = false, IsOrphaned = false },
                new SimulatedEntity { Id = 3, PositionX = 3f, PositionY = 3f, PositionZ = 3f, HasValidPrefab = true, IsOrphaned = false }
            };

            // Act
            int fixedCount = SaveFixerCleanupSystem.RunSanitizePass(entities);

            // Assert
            Assert.Equal(2, fixedCount);
            Assert.Single(entities);
            Assert.Equal(3, entities[0].Id);
        }
    }
}
