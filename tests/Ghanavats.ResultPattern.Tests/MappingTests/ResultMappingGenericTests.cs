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

public class ResultMappingGenericTests
{
    [Fact]
    public async Task ResultMappingGeneric_ShouldCorrectlyMapSuccessResultGenericToResultAsync()
    {
        //arrange
        var expectedResult = Result<string>.Success("Data being returned");
        
        //act
        var actual = await expectedResult.ToResultAsync();

        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as Ok<Result<string>>;
        objectResult.ShouldNotBeNull();
        objectResult.Value.ShouldBeOfType<Result<string>>();
        objectResult.Value.Data.ShouldBe("Data being returned");
        objectResult.StatusCode.ShouldBe(200);
    }
    
    [Fact]
    public void ResultMappingGeneric_ShouldCorrectlyMapSuccessResultGenericToResult()
    {
        //arrange
        var expectedResult = Result<string>.Success("Data being returned");
        
        //act
        var actual = expectedResult.ToResult();

        //assert
        actual.ShouldNotBeNull();
        
        var objectResult = actual as Ok<Result<string>>;
        objectResult.ShouldNotBeNull();
        objectResult.Value.ShouldBeOfType<Result<string>>();
        objectResult.Value.Data.ShouldBe("Data being returned");
        objectResult.StatusCode.ShouldBe(200);
    }
    
    [Fact]
    public async Task ResultMappingGeneric_ShouldCorrectlyMapErrorResultGenericToResultAsync()
    {
        //arrange
        var expectedResult = Result<string>.Error("Error message 1");
        
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
        objectResult.Value.Status.ShouldBe(StatusCodes.Status500InternalServerError);
    }
    
    [Fact]
    public async Task ResultMappingGeneric_ShouldCorrectlyMapErrorResultGenericToResult()
    {
        //arrange
        var expectedResult = Result<string>.Error("Error message 2");
        
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
        objectResult.Value.Status.ShouldBe(StatusCodes.Status500InternalServerError);
        objectResult.Value.Instance.ShouldNotBeNull();
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
        var expectedResult = Result<string>.Error("Error message 1", expectedErrorkind);
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
        objectResult.StatusCode.ShouldBe(expectedStatusCode);
    }
    
    [Fact]
    public async Task ResultMappingGeneric_ShouldCorrectlyMapInvalidResultGenericToResultAsync()
    {
        //arrange
        var expectedResult = Result<string>.Invalid(new ValidationResult
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
    public async Task ResultMappingGeneric_ShouldCorrectlyMapInvalidResultGenericToResult()
    {
        //arrange
        var expectedResult = Result<string>.Invalid(new ValidationResult
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
    public async Task ResultMappingGeneric_ShouldCorrectlyMapNotFoundResultGenericToResultAsync()
    {
        //arrange
        var expectedResult = Result<string>.NotFound();
        
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
        objectResult.Value.Title.ShouldBe("Not found.");
        objectResult.Value.Detail.ShouldBe("The requested resource was not found.");
        objectResult.Value.Status.ShouldBe(StatusCodes.Status404NotFound);
    }
    
    [Fact]
    public async Task ResultMappingGeneric_ShouldCorrectlyMapNotFoundResultGenericToResult()
    {
        //arrange
        var expectedResult = Result<string>.NotFound();
        
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
        objectResult.Value.Title.ShouldBe("Not found.");
        objectResult.Value.Detail.ShouldBe("The requested resource was not found.");
        objectResult.Value.Status.ShouldBe(StatusCodes.Status404NotFound);
        objectResult.Value.Instance.ShouldNotBeNull();
    }
}
