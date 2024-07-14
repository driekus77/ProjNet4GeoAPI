using System.Text;

namespace ProjNet.Wkt.v1.tree
{
    /// <summary>
    /// AuthorityCitation a simple wkt helper class containing a (sub) Attribute.
    /// </summary>
    public class AuthorityCitation: IWktAttribute
    {
        private string Citation { get; set; }

        public AuthorityCitation(string citation)
        {
            Citation = citation;
        }

        public string ToWKT()
        {
            var sb = new StringBuilder();

            sb.Append($@"CITATION[""");
            sb.Append(Citation);
            sb.Append($@"""]");

            return sb.ToString();
        }
    }
}
