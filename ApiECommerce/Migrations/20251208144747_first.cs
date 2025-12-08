using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ApiECommerce.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UrlImagem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Despesas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FormaPagamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Parcelas = table.Column<int>(type: "int", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Despesas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotasEntrada",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroNota = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataEmissao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Fornecedor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasEntrada", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UrlImagem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Barcode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Detalhe = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UrlImagem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PrecoCusto = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    PrecoQuente = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    PrecoGelada = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    PrecoEntrega = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    PrecoRetirar = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    Popular = table.Column<bool>(type: "bit", nullable: false),
                    MaisVendido = table.Column<bool>(type: "bit", nullable: false),
                    EmEstoque = table.Column<int>(type: "int", nullable: false),
                    DiasDisponiveis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disponivel = table.Column<bool>(type: "bit", nullable: false),
                    CategoriaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Caixas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValorAbertura = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataFechamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValorFechamento = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caixas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Caixas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CascosEmprestados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    DataEmprestimo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataDevolucao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CascosEmprestados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CascosEmprestados_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CascosEmprestados_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Comandas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Endereco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    FormaPagamento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorRecebido = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comandas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comandas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comandas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pedidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Endereco = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ValorTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    DataPedido = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FormaPagamento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorPago1 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    FormaPagamento2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValorPago2 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClienteNome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendedorNome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataPagamentoPrazo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataPagamentoPrazo2 = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsuarioId = table.Column<int>(type: "int", nullable: true),
                    ClientesId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedidos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pedidos_Clientes_ClientesId",
                        column: x => x.ClientesId,
                        principalTable: "Clientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Pedidos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DetalhesCascosEmprestados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalhesCascosEmprestados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalhesCascosEmprestados_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalhesComanda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Preco = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    FormaPagamento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    DataAbertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataPedido = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalhesComanda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalhesComanda_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Estoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    QuantidadeMinima = table.Column<int>(type: "int", nullable: true),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataValidade = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PrecoCusto = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Margem = table.Column<int>(type: "int", nullable: true),
                    LocalArmazenamento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estoque_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    QuantidadeFisica = table.Column<int>(type: "int", nullable: false),
                    DataInventario = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioResponsavel = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inventarios_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensCarrinhoCompra",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensCarrinhoCompra", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensCarrinhoCompra_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotaEntradaItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotaEntradaId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Barcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoCusto = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotaEntradaItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotaEntradaItem_NotasEntrada_NotaEntradaId",
                        column: x => x.NotaEntradaId,
                        principalTable: "NotasEntrada",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotaEntradaItem_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItensComanda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    ComandaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensComanda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensComanda_Comandas_ComandaId",
                        column: x => x.ComandaId,
                        principalTable: "Comandas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensComanda_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalhesPedido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Preco = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    ValorTotal = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    FormaPagamento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PedidoId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: true),
                    ProdutoNome = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalhesPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalhesPedido_Pedidos_PedidoId",
                        column: x => x.PedidoId,
                        principalTable: "Pedidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalhesPedido_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimentacoesEstoque",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoCusto = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    DataMovimentacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observacao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstoqueId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimentacoesEstoque", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimentacoesEstoque_Estoque_EstoqueId",
                        column: x => x.EstoqueId,
                        principalTable: "Estoque",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MovimentacoesEstoque_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Nome", "UrlImagem" },
                values: new object[,]
                {
                    { 1, "Refrigerante", "refrigerantes1.png" },
                    { 2, "Cervejas", "cervejas.png" },
                    { 3, "Sucos", "sucos.png" },
                    { 4, "Energético", "energeticos.png" },
                    { 5, "Ices", "ices.png" },
                    { 6, "Cigarros", "cigarros.png" },
                    { 7, "Sedas", "sedas.jpg" },
                    { 8, "Salgadinhos", "salgadinhos.jpg" },
                    { 9, "Doses", "doses.jpg" },
                    { 10, "Água", "aguas.jpg" },
                    { 11, "Carvões", "carvoes.jpg" },
                    { 12, "Vinhos", "vinhos.jpg" },
                    { 13, "Gelos", "gelos.jpg" },
                    { 14, "Drinks", "drinks.jpg" },
                    { 15, "Quentes", "quentes.jpg" },
                    { 16, "Gins", "gins.jpg" },
                    { 17, "Comidas", "comidas.png" },
                    { 18, "Porções", "porcoes.jpg" },
                    { 19, "Combos", "combos.jpg" },
                    { 20, "Doces", "doces.png" },
                    { 21, "Outros", "outros.png" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "PrecoEntrega", "PrecoGelada", "PrecoQuente", "PrecoRetirar", "UrlImagem" },
                values: new object[,]
                {
                    { 1, null, 1, "COCA COLA 2l RET", null, true, 1, true, "COCA COLA 2l RET", true, 8.49m, 8.49m, 8.49m, 8.49m, "cocacola2lret.jpg" },
                    { 2, null, 2, "CERVEJA ORIGINAL LATA 350ML", null, true, 1, true, "CERVEJA ORIGINAL LATA 350ML", true, 4.99m, 4.99m, 4.99m, 4.99m, "cervejaoriginallata350ml.jpg" },
                    { 3, null, 2, "BARRIGUDINHA ORGINAL 300ml", null, true, 1, true, "BARRIGUDINHA ORGINAL 300ml", true, 3.80m, 3.80m, 3.80m, 3.80m, "barrigudinhaorginal300ml.jpg" },
                    { 4, null, 2, "BARRIGUDINHA SKOL 300 ml", null, true, 1, true, "BARRIGUDINHA SKOL 300 ml", true, 2.99m, 2.99m, 2.99m, 2.99m, "barrigudinhaskol300ml.jpg" },
                    { 5, null, 2, "BARRIGUDINHA BRAHMA 300ml", null, true, 1, true, "BARRIGUDINHA BRAHMA 300ml", true, 2.99m, 2.99m, 2.99m, 2.99m, "barrigudinhabrahma300ml.jpg" },
                    { 6, null, 2, "CRISTAL LATA 350ml", null, true, 1, true, "CRISTAL LATA 350ml", true, 2.99m, 2.99m, 2.99m, 2.99m, "cristallata350ml.jpg" },
                    { 7, null, 1, "XERETA SABORES 2l", null, true, 1, true, "XERETA SABORES 2l", true, 5.99m, 5.99m, 5.99m, 5.99m, "xeretasabores2l.jpg" },
                    { 8, null, 1, "COCA COLA 2L PET", null, true, 1, true, "COCA COLA 2L PET", true, 13.99m, 13.99m, 13.99m, 13.99m, "cocacola2lpet.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "PrecoRetirar", "UrlImagem" },
                values: new object[,]
                {
                    { 9, null, 1, "COCA COLA 350ML LATA", null, true, 1, true, "COCA COLA 350ML LATA", true, 4.99m, "cocacola350mllata.jpg" },
                    { 10, null, 1, "COCA COLA 200ML GARR", null, true, 1, true, "COCA COLA 200ML GARR", true, 2.50m, "cocacola200mlgarr.jpg" },
                    { 11, null, 1, "COCA COLA KS", null, true, 1, true, "COCA COLA KS", true, 4.50m, "cocacolaks.jpg" },
                    { 12, null, 3, "TAMPICO 450ML", null, true, 1, true, "TAMPICO 450ML", true, 4.99m, "tampico450ml.jpg" },
                    { 13, null, 3, "TAMPICO 2L", null, true, 1, true, "TAMPICO 2L", true, 11.99m, "tampico2l.jpg" },
                    { 14, null, 3, "TAMPICO 270ML LATA UVA", null, true, 1, true, "TAMPICO 270ML LATA UVA", true, 3.99m, "tampico270mllatauva.jpg" },
                    { 15, null, 3, "TAMPICO 270ML LATA LARANJA", null, true, 1, true, "TAMPICO 270ML LATA LARANJA", true, 3.99m, "tampico270mllatalaranja.jpg" },
                    { 16, null, 3, "DEL VALLE 290ML PESSÊGO", null, true, 1, true, "DEL VALLE 290ML PESSÊGO", true, 4.99m, "delvalle290mlpessêgo.jpg" },
                    { 17, null, 3, "DEL VALLE 290ML MANGA", null, true, 1, true, "DEL VALLE 290ML MANGA", true, 4.99m, "delvalle290mlmanga.jpg" },
                    { 18, null, 3, "DEL VALLE 290ML GOIABA", null, true, 1, true, "DEL VALLE 290ML GOIABA", true, 4.99m, "delvalle290mlgoiaba.jpg" },
                    { 19, null, 4, "MONSTER 473ML GUARANÁ LATA", null, true, 1, true, "MONSTER 473ML GUARANÁ LATA", true, 11.50m, "monster473mlguaranálata.jpg" },
                    { 20, null, 4, "MONSTER 437ML MANGO LATA", null, true, 1, true, "MONSTER 437ML MANGO LATA", true, 11.50m, "monster437mlmangolata.jpg" },
                    { 21, null, 4, "TNT 437ML LATA", null, true, 1, true, "TNT 437ML LATA", true, 10.99m, "tnt437mllata.jpg" },
                    { 22, null, 2, "CERVEJA IMPERIO LATA 350ml", null, true, 1, true, "CERVEJA IMPERIO LATA 350ml", true, 3.99m, "cervejaimperiolata350ml.jpg" },
                    { 23, null, 2, "HEINEKEN LONGE NECK 330ml", null, true, 1, true, "HEINEKEN LONGE NECK 330ml", true, 7.99m, "heinekenlongeneck330ml.jpg" },
                    { 24, null, 2, "ORIGINAL GARRAFA 600 ml", null, true, 1, true, "ORIGINAL GARRAFA 600 ml", true, 10.00m, "originalgarrafa600ml.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "UrlImagem" },
                values: new object[] { 25, null, 2, "CERVEJA HEINEKEM 600 ml", null, true, 1, true, "CERVEJA HEINEKEM 600 ml", true, "cervejaheinekem600ml.jpg" });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "PrecoRetirar", "UrlImagem" },
                values: new object[,]
                {
                    { 26, null, 5, "ICE LEEV 275ml", null, true, 1, true, "ICE LEEV 275ml", true, 6.99m, "iceleev275ml.jpg" },
                    { 27, null, 5, "CABARE ICE 275ml", null, true, 1, true, "CABARE ICE 275ml", true, 7.99m, "cabareice275ml.jpg" },
                    { 28, null, 2, "CERVEJA AMSTEL 355 ML", null, true, 1, true, "CERVEJA AMSTEL 355 ML", true, 3.99m, "cervejaamstel355ml.jpg" },
                    { 29, null, 2, "CERVEJA ITAIPAVA MALZERBIER 330ml", null, true, 1, true, "CERVEJA ITAIPAVA MALZERBIER 330ml", true, 6.00m, "cervejaitaipavamalzerbier330ml.jpg" },
                    { 30, null, 5, "ICE ASKOV 275 ml", null, true, 1, true, "ICE ASKOV 275 ml", true, 6.99m, "iceaskov275ml.jpg" },
                    { 31, null, 4, "ENERGICO BALY 2l DIVERSOS", null, true, 1, true, "ENERGICO BALY 2l DIVERSOS", true, 10.99m, "energicobaly2ldiversos.jpg" },
                    { 32, null, 2, "CERVEJA ITAIPAVA ZERO 330 ml", null, true, 1, true, "CERVEJA ITAIPAVA ZERO 330 ml", true, 4.50m, "cervejaitaipavazero330ml.jpg" },
                    { 33, null, 6, "CIGARRO SAMARINO", null, true, 1, true, "CIGARRO SAMARINO", true, 6.00m, "cigarrosamarino.jpg" },
                    { 34, null, 6, "CIGARRO EGYPT", null, true, 1, true, "CIGARRO EGYPT", true, 6.50m, "cigarroegypt.jpg" },
                    { 35, null, 6, "CIGARRO CHESTERFILD BRANCO", null, true, 1, true, "CIGARRO CHESTERFILD BRANCO", true, 10.50m, "cigarrochesterfildbranco.jpg" },
                    { 36, null, 6, "CIGARRO MALBORO", null, true, 1, true, "CIGARRO MALBORO", true, 14.50m, "cigarromalboro.jpg" },
                    { 37, null, 6, "PALHEIRO BLUE ICE", null, true, 1, true, "PALHEIRO BLUE ICE", true, 8.00m, "palheiroblueice.jpg" },
                    { 38, null, 6, "PALHEIRO TOMBADA MENTa", null, true, 1, true, "PALHEIRO TOMBADA MENTa", true, 8.00m, "palheirotombadamenta.jpg" },
                    { 39, null, 6, "PALHEIRO TRADICIONAL", null, true, 1, true, "PALHEIRO TRADICIONAL", true, 8.00m, "palheirotradicional.jpg" },
                    { 40, null, 7, "SEDA ZOMO", null, true, 1, true, "SEDA ZOMO", true, 4.00m, "sedazomo.jpg" },
                    { 41, null, 6, "DON TABACCO BREZE", null, true, 1, true, "DON TABACCO BREZE", true, 15.00m, "dontabaccobreze.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "UrlImagem" },
                values: new object[] { 42, null, 6, "TRITURADOR.", null, true, 1, true, "TRITURADOR.", true, "triturador..jpg" });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "PrecoRetirar", "UrlImagem" },
                values: new object[,]
                {
                    { 43, null, 10, "AGUA TONICA 350ml", null, true, 1, true, "AGUA TONICA 350ml", true, 4.99m, "aguatonica350ml.jpg" },
                    { 44, null, 2, "CERVEJA AMISTEL 350 LATA", null, true, 1, true, "CERVEJA AMISTEL 350 LATA", true, 5.00m, "cervejaamistel350lata.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "UrlImagem" },
                values: new object[] { 45, null, 4, "ATOMIC ENERG 270ML LATA", null, true, 1, true, "ATOMIC ENERG 270ML LATA", true, "atomicenerg270mllata.jpg" });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "PrecoRetirar", "UrlImagem" },
                values: new object[,]
                {
                    { 46, null, 16, "SCHWEPPERS MIXED GIN TONICA PINK 269ML", null, true, 1, true, "SCHWEPPERS MIXED GIN TONICA PINK 269ML", true, 5.00m, "schweppersmixedgintonicapink269ml.jpg" },
                    { 47, null, 2, "CERVEJA AMSTEL 269ML LATA", null, true, 1, true, "CERVEJA AMSTEL 269ML LATA", true, 2.99m, "cervejaamstel269mllata.jpg" },
                    { 48, null, 1, "COCA COLA KS ZERO", null, true, 1, true, "COCA COLA KS ZERO", true, 4.50m, "cocacolakszero.jpg" },
                    { 49, null, 21, "GATORADE SABOR UVA 500ml", null, true, 1, true, "GATORADE SABOR UVA 500ml", true, 7.99m, "gatoradesaboruva500ml.jpg" },
                    { 50, null, 6, "ISQUEIRO TRANPARESTE", null, true, 1, true, "ISQUEIRO TRANPARESTE", true, 3.00m, "isqueirotranpareste.jpg" },
                    { 51, null, 4, "ENERGICO FLYING HORSE", null, true, 1, true, "ENERGICO FLYING HORSE", true, 7.99m, "energicoflyinghorse.jpg" },
                    { 52, null, 1, "FANTA LARANJA 350ML LATA", null, true, 1, true, "FANTA LARANJA 350ML LATA", true, 4.99m, "fantalaranja350mllata.jpg" },
                    { 53, null, 1, "GUARANA ANTARCTICA LATA 350 ml", null, true, 1, true, "GUARANA ANTARCTICA LATA 350 ml", true, 5.00m, "guaranaantarcticalata350ml.jpg" },
                    { 54, null, 1, "ITUBAINA ORGIANAL LATA 350ml", null, true, 1, true, "ITUBAINA ORGIANAL LATA 350ml", true, 5.00m, "itubainaorgianallata350ml.jpg" },
                    { 55, null, 1, "SCHWEPPES 350ML LATA", null, true, 1, true, "SCHWEPPES 350ML LATA", true, 4.99m, "schweppes350mllata.jpg" },
                    { 56, null, 2, "HEINEKEN LONGE NECK 250 ml", null, true, 1, true, "HEINEKEN LONGE NECK 250 ml", true, 5.00m, "heinekenlongeneck250ml.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "UrlImagem" },
                values: new object[] { 57, null, 2, "HENEKEN ZERO LONGE NECK 330 ml", null, true, 1, true, "HENEKEN ZERO LONGE NECK 330 ml", true, "henekenzerolongeneck330ml.jpg" });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "PrecoRetirar", "UrlImagem" },
                values: new object[,]
                {
                    { 58, null, 8, "TORCIDA SABORES", null, true, 1, true, "TORCIDA SABORES", true, 3.49m, "torcidasabores.jpg" },
                    { 59, null, 8, "FABITOS SABORESN 80gm", null, true, 1, true, "FABITOS SABORESN 80gm", true, 2.50m, "fabitossaboresn80gm.jpg" },
                    { 60, null, 8, "BISCOITIO DE POLVILHO 100gm", null, true, 1, true, "BISCOITIO DE POLVILHO 100gm", true, 5.50m, "biscoitiodepolvilho100gm.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "UrlImagem" },
                values: new object[,]
                {
                    { 61, null, 1, "GUANA ANTARTICA 350ml LATA", null, true, 1, true, "GUANA ANTARTICA 350ml LATA", true, "guanaantartica350mllata.jpg" },
                    { 62, null, 1, "GUANA ANTARTICA 200ml GARRAFA", null, true, 1, true, "GUANA ANTARTICA 200ml GARRAFA", true, "guanaantartica200mlgarrafa.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "PrecoRetirar", "UrlImagem" },
                values: new object[,]
                {
                    { 63, null, 1, "SPRITE 350 ml LATA", null, true, 1, true, "SPRITE 350 ml LATA", true, 5.00m, "sprite350mllata.jpg" },
                    { 64, null, 2, "HEINEKEN 350ml", null, true, 1, true, "HEINEKEN 350ml", true, 5.99m, "heineken350ml.jpg" },
                    { 65, null, 9, "DOSE SEM LIMAO", null, true, 1, true, "DOSE SEM LIMAO", true, 4.00m, "dosesemlimao.jpg" },
                    { 66, null, 9, "DOSE COM LIMAO", null, true, 1, true, "DOSE COM LIMAO", true, 5.00m, "dosecomlimao.jpg" },
                    { 67, null, 9, "DOSE JAMEL COM PARATUDO", null, true, 1, true, "DOSE JAMEL COM PARATUDO", true, 6.00m, "dosejamelcomparatudo.jpg" },
                    { 68, null, 9, "DESE JURUBEBA", null, true, 1, true, "DESE JURUBEBA", true, 3.00m, "desejurubeba.jpg" },
                    { 69, null, 10, "AGUA 510ml", null, true, 1, true, "AGUA 510ml", true, 2.99m, "agua510ml.jpg" },
                    { 70, null, 10, "AGUA COM GAS 510ml", null, true, 1, true, "AGUA COM GAS 510ml", true, 3.99m, "aguacomgas510ml.jpg" },
                    { 71, null, 11, "CARVAO TARTARUGAO 2KG", null, true, 1, true, "CARVAO TARTARUGAO 2KG", true, 14.99m, "carvaotartarugao2kg.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "UrlImagem" },
                values: new object[,]
                {
                    { 72, null, 11, "CARTVAO TARTARUGAO 4KG", null, true, 1, true, "CARTVAO TARTARUGAO 4KG", true, "cartvaotartarugao4kg.jpg" },
                    { 73, null, 20, "PAÇOCAO", null, true, 1, true, "PAÇOCAO", true, "paçocao.jpg" },
                    { 74, null, 9, "PAÇOCA ROLHA", null, true, 1, true, "PAÇOCA ROLHA", true, "paçocarolha.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "PrecoRetirar", "UrlImagem" },
                values: new object[,]
                {
                    { 75, null, 2, "HEINEKEN 600ml", null, true, 1, true, "HEINEKEN 600ml", true, 14.99m, "heineken600ml.jpg" },
                    { 76, null, 2, "EISENBAHM 600ml", null, true, 1, true, "EISENBAHM 600ml", true, 6.00m, "eisenbahm600ml.jpg" },
                    { 77, null, 1, "FANTA UVA 350ml LATA", null, true, 1, true, "FANTA UVA 350ml LATA", true, 5.00m, "fantauva350mllata.jpg" },
                    { 78, null, 2, "EISENBAHN 350ml", null, true, 1, true, "EISENBAHN 350ml", true, 3.99m, "eisenbahn350ml.jpg" },
                    { 79, null, 2, "HENEKEN ZERO LONGE NECK 330 ml", null, true, 1, true, "HENEKEN ZERO LONGE NECK 330 ml", true, 7.99m, "henekenzerolongeneck330ml.jpg" },
                    { 80, null, 2, "CORONA", null, true, 1, true, "CORONA", true, 7.99m, "corona.jpg" },
                    { 81, null, 2, "CERVEJA STELLA LONGE NECK 330ml", null, true, 1, true, "CERVEJA STELLA LONGE NECK 330ml", true, 7.99m, "cervejastellalongeneck330ml.jpg" },
                    { 82, null, 2, "HEINEKEN LATA 269 ml", null, true, 1, true, "HEINEKEN LATA 269 ml", true, 4.50m, "heinekenlata269ml.jpg" },
                    { 83, null, 4, "TNT ORINAL", null, true, 1, true, "TNT ORINAL", true, 9.99m, "tntorinal.jpg" },
                    { 84, null, 4, "ATOMIC ENERGICO", null, true, 1, true, "ATOMIC ENERGICO", true, 5.00m, "atomicenergico.jpg" },
                    { 85, null, 12, "VINHO CHAPINHA", null, true, 1, true, "VINHO CHAPINHA", true, 19.99m, "vinhochapinha.jpg" },
                    { 86, null, 21, "COPÂO DE WISKY", null, true, 1, true, "COPÂO DE WISKY", true, 15.00m, "copâodewisky.jpg" },
                    { 87, null, 13, "GELO PACOTE 2KG", null, true, 1, true, "GELO PACOTE 2KG", true, 7.99m, "gelopacote2kg.jpg" },
                    { 88, null, 13, "GELO SACO 5KG", null, true, 1, true, "GELO SACO 5KG", true, 10.99m, "gelosaco5kg.jpg" },
                    { 89, null, 13, "GELO DE COCO", null, true, 1, true, "GELO DE COCO", true, 3.00m, "gelodecoco.jpg" },
                    { 90, null, 13, "GELO DE COCO MORANGO", null, true, 1, true, "GELO DE COCO MORANGO", true, 3.00m, "gelodecocomorango.jpg" },
                    { 91, null, 13, "GELO DE COCO MARUCAJA", null, true, 1, true, "GELO DE COCO MARUCAJA", true, 3.00m, "gelodecocomarucaja.jpg" },
                    { 92, null, 13, "GELO DE COCO MELANCIA", null, true, 1, true, "GELO DE COCO MELANCIA", true, 3.00m, "gelodecocomelancia.jpg" },
                    { 93, null, 14, "NUSAQUINH IORDUT DE MORANGO", null, true, 1, true, "NUSAQUINH IORDUT DE MORANGO", true, 18.00m, "nusaquinhiordutdemorango.jpg" },
                    { 94, null, 15, "ASKOV LIMÂO 900ml", null, true, 1, true, "ASKOV LIMÂO 900ml", true, 19.99m, "askovlimâo900ml.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "UrlImagem" },
                values: new object[,]
                {
                    { 95, null, 15, "ASKOV KIWI 900ml", null, true, 1, true, "ASKOV KIWI 900ml", true, "askovkiwi900ml.jpg" },
                    { 96, null, 15, "ASKOV MARACUJÀ 900ml", null, true, 1, true, "ASKOV MARACUJÀ 900ml", true, "askovmaracujà900ml.jpg" },
                    { 97, null, 15, "ASKOV BLUEBERRY 900ml", null, true, 1, true, "ASKOV BLUEBERRY 900ml", true, "askovblueberry900ml.jpg" },
                    { 98, null, 15, "ASKOV VODKA 900ml", null, true, 1, true, "ASKOV VODKA 900ml", true, "askovvodka900ml.jpg" },
                    { 99, null, 15, "ASKOV FRUTAS ROXAS 900ml", null, true, 1, true, "ASKOV FRUTAS ROXAS 900ml", true, "askovfrutasroxas900ml.jpg" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Barcode", "CategoriaId", "Detalhe", "DiasDisponiveis", "Disponivel", "EmEstoque", "MaisVendido", "Nome", "Popular", "PrecoRetirar", "UrlImagem" },
                values: new object[,]
                {
                    { 100, null, 16, "ETERNITY GIN TROPICAL FRUTS 900ml", null, true, 1, true, "ETERNITY GIN TROPICAL FRUTS 900ml", true, 23.99m, "eternitygintropicalfruts900ml.jpg" },
                    { 101, null, 16, "ETERNITY GIN MAÇÂ VERDE 900ml", null, true, 1, true, "ETERNITY GIN MAÇÂ VERDE 900ml", true, 23.99m, "eternityginmaçâverde900ml.jpg" },
                    { 102, null, 16, "ETERNITY GIN WATERMWLON 900ml", null, true, 1, true, "ETERNITY GIN WATERMWLON 900ml", true, 23.99m, "eternityginwatermwlon900ml.jpg" },
                    { 103, null, 16, "ETERNITY GIN STRAWBERRY 900ml", null, true, 1, true, "ETERNITY GIN STRAWBERRY 900ml", true, 23.99m, "eternityginstrawberry900ml.jpg" },
                    { 104, null, 15, "MANSAO MAROMBA WHISK+COMBO", null, true, 1, true, "MANSAO MAROMBA WHISK+COMBO", true, 13.99m, "mansaomarombawhisk+combo.jpg" },
                    { 105, null, 15, "MANSAO MAROMBA VODKA+CAFEINA", null, true, 1, true, "MANSAO MAROMBA VODKA+CAFEINA", true, 13.99m, "mansaomarombavodka+cafeina.jpg" },
                    { 106, null, 15, "MANSAO MAROMBA MACA VERDE", null, true, 1, true, "MANSAO MAROMBA MACA VERDE", true, 13.99m, "mansaomarombamacaverde.jpg" },
                    { 107, null, 15, "MANSAO MAROMBA WHISKY+COCONUT", null, true, 1, true, "MANSAO MAROMBA WHISKY+COCONUT", true, 13.99m, "mansaomarombawhisky+coconut.jpg" },
                    { 108, null, 15, "MANSAO MAROMBA MELANCIA", null, true, 1, true, "MANSAO MAROMBA MELANCIA", true, 13.99m, "mansaomarombamelancia.jpg" },
                    { 109, null, 2, "CERVEJA ITAIPAVA ZERO 330 ml", null, true, 1, true, "CERVEJA ITAIPAVA ZERO 330 ml", true, 4.50m, "cervejaitaipavazero330ml.jpg" },
                    { 110, null, 2, "CERVEJA AMSTEL 600ML", null, true, 1, true, "CERVEJA AMSTEL 600ML", true, 9.99m, "cervejaamstel600ml.jpg" },
                    { 111, null, 15, "BAIANINHA AMENDOIN 900", null, true, 1, true, "BAIANINHA AMENDOIN 900", true, 18.99m, "baianinhaamendoin900.jpg" },
                    { 112, null, 15, "SAO BERNADO 475ML", null, true, 1, true, "SAO BERNADO 475ML", true, 3.99m, "saobernado475ml.jpg" },
                    { 113, null, 15, "COROTE JAPIRA 500ML", null, true, 1, true, "COROTE JAPIRA 500ML", true, 3.99m, "corotejapira500ml.jpg" },
                    { 114, null, 6, "ROTHMANS AZUL", null, true, 1, true, "ROTHMANS AZUL", true, 8.50m, "rothmansazul.jpg" },
                    { 115, null, 6, "ROTHMANS AZUL", null, true, 1, true, "ROTHMANS AZUL", true, 8.50m, "rothmansazul.jpg" },
                    { 116, null, 6, "PALHEIRO UNIVERSITARIO MENTA", null, true, 1, true, "PALHEIRO UNIVERSITARIO MENTA", true, 8.00m, "palheirouniversitariomenta.jpg" },
                    { 117, null, 11, "CARVAO TARTARUGAO 5KG", null, true, 1, true, "CARVAO TARTARUGAO 5KG", true, 27.99m, "carvaotartarugao5kg.jpg" },
                    { 118, null, 2, "SKOL LITRAO 1L", null, true, 1, true, "SKOL LITRAO 1L", true, 11.99m, "skollitrao1l.jpg" },
                    { 119, null, 3, "DEL VALLE MARACUJÁ 1L", null, true, 1, true, "DEL VALLE MARACUJÁ 1L", true, 10.99m, "delvallemaracujá1l.jpg" },
                    { 120, null, 21, "COPÃO JACK DANIELS / WHITE HOURSE", null, true, 1, true, "COPÃO JACK DANIELS / WHITE HOURSE", true, 25.00m, "copãojackdaniels/whitehourse.jpg" },
                    { 121, null, 15, "COPAO RED LABEL", null, true, 1, true, "COPAO RED LABEL", true, 16.00m, "copaoredlabel.jpg" },
                    { 122, null, 21, "COMBO PASSAPORTE / GYM / VODKA", null, true, 1, true, "COMBO PASSAPORTE / GYM / VODKA", true, 10.00m, "combopassaporte/gym/vodka.jpg" },
                    { 123, null, 21, "CIGARRO SOLTO CHESTERFIELD / ROTHMANS", null, true, 1, true, "CIGARRO SOLTO CHESTERFIELD / ROTHMANS", true, 0.50m, "cigarrosoltochesterfield/rothmans.jpg" },
                    { 124, null, 21, "CHOP VINHO DRAFT", null, true, 1, true, "CHOP VINHO DRAFT", true, 16.00m, "chopvinhodraft.jpg" },
                    { 125, null, 21, "HEINEKEN ZERO LONG 330ml", null, true, 1, true, "HEINEKEN ZERO LONG 330ml", true, 6.99m, "heinekenzerolong330ml.jpg" },
                    { 126, null, 21, "ESPETO DE CARNE", null, true, 1, true, "ESPETO DE CARNE", true, 49.99m, "espetodecarne.jpg" },
                    { 127, null, 2, "BUDWEISER LATA 350ML", null, true, 1, true, "BUDWEISER LATA 350ML", true, 4.50m, "budweiserlata350ml.jpg" },
                    { 128, null, 1, "COCA COLA 600ML", null, true, 1, true, "COCA COLA 600ML", true, 5.99m, "cocacola600ml.jpg" },
                    { 129, null, 1, "COCA COLA 1L", null, true, 1, true, "COCA COLA 1L", true, 4.99m, "cocacola1l.jpg" },
                    { 130, null, 15, "JACK DANIEILS", null, true, 1, true, "JACK DANIEILS", true, 10.00m, "jackdanieils.jpg" },
                    { 131, null, 2, "CERVEJA ANTARCTICA", null, true, 1, true, "CERVEJA ANTARCTICA", true, 3.00m, "cervejaantarctica.jpg" },
                    { 132, null, 2, "PETRA BARRIGUDINHA", null, true, 1, true, "PETRA BARRIGUDINHA", true, 3.00m, "petrabarrigudinha.jpg" },
                    { 133, null, 4, "ENERGICO MONSTER MELON", null, true, 1, true, "ENERGICO MONSTER MELON", true, 11.50m, "energicomonstermelon.jpg" },
                    { 134, null, 4, "ENERGICO MOSTER PARADISE", null, true, 1, true, "ENERGICO MOSTER PARADISE", true, 10.99m, "energicomosterparadise.jpg" },
                    { 135, null, 2, "CERVEJA PETRA LATA 350ML", null, true, 1, true, "CERVEJA PETRA LATA 350ML", true, 3.50m, "cervejapetralata350ml.jpg" },
                    { 136, null, 4, "RED BULL 250ML", null, true, 1, true, "RED BULL 250ML", true, 9.99m, "redbull250ml.jpg" },
                    { 137, null, 2, "CERVEJA ITAIPAVA LATA 350 ML", null, true, 1, true, "CERVEJA ITAIPAVA LATA 350 ML", true, 3.50m, "cervejaitaipavalata350ml.jpg" },
                    { 138, null, 17, "ESPETO SABORES", null, true, 1, true, "ESPETO SABORES", true, 8.00m, "espetosabores.jpg" },
                    { 139, null, 17, "ESPETO CAFITA", null, true, 1, true, "ESPETO CAFITA", true, 10.00m, "espetocafita.jpg" },
                    { 140, null, 20, "CHICLETE", null, true, 1, true, "CHICLETE", true, 0.15m, "chiclete.jpg" },
                    { 141, null, 20, "CHOQUITO", null, true, 1, true, "CHOQUITO", true, 2.80m, "choquito.jpg" },
                    { 142, null, 1, "ITUBAINA ORIGINAL 600ML", null, true, 1, true, "ITUBAINA ORIGINAL 600ML", true, 4.49m, "itubainaoriginal600ml.jpg" },
                    { 143, null, 1, "ITUBAINA ORIGINAL LONG 355ML", null, true, 1, true, "ITUBAINA ORIGINAL LONG 355ML", true, 5.99m, "itubainaoriginallong355ml.jpg" },
                    { 144, null, 8, "FRITISCO BATATA 40gm", null, true, 1, true, "FRITISCO BATATA 40gm", true, 3.00m, "fritiscobatata40gm.jpg" },
                    { 145, null, 21, "TRIDENT", null, true, 1, true, "TRIDENT", true, 2.69m, "trident.jpg" },
                    { 146, null, 21, "PRESTIGIO", null, true, 1, true, "PRESTIGIO", true, 2.79m, "prestigio.jpg" },
                    { 147, null, 21, "FILTRO ALEDA 150 UNIDADES", null, true, 1, true, "FILTRO ALEDA 150 UNIDADES", true, 6.59m, "filtroaleda150unidades.jpg" },
                    { 148, null, 6, "TABACO ACREMA BLEND 20g", null, true, 1, true, "TABACO ACREMA BLEND 20g", true, 17.49m, "tabacoacremablend20g.jpg" },
                    { 149, null, 6, "CIGARRO WINSTON SABOR", null, true, 1, true, "CIGARRO WINSTON SABOR", true, 12.49m, "cigarrowinstonsabor.jpg" },
                    { 150, null, 21, "CIGARRO WINSTON SELECTED RED", null, true, 1, true, "CIGARRO WINSTON SELECTED RED", true, 11.49m, "cigarrowinstonselectedred.jpg" },
                    { 151, null, 18, "PORÇÃO DE BATATA 300gm", null, true, 1, true, "PORÇÃO DE BATATA 300gm", true, 15.00m, "porçãodebatata300gm.jpg" },
                    { 152, null, 18, "PORÇÃO DE BATATA 500gm", null, true, 1, true, "PORÇÃO DE BATATA 500gm", true, 25.00m, "porçãodebatata500gm.jpg" },
                    { 153, null, 21, "PORÇÃO DE TORRESMO 300gm", null, true, 1, true, "PORÇÃO DE TORRESMO 300gm", true, 25.00m, "porçãodetorresmo300gm.jpg" },
                    { 154, null, 21, "PORÇÃO DE TORRESMO 500gm", null, true, 1, true, "PORÇÃO DE TORRESMO 500gm", true, 35.00m, "porçãodetorresmo500gm.jpg" },
                    { 155, null, 21, "PORÇÃO DE CALABRESA 300gm", null, true, 1, true, "PORÇÃO DE CALABRESA 300gm", true, 20.00m, "porçãodecalabresa300gm.jpg" },
                    { 156, null, 21, "PORÇÃO DE CALABRESA 500gm", null, true, 1, true, "PORÇÃO DE CALABRESA 500gm", true, 35.00m, "porçãodecalabresa500gm.jpg" },
                    { 157, null, 21, "PORÇÃO DE TULIPA 300gm", null, true, 1, true, "PORÇÃO DE TULIPA 300gm", true, 25.00m, "porçãodetulipa300gm.jpg" },
                    { 158, null, 21, "PORÇÃO DE ISCA DE TILAPIA 300gm", null, true, 1, true, "PORÇÃO DE ISCA DE TILAPIA 300gm", true, 35.00m, "porçãodeiscadetilapia300gm.jpg" },
                    { 159, null, 21, "PORÇÃO DE ISCA DE TILAPIA 500gm", null, true, 1, true, "PORÇÃO DE ISCA DE TILAPIA 500gm", true, 50.00m, "porçãodeiscadetilapia500gm.jpg" },
                    { 160, null, 21, "PASTEL CARNE / QUEIJO / PIZZA", null, true, 1, true, "PASTEL CARNE / QUEIJO / PIZZA", true, 10.00m, "pastelcarne/queijo/pizza.jpg" },
                    { 161, null, 21, "PASTEL DE FRANGO COM CATUPIRY / CARNE COM QUEIJO", null, true, 1, true, "PASTEL DE FRANGO COM CATUPIRY / CARNE COM QUEIJO", true, 12.00m, "pasteldefrangocomcatupiry/carnecomqueijo.jpg" },
                    { 162, null, 10, "AGUA COM GAS MINALBA 1,5L", null, true, 1, true, "AGUA COM GAS MINALBA 1,5L", true, 5.99m, "aguacomgasminalba1,5l.jpg" },
                    { 163, null, 21, "SALGADO", null, true, 1, true, "SALGADO", true, 8.00m, "salgado.jpg" },
                    { 164, null, 21, "PORÇAO DE FRANGO A PASSARINHO 300g", null, true, 1, true, "PORÇAO DE FRANGO A PASSARINHO 300g", true, 15.00m, "porçaodefrangoapassarinho300g.jpg" },
                    { 165, null, 18, "BATATA 300g", null, true, 1, true, "BATATA 300g", true, 15.00m, "batata300g.jpg" },
                    { 166, null, 18, "BATATA 500g", null, true, 1, true, "BATATA 500g", true, 25.00m, "batata500g.jpg" },
                    { 167, null, 21, "TULIPA 500g", null, true, 1, true, "TULIPA 500g", true, 35.00m, "tulipa500g.jpg" },
                    { 168, null, 21, "CALABRESA ACEBOLADA 300g", null, true, 1, true, "CALABRESA ACEBOLADA 300g", true, 20.00m, "calabresaacebolada300g.jpg" },
                    { 169, null, 21, "CALABRESA ACEBOLADA 500g", null, true, 1, true, "CALABRESA ACEBOLADA 500g", true, 35.00m, "calabresaacebolada500g.jpg" },
                    { 170, null, 21, "DOSE DE WHISKY", null, true, 1, true, "DOSE DE WHISKY", true, 10.00m, "dosedewhisky.jpg" },
                    { 171, null, 1, "FANTA UVA 2L", null, true, 1, true, "FANTA UVA 2L", true, 7.99m, "fantauva2l.jpg" },
                    { 172, null, 21, "AMSTEL 600ml", null, true, 1, true, "AMSTEL 600ml", true, 6.99m, "amstel600ml.jpg" },
                    { 173, null, 6, "PALHEIRO SOLTO", null, true, 1, true, "PALHEIRO SOLTO", true, 1.50m, "palheirosolto.jpg" },
                    { 174, null, 10, "AGUA 1,5", null, true, 1, true, "AGUA 1,5", true, 4.99m, "agua1,5.jpg" },
                    { 175, null, 20, "PAO DE MEL", null, true, 1, true, "PAO DE MEL", true, 7.99m, "paodemel.jpg" },
                    { 176, null, 1, "SPRITE 2L", null, true, 1, true, "SPRITE 2L", true, 11.00m, "sprite2l.jpg" },
                    { 177, null, 5, "SMIRNOFF ICE", null, true, 1, true, "SMIRNOFF ICE", true, 10.00m, "smirnoffice.jpg" },
                    { 178, null, 15, "GT BEATS", null, true, 1, true, "GT BEATS", true, 8.00m, "gtbeats.jpg" },
                    { 179, null, 20, "FINI", null, true, 1, true, "FINI", true, 2.00m, "fini.jpg" },
                    { 180, null, 9, "FINI", null, true, 1, true, "FINI", true, 2.00m, "fini.jpg" },
                    { 181, null, 21, "ZOMO SEDA/SOLTA", null, true, 1, true, "ZOMO SEDA/SOLTA", true, 0.50m, "zomoseda/solta.jpg" },
                    { 182, null, 1, "PEPSI 2L", null, true, 1, true, "PEPSI 2L", true, 8.00m, "pepsi2l.jpg" },
                    { 183, null, 21, "TUBAINA RICARDO", null, true, 1, true, "TUBAINA RICARDO", true, 6.00m, "tubainaricardo.jpg" },
                    { 184, null, 1, "GUARANA ANTARTICA 2L", null, true, 1, true, "GUARANA ANTARTICA 2L", true, 10.99m, "guaranaantartica2l.jpg" },
                    { 185, null, 1, "FANTA LARANJA LATA 220ML", null, true, 1, true, "FANTA LARANJA LATA 220ML", true, 3.99m, "fantalaranjalata220ml.jpg" },
                    { 186, null, 21, "passport whisk", null, true, 1, true, "passport whisk", true, 59.99m, "passportwhisk.jpg" },
                    { 187, null, 19, "COMBO ENERGETICO BALY+ETERNITY", null, true, 1, true, "COMBO ENERGETICO BALY+ETERNITY", true, 40.00m, "comboenergeticobaly+eternity.jpg" },
                    { 188, null, 2, "COPAO DE CHOPP 700ML", null, true, 1, true, "COPAO DE CHOPP 700ML", true, 10.00m, "copaodechopp700ml.jpg" },
                    { 189, null, 21, "DEL VALLE 1,5", null, true, 1, true, "DEL VALLE 1,5", true, 5.00m, "delvalle1,5.jpg" },
                    { 190, null, 21, "OLEO DE ALGODAO SOYA 14,5KG", null, true, 1, true, "OLEO DE ALGODAO SOYA 14,5KG", true, 177.99m, "oleodealgodaosoya14,5kg.jpg" },
                    { 191, null, 21, "BATATA PALITO TRADICIONAL(9MM)EASY CHEF PCT 1,1KG", null, true, 1, true, "BATATA PALITO TRADICIONAL(9MM)EASY CHEF PCT 1,1KG", true, 12.50m, "batatapalitotradicional(9mm)easychefpct1,1kg.jpg" },
                    { 192, null, 21, "ORIGINAL LATA 350ML CX CARTAO C/12 NPAL", null, true, 1, true, "ORIGINAL LATA 350ML CX CARTAO C/12 NPAL", true, 4.50m, "originallata350mlcxcartaoc/12npal.jpg" },
                    { 193, null, 2, "ORIGINAL 300ML", null, true, 1, true, "ORIGINAL 300ML", true, 3.50m, "original300ml.jpg" },
                    { 194, null, 21, "PEPSI COLA PET 2L CAIXA C/6", null, true, 1, true, "PEPSI COLA PET 2L CAIXA C/6", true, 9.00m, "pepsicolapet2lcaixac/6.jpg" },
                    { 195, null, 21, "RED BULL BR LATA 250ML SIX PACK NPAL .", null, true, 1, true, "RED BULL BR LATA 250ML SIX PACK NPAL .", true, 10.00m, "redbullbrlata250mlsixpacknpal..jpg" },
                    { 196, null, 2, "SPATEN N LT SLEEK 350ML CX CART C 12", null, true, 1, true, "SPATEN N LT SLEEK 350ML CX CART C 12", true, 4.50m, "spatennltsleek350mlcxcartc12.jpg" },
                    { 197, null, 2, "BRAHMA CHOPP LATA 350ML SH C/12 NPAL", null, true, 1, true, "BRAHMA CHOPP LATA 350ML SH C/12 NPAL", true, 4.00m, "brahmachopplata350mlshc/12npal.jpg" },
                    { 198, null, 21, "RED BULL MELANCIA LATA 250ML FOUR PACK NPAL", null, true, 1, true, "RED BULL MELANCIA LATA 250ML FOUR PACK NPAL", true, 10.00m, "redbullmelancialata250mlfourpacknpal.jpg" },
                    { 199, null, 2, "BRAHMA CHOPP GFA VD 1L COM TTC", null, true, 1, true, "BRAHMA CHOPP GFA VD 1L COM TTC", true, 12.00m, "brahmachoppgfavd1lcomttc.jpg" },
                    { 200, null, 21, "ANTARCTICA PILSEN 300ML", null, true, 1, true, "ANTARCTICA PILSEN 300ML", true, 3.00m, "antarcticapilsen300ml.jpg" },
                    { 201, null, 21, "ORIGINAL 600ML", null, true, 1, true, "ORIGINAL 600ML", true, 10.00m, "original600ml.jpg" },
                    { 202, null, 2, "BUDWEISER 300ML", null, true, 1, true, "BUDWEISER 300ML", true, 3.50m, "budweiser300ml.jpg" },
                    { 203, null, 21, "PUREZA VITAL AGUA MIN. C GAS GFA PET 510ML FD C/12", null, true, 1, true, "PUREZA VITAL AGUA MIN. C GAS GFA PET 510ML FD C/12", true, 4.50m, "purezavitalaguamin.cgasgfapet510mlfdc/12.jpg" },
                    { 204, null, 1, "GUARANA CHP ANTARCTICA PET 200ML SH C/12", null, true, 1, true, "GUARANA CHP ANTARCTICA PET 200ML SH C/12", true, 2.00m, "guaranachpantarcticapet200mlshc/12.jpg" },
                    { 205, null, 21, "MINALBA AGUA MINERAL C/GAS GFA PET 1,5L FD C/6", null, true, 1, true, "MINALBA AGUA MINERAL C/GAS GFA PET 1,5L FD C/6", true, 5.00m, "minalbaaguamineralc/gasgfapet1,5lfdc/6.jpg" },
                    { 206, null, 21, "MINALBA AGUA MINERAL S/GAS GFA PET 1,5L FD C/6", null, true, 1, true, "MINALBA AGUA MINERAL S/GAS GFA PET 1,5L FD C/6", true, 4.00m, "minalbaaguaminerals/gasgfapet1,5lfdc/6.jpg" },
                    { 207, null, 21, "PUREZA VITAL AGUA MIN. S GAS GFA PET 1,5L FD C/6", null, true, 1, true, "PUREZA VITAL AGUA MIN. S GAS GFA PET 1,5L FD C/6", true, 4.00m, "purezavitalaguamin.sgasgfapet1,5lfdc/6.jpg" },
                    { 208, null, 2, "ORIGINAL LT 269ML CX CARTAO C/8 NPAL", null, true, 1, true, "ORIGINAL LT 269ML CX CARTAO C/8 NPAL", true, 3.50m, "originallt269mlcxcartaoc/8npal.jpg" },
                    { 209, null, 2, "SKOL LATA 350ML SH C/18 NPAL MULTPACK", null, true, 1, true, "SKOL LATA 350ML SH C/18 NPAL MULTPACK", true, 4.00m, "skollata350mlshc/18npalmultpack.jpg" },
                    { 210, null, 2, "BRAHMA CHOPP ZERO LATA 350ML SH C/12 NPAL", null, true, 1, true, "BRAHMA CHOPP ZERO LATA 350ML SH C/12 NPAL", true, 4.00m, "brahmachoppzerolata350mlshc/12npal.jpg" },
                    { 211, null, 21, "FANTA LARANJA PET RET 2 LITROS 09UN", null, true, 1, true, "FANTA LARANJA PET RET 2 LITROS 09UN", true, 8.00m, "fantalaranjapetret2litros09un.jpg" },
                    { 212, null, 21, "CC PET RET 2L 9 JD", null, true, 1, true, "CC PET RET 2L 9 JD", true, 8.00m, "ccpetret2l9jd.jpg" },
                    { 213, null, 21, "Cerv Heineken Pil 0,60Gfa Rt 24Un", null, true, 1, true, "Cerv Heineken Pil 0,60Gfa Rt 24Un", true, 13.00m, "cervheinekenpil0,60gfart24un.jpg" },
                    { 214, null, 21, "Cerv Heineken 0,0% 0,350ltsleekdes12unpb", null, true, 1, true, "Cerv Heineken 0,0% 0,350ltsleekdes12unpb", true, 7.00m, "cervheineken0,0%0,350ltsleekdes12unpb.jpg" },
                    { 215, null, 21, "RED BULL TROPICAL BR LATA 250ML FOUR PACK NPAL .", null, true, 1, true, "RED BULL TROPICAL BR LATA 250ML FOUR PACK NPAL .", true, 10.00m, "redbulltropicalbrlata250mlfourpacknpal..jpg" },
                    { 216, null, 21, "ORIGINAL LATA 350ML CX CARTAO C/12 NPAL", null, true, 1, true, "ORIGINAL LATA 350ML CX CARTAO C/12 NPAL", true, 4.00m, "originallata350mlcxcartaoc/12npal.jpg" },
                    { 217, null, 21, "FTA LAR LT 220ml 6U FI MAINLINE", null, true, 1, true, "FTA LAR LT 220ml 6U FI MAINLINE", true, 3.00m, "ftalarlt220ml6ufimainline.jpg" },
                    { 218, null, 21, "Cerv Amstel Lager 0,60l Gfa Rt 24un", null, true, 1, true, "Cerv Amstel Lager 0,60l Gfa Rt 24un", true, 10.00m, "cervamstellager0,60lgfart24un.jpg" },
                    { 219, null, 21, "Cerv Heineken Pil 0,60Gfa Rt 24Un", null, true, 1, true, "Cerv Heineken Pil 0,60Gfa Rt 24Un", true, 13.00m, "cervheinekenpil0,60gfart24un.jpg" },
                    { 220, null, 2, "ORIGINAL LATA FINA", null, true, 1, true, "ORIGINAL LATA FINA", true, 3.99m, "originallatafina.jpg" },
                    { 221, null, 6, "CHESTERFIELD", null, true, 1, true, "CHESTERFIELD", true, 10.50m, "chesterfield.jpg" },
                    { 222, null, 6, "DUNHILL", null, true, 1, true, "DUNHILL", true, 15.50m, "dunhill.jpg" },
                    { 223, null, 6, "CHESTERFIELD REMIX BEATS", null, true, 1, true, "CHESTERFIELD REMIX BEATS", true, 12.00m, "chesterfieldremixbeats.jpg" },
                    { 224, null, 4, "RED BULL MELANCIA", null, true, 1, true, "RED BULL MELANCIA", true, 10.00m, "redbullmelancia.jpg" },
                    { 225, null, 4, "RED BULL TROPICAL 250 ML", null, true, 1, true, "RED BULL TROPICAL 250 ML", true, 10.00m, "redbulltropical250ml.jpg" },
                    { 226, null, 1, "SPRITE LEMON FRESH", null, true, 1, true, "SPRITE LEMON FRESH", true, 6.00m, "spritelemonfresh.jpg" },
                    { 227, null, 21, "POWER ADE 500ML", null, true, 1, true, "POWER ADE 500ML", true, 5.00m, "powerade500ml.jpg" },
                    { 228, null, 14, "NUSAQUINHO DIVERSOS", null, true, 1, true, "NUSAQUINHO DIVERSOS", true, 17.99m, "nusaquinhodiversos.jpg" },
                    { 229, null, 12, "VINHO CANTINA AGRICOLA 1L", null, true, 1, true, "VINHO CANTINA AGRICOLA 1L", true, 25.00m, "vinhocantinaagricola1l.jpg" },
                    { 230, null, 3, "TAMPICO LATA 270ML FRUTAS CITRICAS", null, true, 1, true, "TAMPICO LATA 270ML FRUTAS CITRICAS", true, 3.99m, "tampicolata270mlfrutascitricas.jpg" },
                    { 231, null, 20, "AMENDOIM 24G", null, true, 1, true, "AMENDOIM 24G", true, 2.00m, "amendoim24g.jpg" },
                    { 232, null, 20, "BATOM 16G", null, true, 1, true, "BATOM 16G", true, 2.00m, "batom16g.jpg" },
                    { 233, null, 2, "COPO DE CHOP 500ML", null, true, 1, true, "COPO DE CHOP 500ML", true, 7.00m, "copodechop500ml.jpg" },
                    { 234, null, 20, "BALAS", null, true, 1, true, "BALAS", true, 0.15m, "balas.jpg" },
                    { 235, null, 20, "PIRULITO BIG BIG", null, true, 1, true, "PIRULITO BIG BIG", true, 1.00m, "pirulitobigbig.jpg" },
                    { 236, null, 20, "CHOCOLATE LACTEA DIAM.NEG 80G", null, true, 1, true, "CHOCOLATE LACTEA DIAM.NEG 80G", true, 9.00m, "chocolatelacteadiam.neg80g.jpg" },
                    { 237, null, 20, "CHOCOLATE LACTEA BRANCO 80G", null, true, 1, true, "CHOCOLATE LACTEA BRANCO 80G", true, 9.00m, "chocolatelacteabranco80g.jpg" },
                    { 238, null, 20, "PIRULITO ENERGE.28G", null, true, 1, true, "PIRULITO ENERGE.28G", true, 1.00m, "pirulitoenerge.28g.jpg" },
                    { 239, null, 1, "COCA COLA ZERO PET 2L", null, true, 1, true, "COCA COLA ZERO PET 2L", true, 13.99m, "cocazolazeropet2l.jpg" },
                    { 240, null, 21, "copao Jack Daniels Apple", null, true, 1, true, "copao Jack Daniels Apple", true, 25.00m, "copaojackdanielsapple.jpg" },
                    { 241, null, 21, "RED LABEL GARRAFA", null, true, 1, true, "RED LABEL GARRAFA", true, 119.90m, "redlabelgarrafa.jpg" },
                    { 242, null, 4, "MONSTER 269ML ORIGINAL", null, true, 1, true, "MONSTER 269ML ORIGINAL", true, 8.50m, "monster269mloriginal.jpg" },
                    { 243, null, 4, "MONSTER 269 ML MANGO LOCO", null, true, 1, true, "MONSTER 269 ML MANGO LOCO", true, 8.50m, "monster269mlmangoloco.jpg" },
                    { 244, null, 21, "FANTA UVA 220ML LATA", null, true, 1, true, "FANTA UVA 220ML LATA", true, 3.00m, "fantauva220mllata.jpg" },
                    { 245, null, 3, "DEL VALLE 1L", null, true, 1, true, "DEL VALLE 1L", true, 5.00m, "delvalle1l.jpg" },
                    { 246, null, 3, "DEL VALLE 290ML MARACUJA", null, true, 1, true, "DEL VALLE 290ML MARACUJA", true, 4.99m, "delvalle290mlmaracuja.jpg" },
                    { 247, null, 3, "DEL VALLE 290ML UVA", null, true, 1, true, "DEL VALLE 290ML UVA", true, 4.99m, "delvalle290mluva.jpg" },
                    { 248, null, 4, "MONSTER 473ML ULTRA PARADISE", null, true, 1, true, "MONSTER 473ML ULTRA PARADISE", true, 11.50m, "monster473mlultraparadise.jpg" },
                    { 249, null, 2, "Cópia de EISENBAHN 350ml", null, true, 1, true, "Cópia de EISENBAHN 350ml", true, 3.99m, "cópiadeeisenbahn350ml.jpg" },
                    { 250, null, 3, "DEL VALLE UVA1L", null, true, 1, true, "DEL VALLE UVA1L", true, 10.99m, "delvalleuva1l.jpg" },
                    { 251, null, 21, "ESPETO DE CORAÇÃO DE GALINHA", null, true, 1, true, "ESPETO DE CORAÇÃO DE GALINHA", true, 49.99m, "espetodecoraçãodegalinha.jpg" },
                    { 252, null, 21, "GATOREIDE SABOR BERRY BLUE 500 ml", null, true, 1, true, "GATOREIDE SABOR BERRY BLUE 500 ml", true, 7.99m, "gatoreidesaborberryblue500ml.jpg" },
                    { 253, null, 3, "DEL VALLE GOIABA1L", null, true, 1, true, "DEL VALLE GOIABA1L", true, 10.99m, "delvallegoiaba1l.jpg" },
                    { 254, null, 21, "ESPETO DE KAFTA", null, true, 1, true, "ESPETO DE KAFTA", true, 49.99m, "espetodekafta.jpg" },
                    { 255, null, 3, "DEL VALLE PESSEGO1L", null, true, 1, true, "DEL VALLE PESSEGO1L", true, 10.99m, "delvallepessego1l.jpg" },
                    { 256, null, 21, "ESPETO DE FRANGO", null, true, 1, true, "ESPETO DE FRANGO", true, 39.99m, "espetodefrango.jpg" },
                    { 257, null, 3, "DEL VALLE LARANJA1L", null, true, 1, true, "DEL VALLE LARANJA1L", true, 10.99m, "delvallelaranja1l.jpg" },
                    { 258, null, 21, "ESPETO DE LINGUIÇA", null, true, 1, true, "ESPETO DE LINGUIÇA", true, 39.99m, "espetodelinguiça.jpg" },
                    { 259, null, 21, "JUJU GOURMAT", null, true, 1, true, "JUJU GOURMAT", true, 4.50m, "jujugourmet.jpg" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Caixas_UsuarioId",
                table: "Caixas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_CascosEmprestados_ClienteId",
                table: "CascosEmprestados",
                column: "ClienteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CascosEmprestados_UsuarioId",
                table: "CascosEmprestados",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_ClienteId",
                table: "Comandas",
                column: "ClienteId",
                unique: true,
                filter: "[ClienteId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Comandas_UsuarioId",
                table: "Comandas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalhesCascosEmprestados_ProdutoId",
                table: "DetalhesCascosEmprestados",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalhesComanda_ProdutoId",
                table: "DetalhesComanda",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalhesPedido_PedidoId",
                table: "DetalhesPedido",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalhesPedido_ProdutoId",
                table: "DetalhesPedido",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Estoque_ProdutoId",
                table: "Estoque",
                column: "ProdutoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventarios_ProdutoId",
                table: "Inventarios",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensCarrinhoCompra_ProdutoId",
                table: "ItensCarrinhoCompra",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensComanda_ComandaId",
                table: "ItensComanda",
                column: "ComandaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensComanda_ProdutoId",
                table: "ItensComanda",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_EstoqueId",
                table: "MovimentacoesEstoque",
                column: "EstoqueId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimentacoesEstoque_ProdutoId",
                table: "MovimentacoesEstoque",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaEntradaItem_NotaEntradaId",
                table: "NotaEntradaItem",
                column: "NotaEntradaId");

            migrationBuilder.CreateIndex(
                name: "IX_NotaEntradaItem_ProdutoId",
                table: "NotaEntradaItem",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClientesId",
                table: "Pedidos",
                column: "ClientesId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_UsuarioId",
                table: "Pedidos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Caixas");

            migrationBuilder.DropTable(
                name: "CascosEmprestados");

            migrationBuilder.DropTable(
                name: "Despesas");

            migrationBuilder.DropTable(
                name: "DetalhesCascosEmprestados");

            migrationBuilder.DropTable(
                name: "DetalhesComanda");

            migrationBuilder.DropTable(
                name: "DetalhesPedido");

            migrationBuilder.DropTable(
                name: "Inventarios");

            migrationBuilder.DropTable(
                name: "ItensCarrinhoCompra");

            migrationBuilder.DropTable(
                name: "ItensComanda");

            migrationBuilder.DropTable(
                name: "MovimentacoesEstoque");

            migrationBuilder.DropTable(
                name: "NotaEntradaItem");

            migrationBuilder.DropTable(
                name: "Pedidos");

            migrationBuilder.DropTable(
                name: "Comandas");

            migrationBuilder.DropTable(
                name: "Estoque");

            migrationBuilder.DropTable(
                name: "NotasEntrada");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
