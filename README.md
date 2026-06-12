# AIBookingSystem

## Initial setup
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

4. Create a database in PostgreSQL

5. Connect application to a PostgreSQL database using EF Core (https://dev.to/vzldev/integrating-postgresql-with-a-net-a-step-by-step-guide-3hep)
    a. add the following packages.
        dotnet add package Microsoft.EntityFrameworkCore
        dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
        dotnet add package Microsoft.EntityFrameworkCore.Design
        dotnet add package Microsoft.EntityFrameworkCore.Tools
    b. add the following NuGet package
        Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
    c. Add connection string into Secret Manager Tool
        dotnet user-secrets init
        dotnet user-secrets set "[Name of db connection string]" "Host=localhost;Database=[Database Name];Username=[Username];Password=[Password]"
    d. Create DbContext
    e. Configure services in Program.cs

6. Create and Apply Migrations
    dotnet ef migrations add [Name of the migration]]
    dotnet ef database update
