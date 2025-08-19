using Ghanavats.ResultPattern.Enums;
using Ghanavats.ResultPattern.Mapping;
using Ghanavats.ResultPattern.Tests.DummyData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Ghanavats.ResultPattern.Tests;

public class ResultMappingTests
{
    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapSuccessResultToActionResultAsync()
    {
        //arrange
        var expectedController = new DummyController();
        var okResult = Result<string>.Success("Some data", "Some message");
        
        //act
        var actual = await okResult.ToActionResultAsync(expectedController);
        
        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as OkObjectResult;
        objectResult.ShouldNotBeNull();
        
        var objectResultValue = objectResult.Value as Result<string>;
        objectResultValue.ShouldNotBeNull();
        objectResultValue.Status.ShouldBe(ResultStatus.Ok);
        objectResultValue.Data.ShouldBe("Some data");
    }
    
    [Fact]
    public void ResultMapping_ShouldCorrectlyMapSuccessResultToActionResult()
    {
        //arrange
        var expectedController = new DummyController();
        var okResult = Result<string>.Success("Some data");
        
        //act
        var actual = okResult.ToActionResult(expectedController);
        
        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as OkObjectResult;
        objectResult.ShouldNotBeNull();
        
        var objectResultValue = objectResult.Value as Result<string>;
        objectResultValue.ShouldNotBeNull();
        objectResultValue.Status.ShouldBe(ResultStatus.Ok);
        objectResultValue.Data.ShouldBe("Some data");
    }
    
    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapErrorResultToActionResultAsync()
    {
        //arrange
        var expectedController = new DummyController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
        
        var errorResult = Result<string>.Error("Some error");
        
        //act
        var actual = await errorResult.ToActionResultAsync(expectedController);
        
        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as ObjectResult;
        objectResult.ShouldNotBeNull();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        objectResult.Value.ShouldBeOfType<ProblemDetails>();
        
        var objectResultValue = objectResult.Value as ProblemDetails;
        objectResultValue.ShouldNotBeNull();
        objectResultValue.Status.ShouldBe(StatusCodes.Status500InternalServerError);
        objectResultValue.Extensions.ShouldNotBeNull();
        objectResultValue.Extensions.ShouldNotBeEmpty();
        objectResultValue.Extensions["TraceId"].ShouldNotBeNull();
        objectResultValue.Extensions["TraceId"].ShouldBe(expectedController.HttpContext.TraceIdentifier);
    }
}
