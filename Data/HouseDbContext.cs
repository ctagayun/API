using Microsoft.EntityFrameworkCore;

public class HouseDbContext : DbContext
{
    public HouseDbContext(DbContextOptions<HouseDbContext> options) : base(options) { }

    //The DbSet collection represents the table in the database
    public DbSet<HouseEntity> Houses => Set<HouseEntity>();
    //public DbSet<BidEntity> Bids => Set<BidEntity>();

    //Now we have to configure which database to use.
    //Create an override 
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        //the first 2 lines we are determining a good place  for 
        //the database file. this is a special directory where application 
        //data is stored.
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);

        //the final line, we are using the DbContextOptionsBuilder
        //options to tell EF Core we are using Sqlite and we pass
        //a connectionstring that points to path and filename
        //"houses.db"
        options.UseSqlite($"Data Source={Path.Join(path, "houses.db")}");
    }

    //HasData (see SeedData) will check if the listed data 
    //is already present in the database  
    //if not it will create it. not directly but via a
    //migration by creating an override "OnModelCreating"
    //in the HouseDbContext
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SeedData.Seed(modelBuilder);
    }
}
