using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Models;

public class AggregateResultsModel
{
    public ResultStatus Status { get; init; } =  ResultStatus.None;
    public string? TypeName { get; init; }
    public IReadOnlyCollection<object?> Messages { get; init; } = [];
}
