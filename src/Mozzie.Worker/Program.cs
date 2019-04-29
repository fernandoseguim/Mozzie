using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Mozzie.Domain;
using Mozzie.Domain.Commands;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mozzie.Worker
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            const string AWS_ACCESS_KEY_ID = "AKIAVSTVILGI5UEI4K44";
            const string AWS_SECRET_ACCESS_KEY = "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";

            Console.WriteLine("Hello World!");

            var self = await File.ReadAllBytesAsync("assets\\self.jpg");
            var front = await File.ReadAllBytesAsync("assets\\front.png");
            var back = await File.ReadAllBytesAsync("assets\\back.png");

            var command = new AnalizeDocumentCommand { Self = self, Back = back, Front = front };

            var region = RegionEndpoint.USEast1;
            var client = new AmazonRekognitionClient(AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY, region);

            #region Analiza se é documento
            using (var stream = new MemoryStream(command.Back))
            {
                var request = new DetectLabelsRequest { Image = new Image{ Bytes = stream } };
                
                var response = await client.DetectLabelsAsync(request);

                var labels = response.Labels;

                foreach (var label in labels)
                {
                    var accuracy = Accuracy.GetAccuracy(label.Confidence);

                    if (DocumentTypes.IsValidDocument(label.Name))
                    {
                        if (accuracy.IsLow) Console.WriteLine("Não é um documento");
                        if (accuracy.IsMedium) Console.WriteLine("Pode ser que seja um documento");
                        if (accuracy.IsHigh) Console.WriteLine("É muito provável que seja um documento");

                        break;
                    }
                }
            }
            #endregion

            #region Compara com a self
            using (var source = new MemoryStream(command.Self))
            using (var target = new MemoryStream(command.Front))
            {
                var request = new CompareFacesRequest{ SourceImage = new Image { Bytes = source }, TargetImage = new Image { Bytes = target } };
                
                var response = await client.CompareFacesAsync(request);

                var faces = response.FaceMatches;

                if (faces.Count != 1) { Console.WriteLine("Resultado inconsistente"); }

                var accuracy = Accuracy.GetAccuracy(faces.First().Similarity);
                
                if (accuracy.IsLow) Console.WriteLine("Esse documento não da mesma pessoa");
                if (accuracy.IsMedium) Console.WriteLine("Pode ser que este documento seja da mesma pessoa");
                if (accuracy.IsHigh) Console.WriteLine("É muito provável que este documento seja da mesma pessoa");
            }
            #endregion

            #region Verifica se é do portador válido
            using (var stream = new MemoryStream(command.Back))
            {
                var request = new DetectTextRequest { Image = new Image { Bytes = stream } };

                var response = await client.DetectTextAsync(request);

                var texts = response.TextDetections;

                foreach (var text in texts)
                {
                    var accuracy = Accuracy.GetAccuracy(text.Confidence);

                    if ("CPF".Equals(text.DetectedText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (accuracy.IsLow) Console.WriteLine("não contém um número de CPF");
                        if (accuracy.IsMedium) Console.WriteLine("Pode ser que contenha um número de CPF");
                        if (accuracy.IsHigh) Console.WriteLine("É muito provável que contenha um número de CPF");

                        break;
                    }
                }
            }
            #endregion

            Console.WriteLine("That's all folks!");
        }
    }
}
