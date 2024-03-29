using Microsoft.EntityFrameworkCore;

public interface IHouseRepository
{
    Task<List<HouseDto>> GetAll();
    Task<HouseDetailDto?> Get(int id);
    Task<HouseDetailDto> Add(HouseDetailDto house);
    Task<HouseDetailDto> Update(HouseDetailDto house);
    Task Delete(int id);
}

public class HouseRepository : IHouseRepository
{
    private readonly HouseDbContext context;

    //Populate the corresponding HouseDetailDto properties from the house entity
    private static HouseDetailDto EntityToDetailDto(HouseEntity e)
    {
        return new HouseDetailDto(e.Id, e.Address, e.Country, e.Description, e.Price, e.Photo);
    }

    //This method takes a DTO and entity as parameters an simply 
    //copies over the entity data to corresponding properties in the 
    //DTO. Id is not copied because that will be determined by the 
    //database
    private static void DtoToEntity(HouseDetailDto dto, HouseEntity e)
    {
        e.Address = dto.Address;
        e.Country = dto.Country;
        e.Description = dto.Description;
        e.Price = dto.Price;
        e.Photo = dto.Photo;
    }

    public HouseRepository(HouseDbContext context)
    {
        this.context = context;
    }

    public async Task<List<HouseDto>> GetAll()
    {
        return await context.Houses.Select(e => new HouseDto(e.Id, e.Address, e.Country, e.Price)).ToListAsync();
    }

    //Method that gets house detail record
    public async Task<HouseDetailDto?> Get(int id)
    {
        //fetch the house entity with the given id using the 
        //extension method SingleOrDefaultAsync
        var entity = await context.Houses.SingleOrDefaultAsync(h => h.Id == id);
        if (entity == null) //not found
            return null;

        //we need all these properties for the detail page    
        // return new HouseDetailDto(entity.Id, entity.Address,
        //         entity.Country, entity.Description, entity.Price, 
        //          entity.Photo);

        //Populate the corresponding HouseDetailDto properties 
        //from the house entity and then return
        return EntityToDetailDto(entity);
    }

    public async Task<HouseDetailDto> Add(HouseDetailDto dto)
    {
        var entity = new HouseEntity();
        DtoToEntity(dto, entity);  //call the conversion method
        context.Houses.Add(entity); //add the entity to the house dbSet
        await context.SaveChangesAsync(); //this will persist the data to the database

        //Populate the corresponding HouseDetailDto properties 
        //from the house entity and then return
        return EntityToDetailDto(entity);
    }

    public async Task<HouseDetailDto> Update(HouseDetailDto dto)
    {
        var entity = await context.Houses.FindAsync(dto.Id);

        if (entity == null)
            throw new ArgumentException($"Trying to update house: entity with ID {dto.Id} not found.");
       
        //Copy dto propertied to corresponding fields in the house entity
        DtoToEntity(dto, entity);
        context.Entry(entity).State = EntityState.Modified; //There is no such thing as Update command that's why we do this
        await context.SaveChangesAsync(); //this will persist the data to the database

        //Populate the corresponding HouseDetailDto properties 
        //from the house entity and then return the updated record in the DTO
        return EntityToDetailDto(entity);
    }

    public async Task Delete(int id)
    {
        var entity = await context.Houses.FindAsync(id);
        if (entity == null)
            throw new ArgumentException($"Trying to delete house: entity with ID {id} not found.");
        context.Houses.Remove(entity);
        await context.SaveChangesAsync();
    }
}