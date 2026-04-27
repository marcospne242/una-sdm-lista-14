using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CacauShowApiSeuRa.Data;
using CacauShowApiSeuRa.Models;

namespace CacauShowApiSeuRa.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CacauController : ControllerBase
{
    private readonly AppDbContext _context;

    public CacauController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("produto")]
    public IActionResult CriarProduto(Produto p)
    {
        _context.Produtos.Add(p);
        _context.SaveChanges();
        return Ok(p);
    }

    [HttpPost("franquia")]
    public IActionResult CriarFranquia(Franquia f)
    {
        _context.Franquias.Add(f);
        _context.SaveChanges();
        return Ok(f);
    }

    [HttpPost("lote")]
    public IActionResult CriarLote(LoteProducao lote)
    {
        var produto = _context.Produtos.Find(lote.ProdutoId);

        if (produto == null)
            return NotFound("Produto não existe.");

        if (lote.DataFabricacao > DateTime.Now)
            return Conflict("Data inválida.");

        _context.Lotes.Add(lote);
        _context.SaveChanges();

        return Ok(lote);
    }

    [HttpPost("pedido")]
    public IActionResult CriarPedido(Pedido pedido)
    {
        var franquia = _context.Franquias.Find(pedido.UnidadeId);
        var produto = _context.Produtos.Find(pedido.ProdutoId);

        if (franquia == null || produto == null)
            return NotFound();

        int total = _context.Pedidos
            .Where(p => p.UnidadeId == pedido.UnidadeId)
            .Sum(p => p.Quantidade);

        if (total + pedido.Quantidade > franquia.CapacidadeEstoque)
            return BadRequest("Capacidade excedida.");

        decimal valor = produto.PrecoBase * pedido.Quantidade;

        if (produto.Tipo == "Sazonal")
        {
            valor += 15;
            Console.WriteLine("Produto sazonal detectado!");
        }

        pedido.ValorTotal = valor;

        _context.Pedidos.Add(pedido);
        _context.SaveChanges();

        return Ok(pedido);
    }

    [HttpPatch("lote/{id}")]
    public IActionResult AtualizarStatus(int id, [FromBody] string status)
    {
        var lote = _context.Lotes.Find(id);

        if (lote == null)
            return NotFound();

        if (lote.Status == "Descartado" &&
            (status == "Qualidade Aprovada" || status == "Distribuído"))
        {
            return BadRequest("Não pode alterar lote descartado.");
        }

        lote.Status = status;
        _context.SaveChanges();

        return Ok(lote);
    }
}