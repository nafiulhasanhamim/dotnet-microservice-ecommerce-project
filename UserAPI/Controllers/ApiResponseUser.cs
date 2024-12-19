namespace User.Management.Service.Models
{
    public class ApiResponseUser<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public T? Response { get; set; }

    }
}