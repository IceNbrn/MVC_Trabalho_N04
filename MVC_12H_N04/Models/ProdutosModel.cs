using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MVC_12H_N04.Models
{
    public class ProdutosModel
    {
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Campo Nome tem de ser preenchido")]
        [StringLength(50)]
        [MinLength(2, ErrorMessage = "Username muito pequeno")]
        public string Nome { get; set; }

        [Display(Name = "Preço")]
        [Required(ErrorMessage = "Campo Preço tem de ser preenchido")]
        [DataType(DataType.Currency)]
        public decimal Preco { get; set; }

        [Display(Name = "Descrição")]
        [MinLength(5, ErrorMessage = "Descrição muito pequena")]
        public string Descricao { get; set; }

        [Display(Name = "Marca")]
        [MinLength(5, ErrorMessage = "A morada tem de ser preenchido")]
        [StringLength(300)]
        public string Marca { get; set; }

        public int Quantidade { get; set; }

        public bool Estado { get; set; }

        public string Tipo { get; set; }

        [Key]
        public int Id { get; set; }
    }
    public class ProdutosBd
    {
        public List<ProdutosModel> Lista()
        {
            string sql = "SELECT * FROM Produtos";
            DataTable registos = Bd.Instance.DevolveConsulta(sql);
            List<ProdutosModel> lista = new List<ProdutosModel>();

            foreach (DataRow dados in registos.Rows)
            {
                ProdutosModel novo = new ProdutosModel();
                novo.Nome = dados[1].ToString();
                novo.Preco = decimal.Parse(dados[2].ToString());
                novo.Descricao = dados[3].ToString();
                novo.Marca = dados[5].ToString();
                novo.Quantidade = 1;
                novo.Estado = bool.Parse(dados[8].ToString());
                novo.Tipo = dados[9].ToString();
                novo.Id = int.Parse(dados[0].ToString());
                lista.Add(novo);
            }

            return lista;
        }
        public List<ProdutosModel> Lista(int id)
        {
            
            string sql = "SELECT * FROM Produtos WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id },
            };
            DataTable registos = Bd.Instance.DevolveConsulta(sql, parametros);

            List<ProdutosModel> lista = new List<ProdutosModel>();
            foreach (DataRow dados in registos.Rows)
            {
                ProdutosModel novo = new ProdutosModel();
                novo.Nome = dados[1].ToString();
                novo.Preco = decimal.Parse(dados[2].ToString());
                novo.Descricao = dados[3].ToString();
                novo.Marca = dados[5].ToString();
                novo.Quantidade = 1;
                novo.Estado = bool.Parse(dados[8].ToString());
                novo.Tipo = dados[9].ToString();
                novo.Id = int.Parse(dados[0].ToString());
                lista.Add(novo);
            }

            return lista;
        }
        public List<ProdutosModel> ListaDesativados()
        {
            string sql = "SELECT * FROM Produtos WHERE estado='False'";
            DataTable registos = Bd.Instance.DevolveConsulta(sql);
            List<ProdutosModel> lista = new List<ProdutosModel>();

            foreach (DataRow dados in registos.Rows)
            {
                ProdutosModel novo = new ProdutosModel();
                novo.Nome = dados[1].ToString();
                novo.Preco = decimal.Parse(dados[2].ToString());
                novo.Descricao = dados[3].ToString();
                novo.Marca = dados[5].ToString();
                novo.Quantidade = 1;
                novo.Estado = bool.Parse(dados[8].ToString());
                novo.Tipo = dados[9].ToString();
                lista.Add(novo);
            }

            return lista;
        }
        public void AdicionarProdutos(ProdutosModel novo)
        {
            string sql = "INSERT INTO Produtos(nome,preco,descricao,marca,quantidade,estado,tipo) VALUES";
            sql += " (@nome,@preco,@descricao,@marca,@quantidade,@estado,@tipo)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.VarChar,Value=novo.Nome },
                new SqlParameter() {ParameterName="@preco",SqlDbType=SqlDbType.Decimal,Value=novo.Preco },
                new SqlParameter() {ParameterName="@descricao",SqlDbType=SqlDbType.NVarChar,Value=novo.Descricao },
                new SqlParameter() {ParameterName="@marca",SqlDbType=SqlDbType.NVarChar,Value=novo.Marca },
                new SqlParameter() {ParameterName="@quantidade",SqlDbType=SqlDbType.Int,Value=novo.Quantidade },
                new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Bit,Value=novo.Estado },
                new SqlParameter() {ParameterName="@tipo",SqlDbType=SqlDbType.NVarChar,Value=novo.Tipo },
            };
            Bd.Instance.ExecutaComando(sql, parametros);
        }
        public void RemoverProduto(int id)
        {
            string sql = "DELETE FROM Produtos WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=System.Data.SqlDbType.Int,Value=id }
            };
            Bd.Instance.ExecutaComando(sql, parametros);
        }
        public void AtualizarProduto(ProdutosModel produto)
        {
            string sql = "UPDATE Produtos SET nome=@nmome,preco=@preco,descricao=@descricao,marca=@marca,quantidade=@quantidade,estado=@estado,tipo=@tipo ";
            sql += " WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.NVarChar,Value=produto.Nome },
                new SqlParameter() {ParameterName="@preco",SqlDbType=SqlDbType.Decimal,Value=produto.Preco },
                new SqlParameter() {ParameterName="@descricao",SqlDbType=SqlDbType.NVarChar,Value=produto.Descricao },
                new SqlParameter() {ParameterName="@marca",SqlDbType=SqlDbType.NVarChar,Value=produto.Marca },
                new SqlParameter() {ParameterName="@quantidade",SqlDbType=SqlDbType.Int,Value=produto.Quantidade },
                new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Bit,Value=produto.Estado },
                new SqlParameter() {ParameterName="@tipo",SqlDbType=SqlDbType.NVarChar,Value=produto.Tipo },
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.NVarChar,Value=produto.Id }
            };
            Bd.Instance.ExecutaComando(sql, parametros);
        }
    }
}