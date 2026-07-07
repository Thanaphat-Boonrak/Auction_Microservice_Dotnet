namespace SearchService.RequestHelper;

public class SearchParams
{

    private int MaxPageSize { get; set; } = 50;
    private int _pageSize = 5;
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }
    
    
    public string SearchTerm { get; set; } = string.Empty;

    public int PageNumber { get; set; } = 1;



    public string Seller { get; set; } = string.Empty;

    public string Winner { get; set; } = string.Empty;

    public string OrderBy { get; set; } = string.Empty;
    public string FilterBy { get; set; } = string.Empty;
}