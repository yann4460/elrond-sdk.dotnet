using System.Security.Cryptography;

namespace Erdcsharp.Cryptography
{
    public sealed class AesCtr
    {
        public static byte[] Encrypt(byte[] psk, byte[] iv, byte[] inData)
        {
            var aesObj = Aes.Create();
            aesObj.Mode    = CipherMode.ECB;
            aesObj.Padding = PaddingMode.None;
            var zeroIv  = new byte[16];
            var encrypt = aesObj.CreateEncryptor(psk, zeroIv);
            var counter = new byte[16];
            for (var i = 0; i < 16; i++)
            {
                counter[i] = iv[i];
            }

            var ctrOut = new byte[16];
            var output = new byte[inData.Length];
            var pos    = 0;
            while (true)
            {
                if (pos >= inData.Length)
                {
                    break;
                }

                encrypt.TransformBlock(counter, 0, 16, ctrOut, 0);
                for (var i = 0; i < 16; i++)
                {
                    if (pos >= inData.Length)
                    {
                        break;
                    }

                    output[pos] = (byte)(inData[pos] ^ ctrOut[i]);
                    pos++;
                }

                // increment counter
                for (var i = 15; i >= 0; i--)
                {
                    counter[i]++;
                    if (counter[i] != 0)
                    {
                        break;
                    }
                }
            }

            return output;
        }
    }
}
