using ApiECommerce.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiECommerce.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<ItemCarrinhoCompra> ItensCarrinhoCompra { get; set; }
    public DbSet<ItemComanda> ItensComanda { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<DetalhePedido> DetalhesPedido { get; set; }
    public DbSet<Comanda> Comandas { get; set; }
    public DbSet<DetalhesComanda> DetalhesComanda { get; set; }
    public DbSet<Clientes> Clientes { get; set; }
    public DbSet<CascosEmprestados> CascosEmprestados { get; set; }
    public DbSet<DetalheEmpCascp> DetalhesCascosEmprestados { get; set; }
    public DbSet<Estoque> Estoque { get; set; }
    public DbSet<Inventario> Inventarios { get; set; }
    public DbSet<Caixa> Caixas { get; set; }
    public DbSet<MovimentacaoEstoque> MovimentacoesEstoque { get; set; }
    public DbSet<NotaEntrada> NotasEntrada { get; set; }
    public DbSet<Despesas> Despesas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemComanda>()
        .HasOne(i => i.Comanda)
        .WithMany(c => c.Itens)
        .HasForeignKey(i => i.ComandaId)
        .OnDelete(DeleteBehavior.Cascade); // ou Restrict, se preferir

        modelBuilder.Entity<Comanda>()
                .HasMany(c => c.Itens)
                .WithOne(i => i.Comanda)
                .HasForeignKey(i => i.ComandaId);

        modelBuilder.Entity<Estoque>()
            .HasOne(e => e.Produto)
            .WithOne(p => p.Estoque)
            .HasForeignKey<Estoque>(e => e.ProdutoId);

        modelBuilder.Entity<Inventario>()
            .HasOne(i => i.Produto)
            .WithMany(p => p.Inventarios) 
            .HasForeignKey(i => i.ProdutoId);

        modelBuilder.Entity<MovimentacaoEstoque>()
            .HasOne(m => m.Produto)
            .WithMany(p => p.Movimentacoes)
            .HasForeignKey(m => m.ProdutoId)
            .OnDelete(DeleteBehavior.Cascade); //  exclu em cascata;

        // Pedido - DetalhePedido (itens do pedido)
        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Itens)
            .WithOne(d => d.Pedido)
            .HasForeignKey(d => d.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);


        // Pedido - Usuario (vendedor)
        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Usuario)
            .WithMany()
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        // DetalhePedido - Produto
        modelBuilder.Entity<DetalhePedido>()
            .HasOne(d => d.Produto)
            .WithMany()
            .HasForeignKey(d => d.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Caixa>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.ValorAbertura).HasColumnType("decimal(18,2)");
            entity.Property(c => c.ValorFechamento).HasColumnType("decimal(18,2)");
            entity.Property(c => c.Observacao).HasMaxLength(500);

            entity.HasOne(c => c.Usuario)
                  .WithMany()
                  .HasForeignKey(c => c.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Despesas>()
    .Property(d => d.Id)
    .ValueGeneratedOnAdd();


        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nome = "Refrigerante", UrlImagem = "refrigerantes1.png" },
            new Categoria { Id = 2, Nome = "Cervejas", UrlImagem = "cervejas.png" },
            new Categoria { Id = 3, Nome = "Sucos", UrlImagem = "sucos.png" },
            new Categoria { Id = 4, Nome = "Energético", UrlImagem = "energeticos.png" },
            new Categoria { Id = 5, Nome = "Ices", UrlImagem = "ices.png" },
            new Categoria { Id = 6, Nome = "Cigarros", UrlImagem = "cigarros.png" },
            new Categoria { Id = 7, Nome = "Sedas", UrlImagem = "sedas.jpg" },
            new Categoria { Id = 8, Nome = "Salgadinhos", UrlImagem = "salgadinhos.jpg" },
            new Categoria { Id = 9, Nome = "Doses", UrlImagem = "doses.jpg" },
            new Categoria { Id = 10, Nome = "Água", UrlImagem = "aguas.jpg" },
            new Categoria { Id = 11, Nome = "Carvões", UrlImagem = "carvoes.jpg" },
            new Categoria { Id = 12, Nome = "Vinhos", UrlImagem = "vinhos.jpg" },
            new Categoria { Id = 13, Nome = "Gelos", UrlImagem = "gelos.jpg" },
            new Categoria { Id = 14, Nome = "Drinks", UrlImagem = "drinks.jpg" },
            new Categoria { Id = 15, Nome = "Quentes", UrlImagem = "quentes.jpg" },
            new Categoria { Id = 16, Nome = "Gins", UrlImagem = "gins.jpg" },
            new Categoria { Id = 17, Nome = "Comidas", UrlImagem = "comidas.png" },
            new Categoria { Id = 18, Nome = "Porções", UrlImagem = "porcoes.jpg" },
            new Categoria { Id = 19, Nome = "Combos", UrlImagem = "combos.jpg" },
            new Categoria { Id = 20, Nome = "Doces", UrlImagem = "doces.png" },
            new Categoria { Id = 21, Nome = "Outros", UrlImagem = "outros.png" }
            );

        // Ensure new price properties have defaults
        modelBuilder.Entity<Produto>().Property(p => p.PrecoCusto).HasDefaultValue(0m).HasColumnType("decimal(10,2)");
        modelBuilder.Entity<Produto>().Property(p => p.PrecoQuente).HasDefaultValue(0m).HasColumnType("decimal(10,2)");
        modelBuilder.Entity<Produto>().Property(p => p.PrecoGelada).HasDefaultValue(0m).HasColumnType("decimal(10,2)");
        modelBuilder.Entity<Produto>().Property(p => p.PrecoEntrega).HasDefaultValue(0m).HasColumnType("decimal(10,2)");
        modelBuilder.Entity<Produto>().Property(p => p.PrecoRetirar).HasDefaultValue(0m).HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Produto>().HasData(
new Produto { Id = 2, Nome = "CERVEJA ORIGINAL LATA 350ML", UrlImagem = "cervejaoriginallata350ml.jpg", CategoriaId = 2, PrecoRetirar = 4.99M, PrecoCusto = 0M, PrecoQuente = 4.99M, PrecoGelada = 4.99M, PrecoEntrega = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA ORIGINAL LATA 350ML" },
new Produto { Id = 3, Nome = "BARRIGUDINHA ORGINAL 300ml", UrlImagem = "barrigudinhaorginal300ml.jpg", CategoriaId = 2, PrecoRetirar = 3.80M, PrecoCusto = 0M, PrecoQuente = 3.80M, PrecoGelada = 3.80M, PrecoEntrega = 3.80M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BARRIGUDINHA ORGINAL 300ml" },
new Produto { Id = 4, Nome = "BARRIGUDINHA SKOL 300 ml", UrlImagem = "barrigudinhaskol300ml.jpg", CategoriaId = 2, PrecoRetirar = 2.99M, PrecoCusto = 0M, PrecoQuente = 2.99M, PrecoGelada = 2.99M, PrecoEntrega = 2.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BARRIGUDINHA SKOL 300 ml" },
new Produto { Id = 5, Nome = "BARRIGUDINHA BRAHMA 300ml", UrlImagem = "barrigudinhabrahma300ml.jpg", CategoriaId = 2, PrecoRetirar = 2.99M, PrecoCusto = 0M, PrecoQuente = 2.99M, PrecoGelada = 2.99M, PrecoEntrega = 2.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BARRIGUDINHA BRAHMA 300ml" },
new Produto { Id = 6, Nome = "CRISTAL LATA 350ml", UrlImagem = "cristallata350ml.jpg", CategoriaId = 2, PrecoRetirar = 2.99M, PrecoCusto = 0M, PrecoQuente = 2.99M, PrecoGelada = 2.99M, PrecoEntrega = 2.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CRISTAL LATA 350ml" },
new Produto { Id = 7, Nome = "XERETA SABORES 2l", UrlImagem = "xeretasabores2l.jpg", CategoriaId = 1, PrecoRetirar = 5.99M, PrecoCusto = 0M, PrecoQuente = 5.99M, PrecoGelada = 5.99M, PrecoEntrega = 5.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "XERETA SABORES 2l" },
new Produto { Id = 1, Nome = "COCA COLA 2l RET", UrlImagem = "cocacola2lret.jpg", CategoriaId = 1, PrecoRetirar = 8.49M, PrecoCusto = 0M, PrecoQuente = 8.49M, PrecoGelada = 8.49M, PrecoEntrega = 8.49M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COCA COLA 2l RET" },
new Produto { Id = 8, Nome = "COCA COLA 2L PET", UrlImagem = "cocacola2lpet.jpg", CategoriaId = 1, PrecoRetirar = 13.99M, PrecoCusto = 0M, PrecoQuente = 13.99M, PrecoGelada = 13.99M, PrecoEntrega = 13.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COCA COLA 2L PET" },
new Produto { Id = 9, Nome = "COCA COLA 350ML LATA", UrlImagem = "cocacola350mllata.jpg", CategoriaId = 1, PrecoRetirar = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COCA COLA 350ML LATA" },
new Produto { Id = 10, Nome = "COCA COLA 200ML GARR", UrlImagem = "cocacola200mlgarr.jpg", CategoriaId = 1, PrecoRetirar = 2.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COCA COLA 200ML GARR" },
new Produto { Id = 11, Nome = "COCA COLA KS", UrlImagem = "cocacolaks.jpg", CategoriaId = 1, PrecoRetirar = 4.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COCA COLA KS" },
new Produto { Id = 12, Nome = "TAMPICO 450ML", UrlImagem = "tampico450ml.jpg", CategoriaId = 3, PrecoRetirar = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TAMPICO 450ML" },
new Produto { Id = 13, Nome = "TAMPICO 2L", UrlImagem = "tampico2l.jpg", CategoriaId = 3, PrecoRetirar = 11.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TAMPICO 2L" },
new Produto { Id = 14, Nome = "TAMPICO 270ML LATA UVA", UrlImagem = "tampico270mllatauva.jpg", CategoriaId = 3, PrecoRetirar = 3.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TAMPICO 270ML LATA UVA" },
new Produto { Id = 15, Nome = "TAMPICO 270ML LATA LARANJA", UrlImagem = "tampico270mllatalaranja.jpg", CategoriaId = 3, PrecoRetirar = 3.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TAMPICO 270ML LATA LARANJA" },
new Produto { Id = 16, Nome = "DEL VALLE 290ML PESSÊGO", UrlImagem = "delvalle290mlpessêgo.jpg", CategoriaId = 3, PrecoRetirar = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DEL VALLE 290ML PESSÊGO" },
new Produto { Id = 17, Nome = "DEL VALLE 290ML MANGA", UrlImagem = "delvalle290mlmanga.jpg", CategoriaId = 3, PrecoRetirar = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DEL VALLE 290ML MANGA" },
new Produto { Id = 18, Nome = "DEL VALLE 290ML GOIABA", UrlImagem = "delvalle290mlgoiaba.jpg", CategoriaId = 3, PrecoRetirar = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DEL VALLE 290ML GOIABA" },
new Produto { Id = 19, Nome = "MONSTER 473ML GUARANÁ LATA", UrlImagem = "monster473mlguaranálata.jpg", CategoriaId = 4, PrecoRetirar = 11.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "MONSTER 473ML GUARANÁ LATA" },
new Produto { Id = 20, Nome = "MONSTER 437ML MANGO LATA", UrlImagem = "monster437mlmangolata.jpg", CategoriaId = 4, PrecoRetirar = 11.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "MONSTER 437ML MANGO LATA" },
new Produto { Id = 21, Nome = "TNT 437ML LATA", UrlImagem = "tnt437mllata.jpg", CategoriaId = 4, PrecoRetirar = 10.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TNT 437ML LATA" },
new Produto { Id = 22, Nome = "CERVEJA IMPERIO LATA 350ml", UrlImagem = "cervejaimperiolata350ml.jpg", CategoriaId = 2, PrecoRetirar = 3.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA IMPERIO LATA 350ml" },
new Produto { Id = 23, Nome = "HEINEKEN LONGE NECK 330ml", UrlImagem = "heinekenlongeneck330ml.jpg", CategoriaId = 2, PrecoRetirar = 7.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "HEINEKEN LONGE NECK 330ml" },
new Produto { Id = 24, Nome = "ORIGINAL GARRAFA 600 ml", UrlImagem = "originalgarrafa600ml.jpg", CategoriaId = 2, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ORIGINAL GARRAFA 600 ml" },
new Produto { Id = 25, Nome = "CERVEJA HEINEKEM 600 ml", UrlImagem = "cervejaheinekem600ml.jpg", CategoriaId = 2, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA HEINEKEM 600 ml" },
new Produto { Id = 26, Nome = "ICE LEEV 275ml", UrlImagem = "iceleev275ml.jpg", CategoriaId = 5, PrecoRetirar = 6.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ICE LEEV 275ml" },
new Produto { Id = 27, Nome = "CABARE ICE 275ml", UrlImagem = "cabareice275ml.jpg", CategoriaId = 5, PrecoRetirar = 7.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CABARE ICE 275ml" },
new Produto { Id = 28, Nome = "CERVEJA AMSTEL 355 ML", UrlImagem = "cervejaamstel355ml.jpg", CategoriaId = 2, PrecoRetirar = 3.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA AMSTEL 355 ML" },
new Produto { Id = 29, Nome = "CERVEJA ITAIPAVA MALZERBIER 330ml", UrlImagem = "cervejaitaipavamalzerbier330ml.jpg", CategoriaId = 2, PrecoRetirar = 6.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA ITAIPAVA MALZERBIER 330ml" },
new Produto { Id = 30, Nome = "ICE ASKOV 275 ml", UrlImagem = "iceaskov275ml.jpg", CategoriaId = 5, PrecoRetirar = 6.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ICE ASKOV 275 ml" },
new Produto { Id = 31, Nome = "ENERGICO BALY 2l DIVERSOS", UrlImagem = "energicobaly2ldiversos.jpg", CategoriaId = 4, PrecoRetirar = 10.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ENERGICO BALY 2l DIVERSOS" },
new Produto { Id = 32, Nome = "CERVEJA ITAIPAVA ZERO 330 ml", UrlImagem = "cervejaitaipavazero330ml.jpg", CategoriaId = 2, PrecoRetirar = 4.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA ITAIPAVA ZERO 330 ml" },
new Produto { Id = 33, Nome = "CIGARRO SAMARINO", UrlImagem = "cigarrosamarino.jpg", CategoriaId = 6, PrecoRetirar = 6.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CIGARRO SAMARINO" },
new Produto { Id = 34, Nome = "CIGARRO EGYPT", UrlImagem = "cigarroegypt.jpg", CategoriaId = 6, PrecoRetirar = 6.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CIGARRO EGYPT" },
new Produto { Id = 35, Nome = "CIGARRO CHESTERFILD BRANCO", UrlImagem = "cigarrochesterfildbranco.jpg", CategoriaId = 6, PrecoRetirar = 10.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CIGARRO CHESTERFILD BRANCO" },
new Produto { Id = 36, Nome = "CIGARRO MALBORO", UrlImagem = "cigarromalboro.jpg", CategoriaId = 6, PrecoRetirar = 14.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CIGARRO MALBORO" },
new Produto { Id = 37, Nome = "PALHEIRO BLUE ICE", UrlImagem = "palheiroblueice.jpg", CategoriaId = 6, PrecoRetirar = 8.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PALHEIRO BLUE ICE" },
new Produto { Id = 38, Nome = "PALHEIRO TOMBADA MENTa", UrlImagem = "palheirotombadamenta.jpg", CategoriaId = 6, PrecoRetirar = 8.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PALHEIRO TOMBADA MENTa" },
new Produto { Id = 39, Nome = "PALHEIRO TRADICIONAL", UrlImagem = "palheirotradicional.jpg", CategoriaId = 6, PrecoRetirar = 8.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PALHEIRO TRADICIONAL" },
new Produto { Id = 40, Nome = "SEDA ZOMO", UrlImagem = "sedazomo.jpg", CategoriaId = 7, PrecoRetirar = 4.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "SEDA ZOMO" },
new Produto { Id = 41, Nome = "DON TABACCO BREZE", UrlImagem = "dontabaccobreze.jpg", CategoriaId = 6, PrecoRetirar = 15.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DON TABACCO BREZE" },
new Produto { Id = 42, Nome = "TRITURADOR.", UrlImagem = "triturador..jpg", CategoriaId = 6, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TRITURADOR." },
new Produto { Id = 43, Nome = "AGUA TONICA 350ml", UrlImagem = "aguatonica350ml.jpg", CategoriaId = 10, PrecoRetirar = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "AGUA TONICA 350ml" },
new Produto { Id = 44, Nome = "CERVEJA AMISTEL 350 LATA", UrlImagem = "cervejaamistel350lata.jpg", CategoriaId = 2, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA AMISTEL 350 LATA" },
new Produto { Id = 45, Nome = "ATOMIC ENERG 270ML LATA", UrlImagem = "atomicenerg270mllata.jpg", CategoriaId = 4, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ATOMIC ENERG 270ML LATA" },
new Produto { Id = 46, Nome = "SCHWEPPERS MIXED GIN TONICA PINK 269ML", UrlImagem = "schweppersmixedgintonicapink269ml.jpg", CategoriaId = 16, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "SCHWEPPERS MIXED GIN TONICA PINK 269ML" },
new Produto { Id = 47, Nome = "CERVEJA AMSTEL 269ML LATA", UrlImagem = "cervejaamstel269mllata.jpg", CategoriaId = 2, PrecoRetirar = 2.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA AMSTEL 269ML LATA" },
new Produto { Id = 48, Nome = "COCA COLA KS ZERO", UrlImagem = "cocacolakszero.jpg", CategoriaId = 1, PrecoRetirar = 4.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COCA COLA KS ZERO" },
new Produto { Id = 49, Nome = "GATORADE SABOR UVA 500ml", UrlImagem = "gatoradesaboruva500ml.jpg", CategoriaId = 21, PrecoRetirar = 7.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GATORADE SABOR UVA 500ml" },
new Produto { Id = 50, Nome = "ISQUEIRO TRANPARESTE", UrlImagem = "isqueirotranpareste.jpg", CategoriaId = 6, PrecoRetirar = 3.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ISQUEIRO TRANPARESTE" },
new Produto { Id = 51, Nome = "ENERGICO FLYING HORSE", UrlImagem = "energicoflyinghorse.jpg", CategoriaId = 4, PrecoRetirar = 7.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ENERGICO FLYING HORSE" },
new Produto { Id = 52, Nome = "FANTA LARANJA 350ML LATA", UrlImagem = "fantalaranja350mllata.jpg", CategoriaId = 1, PrecoRetirar = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "FANTA LARANJA 350ML LATA" },
new Produto { Id = 53, Nome = "GUARANA ANTARCTICA LATA 350 ml", UrlImagem = "guaranaantarcticalata350ml.jpg", CategoriaId = 1, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GUARANA ANTARCTICA LATA 350 ml" },
new Produto { Id = 54, Nome = "ITUBAINA ORGIANAL LATA 350ml", UrlImagem = "itubainaorgianallata350ml.jpg", CategoriaId = 1, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ITUBAINA ORGIANAL LATA 350ml" },
new Produto { Id = 55, Nome = "SCHWEPPES 350ML LATA", UrlImagem = "schweppes350mllata.jpg", CategoriaId = 1, PrecoRetirar = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "SCHWEPPES 350ML LATA" },
new Produto { Id = 56, Nome = "HEINEKEN LONGE NECK 250 ml", UrlImagem = "heinekenlongeneck250ml.jpg", CategoriaId = 2, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "HEINEKEN LONGE NECK 250 ml" },
new Produto { Id = 57, Nome = "HENEKEN ZERO LONGE NECK 330 ml", UrlImagem = "henekenzerolongeneck330ml.jpg", CategoriaId = 2, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "HENEKEN ZERO LONGE NECK 330 ml" },
new Produto { Id = 58, Nome = "TORCIDA SABORES", UrlImagem = "torcidasabores.jpg", CategoriaId = 8, PrecoRetirar = 3.49M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TORCIDA SABORES" },
new Produto { Id = 59, Nome = "FABITOS SABORESN 80gm", UrlImagem = "fabitossaboresn80gm.jpg", CategoriaId = 8, PrecoRetirar = 2.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "FABITOS SABORESN 80gm" },
new Produto { Id = 60, Nome = "BISCOITIO DE POLVILHO 100gm", UrlImagem = "biscoitiodepolvilho100gm.jpg", CategoriaId = 8, PrecoRetirar = 5.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BISCOITIO DE POLVILHO 100gm" },
new Produto { Id = 61, Nome = "GUANA ANTARTICA 350ml LATA", UrlImagem = "guanaantartica350mllata.jpg", CategoriaId = 1, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GUANA ANTARTICA 350ml LATA" },
new Produto { Id = 62, Nome = "GUANA ANTARTICA 200ml GARRAFA", UrlImagem = "guanaantartica200mlgarrafa.jpg", CategoriaId = 1, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GUANA ANTARTICA 200ml GARRAFA" },
new Produto { Id = 63, Nome = "SPRITE 350 ml LATA", UrlImagem = "sprite350mllata.jpg", CategoriaId = 1, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "SPRITE 350 ml LATA" },
new Produto { Id = 64, Nome = "HEINEKEN 350ml", UrlImagem = "heineken350ml.jpg", CategoriaId = 2, PrecoRetirar = 5.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "HEINEKEN 350ml" },
new Produto { Id = 65, Nome = "DOSE SEM LIMAO", UrlImagem = "dosesemlimao.jpg", CategoriaId = 9, PrecoRetirar = 4.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DOSE SEM LIMAO" },
new Produto { Id = 66, Nome = "DOSE COM LIMAO", UrlImagem = "dosecomlimao.jpg", CategoriaId = 9, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DOSE COM LIMAO" },
new Produto { Id = 67, Nome = "DOSE JAMEL COM PARATUDO", UrlImagem = "dosejamelcomparatudo.jpg", CategoriaId = 9, PrecoRetirar = 6.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DOSE JAMEL COM PARATUDO" },
new Produto { Id = 68, Nome = "DESE JURUBEBA", UrlImagem = "desejurubeba.jpg", CategoriaId = 9, PrecoRetirar = 3.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DESE JURUBEBA" },
new Produto { Id = 69, Nome = "AGUA 510ml", UrlImagem = "agua510ml.jpg", CategoriaId = 10, PrecoRetirar = 2.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "AGUA 510ml" },
new Produto { Id = 70, Nome = "AGUA COM GAS 510ml", UrlImagem = "aguacomgas510ml.jpg", CategoriaId = 10, PrecoRetirar = 3.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "AGUA COM GAS 510ml" },
new Produto { Id = 71, Nome = "CARVAO TARTARUGAO 2KG", UrlImagem = "carvaotartarugao2kg.jpg", CategoriaId = 11, PrecoRetirar = 14.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CARVAO TARTARUGAO 2KG" },
new Produto { Id = 72, Nome = "CARTVAO TARTARUGAO 4KG", UrlImagem = "cartvaotartarugao4kg.jpg", CategoriaId = 11, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CARTVAO TARTARUGAO 4KG" },
new Produto { Id = 73, Nome = "PAÇOCAO", UrlImagem = "paçocao.jpg", CategoriaId = 20, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PAÇOCAO" },
new Produto { Id = 74, Nome = "PAÇOCA ROLHA", UrlImagem = "paçocarolha.jpg", CategoriaId = 9, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PAÇOCA ROLHA" },
new Produto { Id = 75, Nome = "HEINEKEN 600ml", UrlImagem = "heineken600ml.jpg", CategoriaId = 2, PrecoRetirar = 14.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "HEINEKEN 600ml" },
new Produto { Id = 76, Nome = "EISENBAHM 600ml", UrlImagem = "eisenbahm600ml.jpg", CategoriaId = 2, PrecoRetirar = 6.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "EISENBAHM 600ml" },
new Produto { Id = 77, Nome = "FANTA UVA 350ml LATA", UrlImagem = "fantauva350mllata.jpg", CategoriaId = 1, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "FANTA UVA 350ml LATA" },
new Produto { Id = 78, Nome = "EISENBAHN 350ml", UrlImagem = "eisenbahn350ml.jpg", CategoriaId = 2, PrecoRetirar = 3.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "EISENBAHN 350ml" },
new Produto { Id = 79, Nome = "HENEKEN ZERO LONGE NECK 330 ml", UrlImagem = "henekenzerolongeneck330ml.jpg", CategoriaId = 2, PrecoRetirar = 7.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "HENEKEN ZERO LONGE NECK 330 ml" },
new Produto { Id = 80, Nome = "CORONA", UrlImagem = "corona.jpg", CategoriaId = 2, PrecoRetirar = 7.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CORONA" },
new Produto { Id = 81, Nome = "CERVEJA STELLA LONGE NECK 330ml", UrlImagem = "cervejastellalongeneck330ml.jpg", CategoriaId = 2, PrecoRetirar = 7.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA STELLA LONGE NECK 330ml" },
new Produto { Id = 82, Nome = "HEINEKEN LATA 269 ml", UrlImagem = "heinekenlata269ml.jpg", CategoriaId = 2, PrecoRetirar = 4.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "HEINEKEN LATA 269 ml" },
new Produto { Id = 83, Nome = "TNT ORINAL", UrlImagem = "tntorinal.jpg", CategoriaId = 4, PrecoRetirar = 9.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TNT ORINAL" },
new Produto { Id = 84, Nome = "ATOMIC ENERGICO", UrlImagem = "atomicenergico.jpg", CategoriaId = 4, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ATOMIC ENERGICO" },
new Produto { Id = 85, Nome = "VINHO CHAPINHA", UrlImagem = "vinhochapinha.jpg", CategoriaId = 12, PrecoRetirar = 19.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "VINHO CHAPINHA" },
new Produto { Id = 86, Nome = "COPÂO DE WISKY", UrlImagem = "copâodewisky.jpg", CategoriaId = 21, PrecoRetirar = 15.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COPÂO DE WISKY" },
new Produto { Id = 87, Nome = "GELO PACOTE 2KG", UrlImagem = "gelopacote2kg.jpg", CategoriaId = 13, PrecoRetirar = 7.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GELO PACOTE 2KG" },
new Produto { Id = 88, Nome = "GELO SACO 5KG", UrlImagem = "gelosaco5kg.jpg", CategoriaId = 13, PrecoRetirar = 10.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GELO SACO 5KG" },
new Produto { Id = 89, Nome = "GELO DE COCO", UrlImagem = "gelodecoco.jpg", CategoriaId = 13, PrecoRetirar = 3.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GELO DE COCO" },
new Produto { Id = 90, Nome = "GELO DE COCO MORANGO", UrlImagem = "gelodecocomorango.jpg", CategoriaId = 13, PrecoRetirar = 3.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GELO DE COCO MORANGO" },
new Produto { Id = 91, Nome = "GELO DE COCO MARUCAJA", UrlImagem = "gelodecocomarucaja.jpg", CategoriaId = 13, PrecoRetirar = 3.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GELO DE COCO MARUCAJA" },
new Produto { Id = 92, Nome = "GELO DE COCO MELANCIA", UrlImagem = "gelodecocomelancia.jpg", CategoriaId = 13, PrecoRetirar = 3.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GELO DE COCO MELANCIA" },
new Produto { Id = 93, Nome = "NUSAQUINH IORDUT DE MORANGO", UrlImagem = "nusaquinhiordutdemorango.jpg", CategoriaId = 14, PrecoRetirar = 18.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "NUSAQUINH IORDUT DE MORANGO" },
new Produto { Id = 94, Nome = "ASKOV LIMÂO 900ml", UrlImagem = "askovlimâo900ml.jpg", CategoriaId = 15, PrecoRetirar = 19.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ASKOV LIMÂO 900ml" },
new Produto { Id = 95, Nome = "ASKOV KIWI 900ml", UrlImagem = "askovkiwi900ml.jpg", CategoriaId = 15, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ASKOV KIWI 900ml" },
new Produto { Id = 96, Nome = "ASKOV MARACUJÀ 900ml", UrlImagem = "askovmaracujà900ml.jpg", CategoriaId = 15, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ASKOV MARACUJÀ 900ml" },
new Produto { Id = 97, Nome = "ASKOV BLUEBERRY 900ml", UrlImagem = "askovblueberry900ml.jpg", CategoriaId = 15, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ASKOV BLUEBERRY 900ml" },
new Produto { Id = 98, Nome = "ASKOV VODKA 900ml", UrlImagem = "askovvodka900ml.jpg", CategoriaId = 15, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ASKOV VODKA 900ml" },
new Produto { Id = 99, Nome = "ASKOV FRUTAS ROXAS 900ml", UrlImagem = "askovfrutasroxas900ml.jpg", CategoriaId = 15, PrecoRetirar = 0.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ASKOV FRUTAS ROXAS 900ml" },
new Produto { Id = 100, Nome = "ETERNITY GIN TROPICAL FRUTS 900ml", UrlImagem = "eternitygintropicalfruts900ml.jpg", CategoriaId = 16, PrecoRetirar = 23.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ETERNITY GIN TROPICAL FRUTS 900ml" },
new Produto { Id = 101, Nome = "ETERNITY GIN MAÇÂ VERDE 900ml", UrlImagem = "eternityginmaçâverde900ml.jpg", CategoriaId = 16, PrecoRetirar = 23.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ETERNITY GIN MAÇÂ VERDE 900ml" },
new Produto { Id = 102, Nome = "ETERNITY GIN WATERMWLON 900ml", UrlImagem = "eternityginwatermwlon900ml.jpg", CategoriaId = 16, PrecoRetirar = 23.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ETERNITY GIN WATERMWLON 900ml" },
new Produto { Id = 103, Nome = "ETERNITY GIN STRAWBERRY 900ml", UrlImagem = "eternityginstrawberry900ml.jpg", CategoriaId = 16, PrecoRetirar = 23.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ETERNITY GIN STRAWBERRY 900ml" },
new Produto { Id = 104, Nome = "MANSAO MAROMBA WHISK+COMBO", UrlImagem = "mansaomarombawhisk+combo.jpg", CategoriaId = 15, PrecoRetirar = 13.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "MANSAO MAROMBA WHISK+COMBO" },
new Produto { Id = 105, Nome = "MANSAO MAROMBA VODKA+CAFEINA", UrlImagem = "mansaomarombavodka+cafeina.jpg", CategoriaId = 15, PrecoRetirar = 13.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "MANSAO MAROMBA VODKA+CAFEINA" },
new Produto { Id = 106, Nome = "MANSAO MAROMBA MACA VERDE", UrlImagem = "mansaomarombamacaverde.jpg", CategoriaId = 15, PrecoRetirar = 13.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "MANSAO MAROMBA MACA VERDE" },
new Produto { Id = 107, Nome = "MANSAO MAROMBA WHISKY+COCONUT", UrlImagem = "mansaomarombawhisky+coconut.jpg", CategoriaId = 15, PrecoRetirar = 13.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "MANSAO MAROMBA WHISKY+COCONUT" },
new Produto { Id = 108, Nome = "MANSAO MAROMBA MELANCIA", UrlImagem = "mansaomarombamelancia.jpg", CategoriaId = 15, PrecoRetirar = 13.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "MANSAO MAROMBA MELANCIA" },
new Produto { Id = 109, Nome = "CERVEJA ITAIPAVA ZERO 330 ml", UrlImagem = "cervejaitaipavazero330ml.jpg", CategoriaId = 2, PrecoRetirar = 4.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA ITAIPAVA ZERO 330 ml" },
new Produto { Id = 110, Nome = "CERVEJA AMSTEL 600ML", UrlImagem = "cervejaamstel600ml.jpg", CategoriaId = 2, PrecoRetirar = 9.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA AMSTEL 600ML" },
new Produto { Id = 111, Nome = "BAIANINHA AMENDOIN 900", UrlImagem = "baianinhaamendoin900.jpg", CategoriaId = 15, PrecoRetirar = 18.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BAIANINHA AMENDOIN 900" },
new Produto { Id = 112, Nome = "SAO BERNADO 475ML", UrlImagem = "saobernado475ml.jpg", CategoriaId = 15, PrecoRetirar = 3.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "SAO BERNADO 475ML" },
new Produto { Id = 113, Nome = "COROTE JAPIRA 500ML", UrlImagem = "corotejapira500ml.jpg", CategoriaId = 15, PrecoRetirar = 3.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COROTE JAPIRA 500ML" },
new Produto { Id = 114, Nome = "ROTHMANS AZUL", UrlImagem = "rothmansazul.jpg", CategoriaId = 6, PrecoRetirar = 8.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ROTHMANS AZUL" },
new Produto { Id = 115, Nome = "ROTHMANS AZUL", UrlImagem = "rothmansazul.jpg", CategoriaId = 6, PrecoRetirar = 8.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ROTHMANS AZUL" },
new Produto { Id = 116, Nome = "PALHEIRO UNIVERSITARIO MENTA", UrlImagem = "palheirouniversitariomenta.jpg", CategoriaId = 6, PrecoRetirar = 8.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PALHEIRO UNIVERSITARIO MENTA" },
new Produto { Id = 117, Nome = "CARVAO TARTARUGAO 5KG", UrlImagem = "carvaotartarugao5kg.jpg", CategoriaId = 11, PrecoRetirar = 27.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CARVAO TARTARUGAO 5KG" },
new Produto { Id = 118, Nome = "SKOL LITRAO 1L", UrlImagem = "skollitrao1l.jpg", CategoriaId = 2, PrecoRetirar = 11.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "SKOL LITRAO 1L" },
new Produto { Id = 119, Nome = "DEL VALLE MARACUJÁ 1L", UrlImagem = "delvallemaracujá1l.jpg", CategoriaId = 3, PrecoRetirar = 10.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DEL VALLE MARACUJÁ 1L" },
new Produto { Id = 120, Nome = "COPÃO JACK DANIELS / WHITE HOURSE", UrlImagem = "copãojackdaniels/whitehourse.jpg", CategoriaId = 21, PrecoRetirar = 25.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COPÃO JACK DANIELS / WHITE HOURSE" },
new Produto { Id = 121, Nome = "COPAO RED LABEL", UrlImagem = "copaoredlabel.jpg", CategoriaId = 15, PrecoRetirar = 16.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COPAO RED LABEL" },
new Produto { Id = 122, Nome = "COMBO PASSAPORTE / GYM / VODKA", UrlImagem = "combopassaporte/gym/vodka.jpg", CategoriaId = 21, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COMBO PASSAPORTE / GYM / VODKA" },
new Produto { Id = 123, Nome = "CIGARRO SOLTO CHESTERFIELD / ROTHMANS", UrlImagem = "cigarrosoltochesterfield/rothmans.jpg", CategoriaId = 21, PrecoRetirar = 0.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CIGARRO SOLTO CHESTERFIELD / ROTHMANS" },
new Produto { Id = 124, Nome = "CHOP VINHO DRAFT", UrlImagem = "chopvinhodraft.jpg", CategoriaId = 21, PrecoRetirar = 16.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CHOP VINHO DRAFT" },
new Produto { Id = 125, Nome = "HEINEKEN ZERO LONG 330ml", UrlImagem = "heinekenzerolong330ml.jpg", CategoriaId = 21, PrecoRetirar = 6.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "HEINEKEN ZERO LONG 330ml" },
new Produto { Id = 126, Nome = "ESPETO DE CARNE", UrlImagem = "espetodecarne.jpg", CategoriaId = 21, PrecoRetirar = 49.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ESPETO DE CARNE" },
new Produto { Id = 127, Nome = "BUDWEISER LATA 350ML", UrlImagem = "budweiserlata350ml.jpg", CategoriaId = 2, PrecoRetirar = 4.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BUDWEISER LATA 350ML" },
new Produto { Id = 128, Nome = "COCA COLA 600ML", UrlImagem = "cocacola600ml.jpg", CategoriaId = 1, PrecoRetirar = 5.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COCA COLA 600ML" },
new Produto { Id = 129, Nome = "COCA COLA 1L", UrlImagem = "cocacola1l.jpg", CategoriaId = 1, PrecoRetirar = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COCA COLA 1L" },
new Produto { Id = 130, Nome = "JACK DANIEILS", UrlImagem = "jackdanieils.jpg", CategoriaId = 15, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "JACK DANIEILS" },
new Produto { Id = 131, Nome = "CERVEJA ANTARCTICA", UrlImagem = "cervejaantarctica.jpg", CategoriaId = 2, PrecoRetirar = 3.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA ANTARCTICA" },
new Produto { Id = 132, Nome = "PETRA BARRIGUDINHA", UrlImagem = "petrabarrigudinha.jpg", CategoriaId = 2, PrecoRetirar = 3.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PETRA BARRIGUDINHA" },
new Produto { Id = 133, Nome = "ENERGICO MONSTER MELON", UrlImagem = "energicomonstermelon.jpg", CategoriaId = 4, PrecoRetirar = 11.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ENERGICO MONSTER MELON" },
new Produto { Id = 134, Nome = "ENERGICO MOSTER PARADISE", UrlImagem = "energicomosterparadise.jpg", CategoriaId = 4, PrecoRetirar = 10.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ENERGICO MOSTER PARADISE" },
new Produto { Id = 135, Nome = "CERVEJA PETRA LATA 350ML", UrlImagem = "cervejapetralata350ml.jpg", CategoriaId = 2, PrecoRetirar = 3.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA PETRA LATA 350ML" },
new Produto { Id = 136, Nome = "RED BULL 250ML", UrlImagem = "redbull250ml.jpg", CategoriaId = 4, PrecoRetirar = 9.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "RED BULL 250ML" },
new Produto { Id = 137, Nome = "CERVEJA ITAIPAVA LATA 350 ML", UrlImagem = "cervejaitaipavalata350ml.jpg", CategoriaId = 2, PrecoRetirar = 3.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CERVEJA ITAIPAVA LATA 350 ML" },
new Produto { Id = 138, Nome = "ESPETO SABORES", UrlImagem = "espetosabores.jpg", CategoriaId = 17, PrecoRetirar = 8.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ESPETO SABORES" },
new Produto { Id = 139, Nome = "ESPETO CAFITA", UrlImagem = "espetocafita.jpg", CategoriaId = 17, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ESPETO CAFITA" },
new Produto { Id = 140, Nome = "CHICLETE", UrlImagem = "chiclete.jpg", CategoriaId = 20, PrecoRetirar = 0.15M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CHICLETE" },
new Produto { Id = 141, Nome = "CHOQUITO", UrlImagem = "choquito.jpg", CategoriaId = 20, PrecoRetirar = 2.80M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CHOQUITO" },
new Produto { Id = 142, Nome = "ITUBAINA ORIGINAL 600ML", UrlImagem = "itubainaoriginal600ml.jpg", CategoriaId = 1, PrecoRetirar = 4.49M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ITUBAINA ORIGINAL 600ML" },
new Produto { Id = 143, Nome = "ITUBAINA ORIGINAL LONG 355ML", UrlImagem = "itubainaoriginallong355ml.jpg", CategoriaId = 1, PrecoRetirar = 5.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ITUBAINA ORIGINAL LONG 355ML" },
new Produto { Id = 144, Nome = "FRITISCO BATATA 40gm", UrlImagem = "fritiscobatata40gm.jpg", CategoriaId = 8, PrecoRetirar = 3.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "FRITISCO BATATA 40gm" },
new Produto { Id = 145, Nome = "TRIDENT", UrlImagem = "trident.jpg", CategoriaId = 21, PrecoRetirar = 2.69M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TRIDENT" },
new Produto { Id = 146, Nome = "PRESTIGIO", UrlImagem = "prestigio.jpg", CategoriaId = 21, PrecoRetirar = 2.79M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PRESTIGIO" },
new Produto { Id = 147, Nome = "FILTRO ALEDA 150 UNIDADES", UrlImagem = "filtroaleda150unidades.jpg", CategoriaId = 21, PrecoRetirar = 6.59M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "FILTRO ALEDA 150 UNIDADES" },
new Produto { Id = 148, Nome = "TABACO ACREMA BLEND 20g", UrlImagem = "tabacoacremablend20g.jpg", CategoriaId = 6, PrecoRetirar = 17.49M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TABACO ACREMA BLEND 20g" },
new Produto { Id = 149, Nome = "CIGARRO WINSTON SABOR", UrlImagem = "cigarrowinstonsabor.jpg", CategoriaId = 6, PrecoRetirar = 12.49M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CIGARRO WINSTON SABOR" },
new Produto { Id = 150, Nome = "CIGARRO WINSTON SELECTED RED", UrlImagem = "cigarrowinstonselectedred.jpg", CategoriaId = 21, PrecoRetirar = 11.49M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CIGARRO WINSTON SELECTED RED" },
new Produto { Id = 151, Nome = "PORÇÃO DE BATATA 300gm", UrlImagem = "porçãodebatata300gm.jpg", CategoriaId = 18, PrecoRetirar = 15.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PORÇÃO DE BATATA 300gm" },
new Produto { Id = 152, Nome = "PORÇÃO DE BATATA 500gm", UrlImagem = "porçãodebatata500gm.jpg", CategoriaId = 18, PrecoRetirar = 25.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PORÇÃO DE BATATA 500gm" },
new Produto { Id = 153, Nome = "PORÇÃO DE TORRESMO 300gm", UrlImagem = "porçãodetorresmo300gm.jpg", CategoriaId = 21, PrecoRetirar = 25.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PORÇÃO DE TORRESMO 300gm" },
new Produto { Id = 154, Nome = "PORÇÃO DE TORRESMO 500gm", UrlImagem = "porçãodetorresmo500gm.jpg", CategoriaId = 21, PrecoRetirar = 35.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PORÇÃO DE TORRESMO 500gm" },
new Produto { Id = 155, Nome = "PORÇÃO DE CALABRESA 300gm", UrlImagem = "porçãodecalabresa300gm.jpg", CategoriaId = 21, PrecoRetirar = 20.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PORÇÃO DE CALABRESA 300gm" },
new Produto { Id = 156, Nome = "PORÇÃO DE CALABRESA 500gm", UrlImagem = "porçãodecalabresa500gm.jpg", CategoriaId = 21, PrecoRetirar = 35.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PORÇÃO DE CALABRESA 500gm" },
new Produto { Id = 157, Nome = "PORÇÃO DE TULIPA 300gm", UrlImagem = "porçãodetulipa300gm.jpg", CategoriaId = 21, PrecoRetirar = 25.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PORÇÃO DE TULIPA 300gm" },
new Produto { Id = 158, Nome = "PORÇÃO DE ISCA DE TILAPIA 300gm", UrlImagem = "porçãodeiscadetilapia300gm.jpg", CategoriaId = 21, PrecoRetirar = 35.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PORÇÃO DE ISCA DE TILAPIA 300gm" },
new Produto { Id = 159, Nome = "PORÇÃO DE ISCA DE TILAPIA 500gm", UrlImagem = "porçãodeiscadetilapia500gm.jpg", CategoriaId = 21, PrecoRetirar = 50.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PORÇÃO DE ISCA DE TILAPIA 500gm" },
new Produto { Id = 160, Nome = "PASTEL CARNE / QUEIJO / PIZZA", UrlImagem = "pastelcarne/queijo/pizza.jpg", CategoriaId = 21, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PASTEL CARNE / QUEIJO / PIZZA" },
new Produto { Id = 161, Nome = "PASTEL DE FRANGO COM CATUPIRY / CARNE COM QUEIJO", UrlImagem = "pasteldefrangocomcatupiry/carnecomqueijo.jpg", CategoriaId = 21, PrecoRetirar = 12.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PASTEL DE FRANGO COM CATUPIRY / CARNE COM QUEIJO" },
new Produto { Id = 162, Nome = "AGUA COM GAS MINALBA 1,5L", UrlImagem = "aguacomgasminalba1,5l.jpg", CategoriaId = 10, PrecoRetirar = 5.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "AGUA COM GAS MINALBA 1,5L" },
new Produto { Id = 163, Nome = "SALGADO", UrlImagem = "salgado.jpg", CategoriaId = 21, PrecoRetirar = 8.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "SALGADO" },
new Produto { Id = 164, Nome = "PORÇAO DE FRANGO A PASSARINHO 300g", UrlImagem = "porçaodefrangoapassarinho300g.jpg", CategoriaId = 21, PrecoRetirar = 15.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PORÇAO DE FRANGO A PASSARINHO 300g" },
new Produto { Id = 165, Nome = "BATATA 300g", UrlImagem = "batata300g.jpg", CategoriaId = 18, PrecoRetirar = 15.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BATATA 300g" },
new Produto { Id = 166, Nome = "BATATA 500g", UrlImagem = "batata500g.jpg", CategoriaId = 18, PrecoRetirar = 25.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BATATA 500g" },
new Produto { Id = 167, Nome = "TULIPA 500g", UrlImagem = "tulipa500g.jpg", CategoriaId = 21, PrecoRetirar = 35.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TULIPA 500g" },
new Produto { Id = 168, Nome = "CALABRESA ACEBOLADA 300g", UrlImagem = "calabresaacebolada300g.jpg", CategoriaId = 21, PrecoRetirar = 20.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CALABRESA ACEBOLADA 300g" },
new Produto { Id = 169, Nome = "CALABRESA ACEBOLADA 500g", UrlImagem = "calabresaacebolada500g.jpg", CategoriaId = 21, PrecoRetirar = 35.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CALABRESA ACEBOLADA 500g" },
new Produto { Id = 170, Nome = "DOSE DE WHISKY", UrlImagem = "dosedewhisky.jpg", CategoriaId = 21, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DOSE DE WHISKY" },
new Produto { Id = 171, Nome = "FANTA UVA 2L", UrlImagem = "fantauva2l.jpg", CategoriaId = 1, PrecoRetirar = 7.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "FANTA UVA 2L" },
new Produto { Id = 172, Nome = "AMSTEL 600ml", UrlImagem = "amstel600ml.jpg", CategoriaId = 21, PrecoRetirar = 6.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "AMSTEL 600ml" },
new Produto { Id = 173, Nome = "PALHEIRO SOLTO", UrlImagem = "palheirosolto.jpg", CategoriaId = 6, PrecoRetirar = 1.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PALHEIRO SOLTO" },
new Produto { Id = 174, Nome = "AGUA 1,5", UrlImagem = "agua1,5.jpg", CategoriaId = 10, PrecoRetirar = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "AGUA 1,5" },
new Produto { Id = 175, Nome = "PAO DE MEL", UrlImagem = "paodemel.jpg", CategoriaId = 20, PrecoRetirar = 7.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PAO DE MEL" },
new Produto { Id = 176, Nome = "SPRITE 2L", UrlImagem = "sprite2l.jpg", CategoriaId = 1, PrecoRetirar = 11.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "SPRITE 2L" },
new Produto { Id = 177, Nome = "SMIRNOFF ICE", UrlImagem = "smirnoffice.jpg", CategoriaId = 5, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "SMIRNOFF ICE" },
new Produto { Id = 178, Nome = "GT BEATS", UrlImagem = "gtbeats.jpg", CategoriaId = 15, PrecoRetirar = 8.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GT BEATS" },
new Produto { Id = 179, Nome = "FINI", UrlImagem = "fini.jpg", CategoriaId = 20, PrecoRetirar = 2.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "FINI" },
new Produto { Id = 180, Nome = "FINI", UrlImagem = "fini.jpg", CategoriaId = 9, PrecoRetirar = 2.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "FINI" },
new Produto { Id = 181, Nome = "ZOMO SEDA/SOLTA", UrlImagem = "zomoseda/solta.jpg", CategoriaId = 21, PrecoRetirar = 0.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ZOMO SEDA/SOLTA" },
new Produto { Id = 182, Nome = "PEPSI 2L", UrlImagem = "pepsi2l.jpg", CategoriaId = 1, PrecoRetirar = 8.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PEPSI 2L" },
new Produto { Id = 183, Nome = "TUBAINA RICARDO", UrlImagem = "tubainaricardo.jpg", CategoriaId = 21, PrecoRetirar = 6.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TUBAINA RICARDO" },
new Produto { Id = 184, Nome = "GUARANA ANTARTICA 2L", UrlImagem = "guaranaantartica2l.jpg", CategoriaId = 1, PrecoRetirar = 10.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GUARANA ANTARTICA 2L" },
new Produto { Id = 185, Nome = "FANTA LARANJA LATA 220ML", UrlImagem = "fantalaranjalata220ml.jpg", CategoriaId = 1, PrecoRetirar = 3.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "FANTA LARANJA LATA 220ML" },
new Produto { Id = 186, Nome = "passport whisk", UrlImagem = "passportwhisk.jpg", CategoriaId = 21, PrecoRetirar = 59.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "passport whisk" },
new Produto { Id = 187, Nome = "COMBO ENERGETICO BALY+ETERNITY", UrlImagem = "comboenergeticobaly+eternity.jpg", CategoriaId = 19, PrecoRetirar = 40.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COMBO ENERGETICO BALY+ETERNITY" },
new Produto { Id = 188, Nome = "COPAO DE CHOPP 700ML", UrlImagem = "copaodechopp700ml.jpg", CategoriaId = 2, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COPAO DE CHOPP 700ML" },
new Produto { Id = 189, Nome = "DEL VALLE 1,5", UrlImagem = "delvalle1,5.jpg", CategoriaId = 21, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DEL VALLE 1,5" },
new Produto { Id = 190, Nome = "OLEO DE ALGODAO SOYA 14,5KG", UrlImagem = "oleodealgodaosoya14,5kg.jpg", CategoriaId = 21, PrecoRetirar = 177.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "OLEO DE ALGODAO SOYA 14,5KG" },
new Produto { Id = 191, Nome = "BATATA PALITO TRADICIONAL(9MM)EASY CHEF PCT 1,1KG", UrlImagem = "batatapalitotradicional(9mm)easychefpct1,1kg.jpg", CategoriaId = 21, PrecoRetirar = 12.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BATATA PALITO TRADICIONAL(9MM)EASY CHEF PCT 1,1KG" },
new Produto { Id = 192, Nome = "ORIGINAL LATA 350ML CX CARTAO C/12 NPAL", UrlImagem = "originallata350mlcxcartaoc/12npal.jpg", CategoriaId = 21, PrecoRetirar = 4.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ORIGINAL LATA 350ML CX CARTAO C/12 NPAL" },
new Produto { Id = 193, Nome = "ORIGINAL 300ML", UrlImagem = "original300ml.jpg", CategoriaId = 2, PrecoRetirar = 3.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ORIGINAL 300ML" },
new Produto { Id = 194, Nome = "PEPSI COLA PET 2L CAIXA C/6", UrlImagem = "pepsicolapet2lcaixac/6.jpg", CategoriaId = 21, PrecoRetirar = 9.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PEPSI COLA PET 2L CAIXA C/6" },
new Produto { Id = 195, Nome = "RED BULL BR LATA 250ML SIX PACK NPAL .", UrlImagem = "redbullbrlata250mlsixpacknpal..jpg", CategoriaId = 21, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "RED BULL BR LATA 250ML SIX PACK NPAL ." },
new Produto { Id = 196, Nome = "SPATEN N LT SLEEK 350ML CX CART C 12", UrlImagem = "spatennltsleek350mlcxcartc12.jpg", CategoriaId = 2, PrecoRetirar = 4.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "SPATEN N LT SLEEK 350ML CX CART C 12" },
new Produto { Id = 197, Nome = "BRAHMA CHOPP LATA 350ML SH C/12 NPAL", UrlImagem = "brahmachopplata350mlshc/12npal.jpg", CategoriaId = 2, PrecoRetirar = 4.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BRAHMA CHOPP LATA 350ML SH C/12 NPAL" },
new Produto { Id = 198, Nome = "RED BULL MELANCIA LATA 250ML FOUR PACK NPAL", UrlImagem = "redbullmelancialata250mlfourpacknpal.jpg", CategoriaId = 21, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "RED BULL MELANCIA LATA 250ML FOUR PACK NPAL" },
new Produto { Id = 199, Nome = "BRAHMA CHOPP GFA VD 1L COM TTC", UrlImagem = "brahmachoppgfavd1lcomttc.jpg", CategoriaId = 2, PrecoRetirar = 12.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BRAHMA CHOPP GFA VD 1L COM TTC" },
new Produto { Id = 200, Nome = "ANTARCTICA PILSEN 300ML", UrlImagem = "antarcticapilsen300ml.jpg", CategoriaId = 21, PrecoRetirar = 3.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ANTARCTICA PILSEN 300ML" },
new Produto { Id = 201, Nome = "ORIGINAL 600ML", UrlImagem = "original600ml.jpg", CategoriaId = 21, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ORIGINAL 600ML" },
new Produto { Id = 202, Nome = "BUDWEISER 300ML", UrlImagem = "budweiser300ml.jpg", CategoriaId = 2, PrecoRetirar = 3.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BUDWEISER 300ML" },
new Produto { Id = 203, Nome = "PUREZA VITAL AGUA MIN. C GAS GFA PET 510ML FD C/12", UrlImagem = "purezavitalaguamin.cgasgfapet510mlfdc/12.jpg", CategoriaId = 21, PrecoRetirar = 4.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PUREZA VITAL AGUA MIN. C GAS GFA PET 510ML FD C/12" },
new Produto { Id = 204, Nome = "GUARANA CHP ANTARCTICA PET 200ML SH C/12", UrlImagem = "guaranachpantarcticapet200mlshc/12.jpg", CategoriaId = 1, PrecoRetirar = 2.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GUARANA CHP ANTARCTICA PET 200ML SH C/12" },
new Produto { Id = 205, Nome = "MINALBA AGUA MINERAL C/GAS GFA PET 1,5L FD C/6", UrlImagem = "minalbaaguamineralc/gasgfapet1,5lfdc/6.jpg", CategoriaId = 21, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "MINALBA AGUA MINERAL C/GAS GFA PET 1,5L FD C/6" },
new Produto { Id = 206, Nome = "MINALBA AGUA MINERAL S/GAS GFA PET 1,5L FD C/6", UrlImagem = "minalbaaguaminerals/gasgfapet1,5lfdc/6.jpg", CategoriaId = 21, PrecoRetirar = 4.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "MINALBA AGUA MINERAL S/GAS GFA PET 1,5L FD C/6" },
new Produto { Id = 207, Nome = "PUREZA VITAL AGUA MIN. S GAS GFA PET 1,5L FD C/6", UrlImagem = "purezavitalaguamin.sgasgfapet1,5lfdc/6.jpg", CategoriaId = 21, PrecoRetirar = 4.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PUREZA VITAL AGUA MIN. S GAS GFA PET 1,5L FD C/6" },
new Produto { Id = 208, Nome = "ORIGINAL LT 269ML CX CARTAO C/8 NPAL", UrlImagem = "originallt269mlcxcartaoc/8npal.jpg", CategoriaId = 2, PrecoRetirar = 3.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ORIGINAL LT 269ML CX CARTAO C/8 NPAL" },
new Produto { Id = 209, Nome = "SKOL LATA 350ML SH C/18 NPAL MULTPACK", UrlImagem = "skollata350mlshc/18npalmultpack.jpg", CategoriaId = 2, PrecoRetirar = 4.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "SKOL LATA 350ML SH C/18 NPAL MULTPACK" },
new Produto { Id = 210, Nome = "BRAHMA CHOPP ZERO LATA 350ML SH C/12 NPAL", UrlImagem = "brahmachoppzerolata350mlshc/12npal.jpg", CategoriaId = 2, PrecoRetirar = 4.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BRAHMA CHOPP ZERO LATA 350ML SH C/12 NPAL" },
new Produto { Id = 211, Nome = "FANTA LARANJA PET RET 2 LITROS 09UN", UrlImagem = "fantalaranjapetret2litros09un.jpg", CategoriaId = 21, PrecoRetirar = 8.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "FANTA LARANJA PET RET 2 LITROS 09UN" },
new Produto { Id = 212, Nome = "CC PET RET 2L 9 JD", UrlImagem = "ccpetret2l9jd.jpg", CategoriaId = 21, PrecoRetirar = 8.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CC PET RET 2L 9 JD" },
new Produto { Id = 213, Nome = "Cerv Heineken Pil 0,60Gfa Rt 24Un", UrlImagem = "cervheinekenpil0,60gfart24un.jpg", CategoriaId = 21, PrecoRetirar = 13.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "Cerv Heineken Pil 0,60Gfa Rt 24Un" },
new Produto { Id = 214, Nome = "Cerv Heineken 0,0% 0,350ltsleekdes12unpb", UrlImagem = "cervheineken0,0%0,350ltsleekdes12unpb.jpg", CategoriaId = 21, PrecoRetirar = 7.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "Cerv Heineken 0,0% 0,350ltsleekdes12unpb" },
new Produto { Id = 215, Nome = "RED BULL TROPICAL BR LATA 250ML FOUR PACK NPAL .", UrlImagem = "redbulltropicalbrlata250mlfourpacknpal..jpg", CategoriaId = 21, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "RED BULL TROPICAL BR LATA 250ML FOUR PACK NPAL ." },
new Produto { Id = 216, Nome = "ORIGINAL LATA 350ML CX CARTAO C/12 NPAL", UrlImagem = "originallata350mlcxcartaoc/12npal.jpg", CategoriaId = 21, PrecoRetirar = 4.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ORIGINAL LATA 350ML CX CARTAO C/12 NPAL" },
new Produto { Id = 217, Nome = "FTA LAR LT 220ml 6U FI MAINLINE", UrlImagem = "ftalarlt220ml6ufimainline.jpg", CategoriaId = 21, PrecoRetirar = 3.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "FTA LAR LT 220ml 6U FI MAINLINE" },
new Produto { Id = 218, Nome = "Cerv Amstel Lager 0,60l Gfa Rt 24un", UrlImagem = "cervamstellager0,60lgfart24un.jpg", CategoriaId = 21, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "Cerv Amstel Lager 0,60l Gfa Rt 24un" },
new Produto { Id = 219, Nome = "Cerv Heineken Pil 0,60Gfa Rt 24Un", UrlImagem = "cervheinekenpil0,60gfart24un.jpg", CategoriaId = 21, PrecoRetirar = 13.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "Cerv Heineken Pil 0,60Gfa Rt 24Un" },
new Produto { Id = 220, Nome = "ORIGINAL LATA FINA", UrlImagem = "originallatafina.jpg", CategoriaId = 2, PrecoRetirar = 3.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ORIGINAL LATA FINA" },
new Produto { Id = 221, Nome = "CHESTERFIELD", UrlImagem = "chesterfield.jpg", CategoriaId = 6, PrecoRetirar = 10.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CHESTERFIELD" },
new Produto { Id = 222, Nome = "DUNHILL", UrlImagem = "dunhill.jpg", CategoriaId = 6, PrecoRetirar = 15.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DUNHILL" },
new Produto { Id = 223, Nome = "CHESTERFIELD REMIX BEATS", UrlImagem = "chesterfieldremixbeats.jpg", CategoriaId = 6, PrecoRetirar = 12.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CHESTERFIELD REMIX BEATS" },
new Produto { Id = 224, Nome = "RED BULL MELANCIA", UrlImagem = "redbullmelancia.jpg", CategoriaId = 4, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "RED BULL MELANCIA" },
new Produto { Id = 225, Nome = "RED BULL TROPICAL 250 ML", UrlImagem = "redbulltropical250ml.jpg", CategoriaId = 4, PrecoRetirar = 10.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "RED BULL TROPICAL 250 ML" },
new Produto { Id = 226, Nome = "SPRITE LEMON FRESH", UrlImagem = "spritelemonfresh.jpg", CategoriaId = 1, PrecoRetirar = 6.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "SPRITE LEMON FRESH" },
new Produto { Id = 227, Nome = "POWER ADE 500ML", UrlImagem = "powerade500ml.jpg", CategoriaId = 21, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "POWER ADE 500ML" },
new Produto { Id = 228, Nome = "NUSAQUINHO DIVERSOS", UrlImagem = "nusaquinhodiversos.jpg", CategoriaId = 14, PrecoRetirar = 17.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "NUSAQUINHO DIVERSOS" },
new Produto { Id = 229, Nome = "VINHO CANTINA AGRICOLA 1L", UrlImagem = "vinhocantinaagricola1l.jpg", CategoriaId = 12, PrecoRetirar = 25.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "VINHO CANTINA AGRICOLA 1L" },
new Produto { Id = 230, Nome = "TAMPICO LATA 270ML FRUTAS CITRICAS", UrlImagem = "tampicolata270mlfrutascitricas.jpg", CategoriaId = 3, PrecoRetirar = 3.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "TAMPICO LATA 270ML FRUTAS CITRICAS" },
new Produto { Id = 231, Nome = "AMENDOIM 24G", UrlImagem = "amendoim24g.jpg", CategoriaId = 20, PrecoRetirar = 2.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "AMENDOIM 24G" },
new Produto { Id = 232, Nome = "BATOM 16G", UrlImagem = "batom16g.jpg", CategoriaId = 20, PrecoRetirar = 2.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BATOM 16G" },
new Produto { Id = 233, Nome = "COPO DE CHOP 500ML", UrlImagem = "copodechop500ml.jpg", CategoriaId = 2, PrecoRetirar = 7.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COPO DE CHOP 500ML" },
new Produto { Id = 234, Nome = "BALAS", UrlImagem = "balas.jpg", CategoriaId = 20, PrecoRetirar = 0.15M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "BALAS" },
new Produto { Id = 235, Nome = "PIRULITO BIG BIG", UrlImagem = "pirulitobigbig.jpg", CategoriaId = 20, PrecoRetirar = 1.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PIRULITO BIG BIG" },
new Produto { Id = 236, Nome = "CHOCOLATE LACTEA DIAM.NEG 80G", UrlImagem = "chocolatelacteadiam.neg80g.jpg", CategoriaId = 20, PrecoRetirar = 9.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CHOCOLATE LACTEA DIAM.NEG 80G" },
new Produto { Id = 237, Nome = "CHOCOLATE LACTEA BRANCO 80G", UrlImagem = "chocolatelacteabranco80g.jpg", CategoriaId = 20, PrecoRetirar = 9.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "CHOCOLATE LACTEA BRANCO 80G" },
new Produto { Id = 238, Nome = "PIRULITO ENERGE.28G", UrlImagem = "pirulitoenerge.28g.jpg", CategoriaId = 20, PrecoRetirar = 1.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "PIRULITO ENERGE.28G" },
new Produto { Id = 239, Nome = "COCA COLA ZERO PET 2L", UrlImagem = "cocazolazeropet2l.jpg", CategoriaId = 1, PrecoRetirar = 13.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "COCA COLA ZERO PET 2L" },
new Produto { Id = 240, Nome = "copao Jack Daniels Apple", UrlImagem = "copaojackdanielsapple.jpg", CategoriaId = 21, PrecoRetirar = 25.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "copao Jack Daniels Apple" },
new Produto { Id = 241, Nome = "RED LABEL GARRAFA", UrlImagem = "redlabelgarrafa.jpg", CategoriaId = 21, PrecoRetirar = 119.90M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "RED LABEL GARRAFA" },
new Produto { Id = 242, Nome = "MONSTER 269ML ORIGINAL", UrlImagem = "monster269mloriginal.jpg", CategoriaId = 4, PrecoRetirar = 8.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "MONSTER 269ML ORIGINAL" },
new Produto { Id = 243, Nome = "MONSTER 269 ML MANGO LOCO", UrlImagem = "monster269mlmangoloco.jpg", CategoriaId = 4, PrecoRetirar = 8.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "MONSTER 269 ML MANGO LOCO" },
new Produto { Id = 244, Nome = "FANTA UVA 220ML LATA", UrlImagem = "fantauva220mllata.jpg", CategoriaId = 21, PrecoRetirar = 3.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "FANTA UVA 220ML LATA" },
new Produto { Id = 245, Nome = "DEL VALLE 1L", UrlImagem = "delvalle1l.jpg", CategoriaId = 3, PrecoRetirar = 5.00M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DEL VALLE 1L" },
new Produto { Id = 246, Nome = "DEL VALLE 290ML MARACUJA", UrlImagem = "delvalle290mlmaracuja.jpg", CategoriaId = 3, PrecoRetirar = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DEL VALLE 290ML MARACUJA" },
new Produto { Id = 247, Nome = "DEL VALLE 290ML UVA", UrlImagem = "delvalle290mluva.jpg", CategoriaId = 3, PrecoRetirar = 4.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DEL VALLE 290ML UVA" },
new Produto { Id = 248, Nome = "MONSTER 473ML ULTRA PARADISE", UrlImagem = "monster473mlultraparadise.jpg", CategoriaId = 4, PrecoRetirar = 11.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "MONSTER 473ML ULTRA PARADISE" },
new Produto { Id = 249, Nome = "Cópia de EISENBAHN 350ml", UrlImagem = "cópiadeeisenbahn350ml.jpg", CategoriaId = 2, PrecoRetirar = 3.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "Cópia de EISENBAHN 350ml" },
new Produto { Id = 250, Nome = "DEL VALLE UVA1L", UrlImagem = "delvalleuva1l.jpg", CategoriaId = 3, PrecoRetirar = 10.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DEL VALLE UVA1L" },
new Produto { Id = 251, Nome = "ESPETO DE CORAÇÃO DE GALINHA", UrlImagem = "espetodecoraçãodegalinha.jpg", CategoriaId = 21, PrecoRetirar = 49.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ESPETO DE CORAÇÃO DE GALINHA" },
new Produto { Id = 252, Nome = "GATOREIDE SABOR BERRY BLUE 500 ml", UrlImagem = "gatoreidesaborberryblue500ml.jpg", CategoriaId = 21, PrecoRetirar = 7.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "GATOREIDE SABOR BERRY BLUE 500 ml" },
new Produto { Id = 253, Nome = "DEL VALLE GOIABA1L", UrlImagem = "delvallegoiaba1l.jpg", CategoriaId = 3, PrecoRetirar = 10.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DEL VALLE GOIABA1L" },
new Produto { Id = 254, Nome = "ESPETO DE KAFTA", UrlImagem = "espetodekafta.jpg", CategoriaId = 21, PrecoRetirar = 49.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ESPETO DE KAFTA" },
new Produto { Id = 255, Nome = "DEL VALLE PESSEGO1L", UrlImagem = "delvallepessego1l.jpg", CategoriaId = 3, PrecoRetirar = 10.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DEL VALLE PESSEGO1L" },
new Produto { Id = 256, Nome = "ESPETO DE FRANGO", UrlImagem = "espetodefrango.jpg", CategoriaId = 21, PrecoRetirar = 39.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ESPETO DE FRANGO" },
new Produto { Id = 257, Nome = "DEL VALLE LARANJA1L", UrlImagem = "delvallelaranja1l.jpg", CategoriaId = 3, PrecoRetirar = 10.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "DEL VALLE LARANJA1L" },
new Produto { Id = 258, Nome = "ESPETO DE LINGUIÇA", UrlImagem = "espetodelinguiça.jpg", CategoriaId = 21, PrecoRetirar = 39.99M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "ESPETO DE LINGUIÇA" },
new Produto { Id = 259, Nome = "JUJU GOURMAT", UrlImagem = "jujugourmet.jpg", CategoriaId = 21, PrecoRetirar = 4.50M, EmEstoque = 1, Disponivel = true, MaisVendido = true, Popular = true, Detalhe = "JUJU GOURMAT" }
        );
    }
}









