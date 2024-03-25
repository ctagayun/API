var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//ApiExplorer and SwaggerGen are added to DI container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  //this enable swagger to scan for all endpoints
                         //Swagger middleware writes documentation
                        

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
     //Swagger is only added when we are in DEV environment
    app.UseSwagger();  //this creates standard definition of our API in JSON format
    app.UseSwaggerUI(); //this transforms JSON into a UI
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

//The map get extension method on the web application object enables
//us to define an endpoit that responds to a get request directly.
//No need for controllers with the routing table
// weatherforecast" - is the relative URL of the end point
// and the second part (arrow function) is Lambda that executes 
//when the endpoint is hit
app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast") //this property makes it possible 
                                //to refer to this route by name
.WithOpenApi();  //the call to WithOpenAPI is not strictly necessary   
                 //because the swagger endpoint explorer will automatically 
                 //add this endpoint to swagger documentation       

app.Run();  //Finally the app is commanded to run and then we will 
            //an active endpoint.
            //However before we run, open launchSettings.JSON and 
            //delete IAS settings and IAS EXPRESS profile
            //as well as the "//localhost:5151" in "applicationUrl"
            //and change the port to https://localhost:4000"

            //So our API will run in port 4000 and frontend on 3000

//This is the record type of WeatherForecast
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
