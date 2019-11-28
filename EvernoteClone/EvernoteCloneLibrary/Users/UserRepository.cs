using EvernoteCloneLibrary.Database;
using EvernoteCloneLibrary.Notebooks;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Users
{
    class UserRepository : IRepository<UserModel>
    {
        public bool Delete(UserModel ToDelete)
        {
            if (ToDelete != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    {"@Id", ToDelete.Id }
                };
                return DataAccess.Instance.Execute("DELETE FROM [User] WHERE Id = @Id", parameters);
            }
            return false;
        }

        public Dictionary<string, object> GenerateQueryParameters(UserModel ToExtractFrom)
        {
            if (ToExtractFrom != null)
            {
                return new Dictionary<string, object>()
                {
                    { "@Username", ToExtractFrom.Username },
                    { "@Password", ToExtractFrom.Password },
                    { "@FirstName", ToExtractFrom.FirstName },
                    { "@LastName", ToExtractFrom.LastName },
                    { "@IsGoogleAccount", ToExtractFrom.IsGoogleAccount },
                    { "@CreationDate", ToExtractFrom.CreationDate },
                    { "@LastLogin", ToExtractFrom.LastLogin }
                };
            }
            return null;
        }

        public IEnumerable<UserModel> GetBy(string[] Conditions, Dictionary<string, object> Parameters)
        {
            List<User> usersList = new List<User>();
            SqlDataReader fetchedSqlDataReader = DataAccess.Instance.ExecuteAndRead("User", Conditions, Parameters);

            NotebookRepository notebookRepository = new NotebookRepository();

            while (fetchedSqlDataReader.Read())
            {
                User user = new User()
                {
                    Id = (int)fetchedSqlDataReader["Id"],
                    Username = (string)fetchedSqlDataReader["Username"],
                    Password = (string)fetchedSqlDataReader["Password"],
                    FirstName = (string)fetchedSqlDataReader["FirstName"],
                    LastName = (string)fetchedSqlDataReader["LastName"],
                    IsGoogleAccount = (bool)fetchedSqlDataReader["IsGoogleAccount"],
                    CreationDate = (DateTime)fetchedSqlDataReader["CreationDate"],
                    LastLogin = (DateTime)fetchedSqlDataReader["LastLogin"],
                    Notebooks = notebookRepository.GetBy(
                            new string[] { "UserID = @UserID" },
                            new Dictionary<string, object>()
                            {
                                {"@UserID", (int)fetchedSqlDataReader ["Id"] }
                            })
                    .Select((el) => ((Notebook)el)).ToList()
                };

                usersList.Add(user);
            }
            DataAccess.Instance.CloseSqlConnection();
            fetchedSqlDataReader.Close();

            return usersList.AsEnumerable();
        }

        public bool Insert(UserModel ToInsert)
        {

            if (ToInsert != null)
            {
                if (string.IsNullOrEmpty(ToInsert.Username) || string.IsNullOrEmpty(ToInsert.Password)
                    || ToInsert.CreationDate == null || ToInsert.LastLogin == null)
                {
                    return false;
                }

                Dictionary<string, object> Parameters = GenerateQueryParameters(ToInsert);

                int id = DataAccess.Instance.ExecuteAndReturnId("INSERT INTO [User] ([Username], [Password], [FirstName], [LastName], [IsGoogleAccount]," +
                    "[CreationDate], [LastLogin])"
                        + " VALUES (@Username, @Password, @FirstName, @LastName, @IsGoogleAccount, @CreationDate, @LastLogin)", Parameters);

                if (id != -1)
                {
                    ToInsert.Id = id;
                }

                return id != -1;
            }

            return false;
        }

        public bool Update(UserModel ToUpdate)
        {
            if (ToUpdate != null)
            {
                if (string.IsNullOrEmpty(ToUpdate.FirstName) || string.IsNullOrEmpty(ToUpdate.LastName) || string.IsNullOrEmpty(ToUpdate.Password) || string.IsNullOrEmpty(ToUpdate.Username))
                {
                    return false;
                }

                Dictionary<string, object> parameters = GenerateQueryParameters(ToUpdate);
                parameters.Add("@Id", ToUpdate.Id);

                return DataAccess.Instance.Execute("UPDATE [User] SET [FirstName] = @FirstName, [LastName] = @LastName, "
                    + "[Username] = @Username, [Password] = @Password WHERE Id = @Id", parameters);

            }
            return false;
        }
    }
}
