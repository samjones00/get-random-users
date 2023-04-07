[![.NET](https://github.com/samjones00/get-random-users/actions/workflows/dotnet.yml/badge.svg)](https://github.com/samjones00/get-random-users/actions/workflows/dotnet.yml)

# Get Random Users

## Features

* Filenames and number of requests are configurable
* Moved all code into their own services to make it easier to test
* Added unit tests for the services
* Text file logging
* Escaping the application (<kbd>CTRL</kbd>+<kbd>C</kbd>) terminates the async requests and application gracefully

## Getting started
* Clone the repository
* Open the solution in Visual Studio
* Ensure that MyHomework.V2 is set as the startup project
* Run the application

## Configuration
The configuration is configured via the appsettings.json file:
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
    "userServiceBaseUrl": "https://randomuser.me/",
    "UserServiceGetEndpoint": "/api",
    "ResponsesFileName": "c:\\temp\\MyTest-V2.txt",
    "OutputFileName": "c:\\temp\\MyTest-V2.json",
    "ApiCallCount": 5,
    "LogFileName": "c:\\temp\\log-{Date}.txt"
  }
}
```

## Logs
Text file logs are saved to get-random-users\src\MyHomework.V2\bin\Debug\net6.0\Logs

## Next steps
* Add `Microsoft.Extensions.Http.Polly` for handling transient faults when making http requests
* Add config validation to warn of invalid values on app start, probably using fluent validator.
* A possible refactor to use Task.WhenAll() instead of awaiting multiple calls to the API in a loop (https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/task-based-asynchronous-programming)