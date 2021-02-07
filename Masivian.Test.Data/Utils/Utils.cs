using Newtonsoft.Json;
namespace Masivian.Test.Data.Utils
{
    public static class Utils
    {
        public static string CreateMessageError(string message)
        {
            return JsonConvert.SerializeObject(
                new ResponseError
                {
                    Message = message,
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                }
                );
        }
    }
}
