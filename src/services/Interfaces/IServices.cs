public interface IServices<T>
{
    Task Add(T model);
    Task<List<T>> List_All();
    Task Delete(long id);
    Task Update(long id, T model);
    Task<T> Get(long id);
}
