using System.Text;

namespace ProjNet.Wkt.v1.tree
{
    /// <summary>
    /// Remark a simple wkt Attribute.
    /// </summary>
    public class Remark: IWktAttribute
    {
        /// <summary>
        /// Text property.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text"></param>
        public Remark(string text)
        {
            Text = text;
        }

        /// <summary>
        /// ToWKT version of this WKT Remark.
        /// </summary>
        /// <returns></returns>
        public string ToWKT()
        {
            var sb = new StringBuilder();

            sb.Append($@"REMARK[""");
            sb.Append(Text);
            sb.Append($@"""]");

            return sb.ToString();
        }

        /// <summary>
        /// ToString version of this WKT Uri.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Text;
        }

    }
}
