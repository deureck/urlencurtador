using Microsoft.AspNetCore.Mvc;
using urlencurtador.services;


public record CreateInput(string url);

[ApiController]
[Route("/")]
public class ControllerUrl : ControllerBase
{
    private readonly ServicesUrl _services;
    public ControllerUrl(ServicesUrl services)
    {
        _services = services;
    }
    [HttpPost]
    public async Task<IActionResult> CreateUrl([FromBody] CreateInput url)
    {
        var newUrl = await _services.Create(url.url);
        return Created("", newUrl);
    }
    [HttpGet("/get/{code}")]
    public async Task<IActionResult> GetUrlById(string code)
    {
        var url = await _services.Get(code);
        if (url == null)
        {
            return NotFound();
        }
        return Ok(url.Url);
    }

    [HttpGet("/{code}")]
    public async Task<IActionResult> RedirectToUrl(string code)
    {
        modelurl? url = await _services.Get(code);
        if (url == null)
        {
            return NotFound();
        }
        return RedirectPermanent(url.Url);
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
        await _services.Update(id, url.url);
        return Ok();
    }
    [HttpDelete("/delete/{id}")]
    public async Task<IActionResult> DeleteUrlById(long id)
    {
        await _services.Delete(id);
        return Ok();
    }


}
