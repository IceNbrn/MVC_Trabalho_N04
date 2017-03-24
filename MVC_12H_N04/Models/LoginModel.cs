using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MVC_12H_N04.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "O Username de utilizador é obrigatório")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Palavra passe é obrigatória")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public class LoginBd
    {

        public UtilizadoresModel ValidarLogin(LoginModel login)
        {
            string sql = "SELECT * FROM utilizadores WHERE username=@username AND ";
            sql += " password=HASHBYTES('SHA2_512',@password)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@username",SqlDbType=SqlDbType.NVarChar,Value=login.Username },
                new SqlParameter() {ParameterName="@password",SqlDbType=SqlDbType.NVarChar,Value=login.Password },
            };
            DataTable dados = Bd.Instance.DevolveConsulta(sql, parametros);
            UtilizadoresModel utilizador = null;

            if (dados != null && dados.Rows.Count > 0)
            {
                utilizador = new UtilizadoresModel();
                utilizador.Username = dados.Rows[0][1].ToString();
                utilizador.Perfil = int.Parse(dados.Rows[0][6].ToString());
            }
            return utilizador;
        }
    }
}