using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktProjectedCoordinateSystem class.
    /// </summary>
    public class WktProjectedCoordinateSystem : WktCoordinateSystem, IEquatable<WktProjectedCoordinateSystem>
    {
        /// <summary>
        /// GeographicCoordinateSystem property.
        /// </summary>
        public WktGeographicCoordinateSystem GeographicCoordinateSystem { get; internal set; }

        /// <summary>
        /// Projection property.
        /// </summary>
        public WktProjection Projection { get; internal set; }

        /// <summary>
        /// Parameters property.
        /// </summary>
        public IEnumerable<WktParameter> Parameters { get; internal set; }

        /// <summary>
        /// Unit property.
        /// </summary>
        public WktUnit Unit { get; internal set; }

        /// <summary>
        /// Axes property.
        /// </summary>
        public IEnumerable<WktAxis> Axes { get; internal set; }

        /// <summary>
        /// Authority property.
        /// </summary>
        public WktAuthority Authority { get; internal set; }

        /// <summary>
        /// Extension property.
        /// </summary>
        public WktExtension Extension { get; internal set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        public WktProjectedCoordinateSystem(string name = null) : base(name)
        {
        }


        /// <summary>
        /// Setter method for GeographicCoordinateSystem.
        /// </summary>
        /// <param name="cs"></param>
        /// <returns></returns>
        public WktProjectedCoordinateSystem SetGeographicCoordinateSystem(WktGeographicCoordinateSystem cs)
        {
            GeographicCoordinateSystem = cs;
            return this;
        }

        /// <summary>
        /// Setter method for Projection.
        /// </summary>
        /// <param name="projection"></param>
        /// <returns></returns>
        public WktProjectedCoordinateSystem SetProjection(WktProjection projection)
        {
            Projection = projection;
            return this;
        }

        /// <summary>
        /// Setter method for Parameters.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public WktProjectedCoordinateSystem SetParameters(IEnumerable<WktParameter> parameters)
        {
            Parameters = parameters;
            return this;
        }

        /// <summary>
        /// Setter method for Unit.
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public WktProjectedCoordinateSystem SetUnit(WktUnit unit)
        {
            Unit = unit;
            return this;
        }

        /// <summary>
        /// Setter method for Axes.
        /// </summary>
        /// <param name="axes"></param>
        /// <returns></returns>
        public WktProjectedCoordinateSystem SetAxes(IEnumerable<WktAxis> axes)
        {
            Axes = axes;
            return this;
        }

        /// <summary>
        /// Setter method for Authority.
        /// </summary>
        /// <param name="authority"></param>
        /// <returns></returns>
        public WktProjectedCoordinateSystem SetAuthority(WktAuthority authority)
        {
            Authority = authority;
            return this;
        }

        /// <summary>
        /// Setter method for Extension.
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public WktProjectedCoordinateSystem SetExtension(WktExtension extension)
        {
            Extension = extension;
            return this;
        }


        public bool Equals(WktProjectedCoordinateSystem other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(GeographicCoordinateSystem, other.GeographicCoordinateSystem) &&
                   Equals(Projection, other.Projection) &&
                   Parameters.SequenceEqual(other.Parameters) &&
                   Equals(Unit, other.Unit) &&
                   Axes.SequenceEqual(other.Axes) &&
                   Equals(Authority, other.Authority) &&
                   Equals(Extension, other.Extension);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktProjectedCoordinateSystem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (GeographicCoordinateSystem != null ? GeographicCoordinateSystem.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Projection != null ? Projection.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Parameters != null ? Parameters.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Unit != null ? Unit.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Axes != null ? Axes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Authority != null ? Authority.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Extension != null ? Extension.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
