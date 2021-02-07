using System.Net;

namespace Masivian.Test.Data.Utils
{
    public class ResponseError
    {
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
