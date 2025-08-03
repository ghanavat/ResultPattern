<!-- TOC -->
* [Ghanavats Result Pattern package](#ghanavats-result-pattern-package)
  * [Overview](#overview)
  * [Key features](#key-features)
  * [How does it work?](#how-does-it-work)
    * [General Usage Example](#general-usage-example)
    * [Aggregate](#aggregate)
      * [Usage Example](#usage-example)
      * [Key Behaviours](#key-behaviours)
<!-- TOC -->

# Ghanavats Result Pattern package
Consistent and Flexible Result Handling for Developers to have better control over responses.

## Overview
The Ghanavats.ResultPattern NuGet package offers a powerful and adaptable approach 
to returning results from methods and handlers. 

Built on a well-known pattern, 
this implementation provides flexibility for developers to design and extend their own result-handling strategies.

## Key features
* Generic Results: Support for results based on any type, ensuring versatility across various use cases.
* Non-Generic Convenience**: Use `Result.Success()`, `Result.Error("...")`,
  or `Result.Invalid(...)` without specifying a type when no return value is needed.
* Built-in Statuses (for now!):
    * Ok – Operation succeeded with a value.
    * Error – Operation failed with one or more error messages.
    * Invalid – Validation failed, optionally with detailed ValidationError objects.
    * NotFound – Target entity was not found.
* Rich Error Handling: Supports both simple error messages and structured validation details.
* Implicit Conversions: Provides convenient operators for automatic conversion between Result<T> and common return patterns.
* Validation Support: Integrated with FluentAssertion's ValidationResult, it includes an extension method - `PopulateValidationErrors` - to populate a list of ValidationError instances. The custom ValidationError class simplifies returning invalid results with validation failures clearly defined.
* Aggregating multiple results into a single object.

## How does it work?
Ghanavats Result Pattern supports both generic and non-generic scenarios. 
If it's more convenient, use it without specifying a type, otherwise use the generic variant.

### General Usage Example
Let's say you have a method to create a User via a repository.
You want to use Result Pattern for flow control.
```csharp
public Result<User> CreateUser(CreateUserRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Email))
    {
        return Result<User>.Invalid(new[]
        {
            new ValidationError("Email is required.", "Email")
        });
    }

    if (_userRepository.Exists(request.Email))
    {
        return Result<User>.Error("User already exists.");
    }

    var user = new User(request.Email);
    _userRepository.Add(user);

    return Result<User>.Success(user, "User created successfully.");
}
```
Now, let's say the caller consumes `CreateUser` and needs to check the result.
```csharp
var result = userService.CreateUser(request);

if(result.IsSuccess)
{
    // take care of the logic here
}

if(result.Status == ResultStatus.Error)
{
    // Do what you need to do
}
```

### Aggregate
The Aggregate method provides a simple and consistent way to group multiple Result instances 
(such as from multiple internal service calls or business rule validations) 
into a summarised collection of outcomes by status (e.g. Error, Invalid).

It’s ideal when you:
* Need to collate results from multiple operations
* Want to detect overall failure status (e.g. if any error occurred)
* Need to collect all error messages in a structured way
* Prefer a clean format to return or log aggregated outcomes

#### Usage Example
```csharp
var results = new[]
{
    Result.Ok(),
    Result.Error("Database connection failed."),
    Result.Invalid([new ValidationError("Email is required.")])
};

var aggregated = Result.Aggregate(results);
```

**Output (simplified):**
```json
[
  {
    "Status": "Error",
    "Messages": ["Database connection failed."],
    "ValidationErrors": null
  },
  {
    "Status": "Invalid",
    "Messages": ["Email is required."],
    "ValidationErrors": null
  }
]
```

**Optional: Include Structured Validation Errors**

By default, Invalid results will only expose error messages as strings.
If you need structured validation detail (ValidationError objects), use:

```csharp
var aggregated = Result.Aggregate(results)
                       .WithFullValidationErrors();
```

**Output**
```json
[
  {
    "Status": "Invalid",
    "Messages": [],
    "ValidationErrors": [
      { "ErrorMessage": "Email is required.", "ErrorCode": "Email", "ValidationErrorType": "Error" }
    ]
  }
]
```
> When `WithFullValidationErrors()` is used, `Messages` for _Invalid_ entries will be cleared to avoid duplication or inconsistency.

#### Key Behaviours
* ResultStatus.Ok and NotFound are excluded from the aggregation by default.
* Aggregated results are grouped by ResultStatus.
* Only Messages or ValidationErrors are populated — never both for a single result.
* Safe to use in the response DTOs, API logging, or composite operations.
