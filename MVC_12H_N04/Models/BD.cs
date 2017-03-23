using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace MVC_12H_N04.Models
{
    public class BD
    {
        private static BD instance;
        public static BD Instance {
            get {
                if (instance == null)
                    instance = new BD();
                return instance;
            }
        }
        private string strLigacao;
        private SqlConnection ligacaoBD;
        public BD()
        {
            //ligação à bd
            strLigacao = ConfigurationManager.ConnectionStrings["sql"].ToString();
            ligacaoBD = new SqlConnection(strLigacao);
            ligacaoBD.Open();
        }
        ~BD()
        {
            try
            {
                ligacaoBD.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #region Funções genéricas
        //devolve consulta
        public DataTable devolveConsulta(string sql)
        {
            SqlCommand comando = new SqlCommand(sql, ligacaoBD);
            DataTable registos = new DataTable();
            SqlDataReader dados = comando.ExecuteReader();
            registos.Load(dados);
            registos.Dispose();
            comando.Dispose();
            return registos;
        }
        public DataTable devolveConsulta(string sql, List<SqlParameter> parametros)
        {
            SqlCommand comando = new SqlCommand(sql, ligacaoBD);
            DataTable registos = new DataTable();
            comando.Parameters.AddRange(parametros.ToArray());
            SqlDataReader dados = comando.ExecuteReader();
            registos.Load(dados);
            registos.Dispose();
            comando.Dispose();
            return registos;
        }


        public DataTable devolveConsulta(string sql, List<SqlParameter> parametros, SqlTransaction transacao)
        {
            SqlCommand comando = new SqlCommand(sql, ligacaoBD);
            comando.Transaction = transacao;
            DataTable registos = new DataTable();
            comando.Parameters.AddRange(parametros.ToArray());
            SqlDataReader dados = comando.ExecuteReader();
            registos.Load(dados);
            registos.Dispose();
            comando.Dispose();
            return registos;
        }

        //executar comando
        public bool executaComando(string sql)
        {
            try
            {
                SqlCommand comando = new SqlCommand(sql, ligacaoBD);
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception erro)
            {
                Debug.WriteLine(erro.Message);
                return false;
            }
            return true;
        }
        public bool executaComando(string sql, List<SqlParameter> parametros)
        {
            try
            {
                SqlCommand comando = new SqlCommand(sql, ligacaoBD);
                comando.Parameters.AddRange(parametros.ToArray());
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception erro)
            {
                Console.Write(erro.Message);
                //throw erro;
                return false;
            }
            return true;
        }
        public bool executaComando(string sql, List<SqlParameter> parametros, SqlTransaction transacao)
        {
            try
            {
                SqlCommand comando = new SqlCommand(sql, ligacaoBD);
                comando.Parameters.AddRange(parametros.ToArray());
                comando.Transaction = transacao;
                comando.ExecuteNonQuery();
                comando.Dispose();
            }
            catch (Exception erro)
            {
                Console.Write(erro.Message);
                return false;
            }
            return true;
        }
        #endregion
        #region comentarios
        public bool adicionaComentario(string comentario, string autor, int idProduto)
        {

            string sql = "INSERT INTO Comentarios(id_produto,autor,comentario)";
            sql += "VALUES (@id_produto,@autor,@comentario)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id_produto",SqlDbType=SqlDbType.Int,Value=idProduto},
                new SqlParameter() {ParameterName="@autor",SqlDbType=SqlDbType.NVarChar,Value=autor},
                new SqlParameter() {ParameterName="@comentario",SqlDbType=SqlDbType.NVarChar,Value=comentario}
            };
            return executaComando(sql, parametros);
        }
        public DataTable listaComentario(int id)
        {
            string sql = "SELECT * FROM Comentarios WHERE id_produto = @id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id}
            };
            DataTable dados = devolveConsulta(sql, parametros);
            return dados;
        }
        #endregion
        public void compra(string produtos,int id_utilizador)
        {
            /*HttpContext context = HttpContext.Current;
            context.Session["produtos"] = "";*/
            string produtosSemBug = produtos.Remove(produtos.Length - 1);
            string[] produtosArray = produtosSemBug.Split(',');

            foreach (var item in produtosArray)
            {
                remove1Produto(int.Parse(item));
            }
            string sql = "INSERT INTO Compras(id_utilizador,produtos) VALUES (@id_utilizador,@produtos)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id_utilizador",SqlDbType=SqlDbType.Int,Value=id_utilizador },
                new SqlParameter() {ParameterName="@produtos",SqlDbType=SqlDbType.NVarChar,Value=produtosSemBug }
            };
            executaComando(sql, parametros);


        }
        public int executaScalar(string sql)
        {
            int valor = -1;
            try
            {
                SqlCommand comando = new SqlCommand(sql, ligacaoBD);
                valor = (int)comando.ExecuteScalar();
                comando.Dispose();
            }
            catch (Exception erro)
            {
                Console.Write(erro.Message);
                return valor;
            }
            return valor;
        }
        public int executaScalar(string sql, List<SqlParameter> parametros)
        {
            int valor = -1;
            try
            {
                SqlCommand comando = new SqlCommand(sql, ligacaoBD);
                comando.Parameters.AddRange(parametros.ToArray());
                valor = (int)comando.ExecuteScalar();
                comando.Dispose();
            }
            catch (Exception erro)
            {
                Console.Write(erro.Message);
                return valor;
            }
            return valor;
        }
        public void remove1Produto(int id)
        {
            DataTable dados = devolveDadosProduto(id);

            int quantidade = int.Parse(dados.Rows[0]["quantidade"].ToString()) - 1;
            string sql = "UPDATE Produtos SET quantidade=@quantidade WHERE id=@id";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value= id},
                new SqlParameter() {ParameterName="@quantidade",SqlDbType=SqlDbType.Int,Value= quantidade}
            };
            executaComando(sql, parametros);
        }
        public DataTable getProdutosCarrinho(int id)
        {
            string sql = "SELECT * FROM Produtos WHERE id = @id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id}
            };
            DataTable dados = devolveConsulta(sql, parametros);
            return dados;

        }
        #region marca
        public bool addMarca(string nome)
        {
            string sql = "INSERT INTO Marcas(nome) VALUES (@nome)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.VarChar,Value=nome }
            };
            return executaComando(sql, parametros);
        }
        public void apagaMarca(int id)
        {
            
            string sql = "DELETE FROM Marcas WHERE idMarca=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            executaComando(sql, parametros);
        }
        public void atualizarMarca(int id, string nome)
        {
            string sql = @"UPDATE Marcas SET nome=@nome WHERE IdMarca=@id";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.NVarChar,Value=nome },
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id },
            };
            executaComando(sql, parametros);
        }
        #endregion
        #region utilizadores


        public bool registarUtilizador(string email, string username, string morada, string nif, string password, int perfil)
        {
            string sql = "INSERT INTO utilizadores(email,username,morada,nif,password,estado,perfil) ";
            sql += "VALUES (@email,@username,@morada,@nif,HASHBYTES('SHA2_512',@password),@estado,@perfil)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@email",SqlDbType=SqlDbType.VarChar,Value=email },
                new SqlParameter() {ParameterName="@username",SqlDbType=SqlDbType.VarChar,Value=username },
                new SqlParameter() {ParameterName="@morada",SqlDbType=SqlDbType.VarChar,Value=morada },
                new SqlParameter() {ParameterName="@nif",SqlDbType=SqlDbType.VarChar,Value=nif },
                new SqlParameter() {ParameterName="@password",SqlDbType=SqlDbType.VarChar,Value=password },
                new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Int,Value=0 },
                new SqlParameter() {ParameterName="perfil",SqlDbType=SqlDbType.Int,Value=perfil },
            };
            return executaComando(sql, parametros);
        }
        public bool registarUtilizador(string email, string username, string morada, string nif, string password, int estado, int perfil)
        {
            string sql = "INSERT INTO utilizadores(email,username,morada,nif,password,estado,perfil) ";
            sql += "VALUES (@email,@username,@morada,@nif,HASHBYTES('SHA2_512',@password),@estado,@perfil)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@email",SqlDbType=SqlDbType.VarChar,Value=email },
                new SqlParameter() {ParameterName="@username",SqlDbType=SqlDbType.VarChar,Value=username },
                new SqlParameter() {ParameterName="@morada",SqlDbType=SqlDbType.VarChar,Value=morada },
                new SqlParameter() {ParameterName="@nif",SqlDbType=SqlDbType.VarChar,Value=nif },
                new SqlParameter() {ParameterName="@password",SqlDbType=SqlDbType.VarChar,Value=password },
                new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Int,Value=estado },
                new SqlParameter() {ParameterName="perfil",SqlDbType=SqlDbType.Int,Value=perfil },
            };
            return executaComando(sql, parametros);
        }
        public void atualizarUtilizador(int id, string username, string email, string morada, string nif)
        {
            string sql = @"UPDATE utilizadores SET email=@email,username=@username,morada=@morada,nif=@nif 
                            WHERE id=@id";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@email",SqlDbType=SqlDbType.VarChar,Value=email },
                new SqlParameter() {ParameterName="@username",SqlDbType=SqlDbType.VarChar,Value=username },
                new SqlParameter() {ParameterName="@morada",SqlDbType=SqlDbType.VarChar,Value=morada },
                new SqlParameter() {ParameterName="@nif",SqlDbType=SqlDbType.VarChar,Value=nif },
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id },
            };
            executaComando(sql, parametros);
        }
        public DataTable devolveDadosUtilizador(int id)
        {
            string sql = "SELECT * FROM utilizadores WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            DataTable dados = devolveConsulta(sql, parametros);
            return dados;
        }
        public int estadoUtilizador(int id)
        {
            string sql = "SELECT estado FROM utilizadores WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            DataTable dados = devolveConsulta(sql, parametros);
            return int.Parse(dados.Rows[0][0].ToString());
        }
        public void ativarDesativarUtilizador(int id)
        {
            int estado = estadoUtilizador(id);
            if (estado == 0) estado = 1;
            else estado = 0;
            string sql = "UPDATE utilizadores SET estado = @estado WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Bit,Value=estado },
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            executaComando(sql, parametros);
        }
        public DataTable verificarLogin(string email, string password)
        {
            string sql = "SELECT * FROM Utilizadores WHERE email=@email AND ";
            sql += "password=HASHBYTES('SHA2_512',@password) AND estado=1";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@email",SqlDbType=SqlDbType.NVarChar,Value=email },
                new SqlParameter() {ParameterName="@password",SqlDbType=SqlDbType.NVarChar,Value=password }
            };
            DataTable utilizador = devolveConsulta(sql, parametros);
            if (utilizador == null || utilizador.Rows.Count == 0)
                return null;
            string id = utilizador.Rows[0]["id"].ToString();
            executaComando(sql);
            return utilizador;
        }

        public DataTable listaUtilizadoresDisponiveis()
        {
            string sql = "SELECT id, email,username, morada, nif,  estado, perfil FROM utilizadores where perfil=1";
            return devolveConsulta(sql);
        }
        public DataTable listaMarcas()
        {
            string sql = "SELECT * FROM Marcas";
            return devolveConsulta(sql);
        }
        public DataTable listaMarcas(int id)
        {
            string sql = "SELECT * FROM Marcas WHERE IdMarca=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            DataTable dados = devolveConsulta(sql, parametros);
            return dados;
        }
        public DataTable listaTodosUtilizadores()
        {
            string sql = "SELECT id, email,username, morada, nif,  estado, perfil FROM utilizadores";
            return devolveConsulta(sql);
        }
        public bool removerUtilizador(int id)
        {
            string sql = "DELETE FROM Utilizadores WHERE id=@id";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value= id},
            };
            return executaComando(sql, parametros);
        }
        public void recuperarPassword(string email, string guid)
        {
            string sql = "UPDATE Utilizadores set linkRecuperar=@lnk WHERE email=@email";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@email",SqlDbType=SqlDbType.NVarChar,Value=email },
                new SqlParameter() {ParameterName="@lnk",SqlDbType=SqlDbType.NVarChar,Value=guid },
            };
            executaComando(sql, parametros);
        }
        public void atualizarPassword(string guid, string password)
        {
            string sql = "UPDATE Utilizadores set password=HASHBYTES('SHA2_512',@password),estado=1,linkRecuperar=null WHERE linkRecuperar=@lnk";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@password",SqlDbType=SqlDbType.NVarChar,Value=password},
                new SqlParameter() {ParameterName="@lnk",SqlDbType=SqlDbType.NVarChar,Value=guid },
            };
            executaComando(sql, parametros);
        }
        public DataTable devolveDadosUtilizador(string email)
        {
            string sql = "SELECT * FROM utilizadores WHERE email=@email";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@email",SqlDbType=SqlDbType.VarChar,Value=email }
            };
            DataTable dados = devolveConsulta(sql, parametros);
            return dados;
        }
        #endregion

        #region livros
        public DataTable pesquisaProdutos(String tipo)
        {

            string sql = "SELECT * FROM Produtos WHERE tipo like @tipo";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@tipo",SqlDbType=SqlDbType.NVarChar,Value="%"+tipo+"%"}
            };
            return devolveConsulta(sql, parametros);
        }
        public DataTable pesquisaProdutosPorNome(String nome)
        {

            string sql = "SELECT * FROM Produtos WHERE nome like @nome";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.NVarChar,Value="%"+nome+"%"}
            };
            return devolveConsulta(sql, parametros);
        }
        public DataTable listaProdutos()
        {
            string sql = "SELECT * FROM Produtos";
            return devolveConsulta(sql);
        }
        public DataTable listaCompras(int id)
        {
            string sql = "SELECT * FROM Compras WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id}
            };
            return devolveConsulta(sql,parametros);
        }
        public int getNComentarios(int id)
        {
            string sql = "SELECT comentario FROM Produtos WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter(){ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id}
            };
            DataTable dados = devolveConsulta(sql, parametros);

            string comentarios = dados.Rows[0]["comentario"].ToString();
            var _element = JObject.Parse(comentarios);
            var array = JArray.Parse(_element["Comentario"].ToString());
            return array.Count;
        }
        public JArray getComentarios(int id)
        {
            try
            {


                string sql = "SELECT comentario FROM Produtos WHERE id=@id";
                List<SqlParameter> parametros = new List<SqlParameter>()
                {
                    new SqlParameter(){ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id}
                };

                DataTable dados = devolveConsulta(sql, parametros);

                string comentarios = dados.Rows[0]["comentario"].ToString();
                var _element = JObject.Parse(comentarios);
                return JArray.Parse(_element["Comentario"].ToString());


            }
            catch (Exception)
            {

                throw;
            }

        }
        public DataTable devolveDadosProduto(int id)
        {
            string sql = "SELECT * FROM Produtos WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            return devolveConsulta(sql, parametros);
        }
        public DataTable listaLivrosComPrecoInferior(int nlivro)
        {
            string sql = "SELECT * FROM LIVROS where preco<=(select preco from livros where nlivro=@nlivro)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nlivro",SqlDbType=SqlDbType.Int,Value=nlivro }
            };
            return devolveConsulta(sql, parametros);
        }
        public int adicionarProduto(string nome, decimal preco, string descricao, string marca, string tipo)
        {
            string sql = "INSERT INTO Produtos (nome,preco,descricao,marca,quantidade,estado,tipo) VALUES ";
            sql += "(@nome,@preco,@descricao,@marca,@quantidade,@estado,@tipo);SELECT CAST(SCOPE_IDENTITY() AS INT);";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.NVarChar,Value= nome},
                new SqlParameter() {ParameterName="@preco",SqlDbType=SqlDbType.Decimal,Value= preco},
                new SqlParameter() {ParameterName="@descricao",SqlDbType=SqlDbType.NVarChar,Value= descricao},
                new SqlParameter() {ParameterName="@marca",SqlDbType=SqlDbType.NVarChar,Value= marca},
                new SqlParameter() {ParameterName="@quantidade",SqlDbType=SqlDbType.Int,Value=1},
                new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Bit,Value=1},
                new SqlParameter() {ParameterName="@tipo",SqlDbType=SqlDbType.NVarChar,Value=tipo}
            };
            SqlCommand comando = new SqlCommand(sql, ligacaoBD);
            comando.Parameters.AddRange(parametros.ToArray());
            int id = (int)comando.ExecuteScalar();
            comando.Dispose();
            return id;
        }

        public void removerProduto(int id)
        {
            string sql = "DELETE FROM Produtos WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            executaComando(sql, parametros);
        }
        public void atualizaProduto(int id, string nome, int quantidade, decimal preco, string marca, string descricao,string tipo)
        {
            string sql = "UPDATE Produtos SET marca=@marca,nome=@nome,quantidade=@quantidade,preco=@preco,descricao=@descricao,tipo=@tipo WHERE id=@id;";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.NVarChar,Value= nome},
                new SqlParameter() {ParameterName="@marca",SqlDbType=SqlDbType.NVarChar,Value= marca},
                new SqlParameter() {ParameterName="@quantidade",SqlDbType=SqlDbType.Int,Value= quantidade},
                new SqlParameter() {ParameterName="@preco",SqlDbType=SqlDbType.Decimal,Value= preco},
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id},
                new SqlParameter() {ParameterName="@tipo",SqlDbType=SqlDbType.NVarChar,Value= tipo},
                new SqlParameter() {ParameterName="@descricao",SqlDbType=SqlDbType.NVarChar,Value=descricao}
            };
            executaComando(sql, parametros);
        }
        public void addProdutoQt(int id)
        {
            DataTable dados = devolveDadosProduto(id);

            int quantidade = int.Parse(dados.Rows[0]["quantidade"].ToString()) + 1; 
            string sql = "UPDATE Produtos SET quantidade=@quantidade WHERE id=@id";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value= id},
                new SqlParameter() {ParameterName="@quantidade",SqlDbType=SqlDbType.Int,Value= quantidade}
            };
            executaComando(sql, parametros);
        }
        #endregion
        #region empréstimos
        public DataTable listaEmprestimosPorConcluir()
        {
            string sql = "SELECT * FROM Emprestimos where estado=1";
            return devolveConsulta(sql);
        }
        public DataTable listaTodosEmprestimos()
        {
            string sql = "SELECT * FROM Emprestimos";
            return devolveConsulta(sql);
        }
        public DataTable listaTodosEmprestimosComNomes()
        {
            string sql = @"SELECT nemprestimo,livros.nome as nomeLivro,utilizadores.nome as nomeLeitor,data_emprestimo,data_devolve,emprestimos.estado
                        FROM Emprestimos inner join livros on emprestimos.nlivro=livros.nlivro
                        inner join utilizadores on emprestimos.idutilizador=utilizadores.id";
            return devolveConsulta(sql);
        }
        public DataTable listaTodosEmprestimosPorConcluirComNomes()
        {
            string sql = @"SELECT nemprestimo,livros.nome as nomeLivro,utilizadores.nome as nomeLeitor,data_emprestimo,data_devolve,emprestimos.estado
                        FROM Emprestimos inner join livros on emprestimos.nlivro=livros.nlivro
                        inner join utilizadores on emprestimos.idutilizador=utilizadores.id where emprestimos.estado=1";
            return devolveConsulta(sql);
        }
        public DataTable listaTodosEmprestimos(int nleitor)
        {
            string sql = "SELECT * FROM Emprestimos Where idutilizador=@idutilizador";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@idutilizador",SqlDbType=SqlDbType.Int,Value=nleitor }
            };
            return devolveConsulta(sql, parametros);
        }
        public DataTable listaTodosEmprestimosComNomes(int nleitor)
        {
            string sql = @"SELECT nemprestimo,livros.nome as nomeLivro,utilizadores.nome as nomeLeitor,data_emprestimo,data_devolve,emprestimos.estado
                        FROM Emprestimos inner join livros on emprestimos.nlivro=livros.nlivro
                        inner join utilizadores on emprestimos.idutilizador=utilizadores.id Where idutilizador=@idutilizador";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@idutilizador",SqlDbType=SqlDbType.Int,Value=nleitor }
            };
            return devolveConsulta(sql, parametros);
        }

        public DataTable listaEmprestimosPorConcluir(int nleitor)
        {
            string sql = "SELECT * FROM Emprestimos Where idutilizador=@idutilizador and estado=0";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@idutilizador",SqlDbType=SqlDbType.Int,Value=nleitor }
            };
            return devolveConsulta(sql, parametros);
        }
        public DataTable listaTodosEmprestimosPorConcluirComNomes(int nleitor)
        {
            string sql = @"SELECT nemprestimo,livros.nome as nomeLivro,utilizadores.nome as nomeLeitor,data_emprestimo,data_devolve,emprestimos.estado
                        FROM Emprestimos inner join livros on emprestimos.nlivro=livros.nlivro
                        inner join utilizadores on emprestimos.idutilizador=utilizadores.id where idutilizador=@idutilizador and emprestimos.estado=1";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@idutilizador",SqlDbType=SqlDbType.Int,Value=nleitor }
            };
            return devolveConsulta(sql, parametros);
        }
        public DataTable listaEmprestimosAtrasados()
        {
            string sql = "SELECT * FROM Emprestimos where data_devolve<getdate()";
            return devolveConsulta(sql);
        }
        public void adicionarEmprestimo(int nlivro, int nleitor, DateTime dataDevolve)
        {
            string sql = "SELECT * FROM livros WHERE nlivro=@nlivro";
            List<SqlParameter> parametrosBloquear = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nlivro",SqlDbType=SqlDbType.Int,Value=nlivro }
            };
            //iniciar transação
            SqlTransaction transacao = ligacaoBD.BeginTransaction(IsolationLevel.Serializable);
            DataTable dados = devolveConsulta(sql, parametrosBloquear, transacao);

            try
            {
                //alterar estado do livro
                sql = "UPDATE Livros SET estado=@estado WHERE nlivro=@nlivro";
                List<SqlParameter> parametrosUpdate = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName="@nlivro",SqlDbType=SqlDbType.Int,Value=nlivro },
                    new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Int,Value=0 },
                };
                executaComando(sql, parametrosUpdate, transacao);
                //registar empréstimo
                sql = @"INSERT INTO Emprestimos(nlivro,idutilizador,data_emprestimo,data_devolve,estado) 
                            VALUES (@nlivro,@idutilizador,@data_emprestimo,@data_devolve,@estado)";
                List<SqlParameter> parametrosInsert = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName="@nlivro",SqlDbType=SqlDbType.Int,Value=nlivro },
                    new SqlParameter() {ParameterName="@idutilizador",SqlDbType=SqlDbType.Int,Value=nleitor },
                    new SqlParameter() {ParameterName="@data_emprestimo",SqlDbType=SqlDbType.Date,Value=DateTime.Now.Date},
                    new SqlParameter() {ParameterName="@data_devolve",SqlDbType=SqlDbType.Date,Value=dataDevolve },
                    new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Int,Value=1 },
                };
                executaComando(sql, parametrosInsert, transacao);
                //concluir transação
                transacao.Commit();
            }
            catch (Exception e)
            {
                transacao.Rollback();
            }
            dados.Dispose();
        }
        /// <summary>
        /// Termina um empréstimo e altera o estado do livro
        /// </summary>
        public void concluirEmprestimo(int nemprestimo)
        {
            DataTable dadosEmprestimo = devolveDadosEmprestimo(nemprestimo);
            int nlivro = int.Parse(dadosEmprestimo.Rows[0]["nlivro"].ToString());
            string sql = "SELECT * FROM livros WHERE nlivro=@nlivro";
            List<SqlParameter> parametrosBloquear = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nlivro",SqlDbType=SqlDbType.Int,Value=nlivro }
            };
            //iniciar transação
            SqlTransaction transacao = ligacaoBD.BeginTransaction(IsolationLevel.Serializable);
            DataTable dados = devolveConsulta(sql, parametrosBloquear, transacao);

            try
            {
                //alterar estado do livro
                sql = "UPDATE Livros SET estado=@estado WHERE nlivro=@nlivro";
                List<SqlParameter> parametrosUpdate = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName="@nlivro",SqlDbType=SqlDbType.Int,Value=nlivro },
                    new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Int,Value=1 },
                };
                executaComando(sql, parametrosUpdate, transacao);
                //terminar empréstimo
                sql = @"UPDATE Emprestimos SET estado=@estado WHERE nemprestimo=@nemprestimo";
                List<SqlParameter> parametrosInsert = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName="@nemprestimo",SqlDbType=SqlDbType.Int,Value=nemprestimo },
                    new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Int,Value=0 },
                };
                executaComando(sql, parametrosInsert, transacao);
                //concluir transação
                transacao.Commit();
            }
            catch (Exception e)
            {
                transacao.Rollback();
            }
            dados.Dispose();
        }
        public DataTable devolveDadosEmprestimo(int nemprestimo)
        {
            string sql = "SELECT * FROM emprestimos WHERE nemprestimo=@nemprestimo";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nemprestimo",SqlDbType=SqlDbType.Int,Value=nemprestimo }
            };
            return devolveConsulta(sql, parametros);
        }
        #endregion
    }
}