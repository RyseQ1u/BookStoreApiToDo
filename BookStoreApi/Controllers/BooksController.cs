using BookStoreApi.Dto;
using BookStoreApi.Models;
using BookStoreApi.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BookStoreApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BooksService _booksService;

    public BooksController(BooksService booksService) =>
        _booksService = booksService;

    [HttpGet("Book")]
    public async Task<List<Book>> Get() =>
        await _booksService.GetAsync();

    [HttpGet("BookContent")]
    public async Task<List<BookContent>> GetBookContent() =>
        await _booksService.GetBookContentAsync();

    [HttpGet("BuildingBookStore")]
    public async Task BuildingBookStore() =>
        await _booksService.BuildingBookStoreAsync();

    [HttpGet("GetBookByBookName/{bookName}")]
    public async Task<List<Book>> GetBookByBookName(string bookName)
    {
        var book = await _booksService.GetBookByBookNameAsync(bookName);

        return book;
    }

    [HttpGet("GetBookByAuthor/{author}")]
    public async Task<List<Book>> GetBookByAuthor(string author)
    {
        var book = await _booksService.GetBookByAuthorAsync(author);

        return book;
    }  
    [HttpGet("GetBookContentByBookId/{bookId:length(24)}")]
    public async Task<List<BookContent>> GetBookContentByBookId(string bookId)
    {
        var bookContent = await _booksService.GetBookContentByBookIdAsync(bookId);

        return bookContent;
    }

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Book>> Get(string id)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        return book;
    }

    [HttpPost]
    public async Task<IActionResult> Post(Book newBook)
    {
        await _booksService.CreateAsync(newBook);

        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
    }

    [HttpPost("AddBook")]
    public async Task<IActionResult> PostSplit(BookDto newBook)
    {
        await _booksService.CreateSplitAsync(newBook);

        return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Book updatedBook)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        updatedBook.Id = book.Id;

        await _booksService.UpdateAsync(id, updatedBook);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var book = await _booksService.GetAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        await _booksService.RemoveAsync(id);

        return NoContent();
    }

    [HttpDelete("DeleteAllBook")]
    public async Task<DeleteResult> DeleteAllBook()
    {
        return await _booksService.RemoveAllBookAsync();
    }

    [HttpDelete("DeleteAllBookContent")]
    public async Task<DeleteResult> DeleteAllBookContent()
    {
        return await _booksService.RemoveAllBookContentAsync();
    }
}