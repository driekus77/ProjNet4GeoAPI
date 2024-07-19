using System;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktSpheroid class.
    /// </summary>
    public class WktSpheroid : WktBaseObject, IEquatable<WktSpheroid>
    {
        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// SemiMajorAxis property.
        /// </summary>
        public double SemiMajorAxis { get; internal set; }
        /// <summary>
        /// InverseFlattening property.
        /// </summary>
        public double InverseFlattening { get; internal set; }
        /// <summary>
        /// Authority property.
        /// </summary>
        public WktAuthority Authority { get; internal set; }

        /// <summary>
        /// Constructor for a WktSpheroid.
        /// </summary>
        /// <param name="name"></param>
        public WktSpheroid(string name = null)
        {
            Name = name;
        }


        /// <summary>
        /// Setter method for Name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WktSpheroid SetName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        /// Setter method for SemiMajorAxis.
        /// </summary>
        /// <param name="axis"></param>
        /// <returns></returns>
        public WktSpheroid SetSemiMajorAxis(double axis)
        {
            SemiMajorAxis = axis;
            return this;
        }

        /// <summary>
        /// Setter method for InverseFlattening.
        /// </summary>
        /// <param name="flattening"></param>
        /// <returns></returns>
        public WktSpheroid SetInverseFlattening(double flattening)
        {
            InverseFlattening = flattening;
            return this;
        }

        /// <summary>
        /// Setter method for Authority.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public WktSpheroid SetAuthority(WktAuthority authority)
        {
            Authority = authority;
            return this;
        }

        public bool Equals(WktSpheroid other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && SemiMajorAxis.Equals(other.SemiMajorAxis) && InverseFlattening.Equals(other.InverseFlattening) && Equals(Authority, other.Authority);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktSpheroid) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ SemiMajorAxis.GetHashCode();
                hashCode = (hashCode * 397) ^ InverseFlattening.GetHashCode();
                hashCode = (hashCode * 397) ^ (Authority != null ? Authority.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
