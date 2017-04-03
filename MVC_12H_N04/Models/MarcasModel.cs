using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MVC_12H_N04.Models
{
    public class MarcasModel
    {
        [Required(ErrorMessage = "Campo Nome tem de ser preenchido")]
        [StringLength(50)]
        [MinLength(2, ErrorMessage = "Nome muito pequeno")]
        public string Nome { get; set; }

        [Key]
        public int Id { get; set; }
    }
    public class MarcasBd
    {
        public List<MarcasModel> Lista()
        {
            string sql = "SELECT * FROM Marcas";
            DataTable registos = Bd.Instance.DevolveConsulta(sql);
            List<MarcasModel> lista = new List<MarcasModel>();

            foreach (DataRow dados in registos.Rows)
            {
                MarcasModel novo = new MarcasModel();
                novo.Nome = dados[1].ToString();
                novo.Id = int.Parse(dados[0].ToString());
                lista.Add(novo);
            }

            return lista;
        }
        public List<MarcasModel> Lista(int id)
        {

            string sql = "SELECT * FROM Marcas WHERE IdMarca=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id },
            };
            DataTable registos = Bd.Instance.DevolveConsulta(sql, parametros);

            List<MarcasModel> lista = new List<MarcasModel>();
            foreach (DataRow dados in registos.Rows)
            {
                MarcasModel novo = new MarcasModel();
                novo.Nome = dados[1].ToString();
                novo.Id = int.Parse(dados[0].ToString());
                lista.Add(novo);
            }

            return lista;
        }
        public List<ProdutosModel> ListaMarcasProdutos(string nome)
        {

            string sql = "SELECT * FROM Produtos INNER JOIN Marcas ON Produtos.marca = Marcas.Nome WHERE Marcas.nome = @nome";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.Int,Value=nome },
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
        public void AdicionarMarca(MarcasModel novo)
        {
            string sql = "INSERT INTO Marcas(Nome) VALUES (@nome)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.NVarChar,Value=novo.Nome }
            };
            Bd.Instance.ExecutaComando(sql, parametros);
        }
        public void RemoverMarca(int id)
        {
            string sql = "DELETE FROM Marcas WHERE IdMarca=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=System.Data.SqlDbType.Int,Value=id }
            };
            Bd.Instance.ExecutaComando(sql, parametros);
        }
        public void AtualizarMarca(MarcasModel produto)
        {
            string sql = "UPDATE Marcas SET nome=@nome WHERE IdMarca=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.NVarChar,Value=produto.Nome },
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=produto.Id }
            };
            Bd.Instance.ExecutaComando(sql, parametros);
        }
        
    }
}