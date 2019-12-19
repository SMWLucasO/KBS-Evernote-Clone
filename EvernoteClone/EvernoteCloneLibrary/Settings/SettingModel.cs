using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Settings
{
    public class SettingModel : IModel
    {
        public int Id { get; set; } = -1;
        public int UserId { get; set; }
        public string KeyWord { get; set; }
        public object SettingValue { get; set; }
    }
}