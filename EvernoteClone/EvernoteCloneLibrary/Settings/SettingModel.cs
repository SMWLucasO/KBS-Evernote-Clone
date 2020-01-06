using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Settings
{
    /// <summary>
    /// This is a 'blueprint' of how a setting should look like (according to the database)
    /// </summary>
    public class SettingModel : IModel
    {
        public int Id { get; set; } = -1;
        public int UserId { get; set; }
        public string KeyWord { get; set; }
        public object SettingValue { get; set; }
    }
}