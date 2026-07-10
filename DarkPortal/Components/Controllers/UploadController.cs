using Microsoft.AspNetCore.Mvc;

namespace DarkPortal.Controllers;

[Route("api/[controller]")]
public class UploadController : Controller
{
    [HttpPost("banner")]
    public async Task<IActionResult> UploadBanner(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Файл не выбран");

        if (file.Length > 2 * 1024 * 1024)
            return BadRequest("Файл больше 2 МБ");

        var ext = Path.GetExtension(file.FileName).ToLower();
        if (ext != ".png" && ext != ".jpg" && ext != ".jpeg")
            return BadRequest("Только PNG или JPG");

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid().ToString("N")[..8]}_{DateTime.UtcNow.Ticks}{ext}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Ok(new { url = $"/uploads/{fileName}" });
    }
}