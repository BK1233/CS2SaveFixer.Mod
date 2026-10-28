using System;
using System.Collections.Generic;
using CS2SaveFixer.Mod;
using Xunit;

namespace CS2SaveFixer.Tests
{
    public class SaveFixerCleanupSystemTests
    {
        [Fact]
        public void RunSanitizePass_WithValidEntities_ReturnsZeroAndDoesNotModifyEntities()
        {
            // Arrange
            var entities = new List<SimulatedEntity>
            {
                new SimulatedEntity { Id = 1, PositionX = 10f, PositionY = 20f, PositionZ = 30f, HasValidPrefab = true, IsOrphaned = false },
                new SimulatedEntity { Id = 2, PositionX = -5.5f, PositionY = 0f, PositionZ = 100f, HasValidPrefab = true, IsOrphaned = false }
            };

            // Act
            int fixedCount = SaveFixerCleanupSystem.RunSanitizePass(entities);

            // Assert
            Assert.Equal(0, fixedCount);
            Assert.Equal(2, entities.Count);
            Assert.Equal(10f, entities[0].PositionX);
            Assert.Equal(20f, entities[0].PositionY);
            Assert.Equal(30f, entities[0].PositionZ);
            Assert.Equal(-5.5f, entities[1].PositionX);
            Assert.Equal(0f, entities[1].PositionY);
            Assert.Equal(100f, entities[1].PositionZ);
        }

        [Fact]
        public void RunSanitizePass_WithEmptyList_ReturnsZero()
        {
            // Arrange
            var entities = new List<SimulatedEntity>();

            // Act
            int fixedCount = SaveFixerCleanupSystem.RunSanitizePass(entities);

            // Assert
            Assert.Equal(0, fixedCount);
            Assert.Empty(entities);
        }

        [Theory]
        [InlineData(float.NaN, 0f, 0f)]
        [InlineData(0f, float.PositiveInfinity, 0f)]
        [InlineData(0f, 0f, float.NegativeInfinity)]
        [InlineData(float.NaN, float.NaN, float.NaN)]
        public void RunSanitizePass_WithInvalidPosition_ResetsPositionToZeroAndIncrementsFixedCount(float x, float y, float z)
        {
            // Arrange
            var entity = new SimulatedEntity
            {
                Id = 10,
                PositionX = x,
                PositionY = y,
                PositionZ = z,
                HasValidPrefab = true,
                IsOrphaned = false
            };
            var entities = new List<SimulatedEntity> { entity };

            // Act
            int fixedCount = SaveFixerCleanupSystem.RunSanitizePass(entities);

            // Assert
            Assert.Equal(1, fixedCount);
            Assert.Single(entities);
            Assert.Equal(0f, entity.PositionX);
            Assert.Equal(0f, entity.PositionY);
            Assert.Equal(0f, entity.PositionZ);
        }

        [Fact]
        public void RunSanitizePass_WithOrphanedEntity_RemovesEntityAndIncrementsFixedCount()
        {
            // Arrange
            var validEntity = new SimulatedEntity { Id = 1, PositionX = 1f, PositionY = 1f, PositionZ = 1f, HasValidPrefab = true, IsOrphaned = false };
            var orphanedEntity = new SimulatedEntity { Id = 2, PositionX = 1f, PositionY = 1f, PositionZ = 1f, HasValidPrefab = true, IsOrphaned = true };
            var entities = new List<SimulatedEntity> { validEntity, orphanedEntity };

            // Act
            int fixedCount = SaveFixerCleanupSystem.RunSanitizePass(entities);

            // Assert
            Assert.Equal(1, fixedCount);
            Assert.Single(entities);
            Assert.Equal(1, entities[0].Id);
        }

        [Fact]
        public void RunSanitizePass_WithInvalidPrefabEntity_RemovesEntityAndIncrementsFixedCount()
        {
            // Arrange
            var invalidPrefabEntity = new SimulatedEntity { Id = 3, PositionX = 1f, PositionY = 1f, PositionZ = 1f, HasValidPrefab = false, IsOrphaned = false };
            var entities = new List<SimulatedEntity> { invalidPrefabEntity };

            // Act
            int fixedCount = SaveFixerCleanupSystem.RunSanitizePass(entities);

            // Assert
            Assert.Equal(1, fixedCount);
            Assert.Empty(entities);
        }

        [Fact]
        public void RunSanitizePass_WithBothInvalidPositionAndOrphaned_SanitizesAndPurgesEntity()
        {
            // Arrange
            var badEntity = new SimulatedEntity
            {
                Id = 4,
                PositionX = float.NaN,
                PositionY = 0f,
                PositionZ = 0f,
                HasValidPrefab = true,
                IsOrphaned = true
            };
            var entities = new List<SimulatedEntity> { badEntity };

            // Act
            int fixedCount = SaveFixerCleanupSystem.RunSanitizePass(entities);

            // Assert
            Assert.Equal(2, fixedCount); // 1 for position reset + 1 for removal
            Assert.Empty(entities);
        }

        [Fact]
        public void RunSanitizePass_WithMixedEntities_ProcessesAllCorrectly()
        {
            // Arrange
            var e1 = new SimulatedEntity { Id = 1, PositionX = 10f, PositionY = 10f, PositionZ = 10f, HasValidPrefab = true, IsOrphaned = false };
            var e2 = new SimulatedEntity { Id = 2, PositionX = float.NaN, PositionY = 0f, PositionZ = 0f, HasValidPrefab = true, IsOrphaned = false };
            var e3 = new SimulatedEntity { Id = 3, PositionX = 5f, PositionY = 5f, PositionZ = 5f, HasValidPrefab = true, IsOrphaned = true };
            var e4 = new SimulatedEntity { Id = 4, PositionX = 0f, PositionY = float.PositiveInfinity, PositionZ = 0f, HasValidPrefab = false, IsOrphaned = false };

            var entities = new List<SimulatedEntity> { e1, e2, e3, e4 };

            // Act
            int fixedCount = SaveFixerCleanupSystem.RunSanitizePass(entities);

            // Assert
            // e1: valid (0 fixed)
            // e2: position reset to 0,0,0 (1 fixed)
            // e3: orphaned -> removed (1 fixed)
            // e4: pos reset + invalid prefab -> reset and removed (2 fixed)
            // Total fixedCount = 4
            Assert.Equal(4, fixedCount);
            Assert.Equal(2, entities.Count);
            Assert.Contains(e1, entities);
            Assert.Contains(e2, entities);
            Assert.Equal(0f, e2.PositionX);
        }

        [Fact]
        public void Initialize_RunsWithoutException()
        {
            // Act & Assert
            var exception = Record.Exception(() => SaveFixerCleanupSystem.Initialize());
            Assert.Null(exception);
        }
    }
}
