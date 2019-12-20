using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Settings.Locales
{
    public class LocaleModel : IModel
    {
        public int Id { get; set; }
        public string Locale { get; set; }
        public string Language { get; set; }
    }
}