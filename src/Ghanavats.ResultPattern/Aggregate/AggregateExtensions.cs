using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Aggregate;

public static class AggregateExtensions
{
    internal static IReadOnlyCollection<string> GetErrorMessages(this IEnumerable<Result> results)
    {
        return results
            .Where(result => result.Status == ResultStatus.Error
                             && result.ErrorMessages.Any())
            .SelectMany(error => error.ErrorMessages).ToList();
    }
}
