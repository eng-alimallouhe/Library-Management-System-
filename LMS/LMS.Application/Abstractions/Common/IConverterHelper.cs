namespace LMS.Application.Abstractions.Common
{
    public interface IConverterHelper
    {
        double[] ConvertBytesToDoubleArray(byte[] bytes);
        byte[] ConvertDoubleArrayToBytes(double[] floats);
    }
}