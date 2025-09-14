using Ghanavats.ResultPattern.Attributes;
using Ghanavats.ResultPattern.Enums;

namespace Ghanavats.ResultPattern.Extensions;

internal static class ErrorKindExtensions
{
    internal static Uri GetProblemDetailsTypeUri(this ErrorKind errorKind)
    {
        if (errorKind
                .GetType()
                .GetField(errorKind.ToString())?
                .GetCustomAttributes(typeof(ProblemDetailsTypeAttribute), false)
                .FirstOrDefault() is ProblemDetailsTypeAttribute typeAttribute 
            && Uri.IsWellFormedUriString(typeAttribute.ProblemDetailsType, UriKind.Absolute))
        {
            return new Uri(typeAttribute.ProblemDetailsType);
        }

        return new Uri("about:blank");
    }
}
