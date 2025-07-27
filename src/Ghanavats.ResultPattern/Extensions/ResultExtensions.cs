using Ghanavats.ResultPattern.Enums;
using Ghanavats.ResultPattern.Models;

namespace Ghanavats.ResultPattern.Extensions;

public static class ResultExtensions
{    
    public static IReadOnlyCollection<string> GetErrorMessages(this IEnumerable<Result> results)
    {
        var resultErrors = results
            .Where(result => result.Status == ResultStatus.Error
                             && result.ErrorMessages.Any())
            .SelectMany(error => error.ErrorMessages).ToList();

        return resultErrors;
    }

    public static IReadOnlyCollection<string> GetValidationMessages(this IEnumerable<Result> results)
    {
        return results
            .Where(invalids => invalids.ValidationErrors.Any()
                               && invalids.Status == ResultStatus.Invalid)
            .SelectMany(invalidMessages => invalidMessages.ValidationErrors
                .Select(x => x.ErrorMessage)).ToList();
    }
    
    public static IReadOnlyCollection<AggregateResultsModel> WithFullValidationErrors(
        this IReadOnlyCollection<AggregateResultsModel> aggregated)
    {
        var updated = aggregated.Select(result =>
        {
            if (result.Status != ResultStatus.Invalid 
                || result.OriginalResults.Count == 0)
            {
                return result;
            }

            return result with
            {
                Messages = [],
                ValidationErrors = result.OriginalResults
                    .Where(r => r.Status == ResultStatus.Invalid)
                    .SelectMany(r => r.ValidationErrors)
                    .ToList()
            };
        });

        return updated.ToList().AsReadOnly();
    }
}
