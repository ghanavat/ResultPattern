using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using FluentValidation.Results;
using Ghanavats.ResultPattern.Enums;
using Microsoft.AspNetCore.Mvc;

[assembly: InternalsVisibleTo(assemblyName: "Ghanavats.ResultPattern.Tests")]

namespace Ghanavats.ResultPattern;

/// <summary>
/// Result class.
/// Use this when you want to return a result from an implementation
/// </summary>
/// <typeparam name="T"></typeparam>
public class Result<T>
{
    /// <summary>
    /// Gets the data payload associated with a successful result.
    /// </summary>
    /// <remarks>
    /// This property is only meaningful when <see cref="Status"/> is <see cref="ResultStatus.Ok"/>.  
    /// It is included in the JSON response for consumers.  
    /// </remarks>
    [JsonInclude]
    public T Data { get; init; }
    
    /// <summary>
    /// Gets the status of the result (e.g. Ok, Error, Invalid, NotFound).
    /// </summary>
    /// <remarks>
    /// This property drives internal logic and mapping to <c>ProblemDetails</c> or <c>ValidationProblemDetails</c>.  
    /// It is excluded from JSON output, as it would otherwise duplicate the semantics already conveyed
    /// by the HTTP status code and/or <c>ProblemDetails</c>.
    /// </remarks>
    [JsonIgnore]
    public ResultStatus Status { get; private init; } = ResultStatus.Ok;

    /// <summary>
    /// Gets or sets a success message associated with a successful result.
    /// </summary>
    /// <remarks>
    /// Included in the JSON response when present.
    /// Useful for conveying additional context alongside a successful payload.  
    /// </remarks>
    [JsonInclude]
    public string SuccessMessage { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets the classification of the error to map to an appropriate HTTP status code.
    /// </summary>
    /// <remarks>
    /// Used internally by the mapping layer to determine which HTTP status code to emit (e.g. 401, 403, 409, 500).  
    /// This property is excluded from JSON output as it is only relevant for transport mapping and not intended
    /// for clients.
    /// </remarks>
    [JsonIgnore]
    public ErrorKind? Kind { get; protected init; }

    /// <summary>
    /// Gets the collection of error messages associated with this result.
    /// </summary>
    /// <remarks>
    /// Used internally to populate <c>ProblemDetails.Detail</c>.  
    /// It is excluded from JSON output to prevent leaking implementation details or duplicate error data.  
    /// Consumers should rely on <c>ProblemDetails</c> or <c>ValidationProblemDetails</c> instead.
    /// </remarks>
    [JsonIgnore]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal IEnumerable<string> ErrorMessages { get; init; } = [];

    /// <summary>
    /// Gets a dictionary of validation errors grouped by field name.
    /// </summary>
    /// <remarks>
    /// Keys represent field/property names and values are arrays of validation error messages.  
    /// This property is excluded from JSON output.  
    /// It is only used internally to initialise <see cref="ValidationProblemDetails.Errors"/> when mapping results
    /// to API responses.
    /// </remarks>
    [JsonIgnore]
    public IReadOnlyDictionary<string, string[]>? ValidationErrorsByField { get; protected init; }

    /// <summary>
    /// A constructor that accepts <paramref name="data"/>.
    /// </summary>
    /// <param name="data">Constructor parameter of type <paramref name="data"/></param>
    public Result(T data)
    {
        Data = data;
    }

    /// <summary>
    /// A constructor that accepts <paramref name="status"/>.
    /// It is used internally in this class and Result.Void
    /// </summary>
    /// <param name="status">Constructor parameter of type <paramref name="status"/></param>
    internal Result(ResultStatus status)
    {
        Status = status;
        Data = default!;
    }

    /// <summary>
    /// Default protected constructor.
    /// </summary>
    /// <remarks>
    /// It is used in Result.Void to return an instance of
    /// Success status without needing to pass any extra types.
    /// </remarks>
    protected Result() { Data = default!; }
    
    /// <summary>
    /// A constructor that accepts <paramref name="data"/>.
    /// It is used by the 'Success' method when a Success Message needed to be set.
    /// </summary>
    /// <param name="data">Constructor parameter of type <paramref name="data"/></param>
    /// <param name="successMessage">Constructor parameter of type <paramref name="successMessage"/></param>
    internal Result(T data, string successMessage)
    {
        Data = data;
        SuccessMessage = successMessage;
    }
    
    /// <summary>
    /// Creates a successful <see cref="Result{T}"/> containing the specified <paramref name="data"/>.
    /// </summary>
    /// <param name="data">
    /// The value to associate with the result.  
    /// This represents the payload returned when the operation completes successfully.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> instance with <see cref="ResultStatus.Ok"/> status and the provided <paramref name="data"/>.
    /// </returns>
    /// <remarks>
    /// This factory is the primary way to indicate a successful operation.  
    /// Be mindful that <paramref name="data"/> is not validated here;
    /// if you pass <c>null</c>, the <see cref="Result{T}.Data"/> will be <c>null</c>, 
    /// which can lead to <see cref="NullReferenceException"/> if consumed carelessly.  
    /// Use only when the operation truly represents success.
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = Result&lt;User&gt;.Success(new User("Alice"));
    ///
    /// if (result.IsSuccess())
    /// {
    ///     Console.WriteLine($"User created: {result.Data.Name}");
    /// }
    /// </code>
    /// </example>
    public static Result<T> Success(T data)
    {
        return new Result<T>(data);
    }

    /// <summary>
    /// Creates a successful <see cref="Result{T}"/> with the specified <paramref name="data"/> 
    /// and an accompanying <paramref name="successMessage"/>.
    /// </summary>
    /// <param name="data">The value returned by the successful operation.</param>
    /// <param name="successMessage">A message describing the success outcome.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> instance with <see cref="ResultStatus.Ok"/> status, 
    /// containing the provided <paramref name="data"/> and <paramref name="successMessage"/>.
    /// </returns>
    /// <example>
    /// <code>
    /// var result = Result&lt;Order&gt;.Success(order, "Order created successfully");
    /// Console.WriteLine(result.SuccessMessage); // "Order created successfully"
    /// </code>
    /// </example>

    public static Result<T> Success(T data, string successMessage)
    {
        return new Result<T>(data, successMessage);
    }

    /// <summary>
    /// Creates a failed <see cref="Result{T}"/> with the specified error message and classification.
    /// </summary>
    /// <param name="errorMessage">
    /// A descriptive message explaining the cause of the error. 
    /// This value will be included in the internal <c>ErrorMessages</c> collection and may be mapped 
    /// into <see cref="ProblemDetails.Detail"/> during API response generation.
    /// </param>
    /// <param name="errorKind">
    /// An optional classification of the error that determines which HTTP status code 
    /// is emitted when mapped to an API response.
    /// Defaults to <see cref="ErrorKind.Unknown"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> representing a failed operation.
    /// The <see cref="Result{T}.Status"/> will be <c>Error</c>.
    /// </returns>
    /// <remarks>
    /// Use this method when an operation fails due to business logic or an application-level error.  
    /// For validation-related errors, prefer <c>Invalid</c> instead,
    /// as it maps to <see cref="ValidationProblemDetails"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// return Result&lt;User&gt;.Error("Database connection failed", ErrorKind.Database);
    /// </code>
    /// </example>
    public static Result<T> Error(string errorMessage, ErrorKind errorKind = ErrorKind.Unknown)
    {
        return new Result<T>(ResultStatus.Error)
        {
            ErrorMessages = [errorMessage],
            Kind = errorKind
        };
    }

    /// <summary>
    /// Creates a <see cref="Result{T}"/> representing a not found outcome.
    /// </summary>
    /// <returns>
    /// A <see cref="Result{T}"/> instance with <see cref="ResultStatus.NotFound"/> status,
    /// indicating that the requested resource could not be found.
    /// </returns>
    /// <example>
    /// <code>
    /// var result = Result&lt;Customer&gt;.NotFound();
    /// if (result.IsNotFound())
    /// {
    ///     Console.WriteLine("Customer not found.");
    /// }
    /// </code>
    /// </example>
    public static Result<T> NotFound()
    {
        return new Result<T>(ResultStatus.NotFound);
    }

    /// <summary>
    /// Creates an <see cref="Result{T}"/> representing an invalid operation,
    /// based on the supplied <see cref="FluentValidation.Results.ValidationResult"/>.
    /// </summary>
    /// <param name="validationResult">
    /// The <see cref="FluentValidation.Results.ValidationResult"/> produced by FluentValidation.  
    /// Its validation failures are transformed into a <c>ValidationErrorsByField</c> dictionary,
    /// which is later mapped into <see cref="ValidationProblemDetails.Errors"/> when returning API responses.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> with <see cref="ResultStatus.Invalid"/>.  
    /// This result indicates that input validation failed and should be returned to the API consumer
    /// as <see cref="ValidationProblemDetails"/>.
    /// </returns>
    /// <remarks>
    /// This method is tightly integrated with FluentValidation.  
    /// If you are not using FluentValidation,
    /// use the <see cref="Invalid(IDictionary{string, string[]})"/> factory to supply
    /// your own field-to-messages map.
    /// </remarks>
    /// <example>
    /// <code>
    /// var validator = new UserValidator();
    /// var validationResult = validator.Validate(userInput);
    ///
    /// if (!validationResult.IsValid)
    /// {
    ///     var result = Result&lt;User&gt;.Invalid(validationResult);
    ///     return await result.ToActionResultAsync(controller); // maps to ValidationProblemDetails
    /// }
    /// </code>
    /// </example>
    public static Result<T> Invalid(ValidationResult validationResult)
    {
        return new Result<T>(ResultStatus.Invalid)
        {
            ValidationErrorsByField = validationResult.ToDictionary().AsReadOnly()
        };
    }
    
    /// <summary>
    /// Creates an <see cref="Result{T}"/> representing an invalid operation
    /// using the provided field-to-messages dictionary.
    /// </summary>
    /// <param name="validationErrors">
    /// A dictionary of validation errors where the key is the field or property name,
    /// and the value is an array of validation error messages associated with that field.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> with <see cref="ResultStatus.Invalid"/> status,
    /// containing the supplied <paramref name="validationErrors"/> in 
    /// <see cref="ValidationErrorsByField"/>.
    /// </returns>
    /// <remarks>
    /// This overload is useful when you are not using FluentValidation, or when you
    /// already have validation errors represented as a dictionary.  
    /// The dictionary is defensively copied to ensure immutability and prevent external modification.
    /// </remarks>
    /// <example>
    /// <code>
    /// var errors = new Dictionary&lt;string, string[]&gt;
    /// {
    ///     ["Email"] = new[] { "Email is required." },
    ///     ["Password"] = new[] { "Password must be at least 8 characters." }
    /// };
    ///
    /// var result = Result&lt;User&gt;.Invalid(errors);
    /// if (result.IsInvalid())
    /// {
    ///     // result.ValidationErrorsByField now contains the errors
    /// }
    /// </code>
    /// </example>
    public static Result<T> Invalid(IDictionary<string, string[]> validationErrors)
    {
        // Defensively copy to prevent external mutation and normalise comparer
        var copy = new Dictionary<string, string[]>(validationErrors.Count, StringComparer.Ordinal);
        
        foreach (var kvp in validationErrors)
            copy[kvp.Key] = kvp.Value.ToArray();

        return new Result<T>(ResultStatus.Invalid)
        {
            ValidationErrorsByField = copy
        };
    }
    
    public static implicit operator Result<T>(T data) => new(data);
    public static implicit operator T(Result<T> result) => result.Data;
    public static implicit operator Result<T>(Result result) => new()
    {
        Status = result.Status,
        ErrorMessages = result.ErrorMessages,
        SuccessMessage = result.SuccessMessage,
        Kind = result.Kind,
        ValidationErrorsByField = result.ValidationErrorsByField
    };
}
