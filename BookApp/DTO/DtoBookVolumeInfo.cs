namespace BookApp.DTO;

public class DtoBookVolumeInfo
{
    public string Title { get; set; }
    public List<string> Authors { get; set; }
    public string PublishedDate { get; set; } 
    public string Description { get; set; }
    public int? PageCount { get; set; }
    public DtoBookImageLinks ImageLinks { get; set; }
}