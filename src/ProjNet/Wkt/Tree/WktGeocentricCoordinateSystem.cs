using System;
using System.Collections.Generic;
using System.Linq;
using ProjNet.CoordinateSystems;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktGeocentricCoordinateSystem class.
    /// </summary>
    public class WktGeocentricCoordinateSystem : WktCoordinateSystem, IEquatable<WktGeocentricCoordinateSystem>
    {
        /// <summary>
        /// Datum property.
        /// </summary>
        public WktDatum Datum { get; internal set; }

        /// <summary>
        /// PrimeMeridian property.
        /// </summary>
        public WktPrimeMeridian PrimeMeridian { get; internal set; }

        /// <summary>
        /// LinearUnit property.
        /// </summary>
        public WktLinearUnit LinearUnit { get; internal set; }

        /// <summary>
        /// Authority property.
        /// </summary>
        public WktAuthority Authority { get; internal set; }

        /// <summary>
        /// Axes property.
        /// </summary>
        public IEnumerable<WktAxis> Axes { get; internal set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        public WktGeocentricCoordinateSystem(string name = null) : base(name)
        {
        }


        /// <summary>
        /// Setter method for Datum.
        /// </summary>
        /// <param name="datum"></param>
        /// <returns></returns>
        public WktGeocentricCoordinateSystem SetDatum(WktDatum datum)
        {
            Datum = datum;
            return this;
        }

        /// <summary>
        /// Setter method for PrimeMeridian.
        /// </summary>
        /// <param name="primeMeridian"></param>
        /// <returns></returns>
        public WktGeocentricCoordinateSystem SetPrimeMeridian(WktPrimeMeridian primeMeridian)
        {
            PrimeMeridian = primeMeridian;
            return this;
        }

        /// <summary>
        /// Setter method for LinearUnit
        /// </summary>
        /// <param name="linearUnit"></param>
        /// <returns></returns>
        public WktGeocentricCoordinateSystem SetLinearUnit(WktLinearUnit linearUnit)
        {
            LinearUnit = linearUnit;
            return this;
        }

        /// <summary>
        /// Setter method for Authority.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public WktGeocentricCoordinateSystem SetAuthority(WktAuthority authority)
        {
            Authority = authority;
            return this;
        }

        /// <summary>
        /// Setter method for Axes.
        /// </summary>
        /// <param name="axes"></param>
        /// <returns></returns>
        public WktGeocentricCoordinateSystem SetAxes(IEnumerable<WktAxis> axes)
        {
            Axes = axes;
            return this;
        }

        public bool Equals(WktGeocentricCoordinateSystem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Datum, other.Datum) && Equals(PrimeMeridian, other.PrimeMeridian) && Equals(LinearUnit, other.LinearUnit) && Equals(Authority, other.Authority) && Axes.SequenceEqual(other.Axes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktGeocentricCoordinateSystem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Datum != null ? Datum.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PrimeMeridian != null ? PrimeMeridian.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (LinearUnit != null ? LinearUnit.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Authority != null ? Authority.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Axes != null ? Axes.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
