namespace ProjNet.Wkt.Tree
{

    /// <summary>
    /// Base interface for all WKT Objects
    /// </summary>
    public interface IWktObject
    {
        /// <summary>
        /// The Keyword as set and used by the WktParser.
        /// </summary>
        string Keyword { get; }


        /// <summary>
        /// Set Left Delimiter. (For semantic checking).
        /// </summary>
        /// <param name="leftDelimiter"></param>
        /// <returns></returns>
        IWktObject SetLeftDelimiter(char leftDelimiter);

        /// <summary>
        /// Set Right Delimiter. (For semantic checking).
        /// </summary>
        /// <param name="rightDelimiter"></param>
        /// <returns></returns>
        IWktObject SetRightDelimiter(char rightDelimiter);

        /// <summary>
        /// Cast function to reach the "lower" interfaces.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T As<T>() where T : IWktObject;
    }
}
