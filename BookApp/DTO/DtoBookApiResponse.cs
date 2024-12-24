namespace BookApp.DTO;

public class DtoBookApiResponse
{
    public string Kind { get; set; }
    public int TotalItems { get; set; }
    public List<DtoBook> Items { get; set; } 
}