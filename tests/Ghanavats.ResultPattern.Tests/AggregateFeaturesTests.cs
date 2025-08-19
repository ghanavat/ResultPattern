using FluentValidation;
using FluentValidation.Results;
using Ghanavats.ResultPattern.Aggregate;
using Ghanavats.ResultPattern.Enums;
using Shouldly;

namespace Ghanavats.ResultPattern.Tests;

public class AggregateFeaturesTests
{
    [Fact]
    public void Result_ShouldAggregateAllResultsCorrectly_WhenItIsCalledWithMultipleResultsWithoutFullValidationErrorsExtension()
    {
        //arrange
        var result1 = Result.Invalid(new ValidationResult
        {
            Errors =
            [
                new ValidationFailure
                {
                    ErrorCode = "3434",
                    ErrorMessage = "Validation error Jan",
                    PropertyName = "Property1",
                    Severity = Severity.Error
                }
            ]
        });

        var result2 = Result.Invalid(new ValidationResult
        {
            Errors =
            [
                new ValidationFailure
                {
                    ErrorCode = "5554",
                    ErrorMessage = "Validation error Feb",
                    PropertyName = "Property2",
                    Severity = Severity.Error
                }
            ]
        });

        var result3 = Result.Invalid(new ValidationResult
        {
            Errors =
            [
                new ValidationFailure
                {
                    ErrorCode = "6678",
                    ErrorMessage = "Validation error March",
                    PropertyName = "Property3",
                    Severity = Severity.Error
                }
            ]
        });
        
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
            item.ValidationErrorsPair.ShouldBeEmpty();
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
        var result1 = Result.Invalid(new ValidationResult
        {
            Errors =
            [
                new ValidationFailure
                {
                    ErrorCode = "666",
                    ErrorMessage = "Validation error April",
                    PropertyName = "Property1",
                    Severity = Severity.Error
                }
            ]
        });

        var result2 = Result.Invalid(new ValidationResult
        {
            Errors =
            [
                new ValidationFailure
                {
                    ErrorCode = "777",
                    ErrorMessage = "Validation error May",
                    PropertyName = "Property2",
                    Severity = Severity.Error
                }
            ]
        });

        var result3 = Result.Invalid(new ValidationResult
        {
            Errors =
            [
                new ValidationFailure
                {
                    ErrorCode = "888",
                    ErrorMessage = "Validation error June",
                    PropertyName = "Property3",
                    Severity = Severity.Error
                }
            ]
        });
        
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
            item.ValidationErrorsPair.ShouldNotBeEmpty();
            item.ValidationErrorsPair.Count.ShouldBe(3);
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
}
