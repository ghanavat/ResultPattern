<!-- TOC -->
* [Ghanavats Result Pattern](#ghanavats-result-pattern)
  * [Overview](#overview)
  * [Key features](#key-features)
  * [General Usage Example](#general-usage-example)
  * [Aggregate](#aggregate)
    * [When use it](#when-use-it)
    * [Usage Example](#usage-example)
    * [Key Behaviours](#key-behaviours)
  * [Mapping Results to HTTP Responses](#mapping-results-to-http-responses)
      * [Why use the Mapping feature?](#why-use-the-mapping-feature)
      * [Available Mapping Methods](#available-mapping-methods)
      * [Usage Examples](#usage-examples)
      * [Notes & Best Practices](#notes--best-practices)
<!-- TOC -->

# Ghanavats Result Pattern
Consistent and Flexible Result Handling for Developers to have better control over responses.

## Overview
The Ghanavats.ResultPattern NuGet package offers a powerful and adaptable approach 
to returning results from methods and handlers. 

Built on a well-known pattern, 
this implementation provides flexibility for developers to design and extend their own result-handling strategies.

## Key features
Ghanavats Result Pattern supports both generic and non-generic scenarios.
If it's more convenient, use it without specifying a type, otherwise use the generic variant.

* Generic Results: Support for results based on any type, ensuring versatility across various use cases.
* Non-Generic Convenience**: Use `Result.Success()`, `Result.Error("...")`,
  or `Result.Invalid(...)` without specifying a type when no return value is needed.
* Built-in Statuses (for now!):
    * Ok – Operation succeeded with/without a value.
    * Error – Operation failed with one or more error messages. Returns a `ProblemDetails` object for better error representation.
    * Invalid – Validation failed, with FluentValidation's ValidationResult object or supply your own dictionary of validation errors, `Dictionary<string, string[]>`. We are returning ValidationProblemDetails object for better error representation.
    * NotFound – Target entity was not found. We are returning a `ProblemDetails` object for better error representation.
* Rich Error Handling: Supports both simple error messages and structured validation details.
* Implicit Conversions: Provides convenient operators for automatic conversion between Result<T> and common return patterns.
* Validation Support: Integrated with FluentValidation's ValidationResult.
* Aggregating multiple results into a single object.
* HTTP Response Mapping: Seamlessly convert results to HTTP responses in ASP.NET Core applications.

## General Usage Example
Let's say you have a method to create a User via a repository.
You want to use Result Pattern for flow control.
```csharp
public Result<User> CreateUser(CreateUserRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Email))
    {
        return Result<User>.Invalid(validationResult); // where validationResult is a FluentValidation ValidationResult instance
        // Optionally, you can also do:
        // return Result<User>.Invalid(new Dictionary<string, string[]>
    }

    if (_userRepository.Exists(request.Email))
    {
        return Result<User>.Error("User already exists.");
        // Or optionally, you can also specify an error kind:
        // return Result<User>.Error("User already exists.", ErrorKind.BusinessRule);
    }

    var user = new User(request.Email);
    _userRepository.Add(user);

    return Result<User>.Success(user, "User created successfully.");
}
```

**Expected Output for `Result.Error` (Also similar for `Result.NotFound`)**
```json
{
  "status": 500,
  "type": "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1",
  "title": "Unknown. There has been a problem with your request.",
  "detail": "Sample error message.",
  "instance": "/users/add",
  "traceId": "Trace Id"
}
```

**Expected Output for `Result.Invalid`**
```json
{
  "status": 400,
  "title": "One or more validation errors occurred.",
  "detail": "See the errors property for details.",
  "errors": {
    "Email": [
      "Email is required.",
      "Email must be a valid email address."
    ]
  },
  "instance": "/users/add",
  "traceId": "Trace Id"
}
```
Now, let's say the caller consumes `CreateUser` and needs to check the result:
```csharp
var result = userService.CreateUser(request);

if(result.IsSuccess())
{
    // take care of the logic here
}

if(result.IsError())
{
    // Do what you need to do
}
```

## Aggregate
The Aggregate method provides a simple and consistent way to group multiple Result instances 
(such as from multiple internal service calls or business rule validations) 
into a summarised collection of outcomes by status (e.g. Error, Invalid).

### When use it
* Need to collate results from multiple operations
* Want to detect overall failure status (e.g. if any error occurred)
* When you need to collect all error messages in a structured way
* Prefer a clean format to return or log aggregated outcomes

### Usage Example
```csharp
var results = new[]
{
    Result.Ok(),
    Result.Error("Database connection failed."),
    Result.Invalid(validationResult), // where validationResult is a FluentValidation ValidationResult instance
};

var aggregated = Result.Aggregate(results);
```

**Output (simplified):**
```json
[
  {
    "Status": "Error",
    "Messages": ["Database connection failed."],
    "ValidationErrorsPair": null
  },
  {
    "Status": "Invalid",
    "Messages": [],
    "ValidationErrorsPair": {
        "Property1": ["Error message 1"]
      }
  }
]
```

### Key Behaviours
* ResultStatus.Ok and ResultStatus.NotFound are excluded from the aggregation by default.
* Aggregated results are grouped by ResultStatus.
* Only Messages or ValidationErrorsPair are populated — never both for a single result.
* Safe to use in the response DTOs, API logging, or composite operations.

## Mapping Results to HTTP Responses
The Mapping feature in Ghanavats.ResultPattern allows you to seamlessly convert Result or Result<T> objects into framework-specific HTTP responses in ASP.NET Core.
It removes boilerplate code from your controllers and minimal APIs 
by providing ready-to-use extension methods 
that map your result status to the correct HTTP status code and response format.

#### Why use the Mapping feature?
- Consistency – Every API endpoint returns response in the same shape and status code mapping.
- Less boilerplate – No more repetitive switch statements in your controllers.
- Integration with both MVC and Minimal APIs – Supports IActionResult and IResult mappings.
- Async-friendly – Supports both synchronous and asynchronous action signatures without unnecessary allocations.

#### Available Mapping Methods

**For Minimal APIs**
```csharp
IResult ToResult(this Result result)
IResult ToResult<T>(this Result<T> result)
ValueTask<IResult> ToResultAsync(this Result result)
ValueTask<IResult> ToResultAsync<T>(this Result<T> result)
```

**For MVC Controllers**
```csharp
IActionResult ToActionResult(this Result result, ControllerBase controller)
IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
ValueTask<IActionResult> ToActionResultAsync(this Result result, ControllerBase controller)
ValueTask<IActionResult> ToActionResultAsync<T>(this Result<T> result, ControllerBase controller)
```

#### Usage Examples
**Minimal API – synchronous mapping**

```csharp
app.MapPost("/users", (SomeRequest req, ISomeService svc) =>
{
    var result = svc.DoSomething(req);
    return result.ToResult(); // Converts Result to IResult
});
```
**Minimal API – async mapping**

```csharp
app.MapPost("/users", async (SomeRequest req, ISomeService svc) =>
{
    var result = await svc.DoSomethingAsync(req);
    return await result.ToResultAsync(); // Converts Result<T> to IResult
});
```

**MVC Controller – synchronous mapping**

```csharp
[HttpPost]
public IActionResult CreateUser(CreateUserRequest req)
{
    var result = _svc.CreateUser(req);
    return result.ToActionResult(this); // Converts Result to IActionResult
}
```

**MVC Controller – async mapping**

```csharp
[HttpPost]
public async Task<IActionResult> CreateUser(CreateUserRequest req)
{
    var result = await _svc.CreateUserAsync(req);
    return await result.ToActionResultAsync(this); // Converts Result<T> to IActionResult
}
```

> Note: You don't have to `await` the mapping. For Minimal API, you can do:
> ```csharp
> return result.ToResultAsync();
> ```
> Or for MVC Controller action, you can do:
> ```csharp
> return result.ToActionResultAsync(this);
>```

#### Notes & Best Practices
- Use the sync mapping methods in synchronous controller or endpoint actions.
- Use the ValueTask async mapping methods in asynchronous actions to avoid extra allocations.
- Do not block on async mapping methods in synchronous actions.
- The mapping is purely synchronous internally — the async versions are wrappers to integrate cleanly into async pipelines.
