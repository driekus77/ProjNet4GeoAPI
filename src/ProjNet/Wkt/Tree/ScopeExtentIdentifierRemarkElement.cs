using System.Collections.Generic;
using System.Text;

namespace ProjNet.Wkt.v1.tree
{
    /// <summary>
    /// WktScopeExtentIdentifierRemarkElement class.
    /// </summary>
    public class ScopeExtentIdentifierRemarkElement : IWktAttribute
    {
        /// <summary>
        /// Optional Scope attribute.
        /// </summary>
        public Scope Scope { get; set; }

        /// <summary>
        /// Zero or more Extent attribute(s).
        /// </summary>
        public IList<Extent> Extents { get; set; } = new List<Extent>();

        /// <summary>
        /// Zero or more Identifier attribute(s).
        /// </summary>
        public IList<Identifier> Identifiers { get; set; } = new List<Identifier>();

        /// <summary>
        /// Optional Remark attrbiute.
        /// </summary>
        public Remark Remark { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="extents"></param>
        /// <param name="identifiers"></param>
        /// <param name="remark"></param>
        public ScopeExtentIdentifierRemarkElement(
            Scope scope = null,
            IList<Extent> extents = null,
            IList<Identifier> identifiers = null,
            Remark remark = null)
        {
            Scope = scope;
            Extents = extents ?? Extents;
            Identifiers = identifiers ?? Identifiers;
            Remark = remark;
        }

        /// <summary>
        /// ToWKT.
        /// </summary>
        /// <returns></returns>
        public string ToWKT()
        {
            var sb = new StringBuilder();

            if (Scope != null)
            {
                sb.Append($",{Scope.ToWKT()}");
            }

            if (Extents != null)
            {
                foreach (var extent in Extents)
                {
                    sb.Append($",{extent.ToWKT()})");
                }
            }
            if (Identifiers != null)
            {
                foreach (var identifier in Identifiers)
                {
                    sb.Append($",{identifier.ToWKT()})");
                }
            }

            if (Remark != null)
            {
                sb.Append($",{Remark.ToWKT()}");
            }

            return sb.ToString();
        }
    }
}
