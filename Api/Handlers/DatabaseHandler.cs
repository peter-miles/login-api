using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Web.Http;
using Lib;
using Microsoft.Extensions.Configuration;

namespace Api.Handlers
{
    public class DatabaseHandler
    {
        public DatabaseHandler(IConfiguration AppSettings)
        {
            this.AppSettings = AppSettings;
            this.ConnectionString = AppSettings.GetValue<string>("ConnectionString");
        }

        private readonly IConfiguration AppSettings;
        private readonly string ConnectionString;

        /* database handler helper functions */
        void Add(SqlCommand sqlCommand, string parameterName, SqlDbType sqlDbType, object Value)
        {
            sqlCommand.Parameters.Add(parameterName, sqlDbType);
            sqlCommand.Parameters[parameterName].Value = Value;
        }

        /* user functions */
        public User GetUserByID(int UserID)
        {
            User OutputUser = new User();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                SqlCommand sqlCommand = new SqlCommand("SELECT USER_ID, USERNAME FROM USER WHERE USER_ID", sqlConnection);
                
                Add(sqlCommand, "@USER_ID", SqlDbType.Int, UserID);

                sqlConnection.Open();

                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        OutputUser = new User((int)sqlDataReader[0], sqlDataReader[1].ToString());

                        sqlConnection.Close();
                    }
                }
            }

            return OutputUser;
        }

        public string RegisterUser(string Username, string PasswordHashed)
        {
            string ReturnMessage = "OK";

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                try
                {
                    SqlCommand sqlCommand = new SqlCommand("INSERT INTO USER VALUES (@USERNAME, @PASSWORD_HASH, @DATE_REGISTERED)", sqlConnection);

                    Add(sqlCommand, "@USERNAME", SqlDbType.NVarChar, Username);

                    sqlCommand.Parameters.AddWithValue("@PASSWORD_HASH", SqlDbType.NVarChar);
                    sqlCommand.Parameters["@PASSWORD_HASH"].Value = PasswordHashed;

                    DateTime DateTime = DateTime.Now;
                    string FormattedDateTime = DateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    sqlCommand.Parameters.AddWithValue("@DATE_REGISTERED", SqlDbType.DateTime);
                    sqlCommand.Parameters["@DATE_REGISTERED"].Value = FormattedDateTime;

                    sqlConnection.Open();


                    sqlCommand.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // server message error handling 
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                sqlConnection.Close();
            }

            return ReturnMessage;
        }
    }
}