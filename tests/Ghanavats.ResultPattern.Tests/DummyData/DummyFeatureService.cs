namespace Ghanavats.ResultPattern.Tests.DummyData;

public class DummyFeatureService
{
    public Result<DummyResponseModel> DoSomething()
    {
        var response = new DummyResponseModel
        {
            DummyFieldOne = "Some data"
        };
        
        return response;
    }
    
    public DummyResponseModel DoSomethingImplicitConversionOfResultGenericToDataType()
    {
        var response = new DummyResponseModel
        {
            DummyFieldOne = "Test Field Data"
        };
        
        return Result<DummyResponseModel>.Success(response);
    }
    
    public Result<DummyResponseModel> DoSomethingImplicitConversionOfNonGenericResultToGenericResultWithDefault()
    {
        _ = new DummyResponseModel
        {
            DummyFieldOne = "DataNotGoingToBeUsed"
        };
        
        return Result.Success();
    }
}
