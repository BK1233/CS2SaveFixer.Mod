using System;
using CS2SaveFixer.Mod;
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
        }
    }
}
