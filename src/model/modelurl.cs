
public class modelurl
{
    public long Id {get; private set;}
    public string Url {get; set;}
    private modelurl(){}
    public modelurl(string url) 
    {
        Url = url;
    }
}
