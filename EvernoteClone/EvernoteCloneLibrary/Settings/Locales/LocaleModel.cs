using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Settings.Locales
{
    /// <summary>
    /// This is a 'blueprint' of how a locale should look like (according to the database)
    /// </summary>
    public class LocaleModel : IModel
    {
        public int Id { get; set; }
        public string Locale { get; set; }
        public string Language { get; set; }
    }
}