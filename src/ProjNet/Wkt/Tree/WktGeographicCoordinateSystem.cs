using System;
using System.Collections.Generic;
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
        public WktAngularUnit AngularUnit { get; set; }

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
        public WktGeographicCoordinateSystem(string name = null) : base(name)
        {
        }

        /// <summary>
        /// SetAngularUnit method.
        /// </summary>
        /// <param name="angularUnit"></param>
        /// <returns></returns>
        public WktGeographicCoordinateSystem SetAngularUnit(WktAngularUnit angularUnit)
        {
            this.AngularUnit = angularUnit;
            return this;
        }

        /// <summary>
        /// SetUnit method.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public WktGeographicCoordinateSystem SetUnit(WktUnit unit)
        {
            this.Unit = unit;
            return this;
        }

        /// <summary>
        /// SetHorizontalDatum method.
        /// </summary>
        /// <param name="datum"></param>
        /// <returns></returns>
        public WktGeographicCoordinateSystem SetDatum(WktDatum datum)
        {
            this.HorizontalDatum = datum;
            return this;
        }

        /// <summary>
        /// SetPrimeMeridian method.
        /// </summary>
        /// <param name="primeMeridian"></param>
        /// <returns></returns>
        public WktGeographicCoordinateSystem SetPrimeMeridian(WktPrimeMeridian primeMeridian)
        {
            this.PrimeMeridian = primeMeridian;
            return this;
        }

        /// <summary>
        /// SetAxisInfos method.
        /// </summary>
        /// <param name="axes"></param>
        /// <returns></returns>
        public WktGeographicCoordinateSystem SetAxes(IEnumerable<WktAxis> axes)
        {
            this.Axes = axes;
            return this;
        }

        /// <summary>
        /// SetAuthority method.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public WktGeographicCoordinateSystem SetAuthority(WktAuthority authority)
        {
            this.Authority = authority;
            return this;
        }

        /// <summary>
        /// SetAlias method.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public WktGeographicCoordinateSystem SetAlias(string alias)
        {
            this.Alias = alias;
            return this;
        }


        /// <summary>
        /// SetAbbreviation method.
        /// </summary>
        /// <param name="abbreviation"></param>
        /// <returns></returns>
        public WktGeographicCoordinateSystem SetAbbreviation(string abbreviation)
        {
            Abbreviation = abbreviation;
            return this;
        }

        /// <summary>
        /// SetRemarks method.
        /// </summary>
        /// <param name="remarks"></param>
        /// <returns></returns>
        public WktGeographicCoordinateSystem SetRemarks(string remarks)
        {
            Remarks = remarks;
            return this;
        }

        public bool Equals(WktGeographicCoordinateSystem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(AngularUnit, other.AngularUnit) &&
                   Equals(Unit, other.Unit) &&
                   Equals(HorizontalDatum, other.HorizontalDatum) &&
                   Equals(PrimeMeridian, other.PrimeMeridian) &&
                   Equals(Axes, other.Axes) &&
                   Equals(Authority, other.Authority) &&
                   Alias == other.Alias && Abbreviation == other.Abbreviation && Remarks == other.Remarks;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktGeographicCoordinateSystem) obj);
        }

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
