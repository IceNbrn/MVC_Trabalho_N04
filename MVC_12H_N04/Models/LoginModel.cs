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
        public string username { get; set; }

        [Required(ErrorMessage = "Palavra passe é obrigatória")]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
    public class LoginBD
    {

        public UtilizadoresModel validarLogin(LoginModel login)
        {
            string sql = "SELECT * FROM utilizadores WHERE username=@username AND ";
            sql += " password=HASHBYTES('SHA2_512',@password)";
            List<SqlParameter> parametros = new List<SqlParameter>()
            {
                new SqlParameter() {ParameterName="@username",SqlDbType=SqlDbType.NVarChar,Value=login.username },
                new SqlParameter() {ParameterName="@password",SqlDbType=SqlDbType.NVarChar,Value=login.password },
            };
            DataTable dados = BD.Instance.devolveConsulta(sql, parametros);
            UtilizadoresModel utilizador = null;

            if (dados != null && dados.Rows.Count > 0)
            {
                utilizador = new UtilizadoresModel();
                utilizador.username = dados.Rows[0][1].ToString();
                utilizador.perfil = int.Parse(dados.Rows[0][6].ToString());
            }
            return utilizador;
        }
    }
}