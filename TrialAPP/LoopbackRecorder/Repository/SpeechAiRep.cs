using LoopbackRecorderWinForms.Interface;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Transcription;
using NAudio.Wave;
using System.Text;

namespace LoopbackRecorderWinForms.Helper
{
    public class SpeechAiRep : ISpeechAiRep
    {
        private readonly string _subscriptionKey;
        private readonly string _region;

        public SpeechAiRep()
        {
            _subscriptionKey = "9y16VExibb2UcLVqIRAyX3WUTviHjYtZYSmzMzhqoUKhYgoeh3dhJQQJ99BHACYeBjFXJ3w3AAAYACOG9XUD"; // Replace with your Azure Speech service subscription key
            _region = "eastus"; // Replace with your Azure service region (e.g., "westus")
        }



        public async Task<string> ConvertAudioToTextAsync(string audioFilePath)
        {
            var config = SpeechConfig.FromSubscription(_subscriptionKey, _region);
            // اضبط اللغة حسب الملف
            // config.SpeechRecognitionLanguage = "ar-EG"; // أو "en-US"
            config.OutputFormat = OutputFormat.Detailed;
            config.SetProperty("SpeechServiceResponse_RequestWordLevelTimestamps", "true");

            var transcript = new StringBuilder();


            var fmt = AudioStreamFormat.GetWaveFormatPCM(16000, 16, 1);

            using var pushStream = AudioInputStream.CreatePushStream(fmt);

            using var audioConfig = AudioConfig.FromStreamInput(pushStream);

            using var transcriber = new ConversationTranscriber(config, audioConfig);

            var doneTcs = new TaskCompletionSource<bool>();

            string lastSpeaker = null;

            transcriber.Transcribed += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech && !string.IsNullOrWhiteSpace(e.Result.Text))
                {
                    var speaker = string.IsNullOrWhiteSpace(e.Result.SpeakerId) ? "Speaker" : e.Result.SpeakerId;

                    if (lastSpeaker == null || lastSpeaker != speaker)
                    {
                        // متحدث جديد أو أول مرة
                        transcript.AppendLine($"{speaker}: {e.Result.Text}");
                        lastSpeaker = speaker;
                    }
                    else
                    {
                        // نفس المتحدث السابق → فقط النص
                        transcript.AppendLine($"{e.Result.Text}");
                    }
                }
            };

            transcriber.Canceled += (s, e) =>
            {
                doneTcs.TrySetResult(true);
            };

            transcriber.SessionStopped += (s, e) =>
            {
                doneTcs.TrySetResult(true);
            };


            try
            {
                await transcriber.StartTranscribingAsync().ConfigureAwait(false);

                var preRollSilence = new byte[16000];
                pushStream.Write(preRollSilence);

                using (var reader = new AudioFileReader(audioFilePath))
                {
                    ISampleProvider mono = reader.WaveFormat.Channels == 1
                        ? reader
                        : new NAudio.Wave.SampleProviders.StereoToMonoSampleProvider(reader)
                        { LeftVolume = 0.5f, RightVolume = 0.5f };

                    var resampled = new NAudio.Wave.SampleProviders.WdlResamplingSampleProvider(mono, 16000);

                    var pcm16 = new NAudio.Wave.SampleProviders.SampleToWaveProvider16(resampled);

                    var buffer = new byte[4096];
                    int read;
                    while ((read = pcm16.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        pushStream.Write(buffer.AsSpan(0, read).ToArray());
                    }
                }

                pushStream.Close();

                await doneTcs.Task.ConfigureAwait(false);
                await transcriber.StopTranscribingAsync().ConfigureAwait(false);

                return transcript.ToString().Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during transcription: {ex.Message}");
                return string.Empty;
            }
        }


        //public async Task<string> ConvertAudioToTextAsync(string audioFilePath)
        //{
        //    // Use Azure Speech SDK with your subscription and region
        //    var config = SpeechConfig.FromSubscription(_subscriptionKey, _region);

        //    // Set recognition language (change if needed, e.g., "en-US")
        //    //config.SpeechRecognitionLanguage = "en-US";

        //    // Optional: ask for detailed output (confidence, words)
        //    config.OutputFormat = OutputFormat.Detailed;

        //    // Optional: word-level timestamps
        //    config.SetProperty("SpeechServiceResponse_RequestWordLevelTimestamps", "true");

        //    var recognizedText = new StringBuilder();

        //    // Create audio config directly from the WAV file (no splitting)
        //    using var audioConfig = AudioConfig.FromWavFileInput(audioFilePath);

        //    // Create a continuous recognizer for long files
        //    using var recognizer = new SpeechRecognizer(config, audioConfig);

        //    // We'll complete this task when the session stops or gets canceled
        //    var doneTcs = new TaskCompletionSource<bool>();

        //    // Append recognized text chunks as they arrive
        //    recognizer.Recognized += (s, e) =>
        //    {
        //        if (e.Result.Reason == ResultReason.RecognizedSpeech &&
        //            !string.IsNullOrWhiteSpace(e.Result.Text))
        //        {
        //            recognizedText.AppendLine(e.Result.Text);
        //        }
        //    };

        //    // Log intermediate results (optional)
        //    recognizer.Recognizing += (s, e) =>
        //    {
        //        Console.WriteLine($"... {e.Result.Text}");
        //    };

        //    // Stop condition: file fully consumed
        //    recognizer.SessionStopped += (s, e) =>
        //    {
        //        doneTcs.TrySetResult(true);
        //    };

        //    // Handle cancellations and errors
        //    recognizer.Canceled += (s, e) =>
        //    {
        //        // You can inspect e.ErrorCode / e.ErrorDetails for diagnostics
        //        doneTcs.TrySetResult(true);
        //    };

        //    try
        //    {
        //        // Start continuous recognition (processes whole WAV)
        //        await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

        //        // Wait until session finishes (or gets canceled)
        //        await doneTcs.Task.ConfigureAwait(false);

        //        // Stop and flush
        //        await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);

        //        // Return the full transcript
        //        return recognizedText.ToString().Trim();
        //    }
        //    catch (Exception ex)
        //    {
        //        // Basic error logging
        //        Console.WriteLine($"Error during transcription: {ex.Message}");
        //        return string.Empty;
        //    }
        //}
    }
}
