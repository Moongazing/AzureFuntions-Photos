using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TAO.Photos.AnalyzerService.Abstractions;

namespace TAO.Photos.AnalyzerService
{
    public class ComputerVisionAnalyzerService : IAnalyzerService
    {
        private readonly ComputerVisionClient client;
        public ComputerVisionAnalyzerService(IConfiguration configuration)
        {
            var visionKey = configuration["VisionKey"];
            var visionEndPoint = configuration["VisionEndPoint"];
            client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(visionKey))
            {
                Endpoint = visionEndPoint
            };
        }
        public Task<dynamic> AnalyzeAsync(byte[] image)
        {
            throw new NotImplementedException();
        }
    }
}
