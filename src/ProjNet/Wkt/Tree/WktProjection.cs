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
        public WktProjection(string name = null)
        {
            Name = name;
        }


        /// <summary>
        /// Setter for Name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WktProjection SetName(string name)
        {
            Name = name;
            return this;
        }


        /// <summary>
        /// Setter for Authority.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public WktProjection SetAuthority(WktAuthority authority)
        {
            Authority = authority;
            return this;
        }


        public bool Equals(WktProjection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Equals(Authority, other.Authority);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktProjection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Authority != null ? Authority.GetHashCode() : 0);
            }
        }
    }
}
