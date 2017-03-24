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
        public string nome { get; set; }

        [Display(Name = "Preço")]
        [MinLength(5, ErrorMessage = "O preco tem de ser preenchido")]
        [DataType(DataType.Currency)]
        public decimal preco { get; set; }

        [Display(Name = "Descrição")]
        [MinLength(5, ErrorMessage = "Descrição muito pequena")]
        public string descricao { get; set; }

        [Display(Name = "Marca")]
        [MinLength(5, ErrorMessage = "A morada tem de ser preenchido")]
        [StringLength(300)]
        public string marca { get; set; }

        public int quantidade { get; set; }

        public bool estado { get; set; }

        public string tipo { get; set; }
    }
    public class ProdutosBD
    {
        public List<ProdutosModel> lista()
        {
            string sql = "SELECT * FROM Produtos";
            DataTable registos = BD.Instance.devolveConsulta(sql);
            List<ProdutosModel> lista = new List<ProdutosModel>();

            foreach (DataRow dados in registos.Rows)
            {
                ProdutosModel novo = new ProdutosModel();
                novo.nome = dados[1].ToString();
                novo.preco = decimal.Parse(dados[2].ToString());
                novo.descricao = dados[3].ToString();
                novo.marca = dados[5].ToString();
                novo.quantidade = 1;
                novo.estado = bool.Parse(dados[8].ToString());
                novo.tipo = dados[9].ToString();
                lista.Add(novo);
            }

            return lista;
        }
        public List<ProdutosModel> lista(int id)
        {
            string sql = "SELECT * FROM Produtos WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id },
            };
            DataTable registos = BD.Instance.devolveConsulta(sql, parametros);

            List<ProdutosModel> lista = new List<ProdutosModel>();
            foreach (DataRow dados in registos.Rows)
            {
                ProdutosModel novo = new ProdutosModel();
                novo.nome = dados[1].ToString();
                novo.preco = decimal.Parse(dados[2].ToString());
                novo.descricao = dados[3].ToString();
                novo.marca = dados[5].ToString();
                novo.quantidade = 1;
                novo.estado = bool.Parse(dados[8].ToString());
                novo.tipo = dados[9].ToString();
                lista.Add(novo);
            }

            return lista;
        }
        public List<ProdutosModel> listaDesativados()
        {
            string sql = "SELECT * FROM Produtos WHERE estado='False'";
            DataTable registos = BD.Instance.devolveConsulta(sql);
            List<ProdutosModel> lista = new List<ProdutosModel>();

            foreach (DataRow dados in registos.Rows)
            {
                ProdutosModel novo = new ProdutosModel();
                novo.nome = dados[1].ToString();
                novo.preco = decimal.Parse(dados[2].ToString());
                novo.descricao = dados[3].ToString();
                novo.marca = dados[5].ToString();
                novo.quantidade = 1;
                novo.estado = bool.Parse(dados[8].ToString());
                novo.tipo = dados[9].ToString();
                lista.Add(novo);
            }

            return lista;
        }
        public void adicionarProdutos(ProdutosModel novo)
        {
            string sql = "INSERT INTO Produtos(nome,preco,descricao,marca,quantidade,estado,tipo) VALUES";
            sql += " (@nome,@preco,@descricao,@marca,@quantidade,@estado,@tipo)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.VarChar,Value=novo.nome },
                new SqlParameter() {ParameterName="@preco",SqlDbType=SqlDbType.Decimal,Value=novo.preco },
                new SqlParameter() {ParameterName="@descricao",SqlDbType=SqlDbType.NVarChar,Value=novo.descricao },
                new SqlParameter() {ParameterName="@marca",SqlDbType=SqlDbType.NVarChar,Value=novo.marca },
                new SqlParameter() {ParameterName="@quantidade",SqlDbType=SqlDbType.Int,Value=novo.quantidade },
                new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Bit,Value=novo.estado },
                new SqlParameter() {ParameterName="@tipo",SqlDbType=SqlDbType.NVarChar,Value=novo.tipo },
            };
            BD.Instance.executaComando(sql, parametros);
        }
        public void removerProduto(int id)
        {
            string sql = "DELETE FROM Produtos WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=System.Data.SqlDbType.Int,Value=id }
            };
            BD.Instance.executaComando(sql, parametros);
        }
        public void atualizarProduto(ProdutosModel produto)
        {
            string sql = "UPDATE Quartos SET nome=@nmome,preco=@preco,descricao=@descricao,marca=@marca,quantidade=@quantidade,estado=@estado,tipo=@tipo ";
            sql += " WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.VarChar,Value=produto.nome },
                new SqlParameter() {ParameterName="@preco",SqlDbType=SqlDbType.Decimal,Value=produto.preco },
                new SqlParameter() {ParameterName="@descricao",SqlDbType=SqlDbType.NVarChar,Value=produto.descricao },
                new SqlParameter() {ParameterName="@marca",SqlDbType=SqlDbType.NVarChar,Value=produto.marca },
                new SqlParameter() {ParameterName="@quantidade",SqlDbType=SqlDbType.Int,Value=produto.quantidade },
                new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Bit,Value=produto.estado },
                new SqlParameter() {ParameterName="@tipo",SqlDbType=SqlDbType.NVarChar,Value=produto.tipo },
            };
            BD.Instance.executaComando(sql, parametros);
        }
    }
}