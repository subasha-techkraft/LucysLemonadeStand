# Lunch & Learn: Unit Testing & Integration Testing

Welcome to my second Lunch & Learn!
This one is all about Unit testing and Integration testing in C#, though of course the concepts will apply in most other languages.
The code here heavily uses the concepts from my previous L&L on dependency injection. 
If you need a refresher, you can check out the [Dependency Injection Workshop repository](https://bitbucket.org/spheretechsol/dependencyinjectionworkshop/src/master/).

## Table of Contents
* [Unit Tests](#unit-tests)
    * [Definitions](#definitions)
    * [Structure](#structure)
    * [Mocking](#mocking)
    * [Qualities of good unit tests](#qualities-of-good-unit-tests)
    * [Recommended NuGet Packages](#recommended-nuget-packages)
    * [Unit testing FAQ](#unit-testing-faq)
* [Integration Tests](#integration-tests)
    * [Definition](#definition)
    * [Managing side effects of integration tests](#managing-side-effects-of-integration-tests)
* [Lucy's Lemonade Stand Demo Project](#lucys-lemonade-stand-demo-project)
* [Docker](#docker)

## Unit Tests

### Definitions
To understand what a unit test is, we have to define a "Unit".
A unit is a small piece of code that does something.
It should not have a lot of responsibilities or side effects.
Typically we consider a method or function to be a unit. 
A method that has a lot of responsibilities or side effects can colloquially be called a `"megamoth"`, short for "mega monolithic method".
With such methods, it is advisable to refactor them into groups of smaller methods so that, among other useful reasons, the unit tests are more direct.

A unit test is therefore a piece of code that makes sure that a unit does what the programmer intended for it to do.
To define it further, a unit test has the following qualities:

1. It tests only one aspect of the unit. For example, it can test that under a certain category of conditions, the unit will call some function, return an expected value, or throw an Exception.
1. It is repeatable. Given the same conditions, it will yield the same result every time.
1. It is fully contained. 
    1. It has no connection to the environment in which the test program is run.
    1. No other unit test has to run before this one in order to setup any shared objects or configuration.
    1. It has no side effects such as changing files, calling APIs, saving to a database, or changing global variables that will affect other tests.

### Structure
A unit test's structure typically follows the `Arrange-Act-Assert (AAA)` structure.
It first arranges the conditions for the test.
Then it acts by running the unit of the `System Under Test (SUT)`.
Finally, it asserts that the expected result occurred.
If it did, the test passes, if it did not, the test fails.

### Mocking
To support all of the above qualities, it is critical to have total control over any side effects that the unit may normally have.
If it calls other methods, either those methods should be helper methods that can also be tested and don't have side effects, or if the methods are expected to have side effects, you must substitute the definitions of those methods with ones you control.
The most basic way to do this is by using `Dependency Injection` in the class or method that's being tested to provide either your own implementation of an interface or by inheriting from the class that's passed in and overriding its methods so they do the same thing every time with no side effects outside the bounds of the unit test.
For example, if your unit saves logs through an interface like `ILogger`, you can substitute that logger with your own implementation that saves messages in a List.
If your unit calls an API for some information, call it via an interface so that you can swap out the implementation that calls the API with your own mock implementation that directly returns the result.

Without this ability to substitute mock implementations, you can't guarantee that the conditions of a test are completely reproducible.
When you call an API, there's a lot of things that can go wrong that you have no control over, such as the internet not being available, or the API might start throttling you, or your credentials may be expired.
When you write to a file, you may get errors that can skew the test, such as if the space on the drive is insufficient, or write permissions are not present.
This is why total control is a must.

### Qualities of good unit tests
There are additional qualities that re nice to have in a unit test:

1. It checks for both success cases and failure cases.
   If your unit has a requirement that it throws an error given some condition, test to make sure it actually throws that error only when the condition matches.
1. It checks for edge cases.
   This means making sure that you try to test with the craziest values of inputs that the unit needs in order to see if some unwanted behavior emerges.
   This can mean passing 

      * null values for reference types
      * 0, -1, int.MinValue, and int.MaxValue for integers
      * blank strings, whitespace-only strings, strings with leading or trailing whitespace, strings with crazy symbols like &amp;nbsp; or emojis, strings with Chinese or Japanese characters, strings with vowels with different àccént marks
      * empty collections and, if applicable, collections with duplicates
      * purposely misconfigured settings objects
      * anonymous objects where `dynamic` is a possible generic type 

1. It should not concern itself with anything besides the single scenario it's testing.
   If any objects that are required for your test require parameters in construction that are not related to the test, it's desirable for the test to not have to create them.
   This is because 

      * any changes to the signatures of other methods (especially constructors) means you have to edit a test that has nothing to do with those methods
      * any changes to the bodies of those methods may cause errors to be thrown that your test wasn't intended to test
      * it dilutes the intent of the unit test. While it was meant to test one thing, it now tests more than one.

1. It should try to test a property of the thing the unit is intended to represent and not just ensure that some specific inputs have specific outputs.
   If you test with only specific inputs, a developer can add those cases to the unit's code without actually implementing the entire behavior.
   For example, if you want to test an implementation of addition, if you only check that 2+2 returns 4 and some other known additions, the implementation can be `if (a == 2 && b == 2) return 4;` for every case in the test.
   Whenever possible, a good test can also use randomized data with known attributes to make sure the implementation is comprehensive.
   In the case of addition, you can use random non-zero values `a`, `b`, and `c` to test 

      * the identity property (`add(a, 0) == a`, `add(a, b) not in [0, a, b] when a != -b`, and `add(a, -a) == 0` (requires negation to be implemented and tested separately)
      * the commutitive property (`add(a, b) == add(b, a)`)
      * the associative property (`add(add(a, b), c) == add(a, add(b, c))`)
      * the distribuative property (requires multiplication to also be implemented and tested separately) (`mult(a, add(b, c)) == add(mult(a, b), mult(a, c))`)
   
    By satisfying all these cases with random values, the result can't be anything but an implementation of addition.

1. It should run fast. 
   It's expected that a project can have dozens or hundreds of tests, and many of them can be run repeatedly with different inputs. 
   Since tests can be run as part of a CI/CD pipeline, you want them to run fast and fail fast if there's an actual error. So that the pipeline execution time doesn't become unmanageable.
1. It's easy to read and understand the intent.

      * Make sure the test method name shows what's being tested.

        A common practice is to name them with this structure: `{method being tested}_{expected result}_When{condition(s)}`.
        E.g. `Add_ReturnsZero_WhenInputsAreNegationsOfEachother`

      * The assert statements are easy to read.

        The `FluentAssertions` NuGet package helps write assertions in ways that are more human readable.

### Recommended NuGet Packages
The following libraries are recommended for writing unit tests:

* **xunit** - This is the core library of the xUnit testing framework.
  This and a few related libraries will automatically be included when creating an xUnit Test Project in Visual Studio.
  You'll see that xUnit is not only used for unit tests but integration, acceptance, and other types as well.
* **FluentAssertions** - This helps write assert statements that are human readable. They also have better error messages.
* **NSubstitute** - My preferred library for automatically creating mocks of interfaces.
  Inside a test you can specify exactly what a method of the interface returns so that when your SUT calls the interface, it's completely controlled.
  It uses cleaner syntax than what other libraries like **Moq** will give you.
* **AutoFixture.Xunit2** - AutoFixture allows you to instantiate objects and automatically set values for its properties so that you can omit handling any property that's not related to the test.
  This one is a nice-to-have, especially for classes with constructors with a sinful amount of parameters.
* **RichardSzalay.MockHttp** - A library for mocking HttpMessageHandlers, which will help if your code depends on HttpClient.

### Unit testing FAQ

1. **Are all methods unit testable?** 
   Not inherantly. 
   If the methods have side effects and are not written to utilize proper dependency injection, then you won't be able to have full control over the code. 
   Any tests on these methods would become integration tests.
   It's important to use DI to have good testability.
   Additionally, some code that's built on top of frameworks may be tricky to test properly given the sheer number of concerns that they handle.
   For such methods it's important to move as much logic out of them and into other methods that are not tied to the frameworks.
   You may have to use integration tests for the rest.
1. **My method calls additional methods, and those call more methods, and so on. Do I have to unit test all of them?**
   You should unit test anything that will have to be maintained by you, your team, or your company where the implementation isn't completely obvious at a glance.
   We assume that anything in frameworks like .NET, ASP.NET, etc. and NuGet libraries in general, works for the cases we will use them in so it's ok to omit tests for those things, though if they cause side effects, it's important to decouple those things from your code.
   Your code might do some math but it's probably not going to be useful to add your own unit tests of .NET's Math class.
   At some point you have to assume that anything past that point is tested a priori.
1. **I'm using DI but the thing I'm injecting has no interface, is sealed, and has side effects. What should I do?**
   There is a very common case for this, and that's Microsoft's HttpClient. The recommended approach by MS is to inject it directly and it has no useful interfaces.
   Turns out the HttpClient itself doesn't do much but forward calls to an HttpMessageHandler, and that's something that can be mocked, though not in a straight-forward manner (see the RichardSzalay.MockHttp NuGet package).
   HttpClient aside, if you have access to the code that gets the unmockable type injected into it, you can create a class and and interface to wrap around the unmockable type and act as a proxy.

## Integration Tests

### Definition

An integration test is a step above a unit test in complexity and purpose.
It tests code that communicates with some other systems (modules) to ensure that the distinct units can act as one.
Like unit tests, the goal of an integration test is to isolate a part of the application for testing, but in this case it isolates 2 or more units in order to uncover if there is a problem when two modules are communicating.
It can catch an error that a unit test may not catch because a unit test makes assumptions about other systems and is totally isolated from them.

For an integration test to be good, it should isolate exactly two modules and mock anything else in order to determine specifically which pair of modules have an issue.
As a result of the requirement of connecting to a module that's actually used, this means that side effects are going to be real.
To manage this, it's a good idea to run integration tests in a non-production environment only, and for any modules it connects to to also not be in production.
This can also be complicated by the fact that some tests require a certain state in order to be set up and it may be difficult to ensure that state exists when running many integration tests with side effects.
There are a number of recommendations for how to handle these types of situations.

### Managing side effects of integration tests

1. When connecting to a database, either:

   * have setup and cleanup scripts run before and after each test
   * run the database in [Docker](#docker). 
   This approach is recommended over the previous because it can also ensure that running tests in parallel will not cause interference between tests.
1. When calling an external API:

   * If you control the API, 
       * run pre-test setup and post-test cleanup scripts and call the actual API in a lower environment.
       * run the API and all of its dependencies in [Docker](#docker) so you can dispose of the container after the test.
   * If you do not contorl the API, use **WireMock.NET** to create a mock instance locally.
   This library creates an actual API that's called via HTTP and provides mock responses in what's ideally the same structure as the actual API would return.
1. When writing files to the local file system:

   * create a temporary folder per test and clean it up afterwards
     You should be able to swap out target directories by overwriting settings; they should not be hard-coded.

## Lucy's Lemonade Stand Demo Project

I have prepared a sample project to show how to write unit and integration tests in C#.
The project is an API representation of a lemonade stand.
Its core functionality represents actions taken by a store that sells nothing but cups of lemonade.
It stores data in a MS SQL database and calls an external API (Mom) to trigger refilling of the pitcher of lemonade.

[Here is the breakdown of the project structure.](docs/LucysLemonadeStand-solution-tour.md)

[Here are instructions for how to set up the project's environment to run it locally.](docs/project-setup.md)

## Docker

[Docker](https://docker.com) is a set of platform as a service (PaaS) products that use OS-level virtualization to deliver software in packages called containers.
The software that hosts the containers is called Docker Engine.

Docker is a tool that is used to automate the deployment of applications in lightweight containers so that applications can work efficiently in different environments.

There are two critically important keywords to know when talking about Docker:

* **Image** - a saved program along with everything needed to run it, all pre-configured and saved as a blueprint for a **container**.
* **Container** - The live "copy" of an image where the program(s) installed on it are run and connections to it are allowed. Many containers can be launched from an image concurrently and independently as long as they expose different external ports.

After installing Docker Desktop, here are instructions for [setting up the database for Lucy's Lemonade Stand in Docker](docs/docker/SQL-Server-Docker.md).