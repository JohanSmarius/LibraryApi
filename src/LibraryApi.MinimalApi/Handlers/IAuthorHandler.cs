using LibraryApi.MinimalApi.Data;
using LibraryApi.MinimalApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.MinimalApi.Handlers;

public interface IAuthorHandler
{
    Task<IResult> GetAuthorsAsync();
}