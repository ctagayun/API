/* 
  Entities are classes that are modelled around the 
  tables and columns we have in the database
*/
public class HouseEntity
{
    //when i add a property called Id the table on 
    //the database that will hold the houses entity 
    //will have a column Id. And because it is called
    //ID, it will be the primary key assigned by the 
    //database
    public int Id {get; set;}
    public string? Address {get;set;}
    public string? Country {get; set;}
    public string? Description { get; set; }
    public int Price { get; set; }
    public string? Photo { get; set; }
}