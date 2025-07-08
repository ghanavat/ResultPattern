using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern;

/// <summary>
/// A different variant of the global Result.
/// Use this when you want to populate/return Result without having to specify its T 'Data' type.
/// </summary>
public class Result : Result<Result>
{
    /// <summary>
    /// Private constructor that is used in this class.
    /// No need to set Status here as its default is OK.
    /// </summary>
    private Result()
    {
    }

    /// <summary>
    /// A constructor that accepts <paramref name="status"/>
    /// </summary>
    /// <param name="status"></param>
    private Result(ResultStatus status) : base(status)
    {
    }

    /// <summary>
    /// Represents a successful operation without a return type
    /// </summary>
    /// <returns>A Success Result</returns>
    public static Result Success() => new();

    /// <summary>
    /// Represents invalid result with validation errors.
    /// </summary>
    /// <param name="validationErrors"></param>
    /// <returns>An Invalid Result</returns>
    public new static Result Invalid(IEnumerable<ValidationError> validationErrors)
    {
        return new Result(ResultStatus.Invalid)
        {
            ValidationErrors = validationErrors
        };
    }

    /// <summary>
    /// Represents the error result with the error message
    /// </summary>
    /// <returns>An Error Result</returns>
    public new static Result Error(string errorMessage) => new(ResultStatus.Error)
    {
        ErrorMessages = [errorMessage]
    };

    public static Result Aggregate(params Result[] results)
    {
        var resultLocal = new Result();

        PopulateErrorMessagesForError();
        PopulateValidationErrors();

        if (resultLocal.Status == ResultStatus.None)
        {
            resultLocal.Status = ResultStatus.Ok;
        }

        return resultLocal;

        void PopulateErrorMessagesForError()
        {
            var anyErrors = results.Where(x => x.Status == ResultStatus.Error).ToArray();
            resultLocal.Status = anyErrors.Length > 0 ? ResultStatus.Error : ResultStatus.None;

            var errorMessagesLocal = new List<string>();
            foreach (var result in anyErrors)
            {
                errorMessagesLocal.AddRange(result.ErrorMessages);
            }

            resultLocal.ErrorMessages = errorMessagesLocal;
        }
        void PopulateValidationErrors()
        {
            var anyValidationErrors = results.Where(x => x.Status == ResultStatus.Invalid).ToArray();
            
            if (anyValidationErrors.Length > 0 
                && resultLocal.Status != ResultStatus.Error)
            {
                resultLocal.Status = ResultStatus.Invalid;
            }
            
            var validationErrorsLocal = new List<ValidationError>();
            foreach (var result in anyValidationErrors)
            {
                validationErrorsLocal.AddRange(result.ValidationErrors);
            }

            resultLocal.ValidationErrors = validationErrorsLocal;
        }
    }
}
