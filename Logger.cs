using System;
using System.IO;
using System.Text;

namespace LoggerLib
{
    [Author("HANU")]
    public class Logger : IDisposable
    {
        public OutputFlag OutputFlag { get; }

        public string LogFilePath { get; } = String.Empty;

        private static readonly DateTime _startTime = DateTime.Now;

        private readonly FileStream _file;

        private readonly StreamWriter _writer;

        private readonly string _timeFormat;

        public Logger(OutputFlag outputFlag, string logPath = null, string timeFormat_file = "yyMMdd-HH-mm-ss", string timeFormat_others = "HH:mm:ss")
        {
            this.OutputFlag = outputFlag;
            this.LogFilePath = logPath;
            this._timeFormat = timeFormat_others;
            if (!String.IsNullOrWhiteSpace(logPath))
            {
                #region Insert system time into the file name
                int lastPoint = -1;
                for (int i = this.LogFilePath.Length - 1; i >= 0; --i)
                {
                    if (this.LogFilePath[i] == '.')
                    {
                        lastPoint = i;
                        break;
                    }
                }
                if (lastPoint != -1)
                    logPath = String.Format("{0} {1}{2}", logPath.Substring(0, lastPoint),
                        _startTime.ToString(timeFormat_file), logPath.Substring(lastPoint));
                else
                    logPath += _startTime;
                if (logPath.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
                    #endregion
                    #region Open file stream and add some basic information
                    this._file = new FileStream(logPath, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
                this._writer = new StreamWriter(this._file);
                this._writer.AutoFlush = true;
                this._writer.WriteLine(this._file.Name);
                this._writer.WriteLine(_startTime.ToLongDateString());
                this._writer.WriteLine();
                #endregion
            }
        }

        public void Info(params string[] message) => this.Log(this.Process(message, "INF"));

        public void Debug(params string[] message) => this.Log(this.Process(message, "DBG"));

        public void Warn(params string[] message) => this.Log(this.Process(message, "WRN"));

        public void Error(params string[] message) => this.Log(this.Process(message, "ERR"));

        /// <summary>
        /// Process the multi-line string to print with system time and label
        /// </summary>
        /// <param name="rawMessage">Multi-line string to print</param>
        /// <param name="label">Label of the importance</param>
        /// <returns>Processed entire string</returns>
        private string Process(string[] rawMessage, string label)
        {
            StringBuilder result = new StringBuilder();
            string time = DateTime.Now.ToString(this._timeFormat);
            foreach (string message in rawMessage)
                result.AppendLine($"[{time}] [{label}]: {message}");
            return result.ToString();
        }

        private void Log(string processed)
        {
            if (this.OutputFlag.HasFlag(OutputFlag.Console))
                Console.Write(processed);
            if (this.OutputFlag.HasFlag(OutputFlag.Debug))
                System.Diagnostics.Debug.Write(processed);
            if (this.OutputFlag.HasFlag(OutputFlag.Trace))
                System.Diagnostics.Trace.Write(processed);
            if (this.OutputFlag.HasFlag(OutputFlag.File))
                this._writer.Write(processed);
        }

        public void Dispose()
        {
            this._writer.Close();
            this._file.Close();
        }
    }
}
