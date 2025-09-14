using Ghanavats.ResultPattern.Enums;
using Ghanavats.ResultPattern.Extensions;

namespace Ghanavats.ResultPattern.Aggregate;

internal static class AggregateBase
{
    /// <summary>
    /// Aggregates a collection of <see cref="Result"/> instances into a grouped summary by <see cref="ResultStatus"/>.
    /// </summary>
    /// <param name="results">
    /// The collection of <see cref="Result"/> objects to aggregate. 
    /// Results with statuses <see cref="ResultStatus.Ok"/> and <see cref="ResultStatus.NotFound"/> are excluded.
    /// </param>
    /// <returns>
    /// A read-only collection of <see cref="AggregateResultsModel"/> objects, 
    /// each representing a grouped result with status and error information.
    /// </returns>
    /// <remarks>
    /// Use <c>Aggregate</c> to collate outcomes from multiple operations, 
    /// such as service calls or validation checks, into a simplified summary.
    /// By default, <see cref="AggregateResultsModel.Messages"/> contains error messages as strings.
    /// </remarks>
    /// <example>
    /// <code>
    /// var results = new[]
    /// {
    ///     Result.Success(),
    ///     Result.Error("Database connection failed."),
    ///     Result.Invalid([new ValidationError("Email is required.")])
    /// };
    ///
    /// var aggregated = Result.Aggregate(results);
    /// </code>
    /// </example>
    internal static IReadOnlyCollection<AggregateResultsModel> AggregateResults(params Result[] results)
    {
        return results
            .GroupBy(result => result.Status)
            .Skip(results.Count(x => x.Status is ResultStatus.Ok or ResultStatus.NotFound))
            .Select(whatIWant => new AggregateResultsModel
            {
                Status = whatIWant.Key,
                Messages = whatIWant.GetErrorMessages(),
                ValidationErrorsPair = whatIWant.SelectMany(r => r.ValidationErrorsByField ?? new Dictionary<string, string[]>())
                    .ToList().AsReadOnly()
            }).ToList().AsReadOnly();
    }
}
