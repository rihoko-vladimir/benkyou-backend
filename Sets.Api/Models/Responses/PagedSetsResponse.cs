namespace Sets.Api.Models.Responses;

public class PagedSetsResponse
{
    public int CurrentPage { get; set; }

    public int PagesCount { get; set; }

    public int SetsCount { get; set; }

    public ICollection<SetResponse> Sets { get; set; }
}