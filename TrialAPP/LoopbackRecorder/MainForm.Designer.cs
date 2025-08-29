namespace LoopbackRecorderWinForms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblSavedPath;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            btnStart = new Button();
            btnStop = new Button();
            lblStatus = new Label();
            lblSavedPath = new Label();
            txtTranscript = new TextBox();
            btnTranscribeFile = new Button();
            btnOpenFile = new Button();
            openFileDialog1 = new OpenFileDialog();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(58, 66);
            btnStart.Margin = new Padding(7, 8, 7, 8);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(340, 109);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start Recording";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnStop
            // 
            btnStop.Enabled = false;
            btnStop.Location = new Point(437, 66);
            btnStop.Margin = new Padding(7, 8, 7, 8);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(340, 109);
            btnStop.TabIndex = 1;
            btnStop.Text = "Stop Recording";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(58, 362);
            lblStatus.Margin = new Padding(7, 0, 7, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(440, 41);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Status: Ready (no recording yet)";
            // 
            // lblSavedPath
            // 
            lblSavedPath.AutoEllipsis = true;
            lblSavedPath.Location = new Point(58, 433);
            lblSavedPath.Margin = new Padding(7, 0, 7, 0);
            lblSavedPath.Name = "lblSavedPath";
            lblSavedPath.Size = new Size(1117, 126);
            lblSavedPath.TabIndex = 3;
            lblSavedPath.Text = "Last saved: (none)";
            // 
            // txtTranscript
            // 
            txtTranscript.Dock = DockStyle.Bottom;
            txtTranscript.Location = new Point(0, 503);
            txtTranscript.Multiline = true;
            txtTranscript.Name = "txtTranscript";
            txtTranscript.ReadOnly = true;
            txtTranscript.ScrollBars = ScrollBars.Vertical;
            txtTranscript.Size = new Size(1287, 343);
            txtTranscript.TabIndex = 4;
            // 
            // btnTranscribeFile
            // 
            btnTranscribeFile.Enabled = false;
            btnTranscribeFile.Location = new Point(437, 214);
            btnTranscribeFile.Name = "btnTranscribeFile";
            btnTranscribeFile.Size = new Size(340, 109);
            btnTranscribeFile.TabIndex = 5;
            btnTranscribeFile.Text = "Transcribe file";
            btnTranscribeFile.UseVisualStyleBackColor = true;
            btnTranscribeFile.Click += btnTranscribeFile_Click;
            // 
            // btnOpenFile
            // 
            btnOpenFile.Enabled = false;
            btnOpenFile.Location = new Point(58, 214);
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(340, 109);
            btnOpenFile.TabIndex = 6;
            btnOpenFile.Text = "Open File";
            btnOpenFile.UseVisualStyleBackColor = true;
            btnOpenFile.Click += btnOpenFile_Click;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1287, 846);
            Controls.Add(btnOpenFile);
            Controls.Add(btnTranscribeFile);
            Controls.Add(txtTranscript);
            Controls.Add(lblSavedPath);
            Controls.Add(lblStatus);
            Controls.Add(btnStop);
            Controls.Add(btnStart);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(7, 8, 7, 8);
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "System Audio Recorder (WASAPI Loopback)";
            ResumeLayout(false);
            PerformLayout();
        }
        private TextBox txtTranscript;
        private Button btnTranscribeFile;
        private Button btnOpenFile;
        private OpenFileDialog openFileDialog1;
    }
}
