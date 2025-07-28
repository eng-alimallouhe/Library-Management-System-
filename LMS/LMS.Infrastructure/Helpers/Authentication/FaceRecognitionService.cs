using System.Drawing;
using FaceRecognitionDotNet;
using LMS.Application.Abstractions.Authentication;

namespace LMS.Infrastructure.Helpers;

public class FaceRecognitionHelper : IFaceRecognitionHelper
{
    private readonly FaceRecognition _faceRecognition;

    public FaceRecognitionHelper()
    {
        var modelsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AIModels");

        var faceModelPath = Path.Combine(modelsDirectory, "dlib_face_recognition_resnet_model_v1.dat");
        var landmarksModelPath = Path.Combine(modelsDirectory, "shape_predictor_68_face_landmarks.dat");

        _faceRecognition = FaceRecognition.Create(modelsDirectory);
    }

    public double[]? ExtractFaceEncoding(byte[] imageBytes)
    {
        using var bitmap = new Bitmap(new MemoryStream(imageBytes));
        var image = FaceRecognition.LoadImage(bitmap);

        var encodings = _faceRecognition.FaceEncodings(image).ToList();

        if (encodings.Count == 0)
            return null;

        return encodings.First().GetRawEncoding();
    }

    public bool IsFaceMatching(byte[] inputFaceBytes, double[] storedEncoding, double tolerance = 0.6)
    {
        using var bitmap = new Bitmap(new MemoryStream(inputFaceBytes));
        var image = FaceRecognition.LoadImage(bitmap);

        var inputEncodings = _faceRecognition.FaceEncodings(image).ToList();

        if (inputEncodings.Count == 0)
            return false;

        var inputEncoding = inputEncodings.First();

        var storedFaceEncoding = FaceRecognition.LoadFaceEncoding(storedEncoding); 

        var result = FaceRecognition.CompareFaces(
            new[] { storedFaceEncoding }, inputEncoding, tolerance);

        return result.FirstOrDefault();
    }
}