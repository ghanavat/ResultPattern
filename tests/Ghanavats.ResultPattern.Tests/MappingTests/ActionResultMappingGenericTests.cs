using FluentValidation;
using FluentValidation.Results;
using Ghanavats.ResultPattern.Enums;
using Ghanavats.ResultPattern.Helpers;
using Ghanavats.ResultPattern.Mapping;
using Ghanavats.ResultPattern.Tests.DummyData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Ghanavats.ResultPattern.Tests.MappingTests;

public class ActionResultMappingGenericTests
{
    private readonly DummyController _controller = new()
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        }
    };

    [Fact]
    public async Task ActionResultMapping_ShouldCorrectlyMapSuccessResultGenericToActionResultAsync()
    {
        //arrange
        var okResult = Result<string>.Success("Some data", "Some message");
        
        //act
        var actual = await okResult.ToActionResultAsync(_controller);
        
        //assert
        actual.ShouldNotBeNull();
        okResult.IsSuccess().ShouldBeTrue();
        
        var objectResult = actual as OkObjectResult;
        objectResult.ShouldNotBeNull();
        
        var objectResultValue = objectResult.Value as Result<string>;
        objectResultValue.ShouldNotBeNull();
        objectResultValue.Status.ShouldBe(ResultStatus.Ok);
        objectResultValue.Data.ShouldBe("Some data");
    }
    
    [Fact]
    public void ActionResultMapping_ShouldCorrectlyMapSuccessResultGenericToActionResult()
    {
        //arrange
        var okResult = Result<string>.Success("Some data");
        
        //act
        var actual = okResult.ToActionResult(_controller);
        
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
    public async Task ActionResultMapping_ShouldCorrectlyMapErrorResultGenericToActionResultAsync()
    {
        //arrange
        var errorResult = Result<string>.Error("Some error");
        
        //act
        var actual = await errorResult.ToActionResultAsync(_controller);
        
        //assert
        actual.ShouldNotBeNull();
        errorResult.IsError().ShouldBeTrue();
        
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
    }

    [Fact]
    public void ActionResultMapping_ShouldCorrectlyMapErrorResultGenericToActionResult()
    {
        //arrange
        var errorResult = Result<string>.Error("Some error");
        
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
    }
    
    [Fact]
    public async Task ActionResultMapping_ShouldCorrectlyMapInvalidResultGenericToActionResultAsync()
    {
        //arrange
        var invalidResult = Result<string>.Invalid(new ValidationResult
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
        invalidResult.IsInvalid().ShouldBeTrue();
        
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
    }
    
    [Fact]
    public void ActionResultMapping_ShouldCorrectlyMapInvalidResultGenericToActionResult()
    {
        var invalidResult = Result<string>.Invalid(new ValidationResult
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
    }

    [Fact]
    public async Task ActionResultMapping_ShouldCorrectlyMapNotFoundResultGenericToActionResultAsync()
    {
        //arrange
        var notFoundResult = Result<string>.NotFound();
        
        //act
        var actual = await notFoundResult.ToActionResultAsync(_controller);
        
        //assert
        actual.ShouldNotBeNull();
        notFoundResult.IsNotFound().ShouldBeTrue();
        
        var objectResult = actual as NotFoundObjectResult;
        objectResult.ShouldNotBeNull();
        objectResult.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
        objectResult.Value.ShouldBeOfType<ProblemDetails>();
        
        var objectResultValue = objectResult.Value as ProblemDetails;
        objectResultValue.ShouldNotBeNull();
        objectResultValue.Status.ShouldBe(StatusCodes.Status404NotFound);
    }
    
    [Fact]
    public void ActionResultMapping_ShouldCorrectlyMapNotFoundResultGenericToActionResult()
    {
        //arrange
        var notFoundResult = Result<string>.NotFound();
        
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
    }
}
