using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Data;

namespace MVC_12H_N04.Models
{
    public class UtilizadoresModel
    {

        [Required(ErrorMessage = "Campo Username tem de ser preenchido")]
        [StringLength(50)]
        [MinLength(2, ErrorMessage = "Username muito pequeno")]
        public string username { get; set; }
        
        [MinLength(5, ErrorMessage = "O email tem de ser preenchido")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Display(Name = "Palavra passe")]
        [MinLength(5, ErrorMessage = "Palavra passe muito pequena")]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirme a sua palavra passe")]
        [Compare("password", ErrorMessage = "Palavras passe não são iguais")]
        public string confirmaPassword { get; set; }

        [MinLength(5, ErrorMessage = "A morada tem de ser preenchido")]
        [StringLength(300)]
        public string morada { get; set; }

        [MinLength(5, ErrorMessage = "O nif tem de ser preenchido")]
        [StringLength(300)]
        public string nif { get; set; }

        public int perfil { get; set; }

        public bool estado { get; set; }
    }

    public class UtilizadoresBD
    {
        
        //create
        public void adicionarUtilizadores(UtilizadoresModel novo)
        {
            string sql = "INSERT INTO utilizadores(username,email,morada,nif,estado,perfil,password)";
            sql += "VALUES (@username,@email,@morada,@nif,@estado,@perfil,HASHBYTES('SHA2_512',@password))";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter(){ParameterName="@username",
                    SqlDbType =SqlDbType.NVarChar,Value=novo.username},
                new SqlParameter(){ParameterName="@password",
                    SqlDbType =SqlDbType.NVarChar,Value=novo.password},
                 new SqlParameter(){ParameterName="@perfil",
                    SqlDbType =SqlDbType.Int,Value=1},
                 new SqlParameter(){ParameterName="@estado",
                    SqlDbType =SqlDbType.Bit,Value=true},
                 new SqlParameter(){ParameterName="@nif",
                    SqlDbType =SqlDbType.Int,Value=novo.nif},
                 new SqlParameter(){ParameterName="@email",
                    SqlDbType =SqlDbType.NVarChar,Value=novo.email},
                 new SqlParameter(){ParameterName="@morada",
                    SqlDbType =SqlDbType.NVarChar,Value=novo.morada},
            };
            BD.Instance.executaComando(sql, parametros);
        }
        //read
        public List<UtilizadoresModel> lista()
        {
            string sql = "SELECT * FROM utilizadores";
            DataTable registos = BD.Instance.devolveConsulta(sql);
            List<UtilizadoresModel> lista = new List<UtilizadoresModel>();
            foreach (DataRow data in registos.Rows)
            {
                UtilizadoresModel novo = new UtilizadoresModel();
                novo.username = data[1].ToString();
                novo.password = data[8].ToString();
                novo.morada = data[3].ToString();
                novo.nif = data[4].ToString();
                novo.password = data[8].ToString();
                novo.perfil = int.Parse(data[6].ToString());
                novo.estado = bool.Parse(data[5].ToString());
                lista.Add(novo);
            }
            return lista;
        }
        public List<UtilizadoresModel> lista(string id)
        {
            string sql = "SELECT * FROM utilizadores WHERE id = @id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter(){ParameterName="@id",
                    SqlDbType =SqlDbType.Int,Value=id}
            };
            DataTable registos = BD.Instance.devolveConsulta(sql, parametros);
            List<UtilizadoresModel> lista = new List<UtilizadoresModel>();
            foreach (DataRow data in registos.Rows)
            {
                UtilizadoresModel novo = new UtilizadoresModel();
                novo.username = data[1].ToString();
                novo.password = data[8].ToString();
                novo.morada = data[3].ToString();
                novo.nif = data[4].ToString();
                novo.password = data[8].ToString();
                novo.perfil = int.Parse(data[6].ToString());
                novo.estado = bool.Parse(data[5].ToString());
                lista.Add(novo);
            }
            return lista;
        }
        //update
        public void editarUtilizador(UtilizadoresModel novo)
        {
            string sql = @"UPDATE utilizadores SET email=@email,username=@username,morada=@morada,nif=@nif,estado=@estado 
                            WHERE username=@username";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@email",SqlDbType=SqlDbType.VarChar,Value=novo.email },
                new SqlParameter() {ParameterName="@username",SqlDbType=SqlDbType.VarChar,Value=novo.username },
                new SqlParameter() {ParameterName="@morada",SqlDbType=SqlDbType.VarChar,Value=novo.morada },
                new SqlParameter() {ParameterName="@nif",SqlDbType=SqlDbType.VarChar,Value=novo.nif },
                new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.VarChar,Value=novo.estado },
            };
            BD.Instance.executaComando(sql, parametros);
        }
        //delete
        public void removerUtilizador(string id)
        {
            string sql = "DELETE FROM Utilizadores WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter(){ParameterName="@id",
                    SqlDbType =SqlDbType.Int,Value=id}
            };
            BD.Instance.executaComando(sql, parametros);
        }
    }
}