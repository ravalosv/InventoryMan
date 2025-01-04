namespace InventoryMan.Application.Common.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; } = false;
        public T? Data { get; set; }
        public string? Error { get; set; } = null;

        public static Result<T> Success(T data) => new() { IsSuccess = true, Data = data };
        public static Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
    }

}
