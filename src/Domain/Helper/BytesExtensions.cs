namespace Erdcsharp.Domain.Helper
{
    public static class BytesExtensions
    {
        public static byte[] Slice(this byte[] source, int start, int? optEnd = null)
        {
            var end = optEnd.GetValueOrDefault(source.Length);
            var len = end - start;

            // Return new array.
            var res                              = new byte[len];
            for (var i = 0; i < len; i++) res[i] = source[i + start];
            return res;
        }
    }
}
