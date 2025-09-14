using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Aggregate;

/// <summary>
/// Represents a grouped summary of aggregated <see cref="Result"/> instances,
/// containing error or validation details for a specific <see cref="ResultStatus"/>.
/// </summary>
/// <remarks>
/// Instances of this class are returned by <see cref="Result.Aggregate(Result[])"/>. 
/// By default, <see cref="Messages"/> contains plain text messages for both 
/// <see cref="ResultStatus.Error"/> and <see cref="ResultStatus.Invalid"/> results. 
/// <see cref="ValidationErrorsPair"/> is populated for <see cref="ResultStatus.Invalid"/> results 
/// and <see cref="Messages"/> is cleared to avoid duplication.
/// </remarks>
public record AggregateResultsModel
{
    /// <summary>
    /// Gets the <see cref="ResultStatus"/> representing the outcome category of the aggregated results.
    /// </summary>
    public ResultStatus Status { get; init; }

    /// <summary>
    /// Gets a collection of plain text messages associated with the aggregated results.
    /// For <see cref="ResultStatus.Error"/> results, contains error messages.
    /// </summary>
    public IReadOnlyCollection<string?> Messages { get; init; } = [];
    
    /// <summary>
    /// Gets a collection of detailed <see cref="ValidationErrorsPair"/> objects for 
    /// <see cref="ResultStatus.Invalid"/> results, with property names and validation messages.
    /// </summary>
    public IReadOnlyCollection<KeyValuePair<string, string[]>> ValidationErrorsPair { get; init; } = [];
}
