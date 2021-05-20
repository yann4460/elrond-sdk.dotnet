using Elrond.Dotnet.Sdk.Domain.Codec;
using Elrond.Dotnet.Sdk.Domain.Values;
using NUnit.Framework;

namespace Elrond_sdk.dotnet.tests.Domain.Codec
{
    public class StructBinaryCodecTests
    {
        private StructBinaryCodec _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new StructBinaryCodec();
        }

        [Test]
        public void EncodeTopLevel()
        {
            // Arrange
            var fieldDefinition = new FieldDefinition("azeazeaze", "", TypeValue.RustTypes.u16);
            var structField = new StructField(NumericValue.U16Value(12), "azeazeaze");
            var type = TypeValue.StructValue("azeae", new[] {fieldDefinition});

            // Act
            var actual = _sut.EncodeNested(new StructValue(type, new[] {structField}));

            // Assert
        }
    }
}