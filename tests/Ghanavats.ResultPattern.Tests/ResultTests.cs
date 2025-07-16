using Ghanavats.ResultPattern.Enums;
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
    public void Result_ShouldAggregateAllResults_WhenItIsCalledWithMultipleResults_AndIncludeValidationErrorsIsFalse()
    {
        //arrange
        var result1 = Result.Invalid([new ValidationError("Validation error Jan", "3434", ValidationErrorType.Error)]);
        var result2 = Result.Invalid([new ValidationError("Validation error Feb", "5554", ValidationErrorType.Error)]);
        var result3 = Result.Invalid([new ValidationError("Validation error March", "6678", ValidationErrorType.Error)]);
        
        var result4 = Result.Error("Failure Error 234");
        var result5 = Result.Error("Failure Error 2676");
        var result6 = Result.Error("Failure Error 3009");
        
        //act
        var actual = Result.Aggregate(false, result1, result2, result3, result4, result5, result6);
        
        //assert
        actual.ShouldNotBeNull();
        actual.ShouldNotBeEmpty();
        actual.Count.ShouldBe(2);
        
        actual.ToList().Where(x => x.Status == ResultStatus.Error).ShouldNotBeEmpty();
        actual.ToList().Where(x => x.Status == ResultStatus.Invalid).ShouldNotBeEmpty();
        
        foreach (var item in actual.ToList())
        {
            item.Messages.ShouldNotBeEmpty();
            item.Messages.ShouldBeAssignableTo<IReadOnlyCollection<string>>();
        }
    }
    
    [Fact]
    public void Result_ShouldAggregateAllResults_WhenItIsCalledWithMultipleResults_AndIncludeValidationErrorsIsTrue()
    {
        //arrange
        var result1 = Result.Invalid([new ValidationError("Validation error April", "666", ValidationErrorType.Error)]);
        var result2 = Result.Invalid([new ValidationError("Validation error May", "777", ValidationErrorType.Error)]);
        var result3 = Result.Invalid([new ValidationError("Validation error June", "888", ValidationErrorType.Error)]);
        
        var result4 = Result.Error("Failure Error 999");
        var result5 = Result.Error("Failure Error 101010");
        var result6 = Result.Error("Failure Error 111111");
        
        //act
        var actual = Result.Aggregate(true, result1, result2, result3, result4, result5, result6);
        
        //assert
        actual.ShouldNotBeNull();
        actual.ShouldNotBeEmpty();
        actual.Count.ShouldBe(2);
        
        actual.ToList().Where(x => x.Status == ResultStatus.Error).ShouldNotBeEmpty();
        actual.ToList().Where(x => x.Status == ResultStatus.Invalid).ShouldNotBeEmpty();

        var validationErrors = actual.Where(x => x.Status == ResultStatus.Invalid)
            .SelectMany(y => y.Messages.Select(x => x)).ToList().AsReadOnly();
        
        validationErrors.ShouldNotBeEmpty();

        foreach (var item in validationErrors)
        {
            item.ShouldBeAssignableTo<ValidationError>();
            
            var validationError = item as ValidationError;
            validationError.ShouldNotBeNull();
            validationError.ValidationErrorType.ShouldBe(ValidationErrorType.Error);
            validationError.ErrorMessage.ShouldNotBeEmpty();
            validationError.ErrorMessage.ShouldContain("Validation error");
        }
        
        var errors = actual.Where(x => x.Status == ResultStatus.Error)
            .SelectMany(y => y.Messages.Select(x => x)).ToList().AsReadOnly();
        
        errors.ShouldNotBeEmpty();

        foreach (var item in errors)
        {
            item.ShouldBeAssignableTo<string>();
            
            var errorMessage = item as string;
            errorMessage.ShouldNotBeNull();
        }
    }
    
    [Fact]
    public void ResultOfTypeString_ShouldAggregateAllResults_WhenItIsCalledWithMultipleResults_AndIncludeValidationErrorsIsFalse()
    {
        //arrange
        var result1 = Result<string>.Invalid([new ValidationError("Validation error April", "666", ValidationErrorType.Error)]);
        var result2 = Result<string>.Invalid([new ValidationError("Validation error May", "777", ValidationErrorType.Error)]);
        
        var result4 = Result<string>.Error("Failure Error 999");
        var result5 = Result<string>.Error("Failure Error 101010");
        
        //act
        var actual = Result<string>.Aggregate(false, result1, result2, result4, result5);
        
        //assert
        var actualList = actual.ToList();
        actualList.ShouldNotBeEmpty();
        actualList.Where(x => x.Status == ResultStatus.Error).ShouldNotBeEmpty();
        actualList.Where(x => x.Status == ResultStatus.Invalid).ShouldNotBeEmpty();

        foreach (var item in actualList)
        {
            item.TypeName.ShouldNotBeNull();
            item.TypeName.GetType().Name.ShouldBe("String");
        }
        
        var validationErrors = actual.Where(x => x.Status == ResultStatus.Invalid)
            .SelectMany(y => y.Messages.Select(x => x)).ToList().AsReadOnly();
        
        validationErrors.ShouldNotBeEmpty();

        foreach (var item in validationErrors)
        {
            item.ShouldBeAssignableTo<string>();
            
            var validationError = item as string;
            validationError.ShouldNotBeNull();
            validationError.ShouldContain("Validation error");
        }
        
        var errors = actual.Where(x => x.Status == ResultStatus.Error)
            .SelectMany(y => y.Messages.Select(x => x)).ToList().AsReadOnly();
        
        errors.ShouldNotBeEmpty();
        
        foreach (var item in errors)
        {
            item.ShouldBeAssignableTo<string>();
            
            var errorMessage = item as string;
            errorMessage.ShouldNotBeNull();
        }
    }
    
    [Fact]
    public void ResultOfComplexType_ShouldAggregateAllResults_WhenItIsCalledWithMultipleResults_AndIncludeValidationErrorsIsFalse()
    {
        //arrange
        var result1 = Result<DummyResponseModel>.Invalid([new ValidationError("Validation error June", "668787", ValidationErrorType.Error)]);
        var result2 = Result<DummyResponseModel>.Invalid([new ValidationError("Validation error July", "4847", ValidationErrorType.Error)]);
        
        var result4 = Result<DummyResponseModel>.Error("Failure Error 89393");
        var result5 = Result<DummyResponseModel>.Error("Failure Error 111112");
        
        //act
        var actual = Result<DummyResponseModel>.Aggregate(false, result1, result2, result4, result5);
        
        //assert
        var actualList = actual.ToList();
        actualList.ShouldNotBeEmpty();
        actualList.Where(x => x.Status == ResultStatus.Error).ShouldNotBeEmpty();
        actualList.Where(x => x.Status == ResultStatus.Invalid).ShouldNotBeEmpty();

        foreach (var item in actualList)
        {
            item.TypeName.ShouldNotBeNull();
            item.TypeName.ShouldBe(nameof(DummyResponseModel));
        }
        
        var validationErrors = actual.Where(x => x.Status == ResultStatus.Invalid)
            .SelectMany(y => y.Messages.Select(x => x)).ToList().AsReadOnly();
        
        validationErrors.ShouldNotBeEmpty();

        foreach (var item in validationErrors)
        {
            item.ShouldBeAssignableTo<string>();
            
            var validationError = item as string;
            validationError.ShouldNotBeNull();
            validationError.ShouldContain("Validation error");
        }
        
        var errors = actual.Where(x => x.Status == ResultStatus.Error)
            .SelectMany(y => y.Messages.Select(x => x)).ToList().AsReadOnly();
        
        errors.ShouldNotBeEmpty();
        
        foreach (var item in errors)
        {
            item.ShouldBeAssignableTo<string>();
            
            var errorMessage = item as string;
            errorMessage.ShouldNotBeNull();
        }
    }
}
