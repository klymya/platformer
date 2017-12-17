using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyrsovykh
{
    abstract class Exception : System.Exception
    {
        public Exception(string _e)
        {
            exception = _e;
        }

        public string exception;
    }

    class FileReadException : Exception
    {
        public FileReadException(string name) : base(name + " - not found.") { }
    }
}
