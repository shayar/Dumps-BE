public class APIResponse<T>
{
    public T Data { get; set; }
    public string Message { get; set; }
    public bool Success { get; set; }

    // Pagination properties - nullable to indicate optional pagination
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public int? TotalPages { get; set; }
    public int? TotalRecords { get; set; }

    // Constructor for non-paginated response
    public APIResponse(T data, string message, bool success = true)
    {
        Data = data;
        Message = message;
        Success = success;
    }

    // Constructor for paginated response
    public APIResponse(T data, string message, int pageNumber, int pageSize, int totalRecords, bool success = true)
        : this(data, message, success)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
    }
}
