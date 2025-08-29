using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoopbackRecorderWinForms.Interface
{
    public interface ISpeechAiRep
    {
        Task<string> ConvertAudioToTextAsync(string audioFilePath);
    }
}
