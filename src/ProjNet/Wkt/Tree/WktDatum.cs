using System;

namespace ProjNet.Wkt.Tree
{
    public class WktDatum : WktBaseObject, IEquatable<WktDatum>
    {
        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Spheroid property.
        /// </summary>
        public WktSpheroid Spheroid { get; internal set; }

        /// <summary>
        /// ToWgs84 property.
        /// </summary>
        public WktToWgs84 ToWgs84 { get; internal set; }

        /// <summary>
        /// Authority property.
        /// </summary>
        public WktAuthority Authority { get; internal set; }


        /// <summary>
        /// Constructor for WKT Datum.
        /// </summary>
        /// <param name="name"></param>
        public WktDatum(string name = null)
        {
            Name = name;
        }


        /// <summary>
        /// Setter method for Name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WktDatum SetName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        /// Setter method for Spehroid.
        /// </summary>
        /// <param name="spheroid"></param>
        /// <returns></returns>
        public WktDatum SetSpheroid(WktSpheroid spheroid)
        {
            Spheroid = spheroid;
            return this;
        }

        /// <summary>
        /// Setter method for ToWgs84.
        /// </summary>
        /// <param name="toWgs84"></param>
        /// <returns></returns>
        public WktDatum SetToWgs84(WktToWgs84 toWgs84)
        {
            ToWgs84 = toWgs84;
            return this;
        }

        /// <summary>
        /// Setter method for Authority.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public WktDatum SetAuthority(WktAuthority authority)
        {
            Authority = authority;
            return this;
        }

        public bool Equals(WktDatum other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Equals(Spheroid, other.Spheroid) && Equals(ToWgs84, other.ToWgs84) && Equals(Authority, other.Authority);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktDatum) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Spheroid != null ? Spheroid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ToWgs84 != null ? ToWgs84.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Authority != null ? Authority.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
