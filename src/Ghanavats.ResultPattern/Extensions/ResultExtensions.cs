using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Extensions;

public static class ResultExtensions
{
    internal static IReadOnlyCollection<object?> GetMessages(this Result[] results, ResultStatus status,
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
            case ResultStatus.Ok:
            case ResultStatus.None:
            case ResultStatus.NotFound:
                throw new NotSupportedException("Not supported result status.");
            default:
                throw new InvalidOperationException("Operation not supported.");
        }

        IReadOnlyCollection<string> GetErrorMessages()
        {
            var resultErrors = results
                .Where(result => result.Status == ResultStatus.Error
                                 && result.ErrorMessages.Any())
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
    }
}
