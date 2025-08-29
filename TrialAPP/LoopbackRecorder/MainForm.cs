using LoopbackRecorderWinForms.Interface;
using NAudio.Wave;
using System;
using System.IO;
using System.Windows.Forms;

namespace LoopbackRecorderWinForms
{
    public partial class MainForm : Form
    {
        private readonly ISpeechAiRep _speech;
        private WasapiLoopbackCapture? _capture;
        private WaveFileWriter? _writer;
        private string? _currentOutputPath;
        private string? _selectedInputPath;

        public MainForm(ISpeechAiRep speech)
        {
            InitializeComponent();
            _speech = speech;

            btnStart.Enabled = true;
            btnStop.Enabled = false;

            btnOpenFile.Enabled = true;
            btnTranscribeFile.Enabled = false;

            lblStatus.Text = "Status: Ready (no recording yet)";
            lblSavedPath.Text = "Last saved: (none)";
            txtTranscript.ReadOnly = true;
            txtTranscript.Clear();
        }

        private void btnStart_Click(object? sender, EventArgs e)
        {
            try
            {
                _capture = new WasapiLoopbackCapture();

                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var fileName = $"Loopback_{DateTime.Now:yyyyMMdd_HHmmss}.wav";
                _currentOutputPath = Path.Combine(documents, fileName);

                _writer = new WaveFileWriter(_currentOutputPath, _capture.WaveFormat);

                _capture.DataAvailable += Capture_DataAvailable;
                _capture.RecordingStopped += Capture_RecordingStopped;

                _capture.StartRecording();
                lblStatus.Text = "Status: Recording… (system audio)";
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start recording.\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cleanup();
                lblStatus.Text = "Status: Error while starting recording.";
            }
        }

        private void btnStop_Click(object? sender, EventArgs e)
        {
            try
            {
                btnStop.Enabled = false;
                _capture?.StopRecording();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to stop recording.\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cleanup();
                lblStatus.Text = "Status: Error while stopping recording.";
                btnStart.Enabled = true;
            }
        }

        private void Capture_DataAvailable(object? sender, WaveInEventArgs e)
        {
            try
            {
                _writer?.Write(e.Buffer, 0, e.BytesRecorded);
                _writer?.Flush();
            }
            catch
            {
                try { _capture?.StopRecording(); } catch { }
            }
        }

        private void Capture_RecordingStopped(object? sender, StoppedEventArgs e)
        {
            BeginInvoke(new Action(async () =>
            {
                if (e.Exception != null)
                {
                    MessageBox.Show($"Recording stopped with an error:\n{e.Exception.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Cleanup();

                lblStatus.Text = "Status: Stopped.";
                lblSavedPath.Text = _currentOutputPath != null
                    ? $"Last saved: {_currentOutputPath}"
                    : "Last saved: (unknown)";

                btnStart.Enabled = true;
                btnStop.Enabled = false;

                try
                {
                    if (!string.IsNullOrWhiteSpace(_currentOutputPath) && File.Exists(_currentOutputPath))
                    {
                        lblStatus.Text = "Status: Transcribing…";

                        string text = await _speech.ConvertAudioToTextAsync(_currentOutputPath);

                        txtTranscript.Text = text;

                        lblStatus.Text = "Status: Ready.";
                    }
                }
                catch (Exception ex2)
                {
                    MessageBox.Show($"Transcription error:\n{ex2.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatus.Text = "Status: Error during transcription.";
                }
            }));
        }

        private void Cleanup()
        {
            try { _capture?.Dispose(); } catch { } finally { _capture = null; }
            try { _writer?.Dispose(); } catch { } finally { _writer = null; }
        }

        private async void btnTranscribeFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedInputPath) || !File.Exists(_selectedInputPath))
            {
                MessageBox.Show("Please choose an audio file first.", "Info",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                lblStatus.Text = "Status: Preparing audio…";
                btnTranscribeFile.Enabled = false;

                // لو الملف WAV نرسله كما هو؛ غير كده نحوّله لـ WAV PCM 16k Mono
                var ext = Path.GetExtension(_selectedInputPath).ToLowerInvariant();
                string wavPath = ext == ".wav" ? _selectedInputPath : ConvertToPcm16kMono(_selectedInputPath);

                lblStatus.Text = "Status: Transcribing…";
                string text = await _speech.ConvertAudioToTextAsync(wavPath);

                txtTranscript.Text = text;
                lblStatus.Text = "Status: Ready.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Transcription error:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Status: Error during transcription.";
            }
            finally
            {
                btnTranscribeFile.Enabled = true;
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Select audio file";
            openFileDialog1.Filter =
                "Audio Files|*.wav;*.mp3;*.m4a;*.wma;*.aac;*.flac;*.ogg|WAV|*.wav|MP3|*.mp3|All files|*.*";
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                _selectedInputPath = openFileDialog1.FileName;
                lblSavedPath.Text = $"Selected file: {_selectedInputPath}";
                lblStatus.Text = "Status: Ready to transcribe selected file.";
                btnTranscribeFile.Enabled = true;
                txtTranscript.Clear();
            }
        }

        private string ConvertToPcm16kMono(string inputPath)
        {
            var dir = Path.GetDirectoryName(inputPath)!;
            var name = Path.GetFileNameWithoutExtension(inputPath);
            var outPath = Path.Combine(dir, $"{name}_pcm16k_mono.wav");

            using var reader = new NAudio.Wave.AudioFileReader(inputPath);
            ISampleProvider mono = reader.WaveFormat.Channels == 1
                ? reader
                : new NAudio.Wave.SampleProviders.StereoToMonoSampleProvider(reader)
                { LeftVolume = 0.5f, RightVolume = 0.5f };

            var resampled = new NAudio.Wave.SampleProviders.WdlResamplingSampleProvider(mono, 16000);

            var pcm16 = new NAudio.Wave.SampleProviders.SampleToWaveProvider16(resampled);

            WaveFileWriter.CreateWaveFile(outPath, pcm16);
            return outPath;
        }

    }
}
