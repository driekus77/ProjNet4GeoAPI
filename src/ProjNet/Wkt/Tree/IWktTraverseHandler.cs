using System.Collections.Generic;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// IWktTraverseHandler interface for traveling a WktObject tree.
    /// </summary>
    public interface IWktTraverseHandler
    {

        /// <summary>
        /// Handle method for WktAuthority.
        /// </summary>
        /// <param name="authority"></param>
        void Handle(WktAuthority authority);

        void Handle(WktAxis axis);

        void Handle(WktDatum datum);

        void Handle(WktEllipsoid ellipsoid);

        void Handle(WktSpheroid spheroid);

        void Handle(WktExtension extension);

        void Handle(WktUnit unit);

        void Handle(WktParameter parameter);

        void Handle(WktPrimeMeridian meridian);

        void Handle(WktProjection projection);

        void Handle(WktToWgs84 toWgs84);

        void Handle(WktGeocentricCoordinateSystem cs);
        void Handle(WktGeographicCoordinateSystem cs);
        void Handle(WktProjectedCoordinateSystem cs);
    }
}
