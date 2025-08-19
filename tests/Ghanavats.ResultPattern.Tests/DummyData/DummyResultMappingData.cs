using Ghanavats.ResultPattern.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace Ghanavats.ResultPattern.Tests.DummyData;

public class DummyResultMappingData
{
    
}

public class DummyController : ControllerBase
{
    public async Task<IActionResult> DummyDoSomethingSuccessAsync()
    {
        // Some logic
        var task = Task.Run(() => Result<string>.Success("Some data", "Some message"));
        var result = await task;

        return await result.ToActionResultAsync(this);
    }
    
    public IActionResult DummyDoSomethingSuccess()
    {
        // Some logic
        var resultOfSomething = Result<string>.Success("Some data");

        return resultOfSomething.ToActionResult(this);
    }
    
    public async Task<IActionResult> DummyDoSomethingErrorAsync()
    {
        // Some logic
        var task = Task.Run(() => Result<string>.Error("Something went wrong and you need to fix it."));
        var result = await task;

        return await result.ToActionResultAsync(this);
    }
}
