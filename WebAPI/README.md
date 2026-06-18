# AIBookingSystem

## Initial setup of the project in VS code
1. Create a Web API project (https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-10.0&tabs=visual-studio-code)

dotnet new webapi --use-controllers -o [Project Name]

2. Create API testing UI with Swagger

    a. dotnet add package NSwag.AspNetCore

    b. add the following in program.cs within the if (app.Environment.IsDevelopment()) code block 

    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });

3. Add Model classes
For Data Annotations, https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/data-annotations

4. Create a database in PostgreSQL

5. Connect application to a PostgreSQL database using EF Core (https://dev.to/vzldev/integrating-postgresql-with-a-net-a-step-by-step-guide-3hep)

    a. add the following packages.

        dotnet add package Microsoft.EntityFrameworkCore
        dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
        dotnet add package Microsoft.EntityFrameworkCore.Design
        dotnet add package Microsoft.EntityFrameworkCore.Tools

    b. add the following NuGet package

        Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore

    c. Add connection string into Secret Manager Tool (https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-10.0&tabs=linux%2Cpowershell)

        dotnet user-secrets init
        dotnet user-secrets set "[Name of db connection string]" "Host=localhost;Database=[Database Name];Username=[Username];Password=[Password]"

    d. Create DbContext (https://medium.com/@UlbertAO/chapter-4-dbcontext-class-in-asp-net-core-web-api-8c9f7d332602)

    e. Register DbContext in Program.cs

6. Create and Apply Migrations

    dotnet ef migrations add [Name of the migration]]
    dotnet ef database update

7. Create controllers (https://dotnettutorials.net/lesson/controllers-in-asp-net-core-web-api/)

    For Attribute routing with HTTP verb attributes, refer to https://learn.microsoft.com/en-us/aspnet/core/mvc/controllers/routing?view=aspnetcore-10.0#ar6?)

    For dependency injection, refer to https://dotnettutorials.net/lesson/dependency-injection-asp-net-core-web-api/

    For returning proper API response, refer to https://codewithmukesh.com/blog/http-status-codes-aspnet-core-api-responses/

8. Testing web api (https://medium.com/@parserdigital/testing-asp-net-core-8-0-apis-a-comprehensive-guide-42dc3b2a751a)

For unit tests, https://www.c-sharpcorner.com/article/unit-testing-for-a-net-web-api-project/; https://dotnettutorials.net/lesson/unit-testing-service-layer-asp-net-core-web-api/

For integration tests, https://dev.to/imdj/unit-testing-aspnet-core-web-api-with-moq-and-xunit-controllers-services-nci
