using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dumps.Application.Exceptions
{
    public class RestException : Exception
    {
        public RestException(HttpStatusCode code, object errors = null)
        {
            Code = code;
            Errors = errors;
        }
        public HttpStatusCode Code { get; set; }
        public object Errors { get; set; }
    }
}
