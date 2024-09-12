using Microsoft.OpenApi.Any;

namespace OrderSystem.DTOs
{
    public class Response <T>
    {
        public int StatusCode {  get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public bool Success { get; set; }
        

    }
}
