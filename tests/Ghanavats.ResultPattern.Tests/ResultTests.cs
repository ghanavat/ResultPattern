using Ghanavats.ResultPattern.Enums;
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
    
    // [Fact]
    // public void ResultConstructorWithStatus_ShouldBeInitialised_WithCorrectValueAndStatus()
    // {
    //     //Arrange/Act
    //     var sut = new Result(ResultStatus.Ok);
    //     
    //     //Assert
    //     sut.ShouldNotBeNull();
    //     sut.Status.ShouldBe(ResultStatus.Ok);
    // }

    [Fact]
    public void ResultAggregate_ShouldCorrectlyCombineAllResults_WhenThereAreErrors()
    {
        //arrange
        var result1 = Result.Error("Error1");
        var result2 = Result.Error("Error2");
        var result3 = Result.Error("Error3");
        
        //act
        var actual = Result.Aggregate(result1, result2, result3);
        
        //assert
        actual.ShouldNotBeNull();
        actual.Status.ShouldBe(ResultStatus.Error);
        actual.ErrorMessages.ShouldNotBeEmpty();
        actual.ErrorMessages.Count().ShouldBe(3);
    }
    
    [Fact]
    public void ResultAggregate_ShouldCorrectlyCombineAllResults_WhenThereAreValidationErrors()
    {
        //arrange
        var result1 = Result.Invalid([new ValidationError("Validation error 1", "123", ValidationErrorType.Error)]);
        var result2 = Result.Invalid([new ValidationError("Validation error 2", "1234", ValidationErrorType.Error)]);
        var result3 = Result.Invalid([new ValidationError("Validation error 3", "1235", ValidationErrorType.Error)]);
        
        //act
        var actual = Result.Aggregate(result1, result2, result3);
        
        //assert
        actual.ShouldNotBeNull();
        actual.Status.ShouldBe(ResultStatus.Invalid);
        actual.ValidationErrors.ShouldNotBeEmpty();
        actual.ValidationErrors.Count().ShouldBe(3);
    }
    
    [Fact]
    public void ResultAggregate_ShouldCorrectlyCombineAllResults_WhenThereAreValidationErrorsAndErrors()
    {
        //arrange
        var result1 = Result.Invalid([new ValidationError("Validation error Jan", "3434", ValidationErrorType.Error)]);
        var result2 = Result.Invalid([new ValidationError("Validation error Feb", "5554", ValidationErrorType.Error)]);
        var result3 = Result.Invalid([new ValidationError("Validation error March", "6678", ValidationErrorType.Error)]);
        
        var result4 = Result.Error("Error234");
        var result5 = Result.Error("Error2676");
        var result6 = Result.Error("Error3009");
        
        //act
        var actual = Result.Aggregate(result1, result2, result3, result4, result5, result6);
        
        //assert
        actual.ShouldNotBeNull();
        actual.Status.ShouldBe(ResultStatus.Error);
        
        actual.ErrorMessages.ShouldNotBeEmpty();
        actual.ErrorMessages.Count().ShouldBe(3);
        
        actual.ValidationErrors.ShouldNotBeEmpty();
        actual.ValidationErrors.Count().ShouldBe(3);
    }

    [Fact]
    public void ResultAggregate_ShouldReturnCorrectResultStatus_WhenAllResultsAreOK()
    {
        //arrange
        var result1 = Result.Success();
        var result2 = Result.Success();
        var result3 = Result.Success();
        
        //act
        var actual = Result.Aggregate(result1, result2, result3);
        
        //assert
        actual.ShouldNotBeNull();
        actual.Status.ShouldBe(ResultStatus.Ok);
        actual.ErrorMessages.ShouldBeEmpty();
        actual.ValidationErrors.ShouldBeEmpty();
    }
}
