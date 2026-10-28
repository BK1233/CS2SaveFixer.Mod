using System;
using CS2SaveFixer.Mod;
using System.IO;
using System.Reflection;
using Xunit;

namespace CS2SaveFixer.Mod.Tests
{
    public class DeserializationInterceptorTests
    {
        [Fact]
        public void TryDeserializeComponentSafe_WhenActionSucceeds_ExecutesActionAndReturnsTrue()
        {
            // Arrange
            bool actionExecuted = false;
            Action deserializeAction = () => { actionExecuted = true; };
            string componentTypeName = "TestComponent";

            // Act
            bool result = DeserializationInterceptor.TryDeserializeComponentSafe(deserializeAction, componentTypeName);

            // Assert
            Assert.True(result);
            Assert.True(actionExecuted);
        }

        [Fact]
        public void TryDeserializeComponentSafe_WhenActionThrowsException_CatchesExceptionAndReturnsFalse()
        {
            // Arrange
            bool actionStarted = false;
            Action deserializeAction = () =>
            {
                actionStarted = true;
                throw new InvalidOperationException("Corrupted component data");
            };
            string componentTypeName = "CorruptedComponent";

            // Act
            bool result = DeserializationInterceptor.TryDeserializeComponentSafe(deserializeAction, componentTypeName);

            // Assert
            Assert.False(result);
            Assert.True(actionStarted);
        }

        [Theory]
        [InlineData("System.NullReferenceException")]
        [InlineData("System.ArgumentException")]
        [InlineData("System.IndexOutOfRangeException")]
        public void TryDeserializeComponentSafe_WhenActionThrowsVariousExceptions_CatchesExceptionAndReturnsFalse(string exceptionType)
        {
            // Arrange
            Action deserializeAction = exceptionType switch
            {
                "System.NullReferenceException" => () => throw new NullReferenceException("Null ref"),
                "System.ArgumentException" => () => throw new ArgumentException("Invalid arg"),
                "System.IndexOutOfRangeException" => () => throw new IndexOutOfRangeException("Out of range"),
                _ => throw new Exception("General exception")
            };

            // Act
            bool result = DeserializationInterceptor.TryDeserializeComponentSafe(deserializeAction, "TestComponent");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryDeserializeComponentSafe_WhenActionIsNull_CatchesNullReferenceExceptionAndReturnsFalse()
        {
            // Arrange
            Action deserializeAction = null!;

            // Act
            bool result = DeserializationInterceptor.TryDeserializeComponentSafe(deserializeAction, "NullActionComponent");

            // Assert
            Assert.False(result);
        private static void ResetPatchesInstalledFlag()
        {
            var field = typeof(DeserializationInterceptor).GetField("_patchesInstalled", BindingFlags.NonPublic | BindingFlags.Static);
            field?.SetValue(null, false);
        }

        private static bool GetPatchesInstalledFlag()
        {
            var field = typeof(DeserializationInterceptor).GetField("_patchesInstalled", BindingFlags.NonPublic | BindingFlags.Static);
            return (bool)(field?.GetValue(null) ?? false);
        }

        [Fact]
        public void InstallPatches_SetsPatchesInstalledFlag_AndLogsToConsole()
        {
            // Arrange
            ResetPatchesInstalledFlag();
            using var sw = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);

            try
            {
                // Act
                DeserializationInterceptor.InstallPatches();

                // Assert
                Assert.True(GetPatchesInstalledFlag());
                string consoleOutput = sw.ToString();
                Assert.Contains("[CS2SaveFixer] Installing Harmony/Deserialization exception handling hooks...", consoleOutput);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Fact]
        public void InstallPatches_IsIdempotent_DoesNotReinstallOrLogTwice()
        {
            // Arrange
            ResetPatchesInstalledFlag();
            DeserializationInterceptor.InstallPatches();

            using var sw = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);

            try
            {
                // Act
                DeserializationInterceptor.InstallPatches();

                // Assert
                Assert.True(GetPatchesInstalledFlag());
                string consoleOutput = sw.ToString();
                Assert.Empty(consoleOutput);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Theory]
        [InlineData("BuildingPrefab", "BaseGame.DefaultBuilding")]
        [InlineData("NetPrefab", "BaseGame.TwoLaneRoad")]
        [InlineData("VehiclePrefab", "BaseGame.SedanVehicle")]
        [InlineData("OtherType", "BaseGame.GenericObject")]
        public void ResolveMissingPrefabGuid_ReturnsCorrectFallback(string prefabType, string expectedFallback)
        {
            // Arrange
            string originalGuid = "test-guid-123";

            // Act
            string actualFallback = DeserializationInterceptor.ResolveMissingPrefabGuid(originalGuid, prefabType);

            // Assert
            Assert.Equal(expectedFallback, actualFallback);
        }

        [Fact]
        public void TryDeserializeComponentSafe_SuccessfulExecution_ReturnsTrue()
        {
            // Arrange
            bool executed = false;
            Action action = () => executed = true;

            // Act
            bool result = DeserializationInterceptor.TryDeserializeComponentSafe(action, "TestComponent");

            // Assert
            Assert.True(result);
            Assert.True(executed);
        }

        [Fact]
        public void TryDeserializeComponentSafe_ExceptionThrown_CatchesExceptionAndReturnsFalse()
        {
            // Arrange
            Action action = () => throw new InvalidOperationException("Simulated deserialization error");

            using var sw = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);

            try
            {
                // Act
                bool result = DeserializationInterceptor.TryDeserializeComponentSafe(action, "TestComponent");

                // Assert
                Assert.False(result);
                string consoleOutput = sw.ToString();
                Assert.Contains("[CS2SaveFixer] Caught deserialization exception on component 'TestComponent': Simulated deserialization error", consoleOutput);
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }
    }
}
