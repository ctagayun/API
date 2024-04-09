using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;

public static class WebApplicationHouseExtensions
{
    //Define an extension method MapHouseEndpoints for web 
    //application TYPE "WebApplication"
    public static void MapHouseEndpoints(this WebApplication app)
    {
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

        app.MapGet("/houses", [Authorize](IHouseRepository repo) => repo.GetAll())
            .Produces<HouseDto[]>(StatusCodes.Status200OK);
            
        app.MapGet("/house/{houseId:int}", [Authorize] async (int houseId, IHouseRepository repo) => 
        {
            //now we can just get the house DTO by calling a new method in the repo
            var house = await repo.Get(houseId);
            if (house == null)
            //Results is a factory (similar to MVC) it produces a response with a 
            //certain HTTP status code
            return Results.Problem($"House with id {houseId} not found.",
            statusCode: 404);

            //If there is a house 
            return Results.Ok(house);
        } ).ProducesProblem(404).Produces<HouseDetailDto>(StatusCodes.Status200OK); //This lets Swagger know that this method could produce a 404

        //CRUD stuff
        //In the lambda there are two parameters:
        //      1. the HouseDetailDto object as parameter is expected.
        //      2. the second parameter is IHouseRpository
        //
        //Here we are letting the API know that it has to look for the Dto in
        //the body of the request.
        //"admin" refers to an authorization policy in program.cs.
        //see builder.Services.AddAuthorization in program.cs
        app.MapPost("/houses", [Authorize("admin")] async ([FromBody]HouseDetailDto dto, IHouseRepository repo) => 
        {
            if(!MiniValidator.TryValidate(dto, out var errors)) //vald
            return Results.ValidationProblem(errors);

            //the repo has to instructed to add the house to the database
            //now we can just add by calling the "ADD" method of the repo.
            var newHouse = await repo.Add(dto);

            //We can use Result again to return success or failure of the Post
            //Results is a factory (similar to MVC) it produces a response with a 
            //certain HTTP status code
            //In a POST by convention we return the endpoint/URL where the info about
            // new house can be foundand the new record id assigned by the database
            return Results.Created($"/house/{newHouse.Id}", newHouse);
                //If there is a house 
            } ).Produces<HouseDetailDto>(StatusCodes.Status201Created) //This lets Swagger know that this method could produce a 201
            .ProducesValidationProblem(); //vald

        app.MapPut("/houses", [Authorize] async ([FromBody]HouseDetailDto dto, IHouseRepository repo) => 
        {
            if(!MiniValidator.TryValidate(dto, out var errors)) //vald
                return Results.ValidationProblem(errors);
            //the repo has to instructed to add the house to the database
            //now we can just add by calling the "ADD" method of the repo.

            //First we need to make sure that the house being updated in the request
            //body actually exists

            if (await repo.Get(dto.Id) == null)
            return Results.Problem($"House {dto.Id} not found", statusCode: 404);

            var updatedHouse = await repo.Update(dto);

            //We can use Result again to return success or failure of the Post
            //Results is a factory (similar to MVC) it produces a response with a 
            //certain HTTP status code
            return Results.Ok(updatedHouse);
        } ).ProducesProblem(404).Produces<HouseDetailDto>(StatusCodes.Status200OK) //This lets Swagger know that this method could produce a 200
            .ProducesValidationProblem(); //vald

        app.MapDelete("/houses/{houseId:int}", [Authorize] async (int houseId, IHouseRepository repo) => 
        {
            //the repo has to instructed to delete the house to the database
            //now we can just add by calling the "Delete" method of the repo.
            //First we need to make sure that the house being deleted in the request
            //body actually exists
            if (await repo.Get(houseId) == null)
            return Results.Problem($"House {houseId} not found", statusCode: 404);

            await repo.Delete(houseId);

            //Since there is no data to return just return OK
            return Results.Ok();

        } ).ProducesProblem(404).Produces<HouseDetailDto>(StatusCodes.Status200OK); //This lets Swagger know that this method could produce a 404

    }
}