public class BidEntity
{
    public int Id { get; set; }  //Primary Key
    public int HouseId { get; set; } //This the foreign key. EF knows that this a 
                                     //foreign key because it ends with "Id" and the 
                                     //HouseEntity key is "Id"
    public HouseEntity? House { get; set; }
    public string Bidder { get; set; } = string.Empty;
    public int Amount { get; set; }
}