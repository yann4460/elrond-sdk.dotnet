using System.IO;
using Erdcsharp.Domain.Helper;

namespace Erdcsharp.Domain.SmartContracts
{
    public class CodeArtifact
    {
        public string Value { get; }

        public CodeArtifact(byte[] bytes)
        {
            Value = Converter.ToHexString(bytes);
        }

        public static CodeArtifact FromFilePath(string filePath)
        {
            var fileBytes = File.ReadAllBytes(filePath);
            return new CodeArtifact(fileBytes);
        }
    }
}
