using System.Text;

namespace ProjNet.Wkt.v1.tree
{
    /// <summary>
    /// AuthorityCitation a simple wkt helper class containing a (sub) Attribute.
    /// </summary>
    public class AuthorityCitation: IWktAttribute
    {
        private string Citation { get; set; }

        /// <summary>
        /// AuthorityCitation
        /// </summary>
        /// <param name="citation"></param>
        public AuthorityCitation(string citation)
        {
            Citation = citation;
        }

        /// <summary>
        /// Convert (back) to WKT.
        /// </summary>
        /// <returns></returns>
        public string ToWKT()
        {
            var sb = new StringBuilder();

            sb.Append($@"CITATION[""");
            sb.Append(Citation);
            sb.Append($@"""]");

            return sb.ToString();
        }

        /// <summary>
        /// ToString basic override.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Citation;
        }
    }
}
