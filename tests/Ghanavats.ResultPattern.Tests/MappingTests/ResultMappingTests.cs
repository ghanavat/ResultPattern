using FluentValidation;
using FluentValidation.Results;
using Ghanavats.ResultPattern.Enums;
using Ghanavats.ResultPattern.Helpers;
using Ghanavats.ResultPattern.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shouldly;

namespace Ghanavats.ResultPattern.Tests.MappingTests;

public class ResultMappingTests
{
    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapSuccessResultToResultAsync()
    {
        //arrange
        var expectedResult = Result.Success();
        
        //act
        var actual = await expectedResult.ToResultAsync();

        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as Ok<Result>;
        objectResult.ShouldNotBeNull();
        objectResult.Value.ShouldBeOfType<Result>();
        objectResult.Value.Data.ShouldBeNull();
        objectResult.StatusCode.ShouldBe(200);
    }
    
    [Fact]
    public void ResultMapping_ShouldCorrectlyMapSuccessResultToResult()
    {
        //arrange
        var expectedResult = Result.Success();
        
        //act
        var actual = expectedResult.ToResult();

        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as Ok<Result>;
        objectResult.ShouldNotBeNull();
        objectResult.Value.ShouldBeOfType<Result>();
        objectResult.Value.Data.ShouldBeNull();
        objectResult.StatusCode.ShouldBe(200);
    }
    
    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapErrorResultToResultAsync()
    {
        //arrange
        var expectedResult = Result.Error("Error message 1");
        
        //act
        var actual = await expectedResult.ToResultAsync();
        await actual.ExecuteAsync(new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        });
        
        //assert
        actual.ShouldNotBeNull();
        actual.ShouldBeOfType<BetterProblemDetailsFactory<ProblemDetails>>();
        
        var objectResult = actual as BetterProblemDetailsFactory<ProblemDetails>;
        objectResult.ShouldNotBeNull();
        
        objectResult.Value.ShouldBeOfType<ProblemDetails>();
        objectResult.Value.Title.ShouldBe("Unknown. There has been a problem with your request.");
        objectResult.Value.Detail.ShouldBe("Error message 1");
        objectResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
    }
    
    [Theory]
    [InlineData(ErrorKind.Conflict)]
    [InlineData(ErrorKind.Unauthorized)]
    [InlineData(ErrorKind.Forbidden)]
    [InlineData(ErrorKind.RateLimited)]
    [InlineData(ErrorKind.Timeout)]
    [InlineData(ErrorKind.DependencyFailure)]
    [InlineData(ErrorKind.Unknown)]
    public async Task ResultMapping_ShouldCorrectlyMapErrorResultOfCustomStatusCodeToResultAsync(ErrorKind expectedErrorkind)
    {
        //arrange
        var expectedResult = Result.Error("Error message 1", expectedErrorkind);
        var expectedStatusCode = MappingErrorKind.MapStatusCode(expectedResult);
        
        //act
        var actual = await expectedResult.ToResultAsync();
        await actual.ExecuteAsync(new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        });
        
        //assert
        actual.ShouldNotBeNull();
        actual.ShouldBeOfType<BetterProblemDetailsFactory<ProblemDetails>>();
        
        var objectResult = actual as BetterProblemDetailsFactory<ProblemDetails>;
        objectResult.ShouldNotBeNull();
        
        objectResult.Value.ShouldBeOfType<ProblemDetails>();
        objectResult.Value.Status.ShouldBe(expectedStatusCode);
    }
    
    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapErrorResultToResult()
    {
        //arrange
        var expectedResult = Result.Error("Error message 2");
        
        //act
        var actual = expectedResult.ToResult();
        await actual.ExecuteAsync(new DefaultHttpContext
        {
            Request = { Path = "/some/test/path" },
            Response = { Body = new MemoryStream() }
        });
        
        //assert
        actual.ShouldNotBeNull();
        actual.ShouldBeOfType<BetterProblemDetailsFactory<ProblemDetails>>();
        
        var objectResult = actual as BetterProblemDetailsFactory<ProblemDetails>;
        objectResult.ShouldNotBeNull();
        
        objectResult.Value.ShouldBeOfType<ProblemDetails>();
        objectResult.Value.Title.ShouldBe("Unknown. There has been a problem with your request.");
        objectResult.Value.Detail.ShouldBe("Error message 2");
        objectResult.StatusCode.ShouldBe(StatusCodes.Status500InternalServerError);
        objectResult.Value.Instance.ShouldNotBeNull();
    }
    
    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapInvalidResultToResultAsync()
    {
        //arrange
        var expectedResult = Result.Invalid(new ValidationResult
        {
            Errors = [new ValidationFailure
            {
                ErrorCode = "Error code test",
                ErrorMessage = "Error message test 1",
                PropertyName = "PropertyTest1",
                Severity = Severity.Error
            }]
        });
        
        //act
        var actual = await expectedResult.ToResultAsync();
        await actual.ExecuteAsync(new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        });
        
        //assert
        actual.ShouldNotBeNull();
        actual.ShouldBeOfType<BetterProblemDetailsFactory<ValidationProblemDetails>>();
        
        var objectResult = actual as BetterProblemDetailsFactory<ValidationProblemDetails>;
        objectResult.ShouldNotBeNull();
        
        objectResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        objectResult.Value.Title.ShouldBe("Invalid request.");
        objectResult.Value.Detail.ShouldBe("Your request is invalid. The details are populated in Errors.");
        objectResult.Value.Errors.ShouldNotBeEmpty();
        objectResult.Value.Status.ShouldBe(StatusCodes.Status400BadRequest);
    }
    
    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapInvalidResultToResult()
    {
        //arrange
        var expectedResult = Result.Invalid(new ValidationResult
        {
            Errors = [new ValidationFailure
            {
                ErrorCode = "Error code test 2",
                ErrorMessage = "Error message test 1",
                PropertyName = "PropertyTest2",
                Severity = Severity.Error
            }]
        });
        
        //act
        var actual = expectedResult.ToResult();
        await actual.ExecuteAsync(new DefaultHttpContext
        {
            Request = { Path = "/some/test/path" },
            Response = { Body = new MemoryStream() }
        });
        
        //assert
        actual.ShouldNotBeNull();
        actual.ShouldBeOfType<BetterProblemDetailsFactory<ValidationProblemDetails>>();
        
        var objectResult = actual as BetterProblemDetailsFactory<ValidationProblemDetails>;
        objectResult.ShouldNotBeNull();
        
        objectResult.Value.ShouldBeOfType<ValidationProblemDetails>();
        objectResult.Value.Title.ShouldBe("Invalid request.");
        objectResult.Value.Detail.ShouldBe("Your request is invalid. The details are populated in Errors.");
        objectResult.Value.Errors.ShouldNotBeEmpty();
        objectResult.Value.Status.ShouldBe(StatusCodes.Status400BadRequest);
        objectResult.Value.Instance.ShouldNotBeNull();
    }

    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapNotFoundResultToResultAsync()
    {
        //arrange
        var expectedResult = Result.NotFound();
        
        //act
        var actual = await expectedResult.ToResultAsync();
        await actual.ExecuteAsync(new DefaultHttpContext
        {
            Request = { Path = "/some/test/path" },
            Response =
            {
                Body = new MemoryStream()
            }
        });
        
        //assert
        actual.ShouldNotBeNull();
        actual.ShouldBeOfType<BetterProblemDetailsFactory<ProblemDetails>>();
        
        var objectResult = actual as BetterProblemDetailsFactory<ProblemDetails>;
        objectResult.ShouldNotBeNull();
        
        objectResult.Value.ShouldBeOfType<ProblemDetails>();
        objectResult.Value.Title.ShouldBe("Not found.");
        objectResult.Value.Detail.ShouldBe("The requested resource was not found.");
        objectResult.Value.Status.ShouldBe(StatusCodes.Status404NotFound);
        objectResult.Value.Instance.ShouldNotBeNull();
    }
    
    [Fact]
    public async Task ResultMapping_ShouldCorrectlyMapNotFoundResultToResult()
    {
        //arrange
        var expectedResult = Result.NotFound();
        
        //act
        var actual = expectedResult.ToResult();
        await actual.ExecuteAsync(new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        });
        
        //assert
        actual.ShouldNotBeNull();
        actual.ShouldBeOfType<BetterProblemDetailsFactory<ProblemDetails>>();
        
        var objectResult = actual as BetterProblemDetailsFactory<ProblemDetails>;
        objectResult.ShouldNotBeNull();
        
        objectResult.Value.ShouldBeOfType<ProblemDetails>();
        objectResult.Value.Title.ShouldBe("Not found.");
        objectResult.Value.Detail.ShouldBe("The requested resource was not found.");
        objectResult.Value.Status.ShouldBe(StatusCodes.Status404NotFound);
    }
}
