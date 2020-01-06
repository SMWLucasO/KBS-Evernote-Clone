using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Notebooks.Notes.Labels;

namespace EvernoteCloneLibrary.Database
{
    /// <summary>
    /// The purpose of this class is giving access to data, specifically to the database data.
    /// </summary>
    public class DataAccess
    {
        /// <summary>
        /// The Singleton accessor to this class.
        /// The only way to access this class' object is through here.
        /// </summary>
        public static readonly DataAccess Instance = new DataAccess();

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
        /// <param name="queryString"></param>
        /// <param name="parameters"></param>
        /// <param name="returnType"></param>
        /// <returns></returns>
        private T Query<T>(string queryString, Dictionary<string, object> parameters, Func<SqlCommand, T> returnType)
        {
            SqlConnection sqlConnection = OpenSqlConnection();

            using (SqlCommand sqlCommand = GenerateParameteredCommand(queryString, sqlConnection, parameters))
            {
                return returnType(sqlCommand);
            }
        }

        /// <summary>
        /// Method for SELECT queries within repositories
        /// </summary>
        /// <param name="table"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteAndRead(string table, string[] conditions, Dictionary<string, object> parameters)
        {
            StringBuilder conditionBuilder = new StringBuilder();

            // build the condition string: ( WHERE ... AND ... AND ... AND ... ) etc
            // conditions array should hold strings of: key = value, key >= value ... etc
            if (conditions != null)
            {
                for (int i = 0; i < conditions.Length; i++)
                {
                    if (i == 0)
                    {
                        conditionBuilder.Append("WHERE ");
                    }
                    else
                    {
                        conditionBuilder.Append("AND ");
                    }

                    conditionBuilder.Append(conditions[i]).Append(" ");
                }
            }

            return Query($"SELECT * FROM [{table}] {conditionBuilder}",
                parameters, SqlDataReaderReturnType);
        }

        /// <summary>
        /// Method for INSERT/DELETE/UPDATE queries within repositories.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool Execute(string query, Dictionary<string, object> parameters)
        {
            bool success = this.Query(query, parameters, RowsAffectedReturnType);
            CloseSqlConnection();
            return success;
        }

        /// <summary>
        /// Return -1 if nothing was returned from the query, otherwise return the last inserted id.
        /// Just in case, this method should only be used for INSERT repository methods.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteAndReturnId(string query, Dictionary<string, object> parameters)
        {
            int id = -1;
            SqlDataReader data = this.Query($"{query} SELECT NewID = SCOPE_IDENTITY()", parameters, SqlDataReaderReturnType);

            if (data.Read())
                id = Convert.ToInt32(Math.Truncate(((decimal)data["NewID"])));

            CloseSqlConnection();
            return id;
        }

        /// <summary>
        /// Method which prepares the SqlCommand for being executed
        /// </summary>
        /// <param name="queryString">string</param>
        /// <param name="connection">SqlConnection</param>
        /// <param name="parameters"></param>
        /// <returns>SqlCommand</returns>
        private SqlCommand GenerateParameteredCommand(string queryString, SqlConnection connection, Dictionary<string, object> parameters)
        {
            SqlCommand command = new SqlCommand(queryString, connection);

            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    command.Parameters.AddWithValue(key, parameters[key]);
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
            string connectionString = "" +
                $"Server=tcp:{(Constant.TEST_MODE ? DatabaseConstant.TEST_DATABASE_HOST : DatabaseConstant.DATABASE_HOST)},{SshConnection.Instance.GetSshPort()};" +
                $"Database={(Constant.TEST_MODE ? DatabaseConstant.TEST_DATABASE_CATALOG : DatabaseConstant.DATABASE_CATALOG)};" +
                $"UID={(Constant.TEST_MODE ? DatabaseConstant.TEST_DATABASE_USERNAME : DatabaseConstant.DATABASE_USERNAME)};" +
                $"Password={(Constant.TEST_MODE ? DatabaseConstant.TEST_DATABASE_PASSWORD : DatabaseConstant.DATABASE_PASSWORD)};" +
                $"Integrated Security={(Constant.TEST_MODE ? DatabaseConstant.TEST_DATABASE_INTEGRATED_SECURITY : DatabaseConstant.DATABASE_INTEGRATED_SECURITY)}";

            _connection = new SqlConnection(connectionString);
            _connection.Open();
            return _connection;
        }

        public DataTable GetLanguageTable(string langCode)
        {
            DataTable _language = new DataTable();
            SqlConnection conn = OpenSqlConnection();
            string _query = $"SELECT * FROM LANGUAGE WHERE Language='{langCode}'";

            SqlCommand cmd = new SqlCommand(_query, conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            da.Fill(_language);
            conn.Close();

            if (_language.Rows.Count < 1)
                return null;
            return _language;
        }

        /// <summary>
        /// Since we don't close the connection in a method, we need to do it explicitly somewhere.
        /// Therefore, this should be done after a query is executed (and perhaps read.)
        /// </summary>
        public void CloseSqlConnection() =>
            _connection?.Close();

        /// <summary>
        /// A return type for the Query method, this is used for checking if insert/update/delete
        /// queries were successful.
        /// </summary>
        private static readonly Func<SqlCommand, bool> RowsAffectedReturnType = sqlCommand
            => sqlCommand.ExecuteNonQuery() >= 1;

        /// <summary>
        /// A return type for the Query method, this is used for retrieving selected data.
        /// </summary>
        private static readonly Func<SqlCommand, SqlDataReader> SqlDataReaderReturnType = sqlCommand
            => sqlCommand.ExecuteReader();

    }
}
