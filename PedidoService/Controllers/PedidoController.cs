using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PedidoService.Models;
using System;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly HttpClient _httpClient;

    public PedidosController(AppDbContext context, HttpClient httpClient)
    {
        _context = context;
        _httpClient = httpClient;
    }

    [HttpGet]
    public IActionResult GetPedidos()
    {
        var pedidos = _context.Pedidos.ToList();
        return Ok(pedidos);
    }

    [HttpGet("{id}")]
    public IActionResult GetPedido(int id)
    {
        var pedido = _context.Pedidos.Find(id);
        if (pedido == null) return NotFound();
        return Ok(pedido);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePedido(int id)
    {
        var pedido = _context.Pedidos.Find(id);
        if (pedido == null) return NotFound("Pedido não encontrado.");

        // Validar produto (get no outro microsserviço)
        var produtoResponse = await _httpClient.GetAsync($"http://localhost:5001/api/Products/{pedido.ProdutoId}");
        if (!produtoResponse.IsSuccessStatusCode)
            return BadRequest("Produto relacionado ao pedido não encontrado.");

        var produto = JsonConvert.DeserializeObject<Produto>(
            await produtoResponse.Content.ReadAsStringAsync()
        );

        produto.Estoque += pedido.Quantidade;

        // Atualizar produto no ProdutoService, devolvendo ao estoque a quantidade solicitada no pedido
        var updateResponse = await _httpClient.PutAsJsonAsync($"http://localhost:5001/api/Products/{produto.Id}", produto);
        if (!updateResponse.IsSuccessStatusCode)
            return StatusCode(500, "Erro ao atualizar o estoque do produto.");

        _context.Pedidos.Remove(pedido);
        _context.SaveChanges();

        return NoContent();
    }



    [HttpPost]
    public async Task<IActionResult> CriarPedido([FromBody] Pedido pedido)
    {
        // Validar cliente (get no outro microsserviço)
        var clienteResponse = await _httpClient.GetAsync($"http://localhost:5002/api/Clientes/{pedido.ClienteId}");
        if (!clienteResponse.IsSuccessStatusCode)
            return BadRequest("Cliente não encontrado.");

        // Validar produto e estoque (get no outro microsserviço) 
        var produtoResponse = await _httpClient.GetAsync($"http://localhost:5001/api/Products/{pedido.ProdutoId}");
        if (!produtoResponse.IsSuccessStatusCode)
            return BadRequest("Produto não encontrado.");

        var produto = JsonConvert.DeserializeObject<Produto>(
            await produtoResponse.Content.ReadAsStringAsync()
        );

        if (produto.Estoque < pedido.Quantidade)
            return BadRequest("Estoque insuficiente.");

        pedido.DataPedido = DateTime.Now;
        _context.Pedidos.Add(pedido);
        _context.SaveChanges();

        // Decrementar do estoque, disparando no ProdutoService
        produto.Estoque -= pedido.Quantidade;
        await _httpClient.PutAsJsonAsync($"http://localhost:5001/api/Products/{produto.Id}", produto);

        return Ok(pedido);
    }
}
