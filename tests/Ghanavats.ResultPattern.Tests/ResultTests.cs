using FluentValidation;
using FluentValidation.Results;
using Ghanavats.ResultPattern.Enums;
using Ghanavats.ResultPattern.Extensions;
using Ghanavats.ResultPattern.Tests.DummyData;
using Shouldly;

namespace Ghanavats.ResultPattern.Tests;

public class ResultTests
{
    [Fact]
    public void ResultConstructorWithData_ShouldBeInitialised_WithCorrectValueAndStatus()
    {
        //Arrange/Act
        var sut = new Result<int>(1234);
        
        //Assert
        sut.ShouldNotBeNull();
        sut.Status.ShouldBe(ResultStatus.Ok);
    }
    
    [Fact]
    public void ResultConstructorWithStatus_ShouldBeInitialised_WithCorrectValueAndStatus()
    {
        //Arrange/Act
        var sut = new Result<int>(ResultStatus.Ok);
        
        //Assert
        sut.ShouldNotBeNull();
        sut.IsSuccess.ShouldBeTrue();
        sut.Status.ShouldBe(ResultStatus.Ok);
    }

    [Fact]
    public void ResultSuccessWithDataAndSuccessMessage_ShouldCorrectlySetStatusWithData()
    {
        //arrange/act
        var actual = Result<string>.Success("SomeValue", "Successfully executed.");
        
        //assert
        actual.ShouldNotBeNull();
        actual.ErrorMessages.ShouldBeEmpty();
        actual.Status.ShouldBe(ResultStatus.Ok);
        actual.Data.ShouldNotBeNull();
        actual.Data.GetType().Name.ShouldBe("String");
        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessMessage.ShouldBe("Successfully executed.");
    }
    
    [Fact]
    public void ResultSuccessWithData_ShouldCorrectlySetStatusWithData()
    {
        //arrange/act
        var actual = Result<int>.Success(1234);
        
        //assert
        actual.ShouldNotBeNull();
        actual.ErrorMessages.ShouldBeEmpty();
        actual.Status.ShouldBe(ResultStatus.Ok);
        actual.Data.ShouldBe(1234);
        actual.IsSuccess.ShouldBeTrue();
    }
    
    [Fact]
    public void ResultNonGenericSuccessWithDataAndSuccessMessage_ShouldCorrectlySetStatusWithData()
    {
        //arrange/act
        var actual = Result.Success();
        
        //assert
        actual.ShouldNotBeNull();
        actual.ErrorMessages.ShouldBeEmpty();
        actual.Status.ShouldBe(ResultStatus.Ok);
        actual.Data.ShouldBeNull();
        actual.IsSuccess.ShouldBeTrue();
    }
    
    [Fact]
    public void ResultError_ShouldCorrectlySetStatusWithNullData()
    {
        //arrange/act
        var actual = Result<string>.Error("Something wrong happened.");
        
        //assert
        actual.ShouldNotBeNull();
        actual.ErrorMessages.ShouldNotBeEmpty();
        actual.Status.ShouldBe(ResultStatus.Error);
        actual.Data.ShouldBeNull();
        actual.IsSuccess.ShouldBeFalse();
    }
    
    [Fact]
    public void ResultNonGenericInvalid_ShouldCorrectlySetStatusAndValidationErrors()
    {
        //arrange/act
        var expectedValidationResult = new ValidationResult
        {
            Errors = [
                new ValidationFailure
                {
                    ErrorMessage = "Test validation failure message.",
                    ErrorCode = "ErrorCodeTest",
                    PropertyName = "TestProperty",
                    Severity = Severity.Error
                }
            ]
        };
        
        var actual = Result.Invalid(expectedValidationResult.PopulateValidationErrors());
        
        //assert
        actual.ShouldNotBeNull();
        actual.ValidationErrors.ShouldNotBeEmpty();
        actual.ErrorMessages.ShouldBeEmpty();
        actual.Status.ShouldBe(ResultStatus.Invalid);
        actual.Data.ShouldBeNull();
        actual.IsSuccess.ShouldBeFalse();
    }
    
    [Fact]
    public void ResultInvalid_ShouldCorrectlySetStatusAndValidationErrors()
    {
        //arrange/act
        var expectedValidationResult = new ValidationResult
        {
            Errors = [
                new ValidationFailure
                {
                    ErrorMessage = "Test validation failure message.",
                    ErrorCode = "ErrorCodeTest",
                    PropertyName = "DummyFieldOne",
                    Severity = Severity.Error
                }
            ]
        };
        
        var actual = Result<DummyResponseModel>.Invalid(expectedValidationResult.PopulateValidationErrors());
        
        //assert
        actual.ShouldNotBeNull();
        actual.ValidationErrors.ShouldNotBeEmpty();
        actual.ErrorMessages.ShouldBeEmpty();
        actual.Status.ShouldBe(ResultStatus.Invalid);
        actual.Data.ShouldBeNull();
        actual.IsSuccess.ShouldBeFalse();

        foreach (var item in actual.ValidationErrors)
        {
            item.ErrorMessage.ShouldNotBeNullOrEmpty();
            item.ErrorCode.ShouldNotBeNullOrEmpty();
            item.ValidationErrorType.ShouldBe(ValidationErrorType.Error);
        }
    }
    
    [Fact]
    public void ResultNotFound_ShouldCorrectlySetStatus()
    {
        //arrange/act
        var actual = Result<string>.NotFound();
        
        //assert
        actual.ShouldNotBeNull();
        actual.Status.ShouldBe(ResultStatus.NotFound);
        actual.Data.ShouldBeNull();
        actual.IsSuccess.ShouldBeFalse();
    }
    
    [Fact]
    public void Result_ShouldAggregateAllResultsCorrectly_WhenItIsCalledWithMultipleResultsWithoutFullValidationErrorsExtension()
    {
        //arrange
        var result1 = Result.Invalid([new ValidationError("Validation error Jan", "3434", ValidationErrorType.Error)]);
        var result2 = Result.Invalid([new ValidationError("Validation error Feb", "5554", ValidationErrorType.Error)]);
        var result3 = Result.Invalid([new ValidationError("Validation error March", "6678", ValidationErrorType.Error)]);
        
        var result4 = Result.Error("Failure Error 234");
        var result5 = Result.Error("Failure Error 2676");
        var result6 = Result.Error("Failure Error 3009");
        
        //act
        var actual = Result.Aggregate(result1, result2, result3, result4, result5, result6);
        
        //assert
        actual.ShouldNotBeNull();
        actual.ShouldNotBeEmpty();
        actual.Count.ShouldBe(2);
        
        actual.ToList().Where(x => x.Status == ResultStatus.Error).ShouldNotBeEmpty();
        
        var invalidResult = actual.ToList().Where(x => x.Status == ResultStatus.Invalid).ToList();
        invalidResult.ShouldNotBeEmpty();
        invalidResult.Count.ShouldBe(1);

        foreach (var item in invalidResult)
        {
            item.ValidationErrors.ShouldBeEmpty();
            item.Messages.ShouldNotBeEmpty();
        }
        
        foreach (var item in actual.ToList())
        {
            item.Messages.ShouldNotBeEmpty();
            item.Messages.ShouldBeAssignableTo<IReadOnlyCollection<string>>();
        }
    }
    
    [Fact]
    public void Result_ShouldAggregateAllResultsCorrectly_WhenItIsCalledWithMultipleResults_AndWithFullValidationErrorsExtensionUsed()
    {
        //arrange
        var result1 = Result.Invalid([new ValidationError("Validation error April", "666", ValidationErrorType.Error)]);
        var result2 = Result.Invalid([new ValidationError("Validation error May", "777", ValidationErrorType.Error)]);
        var result3 = Result.Invalid([new ValidationError("Validation error June", "888", ValidationErrorType.Error)]);
        
        var result4 = Result.Error("Failure Error 999");
        var result5 = Result.Error("Failure Error 101010");
        var result6 = Result.Error("Failure Error 111111");
        
        //act
        var actual = Result.Aggregate(result1, result2, result3, result4, result5, result6)
            .WithFullValidationErrors();
        
        //assert
        actual.ShouldNotBeNull();
        actual.ShouldNotBeEmpty();
        actual.Count.ShouldBe(2);
        
        actual.ToList().Where(x => x.Status == ResultStatus.Error).ShouldNotBeEmpty();
        
        var invalidResult = actual.ToList().Where(x => x.Status == ResultStatus.Invalid).ToList();
        invalidResult.ShouldNotBeEmpty();
        invalidResult.Count.ShouldBe(1);

        foreach (var item in invalidResult)
        {
            item.ValidationErrors.ShouldNotBeEmpty();
            item.ValidationErrors.Count.ShouldBe(3);
            item.Messages.ShouldBeEmpty();
        }
    }

    [Fact]
    public void Result_ShouldIgnoreAggregatingNoneOkNotFoundStatuses()
    {
        //arrange
        var result1 = Result.Success();
        var result2 = Result.NotFound();
        
        //act
        var action = Result.Aggregate([result1, result2]);
        
        //assert
        action.ShouldNotBeNull();
        action.ShouldBeEmpty();
    }
    
    [Fact]
    public void Result_ShouldIgnoreAggregatingNoneOkNotFound_WhenMixedWithErrorStatusResults()
    {
        //arrange
        var result1 = Result.Success();
        var result2 = Result.NotFound();
        var result3 = Result.Error("Something went wrong");
        
        //act
        var actual = Result.Aggregate(result1, result2, result3).WithFullValidationErrors();
        
        //assert 
        actual.ShouldNotBeNull();
        actual.ShouldNotBeEmpty();
        actual.Count.ShouldBe(1);
        actual.ToList()[0].Status.ShouldBe(ResultStatus.Error);
    }
    
    [Fact]
    public void Result_ImplicitOperator_ShouldCorrectlyConvertTheDataTypeToResultOfTypeImplicitly()
    {
        //arrange
        var sut = new DummyFeatureService();
        
        //act
        var actual = sut.DoSomething();

        //assert
        actual.ShouldNotBeNull();
        actual.ShouldBeOfType<Result<DummyResponseModel>>();
        actual.IsSuccess.ShouldBeTrue();
        actual.Data.ShouldNotBeNull();
    }
    
    [Fact]
    public void Result_ImplicitOperator_ShouldCorrectlyConvertTheResultOfGenericTypeToTheDataType()
    {
        //arrange
        var sut = new DummyFeatureService();
        
        //act
        var actual = sut.DoSomethingImplicitConversionOfResultGenericToDataType();

        //assert
        actual.ShouldNotBeNull();
        actual.ShouldBeOfType<DummyResponseModel>();
    }
    
    [Fact]
    public void Result_ImplicitOperator_ShouldCorrectlyConvertNonGenericResultToGenericResultWithDefaultState()
    {
        //arrange
        var sut = new DummyFeatureService();
        
        //act
        var actual = sut.DoSomethingImplicitConversionOfNonGenericResultToGenericResultWithDefault();

        //assert
        actual.ShouldNotBeNull();
        actual.ShouldBeOfType<Result<DummyResponseModel>>();
        actual.IsSuccess.ShouldBeTrue();
        actual.Data.ShouldBeNull();
        actual.ErrorMessages.ShouldBeEmpty();
        actual.ValidationErrors.ShouldBeEmpty();
        actual.SuccessMessage.ShouldBeEmpty();
        actual.Status.ShouldBe(ResultStatus.Ok);
    }
}
