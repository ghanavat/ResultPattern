using System.ComponentModel;
using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Models;

public record AggregateResultsModel
{
    /// <summary>
    /// This member is only populated when WithFullValidationErrors extension is called.
    /// </summary>
    public IReadOnlyCollection<ValidationError> ValidationErrors { get; init; } = [];
    public ResultStatus Status { get; init; }
    
    /// <summary>
    /// This member has two behaviours.
    /// One, by default, it will store the status Error and Invalid messages.
    /// Second, it will be an empty collection when WithFullValidationErrors extension is called.
    /// </summary>
    public IReadOnlyCollection<string?> Messages { get; init; } = [];
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal IReadOnlyCollection<Result> OriginalResults { get; init; } = [];
}
