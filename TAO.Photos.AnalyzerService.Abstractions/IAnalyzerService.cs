using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO.Photos.AnalyzerService.Abstractions
{
    public interface IAnalyzerService
    {
        Task<dynamic> AnalyzeAsync(byte[] image);
    }
}
