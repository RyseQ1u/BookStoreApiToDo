namespace BookStoreApi.Models;

public class BookContentDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string BookContentsCollectionName { get; set; } = null!;
}