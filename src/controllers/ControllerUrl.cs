using Microsoft.AspNetCore.Mvc;


public record CreateInput(string url);

[ApiController]
[Route("/")]
public class ControllerUrl:ControllerBase
{
    private readonly ServicesUrl _services;
    public ControllerUrl(ServicesUrl services)
    {
        _services = services;
    }
    [HttpPost]
    public async Task<IActionResult> CreateUrl([FromBody]CreateInput url)
    {
        
        await _services.Add(_services.Create_Model(url.url));
        return Created();
    }
    [HttpGet("/get/{id}")]
    public async Task<IActionResult> GetUrlById(long id)
    {
        var url = await _services.Get(id);
        if (url == null)
        {
            return NotFound();
        }
        return Ok(url);
    }
    [HttpGet("/createHash/{id}")]
    public async Task<IActionResult> CreateHashById(long id)
    {
        var hash = _services.GetEncode62(id);
        return  Ok(new { hash });
    }
    [HttpGet("/{hash}")]
    public async Task<IActionResult> RedirectToUrl(string hash)
    {
       string url = await _services.SetEncode62(hash);
       if (url == null)
       {
           return NotFound();
       }
       return RedirectPermanent(url);
    }
    [HttpGet("/list")]
    public async Task<IActionResult> GetAllUrls()
    {
        var urls = await _services.List_All();
        return Ok(urls);
    }
    [HttpPut("/update/{id}")]
    public async Task<IActionResult> UpdateUrl(long id, [FromBody] CreateInput url)
    {
        await _services.Update(id, new modelurl(url.url));
        return Ok();
    }
    [HttpDelete("/delete/{id}")]
    public async Task<IActionResult> DeleteUrlById(long id)
    {
        await _services.Delete(id);
        return Ok();
    }

    
}
