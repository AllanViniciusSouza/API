                                                                                                                    using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiECommerce.Entities
{
    public class CascosEmprestados
    {
        public int Id { get; set; }  // Campo Auto Increment no banco de dados

        public int ClienteId { get; set; }

        public Clientes Cliente { get; set; }

        public int Quantidade { get; set; }
        public DateTime DataEmprestimo { get; set; } = DateTime.Now; // Padrão: data atual
        public DateTime? DataDevolucao { get; set; }
        public string Status { get; set; } = "Emprestado";

        // Relacionamento: Um registro tem um usuário
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
