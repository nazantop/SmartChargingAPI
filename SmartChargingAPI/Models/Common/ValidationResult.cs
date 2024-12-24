namespace SmartChargingAPI.Models.Common;
public class ValidationResult<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ValidationResult<T> Success(T data) => new ValidationResult<T> { IsSuccess = true, Data = data };

    public static ValidationResult<T> Failure(string message) => new ValidationResult<T> { IsSuccess = false, Message = message };
}
