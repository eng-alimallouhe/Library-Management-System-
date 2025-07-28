namespace LMS.Application.Abstractions.Authentication
{
    public interface IFaceRecognitionHelper
    {
        double[]? ExtractFaceEncoding(byte[] faceImageBytes);


        bool IsFaceMatching(byte[] inputFaceBytes, double[] storedEncoding, double tolerance = 0.6);
    }
}
