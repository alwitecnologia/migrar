using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exportador
{
    public class BusinessException: Exception
    {
         public BusinessException() : base() { }
         public BusinessException(string message) : base(message) { }
         public BusinessException(string message, System.Exception inner) : base(message, inner) { }
    }
}
