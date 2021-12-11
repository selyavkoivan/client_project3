using System.Text.Json;

public class SortConfiguration {
    public string SortColumn { get; set; }
    public string SortValue { get; set; }
    public int userId { get; set; }

    public SortConfiguration(string sortColumn, string sortValue, int userId)
    {
        SortColumn = sortColumn;
        SortValue = sortValue;
        this.userId = userId;
    }
    public SortConfiguration(string sortColumn, string sortValue)
    {
        SortColumn = sortColumn;
        SortValue = sortValue;
     
    }
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);  
    }
}