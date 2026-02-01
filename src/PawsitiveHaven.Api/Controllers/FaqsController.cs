using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PawsitiveHaven.Api.Models.DTOs;
using PawsitiveHaven.Api.Services;

namespace PawsitiveHaven.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FaqsController : ControllerBase
{
    private readonly IFaqService _faqService;

    public FaqsController(IFaqService faqService)
    {
        _faqService = faqService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FaqDto>>> GetFaqs()
    {
        var faqs = await _faqService.GetActiveFaqsAsync();
        return Ok(faqs);
    }

    [HttpGet("all")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<FaqDto>>> GetAllFaqs()
    {
        var faqs = await _faqService.GetAllFaqsAsync();
        return Ok(faqs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FaqDto>> GetFaq(int id)
    {
        var faq = await _faqService.GetFaqByIdAsync(id);

        if (faq == null)
            return NotFound();

        return Ok(faq);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<FaqDto>> CreateFaq([FromBody] CreateFaqRequest request)
    {
        var faq = await _faqService.CreateFaqAsync(request);

        if (faq == null)
            return BadRequest("Failed to create FAQ");

        return CreatedAtAction(nameof(GetFaq), new { id = faq.Id }, faq);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<FaqDto>> UpdateFaq(int id, [FromBody] UpdateFaqRequest request)
    {
        var faq = await _faqService.UpdateFaqAsync(id, request);

        if (faq == null)
            return NotFound();

        return Ok(faq);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> DeleteFaq(int id)
    {
        var success = await _faqService.DeleteFaqAsync(id);

        if (!success)
            return NotFound();

        return NoContent();
    }
}
