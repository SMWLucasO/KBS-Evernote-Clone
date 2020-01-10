namespace EvernoteCloneLibrary.Files.Parsers
{
    /// <summary>
    /// Interface for classes which export to the Xml format.
    /// </summary>
    public interface IParseable
    {
        /// <summary>
        /// Specifies the Xml presentation the implementing class should be exported as, as Xml. 
        /// </summary>
        /// <returns></returns>
        string[] ToXmlRepresentation();
    }
}
