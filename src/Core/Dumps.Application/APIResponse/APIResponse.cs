namespace Dumps.Application.APIResponse;

public class APIResponse<T>
{
    public T Data { get; set; }
    public string Message { get; set; }

    public bool Success { get; set; }

    public APIResponse(T data, string message, bool success = true)
    {
        Data = data;
        Message = message;
        Success = success;
    }
}
