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
        // TODO: Concurrency breaks the _connection, because it can be closed by one thread whilst another is
        // sending a message. TODO: fix.

        /// <summary>
        /// The Singleton accessor to this class.
        /// The only way to access this class' object is through here.
        /// </summary>
        public static DataAccess Instance = new DataAccess();

        /// <summary>
        /// The ongoing connection with the database.
        /// </summary>
        private SqlConnection _connection;


        /// <summary>
        /// We need a static constructor so that the C# compiler won't complain,
        /// <see href="https://csharpindepth.com/Articles/Singleton"> as stated in this article.</see>
        /// (Fourth version paragraph)
        /// </summary>
        static DataAccess() { }

        /// <summary>
        /// The DataAccess class needs to be private to block any other object from being created.
        /// </summary>
        private DataAccess() { }


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
        private T Query<T>(string QueryString, Dictionary<string, object> Parameters, Func<SqlCommand, T> ReturnType)
        {
            SqlConnection sqlConnection = OpenSqlConnection();
            using (SqlCommand sqlCommand = GenerateParameteredCommand(QueryString, sqlConnection, Parameters))
            {
                return ReturnType(sqlCommand);
            }
        }

        /// <summary>
        /// Method for SELECT queries within repositories
        /// </summary>
        /// <param name="Table"></param>
        /// <param name="Conditions"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteAndRead(string Table, string[] Conditions, Dictionary<string, object> Parameters)
        {
            StringBuilder conditionBuilder = new StringBuilder();

            // build the condition string: ( WHERE ... AND ... AND ... AND ... ) etc
            // conditions array should hold strings of: key = value, key >= value ... etc
            for (int i = 0; i < Conditions.Length; i++)
            {
                if (i == 0)
                {
                    conditionBuilder.Append("WHERE ");
                }
                else
                {
                    conditionBuilder.Append("AND ");
                }

                conditionBuilder.Append(Conditions[i]).Append(" ");
            }

            // Query the database using the specified data
            return Query($"SELECT * FROM {Table} {conditionBuilder.ToString()}",
                Parameters, SqlDataReaderReturnType);

        }

        /// <summary>
        /// Method for INSERT/DELETE/UPDATE queries within repositories.
        /// </summary>
        /// <param name="Query"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public bool Execute(string Query, Dictionary<string, object> Parameters)
        {
            bool success = this.Query(Query, Parameters, RowsAffectedReturnType);
            CloseSqlConnection();
            return success;
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

            _connection = sqlConnection;

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
