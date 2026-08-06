using Microsoft.EntityFrameworkCore;
using System;


namespace urlencurtador.services;

public class ServicesUrl : IServices<modelurl>
{
    private readonly DBurl _contextDB;
    private const long IDOFFSET = 1000000;
    private Random rnd = new Random();
    public ServicesUrl(DBurl contextDB)
    {
        _contextDB = contextDB;
    }
  
    public async Task<modelurl> Create(string url)
    {
        string code = await GerationCode();
        modelurl model = new modelurl(url, code);
        _contextDB.Set<modelurl>().Add(model);
        await _contextDB.SaveChangesAsync();
        return model;
    }


    public async Task<modelurl?> Get(string code)
    {
        return await _contextDB.Urls.FirstOrDefaultAsync(u => u.Code == code);
    }

    public async Task<List<modelurl>> List_All()
    {
        return await _contextDB.Urls.ToListAsync();
    }

    private async Task<string> GerationCode()
    {
        long random = rnd.NextInt64();
        string code = Base62Converter.Encode(random + IDOFFSET);
        if (!await _contextDB.Urls.AnyAsync(u => u.Code == code))
        {
            return code;
        }

        return await GerationCode();
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
    public async Task Update(long id, string newUrl)
    {
        modelurl? url = await _contextDB.Urls.FindAsync(id);
        if (url != null)
        {
            url.Url = newUrl;
            await _contextDB.SaveChangesAsync();
        }
    }


}
