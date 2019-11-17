using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Database
{
    /// <summary>
    /// The purpose of this class is giving access to data, specifically to the database data.
    /// </summary>
    public class DataAccess
    {

        private SqlConnection _connection;

        /// <summary>
        /// A method for querying the database.
        /// </summary>
        /// <remarks>
        /// The dictionary should have keys as placeholder name and values as the placeholder value of the query.
        /// Two default ReturnType methods are provided.
        /// See: <see cref="DataAccess.RowsAffectedReturnType">RowsAffectedReturnType</see> and <see cref="DataAccess.SqlDataReaderReturnType">SqlDataReaderReturnType</see> 
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="QueryString"></param>
        /// <param name="Parameters"></param>
        /// <param name="ReturnType"></param>
        /// <returns></returns>
        public T Query<T>(string QueryString, Dictionary<string, object> Parameters, Func<SqlCommand, T> ReturnType)
        {
            SqlConnection sqlConnection = OpenSqlConnection();
            using (SqlCommand sqlCommand = GenerateParameteredCommand(QueryString, sqlConnection, Parameters))
            {
                return ReturnType(sqlCommand);
            }
        }

        /// <summary>
        /// Method which prepares the SqlCommand for being executed
        /// </summary>
        /// <param name="QueryString">string</param>
        /// <param name="Connection">SqlConnection</param>
        /// <param name="Parameters"></param>
        /// <returns>SqlCommand</returns>
        private SqlCommand GenerateParameteredCommand(string QueryString, SqlConnection Connection, Dictionary<string, object> Parameters)
        {
            SqlCommand command = new SqlCommand(QueryString, Connection);
            if (Parameters != null)
            {
                foreach (string key in Parameters.Keys)
                {
                    command.Parameters.AddWithValue(key, Parameters[key]);
                }
            }

            return command;
        }

        /// <summary>
        /// Method for creating a connection to the database.
        /// </summary>
        /// <returns>SqlConnection</returns>
        private SqlConnection OpenSqlConnection()
        {
            SqlConnection sqlConnection = new SqlConnection("Data Source=.;Initial Catalog=NoteFever_EvernoteClone;Integrated Security=True");

            sqlConnection.Open();

            return sqlConnection;
        }

        /// <summary>
        /// Since we don't close the connection in a method, we need to do it explicitly somewhere.
        /// Therefore, this should be done after a query is executed (and perhaps read.)
        /// </summary>
        public void CloseSqlConnection()
        {
            _connection?.Close();
        }

        /// <summary>
        /// A return type for the Query method, this is used for checking if insert/update/delete
        /// queries were successful.
        /// </summary>
        public static Func<SqlCommand, bool> RowsAffectedReturnType = sqlCommand
            => sqlCommand.ExecuteNonQuery() >= 1;

        /// <summary>
        /// A return type for the Query method, this is used for retrieving selected data.
        /// </summary>
        public static Func<SqlCommand, SqlDataReader> SqlDataReaderReturnType = sqlCommand
            => sqlCommand.ExecuteReader();

    }
}
