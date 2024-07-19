namespace ProjNet.Wkt.v1.tree
{
    /// <summary>
    /// Extent attribute
    /// </summary>
    public abstract class Extent : IWktAttribute
    {
        /// <summary>
        /// Convert (back) to WKT.
        /// </summary>
        /// <returns></returns>
        public abstract string ToWKT();

    }
}
