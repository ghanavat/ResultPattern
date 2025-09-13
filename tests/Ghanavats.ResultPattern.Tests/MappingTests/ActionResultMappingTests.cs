using FluentValidation;
using FluentValidation.Results;
using Ghanavats.ResultPattern.Enums;
using Ghanavats.ResultPattern.Mapping;
using Ghanavats.ResultPattern.Tests.DummyData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Ghanavats.ResultPattern.Tests.MappingTests;

public class ActionResultMappingTests
{
    private readonly DummyController _controller = new()
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request = { Path = "/some/test/path" }
            }
        }
    };

    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapSuccessResultToActionResultAsync()
    {
        //arrange
        var okResult = Result.Success();
        
        //act
        var actual = await okResult.ToActionResultAsync(_controller);
        
        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as OkObjectResult;
        objectResult.ShouldNotBeNull();
        
        var objectResultValue = objectResult.Value as Result;
        objectResultValue.ShouldNotBeNull();
        objectResultValue.Status.ShouldBe(ResultStatus.Ok);
        objectResultValue.Data.ShouldBeNull();
    }
    
    [Fact]
    public void ResultMapping_ShouldCorrectlyMapSuccessResultToActionResult()
    {
        //arrange
        var okResult = Result.Success();
        
        //act
        var actual = okResult.ToActionResult(_controller);
        
        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as OkObjectResult;
        objectResult.ShouldNotBeNull();

        var objectResultValue = objectResult.Value as Result;
        objectResultValue.ShouldNotBeNull();
        objectResultValue.Status.ShouldBe(ResultStatus.Ok);
        objectResultValue.Data.ShouldBeNull();
    }
    
    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapErrorResultToActionResultAsync()
    {
        //arrange
        var errorResult = Result.Error("Some error");
        
        //act
        var actual = await errorResult.ToActionResultAsync(_controller);
        
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
        objectResultValue.Extensions["TraceId"].ShouldBe(_controller.HttpContext.TraceIdentifier);
        objectResultValue.Instance.ShouldNotBeNull();
    }

    [Fact]
    public void ResultMapping_ShouldCorrectlyMapErrorResultToActionResult()
    {
        //arrange
        var errorResult = Result.Error("Some error");
        
        //act
        var actual = errorResult.ToActionResult(_controller);
        
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
        objectResultValue.Extensions["TraceId"].ShouldBe(_controller.HttpContext.TraceIdentifier);
        objectResultValue.Instance.ShouldNotBeNull();
    }
    
    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapInvalidResultToActionResultAsync()
    {
        //arrange
        var invalidResult = Result.Invalid(new ValidationResult
        {
            Errors = [new ValidationFailure
            {
                ErrorCode = "Some error code",
                ErrorMessage = "Some message",
                PropertyName = "FieldNameTest1",
                Severity = Severity.Error
            }]
        });
        
        //act
        var actual = await invalidResult.ToActionResultAsync(_controller);
        
        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as BadRequestObjectResult;
        objectResult.ShouldNotBeNull();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        objectResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        
        var objectResultValue = objectResult.Value as ValidationProblemDetails;
        objectResultValue.ShouldNotBeNull();
        objectResultValue.Status.ShouldBe(StatusCodes.Status400BadRequest);
        objectResultValue.Errors.ShouldNotBeEmpty();
        objectResultValue.Errors.Keys.ToList()[0].ShouldBe("FieldNameTest1");
        objectResultValue.Errors.Values.ShouldNotBeEmpty();
        objectResultValue.Errors.Values.ToList()[0][0].ShouldBe("Some message");
        objectResultValue.Instance.ShouldNotBeNull();
    }
    
    [Fact]
    public void ResultMapping_ShouldCorrectlyMapInvalidResultToActionResult()
    {
        var invalidResult = Result.Invalid(new ValidationResult
        {
            Errors = [new ValidationFailure
            {
                ErrorCode = "Some error code 123",
                ErrorMessage = "Some message about validation",
                PropertyName = "FieldNameTest1223",
                Severity = Severity.Error
            }]
        });
        
        //act
        var actual = invalidResult.ToActionResult(_controller);
        
        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as BadRequestObjectResult;
        objectResult.ShouldNotBeNull();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status400BadRequest);
        objectResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        
        var objectResultValue = objectResult.Value as ValidationProblemDetails;
        objectResultValue.ShouldNotBeNull();
        objectResultValue.Status.ShouldBe(StatusCodes.Status400BadRequest);
        objectResultValue.Errors.ShouldNotBeEmpty();
        objectResultValue.Errors.Keys.ToList()[0].ShouldBe("FieldNameTest1223");
        objectResultValue.Errors.Values.ShouldNotBeEmpty();
        objectResultValue.Errors.Values.ToList()[0][0].ShouldBe("Some message about validation");
        objectResultValue.Instance.ShouldNotBeNull();
    }

    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapNotFoundResultToActionResultAsync()
    {
        //arrange
        var notFoundResult = Result.NotFound();
        
        //act
        var actual = await notFoundResult.ToActionResultAsync(_controller);
        
        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as NotFoundObjectResult;
        objectResult.ShouldNotBeNull();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        objectResult.Value.ShouldBeOfType<ProblemDetails>();
        
        var objectResultValue = objectResult.Value as ProblemDetails;
        objectResultValue.ShouldNotBeNull();
        objectResultValue.Status.ShouldBe(StatusCodes.Status404NotFound);
        objectResultValue.Instance.ShouldNotBeNull();
    }
    
    [Fact]
    public void ResultMapping_ShouldCorrectlyMapNotFoundResultToActionResult()
    {
        //arrange
        var notFoundResult = Result.NotFound();
        
        //act
        var actual = notFoundResult.ToActionResult(_controller);
        
        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as NotFoundObjectResult;
        objectResult.ShouldNotBeNull();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        objectResult.Value.ShouldBeOfType<ProblemDetails>();
        
        var objectResultValue = objectResult.Value as ProblemDetails;
        objectResultValue.ShouldNotBeNull();
        objectResultValue.Status.ShouldBe(StatusCodes.Status404NotFound);
        objectResultValue.Instance.ShouldNotBeNull();
    }
}
