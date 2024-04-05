using Microsoft.AspNetCore.Mvc;
using MiniValidation;

public static class WebApplicationBidExtensions
{
    public static void MapBidEndpoints(this WebApplication app)
    {
          //Added for Bidder stuff
        app.MapGet("/house/{houseId:int}/bids", async (int houseId, 
            IHouseRepository houseRepo, IBidRepository bidRepo) =>  //we ask the DI container for 
                                                                    //IHouseRepository and 
                                                                    //IBidRepository
        {
            //we need the houseRepository to see if a house with a given id exists
            if (await houseRepo.Get(houseId) == null)
            //Results is a factory (similar to MVC) it produces a response with a 
            //certain HTTP status code
            return Results.Problem($"House with id {houseId} not found.",
            statusCode: 404);

            //If there is a house call the get method on the repo to get bid details
            var bids = await bidRepo.Get(houseId);
            return Results.Ok(bids);

        } ).ProducesProblem(404).Produces<HouseDetailDto>(StatusCodes.Status200OK); //Finally we add the Swaggerinfo in the Get endpoint. This lets Swagger know that this method could produce a 404

            //Added for Bidder stuff
        app.MapPost("/house/{houseId:int}/bids", async (int houseId, 
            [FromBody] BidDto dto, IBidRepository bidRepo) =>  //we ask the DI container for IBidRepository
        {
            //we need the houseRepository to see if a house with a given id exists
            if (dto.HouseId != houseId)
            //Results is a factory (similar to MVC) it produces a response with a 
            //certain HTTP status code
            return Results.Problem("No Natch", statusCode: StatusCodes.Status400BadRequest);

            if(!MiniValidator.TryValidate(dto, out var errors)) //bidder stuff
            return Results.ValidationProblem(errors);

            //If there is a house call the get method on the repo to get bid details
            var newBid = await bidRepo.Add(dto);
            return Results.Created($"/houses/{newBid.HouseId}/bids", newBid);

        } ).ProducesProblem(404).Produces<HouseDetailDto>(StatusCodes.Status200OK); //Finally we add the Swaggerinfo in the Get endpoint. This lets Swagger know that this method could produce a 404

    }
}