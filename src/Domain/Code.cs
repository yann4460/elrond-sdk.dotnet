using System;
using System.IO;
using System.Threading.Tasks;

namespace Elrond.Dotnet.Sdk.Domain
{
    public class Code
    {
        public string Value { get; }

        public Code(byte[] bytes)
        {
            Value = Convert.ToHexString(bytes);
        }

        public static async Task<Code> FromFilePath(string filePath)
        {
            var fileBytes = await File.ReadAllBytesAsync(filePath);
            return new Code(fileBytes);
        }
    }
}