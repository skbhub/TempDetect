using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TempDetect
{
    public class Log
    {

        private string _Path;

        public string Path
        {
            get
            {
                return _Path;
            }

            set
            {
                _Path = value;
            }
        }

        public Log(string path)
        {
            this.Path = path;     
        }

        public void Write2LogFile(string msg)
        {
            if (!System.IO.File.Exists(Path))
            {
                FileStream stream = System.IO.File.Create(Path);
                stream.Close();
                stream.Dispose();
            }
            using (StreamWriter writer = new StreamWriter(Path, true))
            {
                writer.WriteLine(DateTime.Now.ToString()+" "+msg);
            }

        }
    }
}
