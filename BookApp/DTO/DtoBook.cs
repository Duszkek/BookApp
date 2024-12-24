namespace BookApp.DTO;

public class DtoBook
{
    public string Kind { get; set; }
    public string Id { get; set; }
    public DtoBookVolumeInfo VolumeInfo { get; set; }
}