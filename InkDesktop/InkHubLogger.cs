using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace InkDesktop
{
    public class InkHubLogger : IDisposable
    {
        private string _logFilePath;
        private FileStream _fs;
        private string _logFolder;

        bool _logInfo = false;
        bool _logException = false;
        bool _logPenData = false;
        
        public InkHubLogger()
        {
            string AppDataLocal = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            string folderName = Assembly.GetExecutingAssembly().GetName().Name;
            _logFolder = AppDataLocal + "\\" + folderName;
            if (!Directory.Exists(_logFolder))
            {
                Directory.CreateDirectory(_logFolder);
            }

            string filename = getLogFilename();
            _logFilePath = _logFolder + "\\" + filename;

            _logInfo = Properties.Settings.Default.LogInfo;
            _logException = Properties.Settings.Default.LogException;
            _logPenData = Properties.Settings.Default.LogPenData;

            _fs = File.Open(_logFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

        }

        public string GetLogFolder()
        {
            return _logFolder;
        }

        public string GetTodayLogFilePath()
        {
            return _logFilePath;
        }
        
        private string getLogFilename()
        {
            return "InkDesktop_" + DateTime.Today.ToString("yyyyMMdd") + ".log";
        }

        public async void Log(string msg, int alertType)
        {
            try
            {
                int alert = alertType % 10;
                if ((_logInfo && alert == 0) || (_logException && alert == 1) || (_logPenData && alert == 2))
                {
                    msg = DateTime.Now.ToString("yyyyMMdd_HHmmss") + " - " + msg;
                    msg = msg + Environment.NewLine;
                    byte[] msgBytes = Encoding.UTF8.GetBytes(msg);
                    _fs.Seek(0, SeekOrigin.End);
                    await _fs.WriteAsync(msgBytes, 0, msgBytes.Length);
                }
            }
            catch (Exception)
            {

            }
            
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _fs.Close();
                    _fs = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~InkHubLogger() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
