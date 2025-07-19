using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Models;

public class AggregateResultsModel
{
    public ResultStatus Status { get; init; }
    public IReadOnlyCollection<object?> Messages { get; init; } = [];
}
