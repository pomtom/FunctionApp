using System;
using System.Collections.Generic;
using System.Text;

namespace DurableFunc
{
    public class BaseResponse
    {
        public int StatusCode { get; set; }
        public string ResponseMessage { get; set; }
    }
}
