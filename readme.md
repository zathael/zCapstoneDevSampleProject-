# Sample Validation Application

This application is a C# program built with .NET Framework 4.7.2 that:
- loads a set of sample records, 
- runs validation on them,
- then sums up all the values and displays the results. 

The application has been optimized to run faster and is designed to take advantage of multiple CPU cores for improved performance.
	
> Dev Note: My machine is designed for extreme multi-threading so my system's results may be elevated

We are hoping the user/system has more than 1 CPU accessible to greatly reduce processing times.

## Features

- Load sample records in parallel
- Validate sample records in parallel
- Write log messages to a text file (check "C:\temp")
- Display the total number of validated samples and their sum
- Calculate and display the cycle time

## Running Tests

The solution includes a test project with unit and integration tests for the application. To run the tests, follow these steps:

1. Open the Test Explorer in Visual Studio by going to `Test` > `Windows` > `Test Explorer`.

2. Build the solution.

3. In the Test Explorer, click `Run All` to run all the tests.

> You may need to reinstall MSTest.TestFramework through NuGet if it is not referencing the Microsoft.VisualStudio.TestTools.UnitTesting .dll correctly

## License

No License was specified at the time of this project. None is overly needed as this doesn't connect anywhere or do specific labor.
