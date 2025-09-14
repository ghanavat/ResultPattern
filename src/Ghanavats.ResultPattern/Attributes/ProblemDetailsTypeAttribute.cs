namespace Ghanavats.ResultPattern.Attributes;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true) ]
public class ProblemDetailsTypeAttribute : Attribute
{
    public string ProblemDetailsType { get; }
    
    public ProblemDetailsTypeAttribute(string problemDetailsType)
    {
        ProblemDetailsType = problemDetailsType;
    }
}
