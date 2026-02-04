using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
[Table("urls")]
public class DBurl:DbContext
{

    public DBurl(DbContextOptions<DBurl> options):base(options)
    {
        
    }
    
    public DbSet<modelurl> Urls{get;set;}
}
