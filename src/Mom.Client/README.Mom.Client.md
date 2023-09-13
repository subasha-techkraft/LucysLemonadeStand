﻿# Mom API Client

All files in this project except this readme are generated by NSwag-codegen off the Sphere Task API.

If you want to run NSwag-codegen yourself:

1. Have your API running. In its Swagger page, look for the link at the very top for the swagger.json file. Copy that URL.
1. In Visual Studio, select any file in this project to focus on it.
1. In the main menu, go to Project / Connected Services / Manage Connected Services.
1. If there is no Service Reference to your API, click the green '+' icon and select OpenAPI. Click Next. 
   If there already is a service to your API, click the `...` icon and select Edit.
1. Select the URL radio button and paste the URL copied earlier into the URL field.
1. For the namespace, use `Mom.Client`. 
   For the class name, use `MomClient`.
   For options, paste this:
   `/UseBaseUrl:false /GenerateClientInterfaces:true`
   This will cause the generated client to use the BaseUrl from the injected HttpClient and for it to have an interface called `IShereTaskAPIClient`.
1. Click Finish to generate the code for the first time. If you want to regenerate the code again, click the `...` menu and select `Refresh`. 
   You can see what the code looks like by using the `View generated code` option in the `...` menu for this "swagger - Client" in the Manage Connected Services menu.
   Included in the generated swaggerClient.cs is 
   * the `MomClient` class that takes in an HTTPClient instance (which should be provided via dependency injection)
   * the `IMomClient` interface `MomClient` implements that you should be injecting into classes that will use `MomClient`
   * methods for calling any endpoint from the API.
   * classes to hold objects necessary for the API.
   The generated code is not otherwise visible from the solution explorer. I'm not sure why exactly, but it helps ensure that developers don't edit generated code.

Here's a useful article I found that shows how to set up a Typed Http Client for injection, and mentions a bunch of other NSwag-codegen options that we may want to explore: [https://stu.dev/generating-typed-client-for-httpclientfactory-with-nswag/](https://stu.dev/generating-typed-client-for-httpclientfactory-with-nswag/)

[List of NSwag options](https://gist.github.com/stevetalkscode/69719465d8271f1e9fa412626fdadfcd)