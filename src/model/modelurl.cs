
public class modelurl
{ 
    public long Id {get; private set;}
    public string Url {get; set;}
    public string Code {get; set;}
    private modelurl(){}
    public modelurl(string url, string code) 
    {
        Url = url;
        Code = code;
    }
}
