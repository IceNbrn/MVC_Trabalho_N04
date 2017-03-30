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
        public string Username { get; set; }

        [Required(ErrorMessage = "Campo Email tem de ser preenchido")]
        [MinLength(5, ErrorMessage = "O email tem de ser preenchido")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo Password tem de ser preenchido")]
        [Display(Name = "Palavra passe")]
        [MinLength(5, ErrorMessage = "Palavra passe muito pequena")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Campo Confirmar Password tem de ser preenchido")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirme a sua palavra passe")]
        [Compare("Password", ErrorMessage = "Palavras passe não são iguais")]
        public string ConfirmaPassword { get; set; }

        [MinLength(5, ErrorMessage = "A morada tem de ser preenchido")]
        [StringLength(300)]
        public string Morada { get; set; }

        [Required(ErrorMessage = "Campo Nif tem de ser preenchido")]
        [MinLength(5, ErrorMessage = "O nif tem de ser preenchido")]
        [StringLength(300)]
        public string Nif { get; set; }

        public int Perfil { get; set; }

        public bool Estado { get; set; }

        public string Id { get; set; }
    }

    public class UtilizadoresBd
    {
        
        //create
        public void AdicionarUtilizadores(UtilizadoresModel novo)
        {
            string sql = "INSERT INTO utilizadores(username,email,morada,nif,estado,perfil,password)";
            sql += "VALUES (@username,@email,@morada,@nif,@estado,@perfil,HASHBYTES('SHA2_512',@password))";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter(){ParameterName="@username",
                    SqlDbType =SqlDbType.NVarChar,Value=novo.Username},
                new SqlParameter(){ParameterName="@password",
                    SqlDbType =SqlDbType.NVarChar,Value=novo.Password},
                 new SqlParameter(){ParameterName="@perfil",
                    SqlDbType =SqlDbType.Int,Value=1},
                 new SqlParameter(){ParameterName="@estado",
                    SqlDbType =SqlDbType.Bit,Value=true},
                 new SqlParameter(){ParameterName="@nif",
                    SqlDbType =SqlDbType.Int,Value=novo.Nif},
                 new SqlParameter(){ParameterName="@email",
                    SqlDbType =SqlDbType.NVarChar,Value=novo.Email},
                 new SqlParameter(){ParameterName="@morada",
                    SqlDbType =SqlDbType.NVarChar,Value=novo.Morada},
            };
            Bd.Instance.ExecutaComando(sql, parametros);
        }
        //read
        public List<UtilizadoresModel> Lista()
        {
            string sql = "SELECT * FROM utilizadores";
            DataTable registos = Bd.Instance.DevolveConsulta(sql);
            List<UtilizadoresModel> lista = new List<UtilizadoresModel>();
            foreach (DataRow data in registos.Rows)
            {
                UtilizadoresModel novo = new UtilizadoresModel();
                novo.Username = data[1].ToString();
                novo.Password = data[8].ToString();
                novo.Morada = data[3].ToString();
                novo.Nif = data[4].ToString();
                novo.Password = data[8].ToString();
                novo.Perfil = int.Parse(data[6].ToString());
                novo.Estado = bool.Parse(data[5].ToString());
                novo.Id = data[0].ToString();
                lista.Add(novo);
            }
            return lista;
        }
        public List<UtilizadoresModel> Lista(string id)
        {
            string sql = "SELECT * FROM utilizadores WHERE id = @id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter(){ParameterName="@id",
                    SqlDbType =SqlDbType.Int,Value=id}
            };
            DataTable registos = Bd.Instance.DevolveConsulta(sql, parametros);
            List<UtilizadoresModel> lista = new List<UtilizadoresModel>();
            foreach (DataRow data in registos.Rows)
            {
                UtilizadoresModel novo = new UtilizadoresModel();
                novo.Username = data[1].ToString();
                novo.Email = data[2].ToString();
                novo.Morada = data[3].ToString();
                novo.Password = data[8].ToString();
                novo.Nif = data[4].ToString();
                novo.Perfil = int.Parse(data[6].ToString());
                novo.Estado = bool.Parse(data[5].ToString());
                lista.Add(novo);
            }
            return lista;
        }
        //update
        public void EditarUtilizador(UtilizadoresModel novo)
        {
            string sql = @"UPDATE utilizadores SET email=@email,username=@username,morada=@morada,nif=@nif,estado=@estado 
                            WHERE username=@username";

            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@email",SqlDbType=SqlDbType.VarChar,Value=novo.Email },
                new SqlParameter() {ParameterName="@username",SqlDbType=SqlDbType.VarChar,Value=novo.Username },
                new SqlParameter() {ParameterName="@morada",SqlDbType=SqlDbType.VarChar,Value=novo.Morada },
                new SqlParameter() {ParameterName="@nif",SqlDbType=SqlDbType.VarChar,Value=novo.Nif },
                new SqlParameter() {ParameterName="@estado",SqlDbType=SqlDbType.VarChar,Value=novo.Estado },
            };
            Bd.Instance.ExecutaComando(sql, parametros);
        }
        //delete
        public void RemoverUtilizador(string id)
        {
            string sql = "DELETE FROM Utilizadores WHERE id=@id";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter(){ParameterName="@id",
                    SqlDbType =SqlDbType.Int,Value=id}
            };
            Bd.Instance.ExecutaComando(sql, parametros);
        }
        //email existe
        public bool EmailExist(string email)
        {
            string sql = "SELECT email FROM Utilizadores WHERE email = @email";
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ParameterName = "@email",SqlDbType = SqlDbType.NVarChar,Value= email}
            };
            return Bd.Instance.DevolveConsulta(sql, parameters).Rows.Count > 0;
        }
        //username existe
        public bool UsernameExist(string username)
        {
            string sql = "SELECT username FROM Utilizadores WHERE username = @username";
            List<SqlParameter> parameters = new List<SqlParameter>()
            {
                new SqlParameter(){ParameterName = "@username",SqlDbType = SqlDbType.NVarChar,Value= username}
            };
            return Bd.Instance.DevolveConsulta(sql, parameters).Rows.Count > 0;
        }
    }
}