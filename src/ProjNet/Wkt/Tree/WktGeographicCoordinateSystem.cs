using System;
using System.Collections.Generic;
using System.Linq;
using ProjNet.CoordinateSystems;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// Builder class for GeographicCoordinateSystem
    /// </summary>
    public class WktGeographicCoordinateSystem : WktCoordinateSystem, IEquatable<WktGeographicCoordinateSystem>
    {
        /// <summary>
        /// AngularUnit property.
        /// </summary>
        public WktUnit AngularUnit { get; set; }

        /// <summary>
        /// Unit property.
        /// </summary>
        public WktUnit Unit { get; set; }

        /// <summary>
        /// HorizontalDatum property.
        /// </summary>
        public WktDatum HorizontalDatum { get; set; }


        /// <summary>
        /// PrimeMeridian property.
        /// </summary>
        public WktPrimeMeridian PrimeMeridian { get; set; }


        /// <summary>
        /// Zero to Many Axes.
        /// </summary>
        public IEnumerable<WktAxis> Axes { get; set; }

        /// <summary>
        /// WktAuthority property. (Optional)
        /// </summary>
        public WktAuthority Authority{ get; set; }

        /// <summary>
        /// Alias property.
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Abbreviation property.
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// Remarks property.
        /// </summary>
        public string Remarks { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public WktGeographicCoordinateSystem(string name, WktDatum datum, WktPrimeMeridian meridian, WktUnit angularUnit,
                                                IEnumerable<WktAxis> axes, WktAuthority authority,
                                                string keyword = "GEOGCS", char leftDelimiter = '[', char rightDelimiter = ']')
            : base(name, keyword, leftDelimiter, rightDelimiter)
        {
            HorizontalDatum = datum;
            PrimeMeridian = meridian;
            AngularUnit = angularUnit;
            Axes = axes;
            Authority = authority;
        }


        /// <summary>
        /// Implementing IEquatable.Equals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(WktGeographicCoordinateSystem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(AngularUnit, other.AngularUnit) &&
                   Equals(Unit, other.Unit) &&
                   Equals(HorizontalDatum, other.HorizontalDatum) &&
                   Equals(PrimeMeridian, other.PrimeMeridian) &&
                   Axes.SequenceEqual(other.Axes) &&
                   Equals(Authority, other.Authority) &&
                   Alias == other.Alias && Abbreviation == other.Abbreviation && Remarks == other.Remarks;
        }

        /// <summary>
        /// Overriding of basic Equals.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktGeographicCoordinateSystem) obj);
        }

        /// <summary>
        /// Overriding of basic GetHashCode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (AngularUnit != null ? AngularUnit.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Unit != null ? Unit.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (HorizontalDatum != null ? HorizontalDatum.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (PrimeMeridian != null ? PrimeMeridian.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Axes != null ? Axes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Authority != null ? Authority.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Alias != null ? Alias.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Abbreviation != null ? Abbreviation.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Remarks != null ? Remarks.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
