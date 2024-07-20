using System;
using System.Collections.Generic;
using System.Linq;
using ProjNet.CoordinateSystems;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktToProjConverter - Visitor for converting Wkt to Proj objects.
    /// </summary>
    public class WktToProjConverter : IWktTraverseHandler
    {
        /// <summary>
        /// Subclass for holding conversion results.
        /// </summary>
        public class WktToProjRecord
        {
            /// <summary>
            /// The Wkt Object.
            /// </summary>
            public IWktObject WktObject { get; set; }

            /// <summary>
            /// The Proj4Net Object.
            /// </summary>
            public object ProjObject { get; set; }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="wktObject"></param>
            /// <param name="projObject"></param>
            public WktToProjRecord(IWktObject wktObject, object projObject)
            {
                WktObject = wktObject;
                ProjObject = projObject;
            }
        }

        private List<WktToProjRecord> table = new List<WktToProjRecord>();

        private CoordinateSystemFactory factory;
        private CoordinateSystem coordinateSystem;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="factory"></param>
        public WktToProjConverter(CoordinateSystemFactory factory = null)
        {
            this.factory = factory ?? new CoordinateSystemFactory();
        }


        /// <summary>
        /// Handler for WktAuthority.
        /// </summary>
        /// <param name="authority"></param>
        public void Handle(WktAuthority authority)
        {
            table.Add(new WktToProjRecord(authority, null));
        }

        /// <summary>
        /// Handler for WktAxis.
        /// </summary>
        /// <param name="axis"></param>
        public void Handle(WktAxis axis)
        {
            var a = new AxisInfo(axis.Name, axis.Direction);
            table.Add(new WktToProjRecord(axis, a));
        }

        /// <summary>
        /// Handler for WktDatum.
        /// </summary>
        /// <param name="datum"></param>
        public void Handle(WktDatum datum)
        {
            table.Add(new WktToProjRecord(datum, null));
        }

        /// <summary>
        /// Handler for WktEllipsoid.
        /// </summary>
        /// <param name="ellipsoid"></param>
        public void Handle(WktEllipsoid ellipsoid)
        {
            table.Add(new WktToProjRecord(ellipsoid, null));
        }

        /// <summary>
        /// Handler for WktSpheroid.
        /// </summary>
        /// <param name="spheroid"></param>
        public void Handle(WktSpheroid spheroid)
        {
            table.Add(new WktToProjRecord(spheroid, null));
        }

        /// <summary>
        /// Handler for WktExtension.
        /// </summary>
        /// <param name="extension"></param>
        public void Handle(WktExtension extension)
        {
            table.Add(new WktToProjRecord(extension, null));
        }

        /// <summary>
        /// Handler for WktUnit.
        /// </summary>
        /// <param name="unit"></param>
        public void Handle(WktUnit unit)
        {
            string authName = unit.Authority?.Name;
            long? authCode = unit.Authority?.Code;

            var u = new Unit(unit.ConversionFactor, unit.Name, authName, authCode.GetValueOrDefault(),
                string.Empty, string.Empty, string.Empty);

            table.Add(new WktToProjRecord(unit, u));
        }

        public void Handle(WktParameter parameter)
        {
            var p = new ProjectionParameter(parameter.Name, parameter.Value);
            table.Add(new WktToProjRecord(parameter, p));
        }

        public void Handle(WktPrimeMeridian meridian)
        {
            table.Add(new WktToProjRecord(meridian, null));
        }

        public void Handle(WktProjection projection)
        {
            table.Add(new WktToProjRecord(projection, null));
        }

        public void Handle(WktToWgs84 toWgs84)
        {
            var wgs84 = new Wgs84ConversionInfo(toWgs84.DxShift, toWgs84.DyShift, toWgs84.DzShift, toWgs84.ExRotation,
                toWgs84.EyRotation, toWgs84.EzRotation, toWgs84.PpmScaling, toWgs84.Description);

            table.Add(new WktToProjRecord(toWgs84, wgs84));
        }

        public void Handle(WktGeocentricCoordinateSystem cs)
        {
            table.Add(new WktToProjRecord(cs, null));
        }

        public void Handle(WktGeographicCoordinateSystem cs)
        {
            table.Add(new WktToProjRecord(cs, null));
        }

        public void Handle(WktProjectedCoordinateSystem cs)
        {
            table.Add(new WktToProjRecord(cs, null));
        }


        /// <summary>
        /// Convert WktObject(s) to Proj CoordinateSystem.
        /// </summary>
        /// <param name="wktObject"></param>
        /// <returns></returns>
        public CoordinateSystem Convert(IWktObject wktObject)
        {
            // Travel the tree and make sure above Handle methods are called.
            wktObject.Traverse(this);

            // Recursive checking and filling the table...
            return (CoordinateSystem)FindOrCreateProjObject(wktObject);
        }

        private object FindOrCreateProjObject<T>(T wkt)
            where T : IWktObject
        {
            var item = table.FirstOrDefault(x => x.WktObject.Equals(wkt));

            // Check if wkt object is in table! (Could be Optional)
            if (item == null)
                return null;

            // Check if already exists
            if (item.ProjObject != null)
                return item.ProjObject;

            // Fill in the gaps where projObject is null;
            // Dispatch to Create methods...
            if (wkt is WktProjectedCoordinateSystem projcs)
                return Create(projcs);
            else if (wkt is WktGeographicCoordinateSystem geogcs)
                return Create(geogcs);
            else if (wkt is WktGeocentricCoordinateSystem geoccs)
                return Create(geoccs);
            else if (wkt is WktProjection prj)
                return Create(prj);
            else if (wkt is WktDatum d)
                return Create(d);
            else if (wkt is WktEllipsoid e)
                return Create(e);
            else if (wkt is WktPrimeMeridian pm)
                return Create(pm);

            return null;
        }

        private object Create(WktProjectedCoordinateSystem projcs)
        {
            var gcs = (GeographicCoordinateSystem)FindOrCreateProjObject(projcs.GeographicCoordinateSystem);
            var p = (Projection) FindOrCreateProjObject(projcs.Projection);

            var u = (Unit) FindOrCreateProjObject(projcs.Unit);
            var lu = (LinearUnit) null;
            if (u != null)
            {
                lu = new LinearUnit(u.ConversionFactor, u.Name, u.Authority, u.AuthorityCode, string.Empty,
                    string.Empty, string.Empty);
            }

            var ax1 = (AxisInfo) FindOrCreateProjObject(projcs.Axes.ElementAtOrDefault(0));
            var ax2 = (AxisInfo) FindOrCreateProjObject(projcs.Axes.ElementAtOrDefault(1));

            var result = this.factory.CreateProjectedCoordinateSystem(projcs.Name, gcs, p, lu, ax1, ax2);
            if (projcs.Authority != null)
            {
                result.Authority = projcs.Authority.Name;
                result.AuthorityCode = projcs.Authority.Code;
            }

            return FillItem(projcs, result);
        }

        private object Create(WktGeographicCoordinateSystem geogcs)
        {
            var u = (Unit) FindOrCreateProjObject(geogcs.AngularUnit);
            AngularUnit au = null;
            if (u != null)
            {
                au = new AngularUnit(u.ConversionFactor, u.Name, u.Authority, u.AuthorityCode, string.Empty,
                    string.Empty, string.Empty);
            }

            var d = (HorizontalDatum) FindOrCreateProjObject(geogcs.HorizontalDatum);
            var pm = (PrimeMeridian) FindOrCreateProjObject(geogcs.PrimeMeridian);
            var ax1 = (AxisInfo) FindOrCreateProjObject(geogcs.Axes.ElementAtOrDefault(0));
            var ax2 = (AxisInfo) FindOrCreateProjObject(geogcs.Axes.ElementAtOrDefault(1));

            var result = this.factory.CreateGeographicCoordinateSystem(geogcs.Name, angularUnit:au, datum: d,
                primeMeridian: pm, axis0: ax1, axis1:ax2);

            return FillItem(geogcs, result);
        }

        private object Create(WktGeocentricCoordinateSystem geoccs)
        {
            var u = (Unit) FindOrCreateProjObject(geoccs.Unit);
            var lu = (LinearUnit) null;
            if (u != null)
            {
                lu = new LinearUnit(u.ConversionFactor, u.Name, u.Authority, u.AuthorityCode, string.Empty,
                    string.Empty, string.Empty);
            }

            var d = (HorizontalDatum) FindOrCreateProjObject(geoccs.Datum);
            var pm = (PrimeMeridian) FindOrCreateProjObject(geoccs.PrimeMeridian);

            var result =
                this.factory.CreateGeocentricCoordinateSystem(geoccs.Name, datum: d, linearUnit: lu, primeMeridian: pm);

            return FillItem(geoccs, result);
        }

        private object Create(WktProjection wkt)
        {
            var pp = new List<ProjectionParameter>();
            // Projection parameters come after Projection in WKT
            var subTable = table
                .SkipWhile(x => !(x.WktObject is WktParameter))
                .TakeWhile(x => x.WktObject is WktParameter)
                .ToList();

            if (subTable.Any())
            {
                foreach (var item in subTable)
                {
                    var p = (ProjectionParameter) FindOrCreateProjObject(item.WktObject);
                    pp.Add(p);
                }
            }

            /*
             Factory method doesn't provide Authority and AuthorityCode and they are not settable afterward.
            var result =
                this.factory.CreateProjection(wkt.Name, string.Empty, pp);
            */
            long ac = -1;
            if (wkt.Authority != null)
            {
                ac = wkt.Authority.Code;
            }

            var result = new Projection(wkt.Name, pp, wkt.Name, wkt.Authority?.Name, ac, string.Empty,
                    string.Empty, string.Empty);

            return FillItem(wkt, result);
        }


        private object Create(WktDatum wkt)
        {
            var wgs84 = (Wgs84ConversionInfo) FindOrCreateProjObject(wkt.ToWgs84);
            var e = (Ellipsoid) FindOrCreateProjObject(wkt.Spheroid);

            var result = this.factory.CreateHorizontalDatum(wkt.Name, DatumType.HD_Geocentric, ellipsoid:e, toWgs84: wgs84);

            return FillItem(wkt, result);
        }


        private object Create(WktEllipsoid wkt)
        {
            var lu = (LinearUnit) null;

            var result = this.factory.CreateEllipsoid(wkt.Name, wkt.SemiMajorAxis, wkt.InverseFlattening, linearUnit: lu);

            return FillItem(wkt, result);
        }


        private object Create(WktPrimeMeridian wkt)
        {
            var au = (AngularUnit) null;

            var result = this.factory.CreatePrimeMeridian(wkt.Name, angularUnit: au, longitude: wkt.Longitude);

            return FillItem(wkt, result);
        }


        private object FillItem(IWktObject wkt, object proj)
        {
            int idx = table.FindIndex(x => x.WktObject.Equals(wkt));
            table[idx].ProjObject = proj;
            return proj;
        }
    }
}
