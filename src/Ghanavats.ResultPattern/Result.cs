using System.Text.Json.Serialization;
using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern;

/// <summary>
/// Result class.
/// Use this when you want to return a result from an implementation
/// </summary>
/// <typeparam name="T"></typeparam>
public class Result<T>
{
    /// <summary>
    /// A constructor that accepts <paramref name="status"/>.
    /// It is used internally in this class and Result.Void
    /// </summary>
    /// <param name="status">Constructor parameter of type <paramref name="status"/></param>
    internal Result(ResultStatus status)
    {
        Status = status;
    }
    
    /// <summary>
    /// Default protected constructor.
    /// </summary>
    /// <remarks>
    /// It is used in Result.Void to return an instance of Success status without needing to pass any extra types.
    /// </remarks>
    protected internal Result() { }
    
    /// <summary>
    /// A constructor that accepts <paramref name="data"/>.
    /// It is used internally by this class.
    /// </summary>
    /// <param name="data">Constructor parameter of type <paramref name="data"/></param>
    private Result(T data)
    {
        Data = data;
    }

    /// <summary>
    /// A constructor that accepts <paramref name="data"/>.
    /// It is used by the 'Success' method when a Success Message needed to be set.
    /// </summary>
    /// <param name="data">Constructor parameter of type <paramref name="data"/></param>
    /// <param name="successMessage">Constructor parameter of type <paramref name="successMessage"/></param>
    private Result(T data, string successMessage) : this(data)
    {
        SuccessMessage = successMessage;
    }
    
    /// <summary>
    /// Use this property to easily determine if the status is OK or not
    /// </summary>
    [JsonIgnore]
    public bool IsSuccess => Status is ResultStatus.Ok;

    /// <summary>
    /// Data property of type <typeparamref name="T"/> which holds the details of the result as a JSON field.
    /// </summary>
    [JsonInclude]
    public T? Data { get; set; }

    /// <summary>
    /// Use this property to accurately determine the exact status of the Result
    /// </summary>
    [JsonInclude]
    public ResultStatus Status { get; protected set; } = ResultStatus.Ok;

    /// <summary>
    /// Error Message collection
    /// </summary>
    [JsonInclude]
    public IEnumerable<string> ErrorMessages { get; protected set; } = [];

    /// <summary>
    /// Set is protected and accessible by derived classes
    /// </summary>
    [JsonInclude]
    public IEnumerable<ValidationError> ValidationErrors { get; protected set; } = [];

    /// <summary>
    /// Use this property to access the success message
    /// </summary>
    [JsonInclude]
    public string SuccessMessage { get; protected set; } = string.Empty;

    /// <summary>
    /// Successful operation with a value as a result
    /// </summary>
    /// <param name="data">Data parameter for setting value to it</param>
    /// <returns>A Result object of <typeparamref name="T"/> </returns>
    public static Result<T> Success(T data)
    {
        return new Result<T>(data);
    }

    /// <summary>
    /// Successful operation with a value as a result and a custom message.
    /// </summary>
    /// <param name="data">Data parameter for setting value to it</param>
    /// <param name="successMessage">A custom success message</param>
    /// <returns>A Result object of <typeparamref name="T"/> </returns>
    public static Result<T> Success(T data, string successMessage)
    {
        return new Result<T>(data, successMessage);
    }

    /// <summary>
    /// Represents a situation where an error occurred.
    /// </summary>
    /// <param name="errorMessage">A custom error message</param>
    /// <returns>A Result object of <typeparamref name="T"/></returns>
    public static Result<T> Error(string errorMessage)
    {
        return new Result<T>(ResultStatus.Error) { ErrorMessages = [errorMessage] };
    }

    /// <summary>
    /// Represents a situation where the resource is not found.
    /// </summary>
    /// <returns>A Result object of <typeparamref name="T"/></returns>
    public static Result<T> NotFound()
    {
        return new Result<T>(ResultStatus.NotFound);
    }

    /// <summary>
    /// Represents an invalid result with validation errors.
    /// Use this when validation in your application failed.
    /// </summary>
    /// <param name="validationErrors">A list of validation errors</param>
    /// <returns>A Result object of <typeparamref name="T"/></returns>
    public static Result<T> Invalid(IEnumerable<ValidationError> validationErrors)
    {
        return new Result<T>(ResultStatus.Invalid) { ValidationErrors = validationErrors };
    }

    /// <summary>
    /// An operator to automatically convert the return type in a method to the type being returned
    /// </summary>
    /// <param name="data">The return data</param>
    public static implicit operator Result<T>(T data) => new(data);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    public static implicit operator T(Result<T> result) => result.Data!;

    /// <summary>
    /// An operator to automatically convert the return type in a method to the default state
    /// </summary>
    /// <param name="result"></param>
    public static implicit operator Result<T>(Result result) => new(default(T)!)
    {
        Status = result.Status,
        ErrorMessages = result.ErrorMessages,
        SuccessMessage = result.SuccessMessage,
        ValidationErrors = result.ValidationErrors
    };
}
