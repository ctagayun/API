using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//ApiExplorer and SwaggerGen are added to DI container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  //this enable swagger to scan for all endpoints
                         //Swagger middleware writes documentation

//Now let register the DbContext in the DI container. I am turning off 
//tracking because it is more efficient and performant because we are 
//the context is recreated everytime anyway.
builder.Services.AddDbContext<HouseDbContext>(o => 
  o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));       

builder.Services.AddScoped<IHouseRepository, HouseRepository>();
//builder.Services.AddScoped<IBidRepository, BidRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
     //Swagger is only added when we are in DEV environment
    app.UseSwagger();  //this creates standard definition of our API in JSON format
    app.UseSwaggerUI(); //this transforms JSON into a UI
}

app.UseHttpsRedirection();

//The map get extension method on the web application object enables
//us to define an endpoit that responds to a get request directly.
//No need for controllers with the routing table
// weatherforecast" - is the relative URL of the end point
// and the second part (arrow function) is Lambda that executes 
//when the endpoint is hit.

//Since HouseDbContext was registered in the DI, the container
//will automaticall provide an instance of HouseDbContext for me. 

//All there is left to do is to return all house entities, 
//by simply accessing the house's property "dbContext.Houses" of the 
//context which is a collection of house entities. these entities
//will be auto serialized to JSON.
//app.MapGet("/houses", (HouseDbContext dbContext) =>
//  dbContext.Houses.Select(h => new HouseDto(h.Id, h.Address, //Convert house entity in to a record format for React to consume
//                          h.Country, h.Price)));

app.MapGet("/houses", (IHouseRepository repo) => repo.GetAll());


app.Run();  //Finally the app is commanded to run and then we will 
            //an active endpoint.    
            //However before we run, open launchSettings.JSON and 
            //delete IAS settings and IAS EXPRESS profile
            //as well as the "//localhost:5151" in "applicationUrl"
            //and change the port to https://localhost:4000"

            //So our API will run in port 4000 and frontend on 3000

