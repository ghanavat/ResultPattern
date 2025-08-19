using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ghanavats.ResultPattern.Mapping;

public static class ResultMapping
{
    /// <summary>
    /// Converts a non-generic <see cref="Result"/> instance into an <see cref="IResult"/> 
    /// for use in ASP.NET Minimal API endpoints.
    /// </summary>
    /// <param name="result">The result instance to convert.</param>
    /// <returns>
    /// A <see cref="ValueTask{IResult}"/> representing the HTTP response mapped 
    /// from the <paramref name="result"/> status and data.
    /// </returns>
    public static ValueTask<IResult> ToResultAsync(this Result result)
    {
        return new ValueTask<IResult>(result.ToResultBase());
    }

    /// <summary>
    /// Converts a generic <see cref="Result{T}"/> instance into an <see cref="IResult"/> 
    /// for use in ASP.NET Minimal API endpoints.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result instance to convert.</param>
    /// <returns>
    /// A <see cref="ValueTask{IResult}"/> representing the HTTP response mapped 
    /// from the <paramref name="result"/> status and value.
    /// </returns>
    public static ValueTask<IResult> ToResultAsync<T>(this Result<T> result)
    {
        return new ValueTask<IResult>(result.ToResultBase());
    }
    
    /// <summary>
    /// Converts a non-generic <see cref="Result"/> instance into an <see cref="IResult"/> 
    /// for use in ASP.NET Minimal API endpoints.
    /// </summary>
    /// <param name="result">The result instance to convert.</param>
    /// <returns>
    /// An <see cref="IResult"/> representing the HTTP response mapped 
    /// from the <paramref name="result"/> status and data.
    /// </returns>
    public static IResult ToResult(this Result result)
    {
        return result.ToResultBase();
    }

    /// <summary>
    /// Converts a generic <see cref="Result{T}"/> instance into an <see cref="IResult"/> 
    /// for use in ASP.NET Minimal API endpoints.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result instance to convert.</param>
    /// <returns>
    /// An <see cref="IResult"/> representing the HTTP response mapped 
    /// from the <paramref name="result"/> status and value.
    /// </returns>
    public static IResult ToResult<T>(this Result<T> result)
    {
        return result.ToResultBase();
    }

    /// <summary>
    /// Converts a non-generic <see cref="Result"/> instance into an <see cref="IActionResult"/> 
    /// for use in ASP.NET MVC controllers.
    /// </summary>
    /// <param name="result">The result instance to convert.</param>
    /// <param name="controller">The controller used to generate the response.</param>
    /// <returns>
    /// A <see cref="ValueTask{IActionResult}"/> representing the HTTP response mapped 
    /// from the <paramref name="result"/> status and data.
    /// </returns>
    public static ValueTask<IActionResult> ToActionResultAsync(this Result result,
        ControllerBase controller)
    {
        return new ValueTask<IActionResult>(result.ToActionResultBase(controller));
    }
    
    /// <summary>
    /// Converts a generic <see cref="Result{T}"/> instance into an <see cref="IActionResult"/> 
    /// for use in ASP.NET MVC controllers.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result instance to convert.</param>
    /// <param name="controller">The controller used to generate the response.</param>
    /// <returns>
    /// A <see cref="ValueTask{IActionResult}"/> representing the HTTP response mapped 
    /// from the <paramref name="result"/> status and value.
    /// </returns>
    public static ValueTask<IActionResult> ToActionResultAsync<T>(this Result<T> result,
        ControllerBase controller)
    {
        return new ValueTask<IActionResult>(result.ToActionResultBase(controller));
    }
    
    /// <summary>
    /// Converts a non-generic <see cref="Result"/> instance into an <see cref="IActionResult"/> 
    /// for use in ASP.NET MVC controllers.
    /// </summary>
    /// <param name="result">The result instance to convert.</param>
    /// <param name="controller">The controller used to generate the response.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> representing the HTTP response mapped 
    /// from the <paramref name="result"/> status and data.
    /// </returns>
    public static IActionResult ToActionResult(this Result result,
        ControllerBase controller)
    {
        return result.ToActionResultBase(controller);
    }
    
    /// <summary>
    /// Converts a generic <see cref="Result{T}"/> instance into an <see cref="IActionResult"/> 
    /// for use in ASP.NET Core controllers.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result instance to convert.</param>
    /// <param name="controller">The controller used to generate the response.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> representing the HTTP response mapped 
    /// from the <paramref name="result"/> status and value.
    /// </returns>
    public static IActionResult ToActionResult<T>(this Result<T> result,
        ControllerBase controller)
    {
        return result.ToActionResultBase(controller);
    }
}
