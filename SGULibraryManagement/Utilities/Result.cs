using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.Utilities
{
    public class Result
    {
        public bool Value { get; private set; }
        public string Message { get; private set; } = string.Empty;

        public Result()
        {

        }

        public Result(bool value, string message)
        {
            Value = value;
            Message = message;
        }
    }
}
