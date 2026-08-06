public interface IServices<T>
{
    Task<T> Create(string url);
    Task<List<T>> List_All();
    Task Delete(long id);
    Task Update(long id, string newUrl);
    Task<T?> Get(string code);
}
