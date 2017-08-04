using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VRUtilityBelt.Utility
{
    class LogHelper
    {
        static FileStream _fileStream;
        static StreamWriter _streamWriter;
        static TextWriter _oldOut = Console.Out;

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
            Console.WriteLine("Log Start: " + DateTime.Now.ToLongTimeString());
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
