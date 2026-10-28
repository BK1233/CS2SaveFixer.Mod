using System;
using Game;
using Xunit;

namespace CS2SaveFixer.Mod.Tests
{
    public class SaveFixerModTests
    {
        [Fact]
        public void OnLoad_SetsInstance_AndInitializesSystems()
        {
            // Arrange
            var mod = new SaveFixerMod();
            var updateSystem = new UpdateSystem();

            // Act
            mod.OnLoad(updateSystem);

            // Assert
            Assert.Same(mod, SaveFixerMod.Instance);
            Assert.True(mod.IsFallbackPrefabResolverEnabled);
            Assert.True(mod.IsAutomaticCleanupSystemEnabled);
        }

        [Fact]
        public void OnLoad_WhenCalledMultipleTimes_UpdatesInstance()
        {
            // Arrange
            var firstMod = new SaveFixerMod();
            var secondMod = new SaveFixerMod();
            var updateSystem = new UpdateSystem();

            // Act
            firstMod.OnLoad(updateSystem);
            Assert.Same(firstMod, SaveFixerMod.Instance);

            secondMod.OnLoad(updateSystem);

            // Assert
            Assert.Same(secondMod, SaveFixerMod.Instance);
        }

        [Fact]
        public void OnLoad_HandlesNullUpdateSystem()
        {
            // Arrange
            var mod = new SaveFixerMod();

            // Act
            mod.OnLoad(null!);

            // Assert
            Assert.Same(mod, SaveFixerMod.Instance);
        }

        [Fact]
        public void OnDispose_RunsWithoutExceptions()
        {
            // Arrange
            var mod = new SaveFixerMod();

            // Act & Assert
            var exception = Record.Exception(() => mod.OnDispose());
            Assert.Null(exception);
        }
    }
}
