using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exportador.Exceptions
{
    public class BusinessException: System.Exception
    {
        public BusinessException() : base() { }
        public BusinessException(string message) : base(message) { }
        public BusinessException(string message, System.Exception inner) : base(message, inner) { }
    }
}
