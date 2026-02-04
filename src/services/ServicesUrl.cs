using Microsoft.EntityFrameworkCore;

public class ServicesUrl:IServices<modelurl>
{
    private readonly DBurl _contextDB;
    private const long IDOFFSET = 1000000;
    public ServicesUrl(DBurl contextDB,Base62Converter base62Converter)
    {
        _contextDB = contextDB;
    }

    public modelurl Create_Model(string url)
    { 
        return new modelurl(url);
    }


    public async Task Add(modelurl model)
    { 
        _contextDB.Set<modelurl>().Add(model);
        await _contextDB.SaveChangesAsync();
    }

    public async Task<modelurl> Get(long id)
    {
        return await _contextDB.Urls.FindAsync(id);
    }
    public async Task<List<modelurl>> List_All()
    {
        return await _contextDB.Urls.ToListAsync();
    }

    public string GetEncode62(long id)
    {
        return Base62Converter.Encode(id + IDOFFSET); 
    }

    public async Task<string> SetEncode62(string base62) 
    {
      long id = Base62Converter.Decode(base62);
      long idReal = id - IDOFFSET;
      if (idReal <= 0) return null;
      modelurl? model = await _contextDB.Urls.AsNoTracking().FirstOrDefaultAsync(x => x.Id == idReal);
      
      return model?.Url;
    }
    public async Task Delete(long id)
    {
       var url = await _contextDB.Urls.FindAsync(id);
       if (url != null)
       {
        _contextDB.Urls.Remove(url);
        await _contextDB.SaveChangesAsync();
       }
    }
    public async Task Update(long id, modelurl model)
    {
        modelurl url = await _contextDB.Urls.FindAsync(id);
        if (url != null)
        {
            url.Url = model.Url;
            await _contextDB.SaveChangesAsync();
        }
    }
        

}
