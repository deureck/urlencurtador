using Microsoft.EntityFrameworkCore;

public class DBurl:DbContext
{

    public DBurl(DbContextOptions<DBurl> options):base(options)
    {
        
    }
    
    public DbSet<modelurl> Urls{get;set;}
}

