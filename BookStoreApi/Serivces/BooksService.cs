using BookStoreApi.Dto;
using BookStoreApi.Helper;
using BookStoreApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BookStoreApi.Services;

public class BooksService
{
    private readonly IMongoCollection<Book> _booksCollection;
    private readonly IMongoCollection<BookContent> _bookContentsCollection;

    public BooksService(
        IOptions<BookStoreDatabaseSettings> bookStoreDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            bookStoreDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            bookStoreDatabaseSettings.Value.DatabaseName);
        //创建集合的时候创建索引
        var collections = mongoDatabase.ListCollectionNames();
        var isNew = !collections.ToList().Any(x => x == bookStoreDatabaseSettings.Value.BooksCollectionName);
        _booksCollection = mongoDatabase.GetCollection<Book>(
            bookStoreDatabaseSettings.Value.BooksCollectionName);
        if (isNew)
        {
            var indexModel = new CreateIndexModel<Book>(Builders<Book>.IndexKeys.Ascending(m => m.BookName));
            _booksCollection.Indexes.CreateOne(indexModel);
        }

        _bookContentsCollection = mongoDatabase.GetCollection<BookContent>(
            bookStoreDatabaseSettings.Value.BookContentsCollectionName);
    }

    public async Task<List<Book>> GetAsync() =>
        await _booksCollection.Find(_ => true).ToListAsync();

    public async Task<List<BookContent>> GetBookContentAsync() =>
    await _bookContentsCollection.Find(_ => true).ToListAsync();

    /// <summary>
    /// 创建数据
    /// </summary>
    /// <returns></returns>
    public async Task BuildingBookStoreAsync()
    {
        var insertList = new List<Book>();
        // 创建一个随机数生成器实例
        Random random = new Random();
        var authorIndex = 0;
        for (int i = 0; i < 50000; i++)
        {
            if (i % 100 == 0) authorIndex++;
            // 生成一个 1 到 999 之间的随机整数
            int price = random.Next(1, 1000);
            insertList.Add(new Book
            {
                BookName = "Book" + i,
                Price = price,
                Author = "Ryse" + authorIndex,
                Category = "Computers",
                //内容比较大
                //Content = BookContentHelper.GetContent(),
            });
        }
        await _booksCollection.InsertManyAsync(insertList);
    }

    public async Task<List<Book>> GetBookByBookNameAsync(string bookName) =>
    await _booksCollection.Find(x => x.BookName == bookName).ToListAsync();

    public async Task<List<Book>> GetBookByAuthorAsync(string author)
    {
        return await _booksCollection.Find(x => x.Author == author).ToListAsync();
    }

    public async Task<List<BookContent>> GetBookContentByBookIdAsync(string bookId)
    {
        return await _bookContentsCollection.Find(x => x.BookId == bookId).ToListAsync();
    }

    public async Task<Book?> GetAsync(string id) =>
        await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Book newBook) =>
        await _booksCollection.InsertOneAsync(newBook);

    /// <summary>
    /// 创建新的文档 拆分内容
    /// </summary>
    /// <param name="newBook"></param>
    /// <returns></returns>
    public async Task CreateSplitAsync(BookDto newBook)
    {
        // 生成新文档的 ID
        var bookId = ObjectId.GenerateNewId().ToString();
        // 生成新文档
        var book = new Book()
        {
            Id = bookId,
            BookName = newBook.BookName,
            Author = newBook.Author,
            Category = newBook.Category,
        };
        // 生成新文档内容
        var bookContent = new BookContent
        {
            Id = ObjectId.GenerateNewId().ToString(),
            BookId = bookId,
            Content = BookContentHelper.GetContent()
        };
        await _bookContentsCollection.InsertOneAsync(bookContent);
        //分开存储
        //保存详细信息的 ID
        book.ContentId = bookContent.Id;
        await _booksCollection.InsertOneAsync(book);
    }

    public async Task UpdateAsync(string id, Book updatedBook) =>
        await _booksCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveAsync(string id) =>
        await _booksCollection.DeleteOneAsync(x => x.Id == id);

    public async Task<DeleteResult> RemoveAllBookAsync() =>
        await _booksCollection.DeleteManyAsync(x => true);

    public async Task<DeleteResult> RemoveAllBookContentAsync() =>
        await _bookContentsCollection.DeleteManyAsync(x => true);
}