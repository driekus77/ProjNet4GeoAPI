using System;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktPrimeMeridian property.
    /// </summary>
    public class WktPrimeMeridian : WktBaseObject, IEquatable<WktPrimeMeridian>
    {

        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Longitude property.
        /// </summary>
        public double Longitude { get; internal set; }

        /// <summary>
        /// Authority property.
        /// </summary>
        public WktAuthority Authority { get; internal set; }


        /// <summary>
        /// Constructor with optional a name.
        /// </summary>
        /// <param name="name"></param>
        public WktPrimeMeridian(string name = null)
        {
            Name = name;
        }


        /// <summary>
        /// Setter method for Name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WktPrimeMeridian SetName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        /// Setter method for Longitude.
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public WktPrimeMeridian SetLongitude(double longitude)
        {
            Longitude = longitude;
            return this;
        }

        /// <summary>
        /// Setter method for Authority.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public WktPrimeMeridian SetAuthority(WktAuthority authority)
        {
            Authority = authority;
            return this;
        }

        public bool Equals(WktPrimeMeridian other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Longitude.Equals(other.Longitude) && Equals(Authority, other.Authority);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktPrimeMeridian) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Longitude.GetHashCode();
                hashCode = (hashCode * 397) ^ (Authority != null ? Authority.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
