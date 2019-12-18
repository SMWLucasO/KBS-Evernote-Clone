using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Setting
{
    public class SettingModel : IModel
    {
        public int Id { get; set; } = -1;
        public int UserId { get; set; }
        public string KeyWord { get; set; }
        public string SettingValue { get; set; }
    }
}