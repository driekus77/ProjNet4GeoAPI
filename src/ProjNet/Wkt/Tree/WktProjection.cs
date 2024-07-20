using System;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktProjection class.
    /// </summary>
    public class WktProjection : WktBaseObject, IEquatable<WktProjection>
    {
        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; internal set; }


        /// <summary>
        /// Authority property.
        /// </summary>
        public WktAuthority Authority { get; internal set; }


        /// <summary>
        /// Constructor for WKT Projection element.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="keyword"></param>
        /// <param name="leftDelimiter"></param>
        /// <param name="rightDelimiter"></param>
        public WktProjection(string name, WktAuthority authority,
                                string keyword = "PROJECTION", char leftDelimiter = '[', char rightDelimiter = ']')
            : base(keyword, leftDelimiter, rightDelimiter)
        {
            Name = name;
            Authority = authority;
        }


        /// <summary>
        /// IEquatable.Equals implementation checking the whole tree.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(WktProjection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Equals(Authority, other.Authority);
        }

        /// <summary>
        /// Basic override of Equals method.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktProjection) obj);
        }

        /// <summary>
        /// Override of basic GetHashCode method.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Authority != null ? Authority.GetHashCode() : 0);
            }
        }
    }
}
