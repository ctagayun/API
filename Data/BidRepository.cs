using Microsoft.EntityFrameworkCore;

public interface IBidRepository  //added for Bidder stuff
{
    Task<List<BidDto>> Get(int houseId);
    Task<BidDto> Add(BidDto bid);
}

public class BidRepository: IBidRepository
{
    private readonly HouseDbContext context;

    public BidRepository(HouseDbContext context)
    {
        this.context = context;
    }

    public async Task<List<BidDto>> Get(int houseId)
    {
        return await context.Bids.Where(b => b.HouseId == houseId)  //use LINQ where method to filter the Bids according to the given house id
            .Select(b => new BidDto(b.Id, b.HouseId, b.Bidder, b.Amount)) //after that we project the Bid entities into the BidDto and convert result to a List
            .ToListAsync(); //then convert the BidDto to a list
    }

    public async Task<BidDto> Add(BidDto dto)
    {
        var entity = new BidEntity(); //First  we create a new BidEntity 
                                     
        entity.HouseId = dto.HouseId;  //next assign the properties to the DTO values
        entity.Bidder = dto.Bidder;
        entity.Amount = dto.Amount;

        context.Bids.Add(entity);  //Next add the BidEntity to the dbSet
        await context.SaveChangesAsync(); //Next save/commit the changes

        return new BidDto(entity.Id, entity.HouseId, //Finally a new BidDto is returned
            entity.Bidder, entity.Amount);
    }
}