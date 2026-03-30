using ApiECommerce.Context;
using ApiECommerce.DTOs;
using ApiECommerce.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace ApiECommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImpressaoController : ControllerBase
    {
        private readonly AppDbContext _context;

        // shared helper to extract a price from a JsonElement using several possible property names
        private static decimal? ExtractPriceFromElement(JsonElement el)
        {
            var names = new[] { "PrecoRetirar", "PrecoEntrega", "PrecoQuente", "PrecoGelada", "Preco", "PrecoUnitario", "ProdutoPreco" };
            foreach (var n in names)
            {
                if (el.TryGetProperty(n, out var prop))
                {
                    if (prop.ValueKind == JsonValueKind.Number && prop.TryGetDecimal(out var d)) return d;
                    if (prop.ValueKind == JsonValueKind.String && decimal.TryParse(prop.GetString(), out var d2)) return d2;
                }
            }
            return null;
        }

        public ImpressaoController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Impressao/pedido
        // Recebe um pedido (pode ser o objeto do front-end ou apenas um DTO com Id)
        // Normaliza itens e retorna um texto pronto para impressão com todos os detalhes
        [HttpPost("pedido")]
        public async Task<IActionResult> PrintPedido([FromBody] JsonElement payload)
        {
            if (payload.ValueKind == JsonValueKind.Undefined || payload.ValueKind == JsonValueKind.Null)
                return BadRequest("Pedido não informado.");

            // Extrair header do pedido do payload
            int pedidoId = 0;
            string clienteNome = null;
            string vendedorNome = null;
            string endereco = null;
            decimal valorTotal = 0M;

            if (payload.TryGetProperty("Id", out var idProp) && idProp.ValueKind == JsonValueKind.Number && idProp.TryGetInt32(out var idv))
                pedidoId = idv;

            if (payload.TryGetProperty("ClienteNome", out var cProp) && cProp.ValueKind == JsonValueKind.String)
                clienteNome = cProp.GetString();

            if (payload.TryGetProperty("VendedorNome", out var vProp) && vProp.ValueKind == JsonValueKind.String)
                vendedorNome = vProp.GetString();

            if (payload.TryGetProperty("Endereco", out var eProp) && eProp.ValueKind == JsonValueKind.String)
                endereco = eProp.GetString();

            if (payload.TryGetProperty("ValorTotal", out var vtProp) && vtProp.ValueKind == JsonValueKind.Number && vtProp.TryGetDecimal(out var vtd))
                valorTotal = vtd;

            // Normalizar itens vindos do JSON (aceita várias formas: ProdutoId, Produto object, ProdutoPreco/PrecoUnitario, SubTotal/Total)
            var items = new List<PedidoDetalheDTO>();
            if (payload.TryGetProperty("Itens", out var itensElem) && itensElem.ValueKind == JsonValueKind.Array)
            {
                // local helper to extract price fields from an element
                static decimal? ExtractPriceFromElement(JsonElement el)
                {
                    var names = new[] { "PrecoRetirar", "PrecoEntrega", "PrecoQuente", "PrecoGelada", "Preco", "PrecoUnitario", "ProdutoPreco" };
                    foreach (var n in names)
                    {
                        if (el.TryGetProperty(n, out var prop))
                        {
                            if (prop.ValueKind == JsonValueKind.Number && prop.TryGetDecimal(out var d)) return d;
                            if (prop.ValueKind == JsonValueKind.String && decimal.TryParse(prop.GetString(), out var d2)) return d2;
                        }
                    }
                    return null;
                }

                foreach (var it in itensElem.EnumerateArray())
                {
                    int produtoId = 0;
                    string produtoNome = null;
                    decimal preco = 0M;
                    int quantidade = 0;
                    decimal subtotal = 0M;

                    if (it.TryGetProperty("ProdutoId", out var pIdP) && pIdP.ValueKind == JsonValueKind.Number && pIdP.TryGetInt32(out var pid))
                        produtoId = pid;

                    if (it.TryGetProperty("Produto", out var prodObj) && prodObj.ValueKind == JsonValueKind.Object)
                    {
                        if (prodObj.TryGetProperty("Id", out var pid2) && pid2.ValueKind == JsonValueKind.Number && pid2.TryGetInt32(out var pidv))
                            produtoId = pidv;
                        if (prodObj.TryGetProperty("Nome", out var pnome) && pnome.ValueKind == JsonValueKind.String)
                            produtoNome = pnome.GetString();
                        var extracted = ExtractPriceFromElement(prodObj);
                        if (extracted.HasValue) preco = extracted.Value;
                    }

                    if (it.TryGetProperty("ProdutoPreco", out var ppreco) && ppreco.ValueKind == JsonValueKind.Number && ppreco.TryGetDecimal(out var pdec2))
                        preco = pdec2;

                    if (it.TryGetProperty("PrecoUnitario", out var ppreco2) && ppreco2.ValueKind == JsonValueKind.Number && ppreco2.TryGetDecimal(out var pdec3))
                        preco = pdec3;

                    if (it.TryGetProperty("Quantidade", out var q) && q.ValueKind == JsonValueKind.Number && q.TryGetInt32(out var qv))
                        quantidade = qv;

                    if (it.TryGetProperty("Total", out var t) && t.ValueKind == JsonValueKind.Number && t.TryGetDecimal(out var tv))
                        subtotal = tv;

                    if (it.TryGetProperty("SubTotal", out var st) && st.ValueKind == JsonValueKind.Number && st.TryGetDecimal(out var stv))
                        subtotal = stv;

                    // Fallback: if subtotal not provided, compute
                    if (subtotal == 0M && preco != 0M && quantidade != 0)
                        subtotal = preco * quantidade;

                    items.Add(new PedidoDetalheDTO
                    {
                        ProdutoId = produtoId,
                        ProdutoNome = produtoNome,
                        ProdutoPreco = preco,
                        Quantidade = quantidade,
                        SubTotal = subtotal
                    });
                }
            }

            // If no items provided but Id present, load from DB
            Pedido pedidoEntity = null;
            if (!items.Any() && pedidoId > 0)
            {
                pedidoEntity = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId);
                var detalhes = await _context.DetalhesPedido
                    .Where(d => d.PedidoId == pedidoId)
                    .Include(d => d.Produto)
                    .ToListAsync();

                items = detalhes.Select(d => new PedidoDetalheDTO
                {
                    Id = d.Id,
                    ProdutoId = d.ProdutoId,
                    ProdutoNome = d.Produto != null ? d.Produto.Nome : d.ProdutoNome,
                    ProdutoImagem = d.Produto != null ? d.Produto.UrlImagem : null,
                    Quantidade = d.Quantidade,
                    ProdutoPreco = d.Preco,
                    SubTotal = d.ValorTotal,
                    CaminhoImagem = d.Produto != null ? d.Produto.UrlImagem : null
                }).ToList();

                if (pedidoEntity != null)
                {
                    clienteNome ??= pedidoEntity.ClienteNome;
                    vendedorNome ??= pedidoEntity.VendedorNome;
                    endereco ??= pedidoEntity.Endereco;
                    if (valorTotal == 0M) valorTotal = pedidoEntity.ValorTotal;
                }
            }

            // For items with ProdutoId but missing ProdutoNome, fetch names
            var idsToFetch = items
                .Where(i => i.ProdutoId.HasValue && i.ProdutoId.Value > 0 && string.IsNullOrWhiteSpace(i.ProdutoNome))
                .Select(i => i.ProdutoId.Value)
                .Distinct()
                .ToList();
            if (idsToFetch.Any())
            {
                var prods = await _context.Produtos.Where(p => idsToFetch.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p.Nome);
                foreach (var it in items)
                {
                    if (it.ProdutoId.HasValue && it.ProdutoId.Value > 0 && string.IsNullOrWhiteSpace(it.ProdutoNome) && prods.TryGetValue(it.ProdutoId.Value, out var nome))
                        it.ProdutoNome = nome;
                }
            }

            // Build printable text
            var headerIdText = pedidoId > 0 ? $"Pedido #{pedidoId}\n" : string.Empty;
            var headerCliente = !string.IsNullOrWhiteSpace(clienteNome) ? $"Cliente: {clienteNome}\n" : string.Empty;
            var headerVendedor = !string.IsNullOrWhiteSpace(vendedorNome) ? $"Vendedor: {vendedorNome}\n" : string.Empty;
            var headerEndereco = !string.IsNullOrWhiteSpace(endereco) ? $"Endereco: {endereco}\n" : string.Empty;

            var itensTexto = items.Any()
                ? string.Join("\n", items.Select(i => $"- {i.Quantidade} x {i.ProdutoNome ?? "Produto"} R${i.ProdutoPreco:0.00} = R${i.SubTotal:0.00}"))
                : string.Empty;

            var totalText = valorTotal != 0M ? $"\nTotal: R${valorTotal:0.00}" : string.Empty;

            var texto = string.Concat(headerIdText, headerCliente, headerVendedor, itensTexto, "\n\n", headerEndereco, totalText);

            var result = new
            {
                Texto = texto.Trim(),
                Itens = items,
                PedidoId = pedidoId
            };

            return Ok(result);
        }

        // POST: api/Impressao/pedido/imprimir
        // Recebe o mesmo payload que PrintPedido e envia para impressora de rede 192.168.1.51:9100
        [HttpPost("pedido/imprimir")]
        public async Task<IActionResult> PrintAndSendPedido([FromBody] JsonElement payload)
        {
            // Reuse the same normalization logic as PrintPedido (duplicated here for clarity)
            if (payload.ValueKind == JsonValueKind.Undefined || payload.ValueKind == JsonValueKind.Null)
                return BadRequest("Pedido não informado.");

            int pedidoId = 0;
            string clienteNome = null;
            string vendedorNome = null;
            string endereco = null;
            decimal valorTotal = 0M;
            DateTime? dataPedido = null;
            string formaPagamento = null;
            DateTime? dataPagamentoPrazo = null;

            if (payload.TryGetProperty("Id", out var idProp) && idProp.ValueKind == JsonValueKind.Number && idProp.TryGetInt32(out var idv))
                pedidoId = idv;
            if (payload.TryGetProperty("ClienteNome", out var cProp) && cProp.ValueKind == JsonValueKind.String)
                clienteNome = cProp.GetString();
            if (payload.TryGetProperty("VendedorNome", out var vProp) && vProp.ValueKind == JsonValueKind.String)
                vendedorNome = vProp.GetString();
            if (payload.TryGetProperty("Endereco", out var eProp) && eProp.ValueKind == JsonValueKind.String)
                endereco = eProp.GetString();
            if (payload.TryGetProperty("ValorTotal", out var vtProp) && vtProp.ValueKind == JsonValueKind.Number && vtProp.TryGetDecimal(out var vtd))
                valorTotal = vtd;
            if (payload.TryGetProperty("DataPedido", out var dpProp) && dpProp.ValueKind == JsonValueKind.String && DateTime.TryParse(dpProp.GetString(), out var dpv))
                dataPedido = dpv;
            if (payload.TryGetProperty("FormaPagamento", out var fpProp) && fpProp.ValueKind == JsonValueKind.String)
                formaPagamento = fpProp.GetString();
            if (payload.TryGetProperty("DataPagamentoPrazo", out var dppProp) && dppProp.ValueKind == JsonValueKind.String && DateTime.TryParse(dppProp.GetString(), out var dppv))
                dataPagamentoPrazo = dppv;

            var items = new List<PedidoDetalheDTO>();
            if (payload.TryGetProperty("Itens", out var itensElem) && itensElem.ValueKind == JsonValueKind.Array)
            {
                foreach (var it in itensElem.EnumerateArray())
                {
                    int produtoId = 0;
                    string produtoNome = null;
                    decimal preco = 0M;
                    int quantidade = 0;
                    decimal subtotal = 0M;

                    if (it.TryGetProperty("ProdutoId", out var pIdP) && pIdP.ValueKind == JsonValueKind.Number && pIdP.TryGetInt32(out var pid))
                        produtoId = pid;
                    if (it.TryGetProperty("Produto", out var prodObj) && prodObj.ValueKind == JsonValueKind.Object)
                    {
                        if (prodObj.TryGetProperty("Id", out var pid2) && pid2.ValueKind == JsonValueKind.Number && pid2.TryGetInt32(out var pidv))
                            produtoId = pidv;
                        if (prodObj.TryGetProperty("Nome", out var pnome) && pnome.ValueKind == JsonValueKind.String)
                            produtoNome = pnome.GetString();
                        var extracted2 = ExtractPriceFromElement(prodObj);
                        if (extracted2.HasValue) preco = extracted2.Value;
                    }
                    if (it.TryGetProperty("ProdutoPreco", out var ppreco) && ppreco.ValueKind == JsonValueKind.Number && ppreco.TryGetDecimal(out var pdec2))
                        preco = pdec2;
                    if (it.TryGetProperty("PrecoUnitario", out var ppreco2) && ppreco2.ValueKind == JsonValueKind.Number && ppreco2.TryGetDecimal(out var pdec3))
                        preco = pdec3;
                    if (it.TryGetProperty("Quantidade", out var q) && q.ValueKind == JsonValueKind.Number && q.TryGetInt32(out var qv))
                        quantidade = qv;
                    if (it.TryGetProperty("Total", out var t) && t.ValueKind == JsonValueKind.Number && t.TryGetDecimal(out var tv))
                        subtotal = tv;
                    if (it.TryGetProperty("SubTotal", out var st) && st.ValueKind == JsonValueKind.Number && st.TryGetDecimal(out var stv))
                        subtotal = stv;
                    if (subtotal == 0M && preco != 0M && quantidade != 0)
                        subtotal = preco * quantidade;

                    items.Add(new PedidoDetalheDTO
                    {
                        ProdutoId = produtoId,
                        ProdutoNome = produtoNome,
                        ProdutoPreco = preco,
                        Quantidade = quantidade,
                        SubTotal = subtotal
                    });
                }
            }

            // If no items provided but Id present, load from DB
            if (!items.Any() && pedidoId > 0)
            {
                var pedidoEntity = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId);
                var detalhes = await _context.DetalhesPedido
                    .Where(d => d.PedidoId == pedidoId)
                    .Include(d => d.Produto)
                    .ToListAsync();

                items = detalhes.Select(d => new PedidoDetalheDTO
                {
                    Id = d.Id,
                    ProdutoId = d.ProdutoId,
                    ProdutoNome = d.Produto != null ? d.Produto.Nome : d.ProdutoNome,
                    ProdutoImagem = d.Produto != null ? d.Produto.UrlImagem : null,
                    Quantidade = d.Quantidade,
                    ProdutoPreco = d.Preco,
                    SubTotal = d.ValorTotal,
                    CaminhoImagem = d.Produto != null ? d.Produto.UrlImagem : null
                }).ToList();

                if (pedidoEntity != null)
                {
                    clienteNome ??= pedidoEntity.ClienteNome;
                    vendedorNome ??= pedidoEntity.VendedorNome;
                    endereco ??= pedidoEntity.Endereco;
                    if (valorTotal == 0M) valorTotal = pedidoEntity.ValorTotal;
                    dataPedido ??= pedidoEntity.DataPedido;
                    formaPagamento ??= pedidoEntity.FormaPagamento;
                    dataPagamentoPrazo ??= pedidoEntity.DataPagamentoPrazo;
                }
            }

            var idsToFetch = items
                .Where(i => i.ProdutoId.HasValue && i.ProdutoId.Value > 0 && string.IsNullOrWhiteSpace(i.ProdutoNome))
                .Select(i => i.ProdutoId.Value)
                .Distinct()
                .ToList();
            if (idsToFetch.Any())
            {
                var prods = await _context.Produtos.Where(p => idsToFetch.Contains(p.Id)).ToDictionaryAsync(p => p.Id, p => p.Nome);
                foreach (var it in items)
                {
                    if (it.ProdutoId.HasValue && it.ProdutoId.Value > 0 && string.IsNullOrWhiteSpace(it.ProdutoNome) && prods.TryGetValue(it.ProdutoId.Value, out var nome))
                        it.ProdutoNome = nome;
                }
            }

            var headerIdText = pedidoId > 0 ? $"Pedido #{pedidoId}\n" : string.Empty;
            var headerDataPedido = dataPedido.HasValue ? $"Data: {dataPedido.Value:dd/MM/yyyy HH:mm}\n" : string.Empty;
            var headerCliente = !string.IsNullOrWhiteSpace(clienteNome) ? $"Cliente: {clienteNome}\n" : string.Empty;
            var headerVendedor = !string.IsNullOrWhiteSpace(vendedorNome) ? $"Vendedor: {vendedorNome}\n" : string.Empty;
            var headerEndereco = !string.IsNullOrWhiteSpace(endereco) ? $"Endereco: {endereco}\n" : string.Empty;
            var headerFormaPagamento = !string.IsNullOrWhiteSpace(formaPagamento) ? $"Forma de Pagamento: {formaPagamento}\n" : string.Empty;
            var headerDataPagamentoPrazo = dataPagamentoPrazo.HasValue && formaPagamento?.ToLower().Contains("prazo") == true 
                ? $"Vencimento: {dataPagamentoPrazo.Value:dd/MM/yyyy}\n" 
                : string.Empty;

            var itensTexto = items.Any()
                ? string.Join("\n", items.Select(i => $"- {i.Quantidade} x {i.ProdutoNome ?? "Produto"} R${i.ProdutoPreco:0.00} = R${i.SubTotal:0.00}"))
                : string.Empty;

            var totalText = valorTotal != 0M ? $"\nTotal: R${valorTotal:0.00}" : string.Empty;

            var texto = string.Concat(
                headerIdText, 
                headerDataPedido, 
                headerCliente, 
                headerVendedor, 
                headerEndereco,
                headerFormaPagamento,
                headerDataPagamentoPrazo,
                "--------------------------\n",
                itensTexto, 
                totalText
            ).Trim();

            // Send to network printer
            var printerIp = "192.168.1.51";
            var printerPort = 9100;

            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(printerIp, printerPort);
                using var ns = client.GetStream();

                // ESC/POS init and formatting
                var init = new byte[] { 0x1B, 0x40 };
                var doubleSize = new byte[] { 0x1D, 0x21, 0x10 };
                var normalSize = new byte[] { 0x1D, 0x21, 0x00 };
                var boldOn = new byte[] { 0x1B, 0x45, 1 };
                var boldOff = new byte[] { 0x1B, 0x45, 0 };
                var cut = new byte[] { 0x1D, 0x56, 0 };

                await ns.WriteAsync(init, 0, init.Length);
                await ns.WriteAsync(doubleSize, 0, doubleSize.Length);
                await ns.WriteAsync(boldOn, 0, boldOn.Length);

                var title = "ROTA 019\n";
                await ns.WriteAsync(Encoding.UTF8.GetBytes(title));

                await ns.WriteAsync(boldOff, 0, boldOff.Length);
                await ns.WriteAsync(normalSize, 0, normalSize.Length);
                await ns.WriteAsync(Encoding.UTF8.GetBytes(texto + "\n\n\n"));
                await ns.WriteAsync(Encoding.UTF8.GetBytes("\n\n\n\n\n"));
                await ns.WriteAsync(cut, 0, cut.Length);

                await ns.FlushAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao enviar para impressora: {ex.Message}");
            }

            return Ok(new { Printed = true, PedidoId = pedidoId });
        }



        [HttpGet]
        public async Task<IActionResult> GetComandasParaImpressao()
        {
            var comandas = await _context.Comandas
                                    .Where(c => c.Status == "Aguardando Impressao")
                                    .OrderBy(c => c.Id)
                                    .Select(c => new
                                    {
                                        Comanda = c,
                                        Itens = _context.ItensComanda
                                            .Where(ic => ic.Nome == c.Nome)
                                            .Include(ic => ic.Produto)
                                            .ToList()
                                    })
                                    .ToListAsync();


            if (!comandas.Any())
                return Ok(new List<ComandaImpressaoDTO>());

            var lista = comandas.Select(c =>
            {
                var itensTexto = string.Join("\n", c.Itens
                    //.Where(i => i.Produto.CategoriaId == 99) // filtro direto
                    .Select(i => $"-{i.Quantidade}x{i.Produto?.Nome ?? "Produto"}"));

                //var itensTexto = string.Join("\n", c.Itens.Select(i =>
                //$"- {i.Quantidade} x {i.Produto?.Nome ?? "Produto"} R${i.PrecoUnitario:0.00}"));

                return new ComandaImpressaoDTO
                {
                    Id = c.Comanda.Id,
                    Texto = $"Comanda #{c.Comanda.Id}\nCliente: {c.Comanda.Nome}\n{itensTexto}\n\nEndereco: {c.Comanda.Endereco}\nTotal: R${c.Comanda.ValorTotal:0.00}"
                };
            });


            return Ok(lista);
        }

        // PUT: Atualiza status das comandas para "Impresso"
        [HttpPut]
        public async Task<IActionResult> AtualizarStatusComandas([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("Nenhuma comanda para atualizar.");

            var comandas = await _context.Comandas
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();

            if (!comandas.Any())
                return NotFound("Nenhuma comanda encontrada com os IDs informados.");

            foreach (var comanda in comandas)
            {
                comanda.Status = "Impresso";
            }

            await _context.SaveChangesAsync();

            return Ok("Status atualizado.");
        }

        [HttpPost("open")]
        public async Task<IActionResult> OpenCashDrawer([FromBody] JsonElement payload)
        {
            try
            {
                // Variantes de comandos ESC/POS para abrir gaveta
                var init = new byte[] { 0x1B, 0x40 }; // ESC @
                var pulseA = new byte[] { 0x1B, 0x70, 0x00, 0x19, 0xFA };
                var pulseB = new byte[] { 0x1B, 0x70, 0x01, 0x3C, 0x78 };
                var pulseC = new byte[] { 0x1B, 0x70, 0x00, 0x3C, 0x78 };
                var pulseD = new byte[] { 0x1B, 0x70, 0x01, 0x19, 0xFA };

                byte[] Combine(params byte[][] arrays)
                {
                    var length = arrays.Sum(a => a.Length);
                    var buffer = new byte[length];
                    var offset = 0;
                    foreach (var a in arrays)
                    {
                        Buffer.BlockCopy(a, 0, buffer, offset, a.Length);
                        offset += a.Length;
                    }
                    return buffer;
                }

                var sequences = new List<(string Name, byte[] Cmd)>
                {
                    ("Init + PulseA", Combine(init, pulseA)),
                    ("Init + PulseB", Combine(init, pulseB)),
                    ("Init + PulseC", Combine(init, pulseC)),
                    ("Init + PulseD", Combine(init, pulseD)),
                    ("PulseA (raw)", pulseA),
                    ("PulseB (raw)", pulseB),
                    ("PulseC (raw)", pulseC),
                    ("PulseD (raw)", pulseD)
                };

                // Detecta se devemos usar impressora de rede ou impressora local
                string printerIp = null;
                int printerPort = 9100;
                string printerName = "TM-T20";

                bool useNetwork = false;
                if (payload.ValueKind == JsonValueKind.Object && payload.TryGetProperty("PrinterIp", out var ipElem) && ipElem.ValueKind == JsonValueKind.String)
                {
                    useNetwork = true;
                    printerIp = ipElem.GetString();
                    if (payload.TryGetProperty("PrinterPort", out var portProp) && portProp.ValueKind == JsonValueKind.Number && portProp.TryGetInt32(out var p))
                        printerPort = p;
                }
                else if (payload.ValueKind == JsonValueKind.Object && payload.TryGetProperty("PrinterName", out var pn) && pn.ValueKind == JsonValueKind.String)
                {
                    printerName = pn.GetString();
                }

                var attempts = new List<object>();
                bool anySuccess = false;

                foreach (var seq in sequences)
                {
                    if (anySuccess) break; // stop after first successful attempt

                    try
                    {
                        if (useNetwork)
                        {
                            using var client = new TcpClient();

                            var connectTask = client.ConnectAsync(printerIp, printerPort);
                            var completed = await Task.WhenAny(connectTask, Task.Delay(3000)); // 3s timeout
                            if (completed != connectTask)
                                throw new System.TimeoutException("Timeout connecting to printer");

                            using var ns = client.GetStream();
                            await ns.WriteAsync(seq.Cmd, 0, seq.Cmd.Length);
                            await ns.FlushAsync();

                            attempts.Add(new { Sequence = seq.Name, Success = true, Transport = "tcp", Info = $"Sent {seq.Cmd.Length} bytes to {printerIp}:{printerPort}" });
                            anySuccess = true;
                        }
                        else
                        {
                            // Local printer via Windows spooler
                            RawPrinterHelper.SendBytesToPrinter(printerName, seq.Cmd);
                            attempts.Add(new { Sequence = seq.Name, Success = true, Transport = "local", Info = $"Sent {seq.Cmd.Length} bytes to printer '{printerName}'" });
                            anySuccess = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        attempts.Add(new { Sequence = seq.Name, Success = false, Error = ex.Message, Transport = useNetwork ? "tcp" : "local" });
                        // continue trying next sequence
                    }
                }

                object printerInfo = useNetwork
                    ? (object)new Dictionary<string, object> { ["Type"] = "network", ["Ip"] = printerIp ?? string.Empty, ["Port"] = printerPort }
                    : new Dictionary<string, object> { ["Type"] = "local", ["Name"] = printerName };

                return Ok(new
                {
                    Printer = printerInfo,
                    AnySuccess = anySuccess,
                    Attempts = attempts
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erro ao abrir caixa registradora",
                    error = ex.Message
                });
            }
        }

        public class RawPrinterHelper
        {
            [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA",
                SetLastError = true, CharSet = CharSet.Ansi)]
            static extern bool OpenPrinter(string pPrinterName,
                out IntPtr phPrinter, IntPtr pDefault);

            [DllImport("winspool.Drv", SetLastError = true)]
            static extern bool ClosePrinter(IntPtr hPrinter);

            [DllImport("winspool.Drv", SetLastError = true)]
            static extern bool WritePrinter(IntPtr hPrinter,
                byte[] pBytes, int dwCount, out int dwWritten);

            public static bool SendBytesToPrinter(string printerName, byte[] bytes)
            {
                IntPtr hPrinter;
                OpenPrinter(printerName, out hPrinter, IntPtr.Zero);

                WritePrinter(hPrinter, bytes, bytes.Length, out _);
                ClosePrinter(hPrinter);

                return true;
            }
        }
    }
}
