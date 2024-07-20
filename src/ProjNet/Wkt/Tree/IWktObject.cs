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
        /// Traverse this object and its descendants calling the handler as we go down.
        /// </summary>
        /// <param name="handler"></param>
        void Traverse(IWktTraverseHandler handler);
    }
}
