using LMS.Application.Abstractions.Common;

namespace LMS.Infrastructure.Helpers.Common
{
    public class ConverterHelper : IConverterHelper
    {
        public ConverterHelper()
        {
            
        }

        public double[] ConvertBytesToDoubleArray(byte[] bytes)
        {
            var doubles = new double[bytes.Length / sizeof(double)];
            Buffer.BlockCopy(bytes, 0, doubles, 0, bytes.Length);
            return doubles;
        }


        public byte[] ConvertDoubleArrayToBytes(double[] doubles)
        {
            var bytes = new byte[doubles.Length * sizeof(double)];
            Buffer.BlockCopy(doubles, 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
