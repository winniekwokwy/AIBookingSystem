# AIBookingSystem

## Initial setup
1. Create a Web API project
dotnet new webapi --use-controllers -o [Project Name]

2. Create API testing UI with Swagger
dotnet add package NSwag.AspNetCore

add the following in program.cs within the if (app.Environment.IsDevelopment()) code block 
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });

3. Add Model classes
4. 