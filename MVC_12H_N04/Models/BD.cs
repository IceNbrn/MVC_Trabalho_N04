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
    public class Bd
    {
        private static Bd _instance;
        public static Bd Instance {
            get {
                if (_instance == null)
                    _instance = new Bd();
                return _instance;
            }
        }
        private string _strLigacao;
        private SqlConnection _ligacaoBd;
        public Bd()
        {
            //ligação à bd
            _strLigacao = ConfigurationManager.ConnectionStrings["sql"].ToString();
            _ligacaoBd = new SqlConnection(_strLigacao);
            _ligacaoBd.Open();
        }
        ~Bd()
        {
            try
            {
                _ligacaoBd.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #region Funções genéricas
        //devolve consulta
        public DataTable DevolveConsulta(string sql)
        {
            SqlCommand comando = new SqlCommand(sql, _ligacaoBd);
            DataTable registos = new DataTable();
            SqlDataReader dados = comando.ExecuteReader();
            registos.Load(dados);
            registos.Dispose();
            comando.Dispose();
            return registos;
        }
        public DataTable DevolveConsulta(string sql, List<SqlParameter> parametros)
        {
            SqlCommand comando = new SqlCommand(sql, _ligacaoBd);
            DataTable registos = new DataTable();
            comando.Parameters.AddRange(parametros.ToArray());
            SqlDataReader dados = comando.ExecuteReader();
            registos.Load(dados);
            registos.Dispose();
            comando.Dispose();
            return registos;
        }


        public DataTable DevolveConsulta(string sql, List<SqlParameter> parametros, SqlTransaction transacao)
        {
            SqlCommand comando = new SqlCommand(sql, _ligacaoBd);
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
        public bool ExecutaComando(string sql)
        {
            try
            {
                SqlCommand comando = new SqlCommand(sql, _ligacaoBd);
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
        public bool ExecutaComando(string sql, List<SqlParameter> parametros)
        {
            try
            {
                SqlCommand comando = new SqlCommand(sql, _ligacaoBd);
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
        public bool ExecutaComando(string sql, List<SqlParameter> parametros, SqlTransaction transacao)
        {
            try
            {
                SqlCommand comando = new SqlCommand(sql, _ligacaoBd);
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
        public bool AdicionaComentario(string comentario, string autor, int idProduto)
        {

            string sql = "INSERT INTO Comentarios(id_produto,autor,comentario)";
            sql += "VALUES (@id_produto,@autor,@comentario)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id_produto",SqlDbType=SqlDbType.Int,Value=idProduto},
                new SqlParameter() {ParameterName="@autor",SqlDbType=SqlDbType.NVarChar,Value=autor},
                new SqlParameter() {ParameterName="@comentario",SqlDbType=SqlDbType.NVarChar,Value=comentario}
            };
            return ExecutaComando(sql, parametros);
        }
        public DataTable ListaComentario(int id)
        {
            string sql = "SELECT * FROM Comentarios WHERE id_produto = @id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id}
            };
            DataTable dados = DevolveConsulta(sql, parametros);
            return dados;
        }
        #endregion
        public void Compra(string produtos,int idUtilizador)
        {
            /*HttpContext context = HttpContext.Current;
            context.Session["produtos"] = "";*/
            string produtosSemBug = produtos.Remove(produtos.Length - 1);
            string[] produtosArray = produtosSemBug.Split(',');

            foreach (var item in produtosArray)
            {
                Remove1Produto(int.Parse(item));
            }
            string sql = "INSERT INTO Compras(id_utilizador,produtos) VALUES (@id_utilizador,@produtos)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id_utilizador",SqlDbType=SqlDbType.Int,Value=idUtilizador },
                new SqlParameter() {ParameterName="@produtos",SqlDbType=SqlDbType.NVarChar,Value=produtosSemBug }
            };
            ExecutaComando(sql, parametros);


        }
        public int ExecutaScalar(string sql)
        {
            int valor = -1;
            try
            {
                SqlCommand comando = new SqlCommand(sql, _ligacaoBd);
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
        public int ExecutaScalar(string sql, List<SqlParameter> parametros)
        {
            int valor = -1;
            try
            {
                SqlCommand comando = new SqlCommand(sql, _ligacaoBd);
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
        public void Remove1Produto(int id)
        {
            DataTable dados = DevolveDadosProduto(id);

            int quantidade = int.Parse(dados.Rows[0]["quantidade"].ToString()) - 1;
            string sql = "UPDATE Produtos SET quantidade=@quantidade WHERE id=@id";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value= id},
                new SqlParameter() {ParameterName="@quantidade",SqlDbType=SqlDbType.Int,Value= quantidade}
            };
            ExecutaComando(sql, parametros);
        }
        public DataTable GetProdutosCarrinho(int id)
        {
            string sql = "SELECT * FROM Produtos WHERE id = @id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id}
            };
            DataTable dados = DevolveConsulta(sql, parametros);
            return dados;

        }
        #region marca
        public bool AddMarca(string nome)
        {
            string sql = "INSERT INTO Marcas(nome) VALUES (@nome)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.VarChar,Value=nome }
            };
            return ExecutaComando(sql, parametros);
        }
        public void ApagaMarca(int id)
        {
            
            string sql = "DELETE FROM Marcas WHERE idMarca=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            ExecutaComando(sql, parametros);
        }
        public void AtualizarMarca(int id, string nome)
        {
            string sql = @"UPDATE Marcas SET nome=@nome WHERE IdMarca=@id";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.NVarChar,Value=nome },
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id },
            };
            ExecutaComando(sql, parametros);
        }
        #endregion
        #region utilizadores


        public bool RegistarUtilizador(string email, string username, string morada, string nif, string password, int perfil)
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
            return ExecutaComando(sql, parametros);
        }
        public bool RegistarUtilizador(string email, string username, string morada, string nif, string password, int estado, int perfil)
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
            return ExecutaComando(sql, parametros);
        }
        public void AtualizarUtilizador(int id, string username, string email, string morada, string nif)
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
            ExecutaComando(sql, parametros);
        }
        public DataTable DevolveDadosUtilizador(int id)
        {
            string sql = "SELECT * FROM utilizadores WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            DataTable dados = DevolveConsulta(sql, parametros);
            return dados;
        }
        public int EstadoUtilizador(int id)
        {
            string sql = "SELECT estado FROM utilizadores WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            DataTable dados = DevolveConsulta(sql, parametros);
            return int.Parse(dados.Rows[0][0].ToString());
        }
        public void AtivarDesativarUtilizador(int id)
        {
            int estado = EstadoUtilizador(id);
            if (estado == 0) estado = 1;
            else estado = 0;
            string sql = "UPDATE utilizadores SET estado = @estado WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Bit,Value=estado },
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            ExecutaComando(sql, parametros);
        }
        public DataTable VerificarLogin(string email, string password)
        {
            string sql = "SELECT * FROM Utilizadores WHERE email=@email AND ";
            sql += "password=HASHBYTES('SHA2_512',@password) AND estado=1";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@email",SqlDbType=SqlDbType.NVarChar,Value=email },
                new SqlParameter() {ParameterName="@password",SqlDbType=SqlDbType.NVarChar,Value=password }
            };
            DataTable utilizador = DevolveConsulta(sql, parametros);
            if (utilizador == null || utilizador.Rows.Count == 0)
                return null;
            string id = utilizador.Rows[0]["id"].ToString();
            ExecutaComando(sql);
            return utilizador;
        }

        public DataTable ListaUtilizadoresDisponiveis()
        {
            string sql = "SELECT id, email,username, morada, nif,  estado, perfil FROM utilizadores where perfil=1";
            return DevolveConsulta(sql);
        }
        public DataTable ListaMarcas()
        {
            string sql = "SELECT * FROM Marcas";
            return DevolveConsulta(sql);
        }
        public DataTable ListaMarcas(int id)
        {
            string sql = "SELECT * FROM Marcas WHERE IdMarca=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            DataTable dados = DevolveConsulta(sql, parametros);
            return dados;
        }
        public DataTable ListaTodosUtilizadores()
        {
            string sql = "SELECT id, email,username, morada, nif,  estado, perfil FROM utilizadores";
            return DevolveConsulta(sql);
        }
        public bool RemoverUtilizador(int id)
        {
            string sql = "DELETE FROM Utilizadores WHERE id=@id";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value= id},
            };
            return ExecutaComando(sql, parametros);
        }
        public void RecuperarPassword(string email, string guid)
        {
            string sql = "UPDATE Utilizadores set linkRecuperar=@lnk WHERE email=@email";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@email",SqlDbType=SqlDbType.NVarChar,Value=email },
                new SqlParameter() {ParameterName="@lnk",SqlDbType=SqlDbType.NVarChar,Value=guid },
            };
            ExecutaComando(sql, parametros);
        }
        public void AtualizarPassword(string guid, string password)
        {
            string sql = "UPDATE Utilizadores set password=HASHBYTES('SHA2_512',@password),estado=1,linkRecuperar=null WHERE linkRecuperar=@lnk";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@password",SqlDbType=SqlDbType.NVarChar,Value=password},
                new SqlParameter() {ParameterName="@lnk",SqlDbType=SqlDbType.NVarChar,Value=guid },
            };
            ExecutaComando(sql, parametros);
        }
        public DataTable DevolveDadosUtilizador(string email)
        {
            string sql = "SELECT * FROM utilizadores WHERE email=@email";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@email",SqlDbType=SqlDbType.VarChar,Value=email }
            };
            DataTable dados = DevolveConsulta(sql, parametros);
            return dados;
        }
        #endregion

        #region livros
        public DataTable PesquisaProdutos(String tipo)
        {

            string sql = "SELECT * FROM Produtos WHERE tipo like @tipo";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@tipo",SqlDbType=SqlDbType.NVarChar,Value="%"+tipo+"%"}
            };
            return DevolveConsulta(sql, parametros);
        }
        public DataTable PesquisaProdutosPorNome(String nome)
        {

            string sql = "SELECT * FROM Produtos WHERE nome like @nome";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nome",SqlDbType=SqlDbType.NVarChar,Value="%"+nome+"%"}
            };
            return DevolveConsulta(sql, parametros);
        }
        public DataTable ListaProdutos()
        {
            string sql = "SELECT * FROM Produtos";
            return DevolveConsulta(sql);
        }
        public DataTable ListaCompras(int id)
        {
            string sql = "SELECT * FROM Compras WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id}
            };
            return DevolveConsulta(sql,parametros);
        }
        public int GetNComentarios(int id)
        {
            string sql = "SELECT comentario FROM Produtos WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter(){ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id}
            };
            DataTable dados = DevolveConsulta(sql, parametros);

            string comentarios = dados.Rows[0]["comentario"].ToString();
            var element = JObject.Parse(comentarios);
            var array = JArray.Parse(element["Comentario"].ToString());
            return array.Count;
        }
        public JArray GetComentarios(int id)
        {
            try
            {


                string sql = "SELECT comentario FROM Produtos WHERE id=@id";
                List<SqlParameter> parametros = new List<SqlParameter>()
                {
                    new SqlParameter(){ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id}
                };

                DataTable dados = DevolveConsulta(sql, parametros);

                string comentarios = dados.Rows[0]["comentario"].ToString();
                var element = JObject.Parse(comentarios);
                return JArray.Parse(element["Comentario"].ToString());


            }
            catch (Exception)
            {

                throw;
            }

        }
        public DataTable DevolveDadosProduto(int id)
        {
            string sql = "SELECT * FROM Produtos WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            return DevolveConsulta(sql, parametros);
        }
        public DataTable ListaLivrosComPrecoInferior(int nlivro)
        {
            string sql = "SELECT * FROM LIVROS where preco<=(select preco from livros where nlivro=@nlivro)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nlivro",SqlDbType=SqlDbType.Int,Value=nlivro }
            };
            return DevolveConsulta(sql, parametros);
        }
        public int AdicionarProduto(string nome, decimal preco, string descricao, string marca, string tipo)
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
            SqlCommand comando = new SqlCommand(sql, _ligacaoBd);
            comando.Parameters.AddRange(parametros.ToArray());
            int id = (int)comando.ExecuteScalar();
            comando.Dispose();
            return id;
        }

        public void RemoverProduto(int id)
        {
            string sql = "DELETE FROM Produtos WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value=id }
            };
            ExecutaComando(sql, parametros);
        }
        public void AtualizaProduto(int id, string nome, int quantidade, decimal preco, string marca, string descricao,string tipo)
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
            ExecutaComando(sql, parametros);
        }
        public void AddProdutoQt(int id)
        {
            DataTable dados = DevolveDadosProduto(id);

            int quantidade = int.Parse(dados.Rows[0]["quantidade"].ToString()) + 1; 
            string sql = "UPDATE Produtos SET quantidade=@quantidade WHERE id=@id";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@id",SqlDbType=SqlDbType.Int,Value= id},
                new SqlParameter() {ParameterName="@quantidade",SqlDbType=SqlDbType.Int,Value= quantidade}
            };
            ExecutaComando(sql, parametros);
        }
        #endregion
        #region empréstimos
        public DataTable ListaEmprestimosPorConcluir()
        {
            string sql = "SELECT * FROM Emprestimos where estado=1";
            return DevolveConsulta(sql);
        }
        public DataTable ListaTodosEmprestimos()
        {
            string sql = "SELECT * FROM Emprestimos";
            return DevolveConsulta(sql);
        }
        public DataTable ListaTodosEmprestimosComNomes()
        {
            string sql = @"SELECT nemprestimo,livros.nome as nomeLivro,utilizadores.nome as nomeLeitor,data_emprestimo,data_devolve,emprestimos.estado
                        FROM Emprestimos inner join livros on emprestimos.nlivro=livros.nlivro
                        inner join utilizadores on emprestimos.idutilizador=utilizadores.id";
            return DevolveConsulta(sql);
        }
        public DataTable ListaTodosEmprestimosPorConcluirComNomes()
        {
            string sql = @"SELECT nemprestimo,livros.nome as nomeLivro,utilizadores.nome as nomeLeitor,data_emprestimo,data_devolve,emprestimos.estado
                        FROM Emprestimos inner join livros on emprestimos.nlivro=livros.nlivro
                        inner join utilizadores on emprestimos.idutilizador=utilizadores.id where emprestimos.estado=1";
            return DevolveConsulta(sql);
        }
        public DataTable ListaTodosEmprestimos(int nleitor)
        {
            string sql = "SELECT * FROM Emprestimos Where idutilizador=@idutilizador";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@idutilizador",SqlDbType=SqlDbType.Int,Value=nleitor }
            };
            return DevolveConsulta(sql, parametros);
        }
        public DataTable ListaTodosEmprestimosComNomes(int nleitor)
        {
            string sql = @"SELECT nemprestimo,livros.nome as nomeLivro,utilizadores.nome as nomeLeitor,data_emprestimo,data_devolve,emprestimos.estado
                        FROM Emprestimos inner join livros on emprestimos.nlivro=livros.nlivro
                        inner join utilizadores on emprestimos.idutilizador=utilizadores.id Where idutilizador=@idutilizador";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@idutilizador",SqlDbType=SqlDbType.Int,Value=nleitor }
            };
            return DevolveConsulta(sql, parametros);
        }

        public DataTable ListaEmprestimosPorConcluir(int nleitor)
        {
            string sql = "SELECT * FROM Emprestimos Where idutilizador=@idutilizador and estado=0";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@idutilizador",SqlDbType=SqlDbType.Int,Value=nleitor }
            };
            return DevolveConsulta(sql, parametros);
        }
        public DataTable ListaTodosEmprestimosPorConcluirComNomes(int nleitor)
        {
            string sql = @"SELECT nemprestimo,livros.nome as nomeLivro,utilizadores.nome as nomeLeitor,data_emprestimo,data_devolve,emprestimos.estado
                        FROM Emprestimos inner join livros on emprestimos.nlivro=livros.nlivro
                        inner join utilizadores on emprestimos.idutilizador=utilizadores.id where idutilizador=@idutilizador and emprestimos.estado=1";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@idutilizador",SqlDbType=SqlDbType.Int,Value=nleitor }
            };
            return DevolveConsulta(sql, parametros);
        }
        public DataTable ListaEmprestimosAtrasados()
        {
            string sql = "SELECT * FROM Emprestimos where data_devolve<getdate()";
            return DevolveConsulta(sql);
        }
        public void AdicionarEmprestimo(int nlivro, int nleitor, DateTime dataDevolve)
        {
            string sql = "SELECT * FROM livros WHERE nlivro=@nlivro";
            List<SqlParameter> parametrosBloquear = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nlivro",SqlDbType=SqlDbType.Int,Value=nlivro }
            };
            //iniciar transação
            SqlTransaction transacao = _ligacaoBd.BeginTransaction(IsolationLevel.Serializable);
            DataTable dados = DevolveConsulta(sql, parametrosBloquear, transacao);

            try
            {
                //alterar estado do livro
                sql = "UPDATE Livros SET estado=@estado WHERE nlivro=@nlivro";
                List<SqlParameter> parametrosUpdate = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName="@nlivro",SqlDbType=SqlDbType.Int,Value=nlivro },
                    new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Int,Value=0 },
                };
                ExecutaComando(sql, parametrosUpdate, transacao);
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
                ExecutaComando(sql, parametrosInsert, transacao);
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
        public void ConcluirEmprestimo(int nemprestimo)
        {
            DataTable dadosEmprestimo = DevolveDadosEmprestimo(nemprestimo);
            int nlivro = int.Parse(dadosEmprestimo.Rows[0]["nlivro"].ToString());
            string sql = "SELECT * FROM livros WHERE nlivro=@nlivro";
            List<SqlParameter> parametrosBloquear = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nlivro",SqlDbType=SqlDbType.Int,Value=nlivro }
            };
            //iniciar transação
            SqlTransaction transacao = _ligacaoBd.BeginTransaction(IsolationLevel.Serializable);
            DataTable dados = DevolveConsulta(sql, parametrosBloquear, transacao);

            try
            {
                //alterar estado do livro
                sql = "UPDATE Livros SET estado=@estado WHERE nlivro=@nlivro";
                List<SqlParameter> parametrosUpdate = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName="@nlivro",SqlDbType=SqlDbType.Int,Value=nlivro },
                    new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Int,Value=1 },
                };
                ExecutaComando(sql, parametrosUpdate, transacao);
                //terminar empréstimo
                sql = @"UPDATE Emprestimos SET estado=@estado WHERE nemprestimo=@nemprestimo";
                List<SqlParameter> parametrosInsert = new List<SqlParameter>()
                {
                    new SqlParameter() {ParameterName="@nemprestimo",SqlDbType=SqlDbType.Int,Value=nemprestimo },
                    new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.Int,Value=0 },
                };
                ExecutaComando(sql, parametrosInsert, transacao);
                //concluir transação
                transacao.Commit();
            }
            catch (Exception e)
            {
                transacao.Rollback();
            }
            dados.Dispose();
        }
        public DataTable DevolveDadosEmprestimo(int nemprestimo)
        {
            string sql = "SELECT * FROM emprestimos WHERE nemprestimo=@nemprestimo";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@nemprestimo",SqlDbType=SqlDbType.Int,Value=nemprestimo }
            };
            return DevolveConsulta(sql, parametros);
        }
        #endregion
    }
}