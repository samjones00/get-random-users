[![.NET](https://github.com/samjones00/get-random-users/actions/workflows/dotnet.yml/badge.svg)](https://github.com/samjones00/get-random-users/actions/workflows/dotnet.yml)

# Get Random Users

## Features
* Filenames, number of requests, etc are configurable
* Moved all code into their own services to make it easier to test
* Added unit tests for the services
* Added DelegatingHandler to handle http client logging
* Added text file logging
* Added rate limiting, as making 5 requests in parallel was hitting the rate limit and causing a 502 to be returned
* Escaping the application (<kbd>CTRL</kbd>+<kbd>C</kbd>) terminates the async requests and application gracefully

## Getting started
* Clone the repository
* Open the solution in Visual Studio
* Ensure that MyHomework.V2 is set as the startup project
* Run the application

## Configuration
The configuration is read from the appsettings.json file:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "Options": {
    "LogFilePath": "c:\\temp\\log-{Date}.txt",
    "MaxParallelization": 4,
    "OutputFilePath": "c:\\temp\\MyTest-V2.json",
    "RequestCount": 5,
    "ResponsesFilePath": "c:\\temp\\MyTest-V2.txt",
    "UserServiceUrl": "https://randomuser.me/api/"
  }
}
```

## Next steps
* Add transient fault handling using `Polly`
* Add appsettings validation to warn of invalid values on app start, using `Microsoft.Extensions.Options.DataAnnotations` or `FluentValidation`