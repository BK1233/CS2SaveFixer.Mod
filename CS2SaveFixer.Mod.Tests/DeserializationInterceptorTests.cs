using Xunit;

namespace CS2SaveFixer.Mod.Tests
{
    public class DeserializationInterceptorTests
    {
        [Theory]
        [InlineData("BuildingPrefab", "BaseGame.DefaultBuilding")]
        [InlineData("NetPrefab", "BaseGame.TwoLaneRoad")]
        [InlineData("VehiclePrefab", "BaseGame.SedanVehicle")]
        public void ResolveMissingPrefabGuid_KnownPrefabTypes_ReturnsExpectedFallback(string prefabType, string expectedFallback)
        {
            // Arrange
            string originalGuid = "test-guid-12345";

            // Act
            string result = DeserializationInterceptor.ResolveMissingPrefabGuid(originalGuid, prefabType);

            // Assert
            Assert.Equal(expectedFallback, result);
        }

        [Theory]
        [InlineData("UnknownType")]
        [InlineData("CustomPrefab")]
        [InlineData("")]
        [InlineData(null)]
        public void ResolveMissingPrefabGuid_UnknownOrNullPrefabType_ReturnsGenericObjectFallback(string? prefabType)
        {
            // Arrange
            string originalGuid = "test-guid-67890";

            // Act
            string result = DeserializationInterceptor.ResolveMissingPrefabGuid(originalGuid, prefabType!);

            // Assert
            Assert.Equal("BaseGame.GenericObject", result);
        }

        [Theory]
        [InlineData("guid-1", "BuildingPrefab", "BaseGame.DefaultBuilding")]
        [InlineData("", "NetPrefab", "BaseGame.TwoLaneRoad")]
        [InlineData(null, "VehiclePrefab", "BaseGame.SedanVehicle")]
        public void ResolveMissingPrefabGuid_VariousOriginalGuids_ResolvesCorrectly(string? originalGuid, string prefabType, string expectedFallback)
        {
            // Act
            string result = DeserializationInterceptor.ResolveMissingPrefabGuid(originalGuid!, prefabType);

            // Assert
            Assert.Equal(expectedFallback, result);
        }
    }
}
