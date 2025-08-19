using FluentValidation;
using FluentValidation.Results;
using Ghanavats.ResultPattern.Enums;
using Ghanavats.ResultPattern.Helpers;
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
        sut.IsSuccess().ShouldBeTrue();
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
        actual.IsSuccess().ShouldBeTrue();
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
        actual.IsSuccess().ShouldBeTrue();
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
        actual.IsSuccess().ShouldBeTrue();
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
        actual.IsSuccess().ShouldBeFalse();
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
        
        var actual = Result.Invalid(expectedValidationResult);
        
        //assert
        actual.ShouldNotBeNull();
        actual.ErrorMessages.ShouldBeEmpty();
        actual.Status.ShouldBe(ResultStatus.Invalid);
        actual.Data.ShouldBeNull();
        actual.IsSuccess().ShouldBeFalse();
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
        
        var actual = Result<DummyResponseModel>.Invalid(expectedValidationResult);
        
        //assert
        actual.ShouldNotBeNull();
        actual.ValidationErrorsByField.ShouldNotBeNull();
        actual.ErrorMessages.ShouldBeEmpty();
        actual.Status.ShouldBe(ResultStatus.Invalid);
        actual.Data.ShouldBeNull();
        actual.IsSuccess().ShouldBeFalse();

        foreach (var item in actual.ValidationErrorsByField)
        {
            item.Key.ShouldNotBeNullOrEmpty();
            item.Value.ShouldNotBeNull();
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
        actual.IsSuccess().ShouldBeFalse();
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
        actual.IsSuccess().ShouldBeTrue();
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
        actual.IsSuccess().ShouldBeTrue();
        actual.Data.ShouldBeNull();
        actual.ErrorMessages.ShouldBeEmpty();
        actual.SuccessMessage.ShouldBeEmpty();
        actual.Status.ShouldBe(ResultStatus.Ok);
    }
}
