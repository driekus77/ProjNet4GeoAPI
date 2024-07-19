using System;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktUnit class.
    /// </summary>
    public class WktUnit : WktBaseObject, IEquatable<WktUnit>
    {
        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// ConversionFactor property.
        /// </summary>
        public double ConversionFactor { get; internal set; }

        /// <summary>
        /// Authority property.
        /// </summary>
        public WktAuthority Authority { get; internal set; }


        /// <summary>
        /// Constructor for WKT Unit.
        /// </summary>
        /// <param name="name"></param>
        public WktUnit(string name)
        {
            Name = name;
        }


        /// <summary>
        /// Setter method for Name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WktUnit SetName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        /// Setter method for ConversionFactor.
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public WktUnit SetConversionFactor(double factor)
        {
            ConversionFactor = factor;
            return this;
        }

        /// <summary>
        /// Setter method for Authority.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public WktUnit SetAuthority(WktAuthority authority)
        {
            Authority = authority;
            return this;
        }

        public bool Equals(WktUnit other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && ConversionFactor.Equals(other.ConversionFactor) && Equals(Authority, other.Authority);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktUnit) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ ConversionFactor.GetHashCode();
                hashCode = (hashCode * 397) ^ (Authority != null ? Authority.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
