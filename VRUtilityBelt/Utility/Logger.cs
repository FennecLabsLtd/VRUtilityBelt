using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRUtilityBelt.Utility
{
    class Logger
    {
        static FileStream _fileStream;
        static StreamWriter _streamWriter;
        static TextWriter _oldOut = Console.Out;

        public enum LogLevel
        {
            Trace,
            Debug,
            Info,
            Warning,
            Error,
            Fatal,
        }

        public static void Log(LogLevel level, string message)
        {
            Console.WriteLine("[{0}|{1}|{2}] {3}", DateTime.Now.ToLongTimeString(), Thread.CurrentThread.ManagedThreadId, level, message);
        }

        public static void Trace(string message)
        {
            Log(LogLevel.Trace, message);
        }

        public static void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public static void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        public static void Warning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        public static void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        public static void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        public static void SetupLogfile(string filename)
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\VRUtilityBelt\\";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string path = folder + filename;
            try
            {
                _fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                _streamWriter = new StreamWriter(_fileStream);
            } catch(Exception e)
            {
                MessageBox.Show("Cannot open " + path + " for writing: " + e.Message);
                return;
            }

            _streamWriter.AutoFlush = true;

            Console.SetOut(_streamWriter);
            Info("Log Start");
        }

        public static void CloseHandle()
        {
            Console.WriteLine("Detatching File Stream");

            if(_streamWriter != null)
                _streamWriter.Close();

            if(_fileStream != null)
                _fileStream.Close();
        }
    }
}
