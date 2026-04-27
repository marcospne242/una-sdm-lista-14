using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CacauShowApiSeuRa.Data;

namespace CacauShowApiSeuRa.Controllers;

[ApiController]
[Route("api/intelligence")]
public class ChocolateIntelligenceController : ControllerBase
{
    private readonly AppDbContext _context;

    public ChocolateIntelligenceController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("estoque-regional")]
    public IActionResult Get()
    {
        Thread.Sleep(2000);

        var dados = _context.Pedidos
            .Include(p => p.Unidade)
            .GroupBy(p => p.Unidade.Cidade)
            .Select(g => new
            {
                Cidade = g.Key,
                Total = g.Sum(x => x.Quantidade)
            }).ToList();

        return Ok(dados);
    }
}