using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Extensions;

public static class ResultExtensions
{
    public static IReadOnlyCollection<object?> GetMessages<T>(this Result<T>[] results, ResultStatus status,
        bool includeValidationErrors = false)
    {
        switch (status)
        {
            case ResultStatus.Error:
            return GetErrorMessages();
            case ResultStatus.Invalid:
            return includeValidationErrors
                ? GetValidationErrors()
                : GetValidationMessages();
            case ResultStatus.None:
            case ResultStatus.Ok:
            return GetSuccessMessages();
            case ResultStatus.NotFound:
            default:
            return [];
        }
        
        IReadOnlyCollection<string> GetErrorMessages()
        {
            var resultErrors = results
                .Where(error => error.Status == ResultStatus.Error
                                && error.ErrorMessages.Any())
                .SelectMany(error => error.ErrorMessages).ToList();
            
            return resultErrors;
        }
        
        IReadOnlyCollection<string?> GetValidationMessages()
        {
            return results
                .Where(invalids => invalids.ValidationErrors.Any()
                                   && invalids.Status == ResultStatus.Invalid)
                .SelectMany(invalidMessages => invalidMessages.ValidationErrors
                    .Select(x => x.ErrorMessage)).ToList();
        }
        
        IReadOnlyCollection<ValidationError> GetValidationErrors()
        {
            return results
                .Where(invalids => invalids.ValidationErrors.Any()
                                   && invalids.Status == ResultStatus.Invalid)
                .SelectMany(invalidMessages => invalidMessages.ValidationErrors).ToList();
        }
        
        IReadOnlyCollection<string?> GetSuccessMessages()
        {
            return results
                .Where(x => x.Status == ResultStatus.Ok
                            && x.ErrorMessages.Any())
                .SelectMany(error => error.ErrorMessages).ToList();
        }
    }
}
