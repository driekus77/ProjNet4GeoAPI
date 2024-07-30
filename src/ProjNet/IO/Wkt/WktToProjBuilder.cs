using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using ProjNet.CoordinateSystems;

namespace ProjNet.IO.Wkt
{
    public class WktToProjBuilder : WktCrsBaseVisitor<object>
    {
        private readonly CoordinateSystemFactory factory;

        public WktToProjBuilder()
        {
            factory = new CoordinateSystemFactory();
        }


        public class Authority
        {
            public string Name { get; set; }

            public int Code { get; set; }
        }


        public override object VisitName(WktCrsParser.NameContext context)
        {
            return context.GetText().Trim(new char[] {'\"', ' '});
        }

        public override object VisitValue(WktCrsParser.ValueContext context)
        {
            string valueStr = context.GetText();
            if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
                return d;

            return double.NaN;
        }

        public override object VisitLongitude(WktCrsParser.LongitudeContext context)
        {
            string valueStr = context.GetText();
            if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
                return d;

            return double.NaN;
        }

        public override object VisitAuthorityName(WktCrsParser.AuthorityNameContext context)
        {
            return context.GetText().Trim(new char[] {'\"', ' '});
        }

        public override object VisitCode(WktCrsParser.CodeContext context)
        {
            if (!context.IsEmpty)
            {
                string codeStr = context.GetText();
                codeStr = codeStr.Trim(new char[] {' ', '\"'});
                if (codeStr.Contains("_"))
                {
                    codeStr = codeStr.Substring(0, codeStr.IndexOf('_'));
                }

                if (!string.IsNullOrWhiteSpace(codeStr) && int.TryParse(codeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out int nmbr))
                {
                    return nmbr;
                }
            }

            return -1;
        }

        public override object VisitAxisOrient(WktCrsParser.AxisOrientContext context)
        {

            if (!context.IsEmpty)
            {
                string direction = context.GetText().Trim(new char[]{' ', '\"'});
                if (Enum.TryParse(direction, true, out AxisOrientationEnum enumVal))
                {
                    return enumVal;
                }
            }

            return AxisOrientationEnum.Other;
        }

        public override object VisitAuthority([NotNull] WktCrsParser.AuthorityContext context)
        {
            string authName = string.Empty;
            if (context.authorityName() != null)
                authName = (string)Visit(context.authorityName());

            int code = -1;
            if (context.code() != null)
                code = (int) Visit(context.code());

            return new Authority {Name = authName, Code = code};
        }

        public override object VisitAxis(WktCrsParser.AxisContext context)
        {
            string name = string.Empty;
            if (context.name()!=null)
                name = (string) Visit(context.name());

            var orient = AxisOrientationEnum.Other;
            if (context.axisOrient()!=null)
                orient = (AxisOrientationEnum) Visit(context.axisOrient());

            return new AxisInfo(name, orient);
        }

        public override object VisitExtension([NotNull] WktCrsParser.ExtensionContext context)
        {
            string name = string.Empty;
            if (context.name() != null)
                name = (string) Visit(context.name());

            string projText = string.Empty;
            if (context.projtext() != null)
                projText = (string) Visit(context.projtext());

            // No ProjNet object for Extension so returning a tuple.
            return (name, projText);
        }

        public override object VisitTowgs84(WktCrsParser.Towgs84Context context)
        {
            double dx = 0.0d;
            if (!context.dx().IsEmpty && double.TryParse(context.dx().GetText(), NumberStyles.Any, CultureInfo.InvariantCulture, out double dxResult ))
                dx = dxResult;

            double dy = 0.0d;
            if (!context.dy().IsEmpty && double.TryParse(context.dy().GetText(), NumberStyles.Any, CultureInfo.InvariantCulture, out double dyResult ))
                dy = dyResult;

            double dz = 0.0d;
            if (!context.dz().IsEmpty && double.TryParse(context.dz().GetText(), NumberStyles.Any, CultureInfo.InvariantCulture, out double dzResult ))
                dz = dzResult;

            double ex = 0.0d;
            if (!context.ex().IsEmpty && double.TryParse(context.ex().GetText(), NumberStyles.Any, CultureInfo.InvariantCulture, out double exResult ))
                ex = exResult;

            double ey = 0.0d;
            if (!context.ey().IsEmpty && double.TryParse(context.ey().GetText(), NumberStyles.Any, CultureInfo.InvariantCulture, out double eyResult ))
                ey = eyResult;

            double ez = 0.0d;
            if (!context.ez().IsEmpty && double.TryParse(context.ez().GetText(), NumberStyles.Any, CultureInfo.InvariantCulture, out double ezResult ))
                ez = ezResult;

            double ppm = 0.0d;
            if (!context.ppm().IsEmpty && double.TryParse(context.ppm().GetText(), NumberStyles.Any, CultureInfo.InvariantCulture, out double ppmResult ))
                ppm = ppmResult;

            return new Wgs84ConversionInfo(dx, dy, dz, ex, ey, ez, ppm);
        }

        public override object VisitProjection(WktCrsParser.ProjectionContext context)
        {
            string name = string.Empty;
            if (context.name() != null)
                name = (string) Visit(context.name());

            Authority authority = null;
            if (context.authority() != null)
                authority = (Authority) Visit(context.authority());

            return new Projection(name, new List<ProjectionParameter>(), name, authority?.Name, authority!=null ? authority.Code:-1, string.Empty, string.Empty, string.Empty);
        }

        public override object VisitParameter(WktCrsParser.ParameterContext context)
        {
            string name = string.Empty;
            if (context.name() != null)
                name = (string) Visit(context.name());

            double value = double.NaN;
            if (context.value() != null)
                value = (double) Visit(context.value());

            return new ProjectionParameter(name, value);
        }


        public override object VisitSpheroid(WktCrsParser.SpheroidContext context)
        {
            string name = string.Empty;
            if (context.name() != null)
                name = (string) Visit(context.name());

            double semiMajorAxis = double.NaN;
            if (context.semiMajorAxis() != null)
                semiMajorAxis = (double)Visit(context.semiMajorAxis());

            double inverseFlattening = double.NaN;
            if (context.inverseFlattening() != null)
                inverseFlattening = (double)Visit(context.inverseFlattening());

            Authority authority = null;
            if (context.authority() != null)
                authority = (Authority) Visit(context.authority());

            return new Ellipsoid(semiMajorAxis, 0.0, inverseFlattening, true, LinearUnit.Metre, name,
                authority?.Name, authority!=null?authority.Code:-1, string.Empty, string.Empty, string.Empty);

        }

        public override object VisitSemiMajorAxis(WktCrsParser.SemiMajorAxisContext context)
        {
            if (!context.IsEmpty && double.TryParse(context.GetText(), NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
                return d;
            return double.NaN;
        }

        public override object VisitInverseFlattening(WktCrsParser.InverseFlatteningContext context)
        {
            if (!context.IsEmpty && double.TryParse(context.GetText(), NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
                return d;
            return double.NaN;
        }

        public override object VisitConversionFactor(WktCrsParser.ConversionFactorContext context)
        {
            if (!context.IsEmpty && double.TryParse(context.GetText(), NumberStyles.Any, CultureInfo.InvariantCulture, out double d))
                return d;
            return double.NaN;
        }


        public override object VisitDatum(WktCrsParser.DatumContext context)
        {
            string name = string.Empty;
            if (context.name() != null)
                name = (string) Visit(context.name());

            Ellipsoid spheroid = null;
            if (context.spheroid() != null)
                spheroid = (Ellipsoid) Visit(context.spheroid());

            Wgs84ConversionInfo wgs84 = null;
            if (context.towgs84() != null)
                wgs84 = (Wgs84ConversionInfo) Visit(context.towgs84());

            Authority authority = null;
            if (context.authority() != null)
            {
                authority = (Authority) Visit(context.authority());
            }

            var result = this.factory.CreateHorizontalDatum(
                name, DatumType.HD_Geocentric, ellipsoid: spheroid, toWgs84: wgs84);

            if (authority != null)
            {
                result.Authority = authority.Name;
                result.AuthorityCode = authority.Code;
            }

            return result;
        }

        public override object VisitUnit(WktCrsParser.UnitContext context)
        {
            string name = string.Empty;
            if (context.name() != null)
                name = (string) Visit(context.name());

            double factor = double.NaN;
            if (context.conversionFactor() != null)
                factor = (double) Visit(context.conversionFactor());

            Authority authority = null;
            if (context.authority() != null)
                authority = (Authority) Visit(context.authority());


            return new Unit(factor,name, authority?.Name, authority!=null?authority.Code:-1 , string.Empty, string.Empty, string.Empty);
        }

        public override object VisitPrimem(WktCrsParser.PrimemContext context)
        {
            string name = string.Empty;
            if (context.name() != null)
                name = (string) Visit(context.name());

            double longitude = double.NaN;
            if (context.longitude() != null)
                longitude = (double) Visit(context.longitude());

            AngularUnit au = null;
            if (context.unit() != null)
            {
                var unit = (Unit) Visit(context.unit());
                if (unit != null && unit.Name.Equals("degree"))
                {
                    au = new AngularUnit(unit.ConversionFactor, unit.Name, unit.Authority, unit.AuthorityCode,
                        string.Empty, string.Empty, string.Empty);
                }
            }


            Authority authority = null;
            if (context.authority() != null)
            {
                authority = (Authority) Visit(context.authority());
            }

            var result = this.factory.CreatePrimeMeridian(name, angularUnit: au, longitude: longitude);

            if (authority is Authority authObj)
            {
                result.AuthorityCode = authObj.Code;
                result.Authority = authObj.Name;
            }

            return result;
        }

        public override object VisitGeogcs(WktCrsParser.GeogcsContext context)
        {
            string name = string.Empty;
            if (context.name() != null)
                name = (string) Visit(context.name());

            AngularUnit au = null;
            if (context.unit() != null)
            {
                var unit = (Unit) Visit(context.unit());
                if (unit != null && unit.Name.Equals("degree"))
                {
                    au = new AngularUnit(unit.ConversionFactor, unit.Name, unit.Authority, unit.AuthorityCode,
                        string.Empty, string.Empty, string.Empty);
                }
            }

            HorizontalDatum datum = null;
            if (context.datum() != null)
                datum = (HorizontalDatum) Visit(context.datum());

            PrimeMeridian pm = null;
            if (context.primem() != null)
                pm = (PrimeMeridian) Visit(context.primem());

            //This is default axis values if not specified.
            var axisCtx = context.axis();
            var ax1 = axisCtx.Length > 0 ? (AxisInfo) Visit(axisCtx[0]) : new AxisInfo("Lon", AxisOrientationEnum.East);
            var ax2 = axisCtx.Length > 1 ? (AxisInfo) Visit(axisCtx[1]) : new AxisInfo("Lat", AxisOrientationEnum.North);

            Authority authority = null;
            if (context.authority() != null)
            {
                authority = (Authority) Visit(context.authority());
            }

            var result = this.factory.CreateGeographicCoordinateSystem(name, au, (HorizontalDatum)datum,
                (PrimeMeridian)pm, axis0: ax1, axis1: ax2);

            if (authority is Authority authObj)
            {
                result.AuthorityCode = authObj.Code;
                result.Authority = authObj.Name;
            }

            return result;
        }

        public override object VisitGeoccs(WktCrsParser.GeoccsContext context)
        {
            string name = string.Empty;
            if (context.name() != null)
                name = (string) Visit(context.name());

            HorizontalDatum datum = null;
            if (context.datum() != null)
                datum = (HorizontalDatum) Visit(context.datum());

            PrimeMeridian meridian = null;
            if (context.primem() != null)
                meridian = (PrimeMeridian) Visit(context.primem());

            var lu = (LinearUnit) null;
            if (context.unit() != null)
            {
                var u = (ProjNet.CoordinateSystems.Unit) Visit(context.unit());
                if (u != null)
                {
                    lu = new LinearUnit(u.ConversionFactor, u.Name, u.Authority, u.AuthorityCode, string.Empty,
                        string.Empty, string.Empty);
                }
            }

            Authority authority = null;
            if (context.authority() != null)
            {
                authority = (Authority) Visit(context.authority());
            }

            var result =
                this.factory.CreateGeocentricCoordinateSystem(name, (HorizontalDatum)datum, lu, (PrimeMeridian)meridian);

            if (authority is Authority authObj)
            {
                result.AuthorityCode = authObj.Code;
                result.Authority = authObj.Name;
            }

            return result;
        }

        public override object VisitProjcs(WktCrsParser.ProjcsContext context)
        {
            string name = string.Empty;
            if (context.name() != null)
                name = (string) Visit(context.name());

            GeographicCoordinateSystem gcs = null;
            if (context.geogcs() != null)
                gcs = (GeographicCoordinateSystem) Visit(context.geogcs());

            var lu = (LinearUnit) null;
            if (context.unit() != null)
            {
                var u = (ProjNet.CoordinateSystems.Unit) Visit(context.unit());
                if (u != null)
                {
                    lu = new LinearUnit(u.ConversionFactor, u.Name, u.Authority, u.AuthorityCode, string.Empty,
                        string.Empty, string.Empty);
                }
            }

            Projection p = null;
            if (context.projection() != null)
                p = (Projection) Visit(context.projection());


            var axisCtx = context.axis();
            var ax1 = axisCtx.Length > 0 ? (AxisInfo) Visit(axisCtx[0]) : new AxisInfo("X", AxisOrientationEnum.East);
            var ax2 = axisCtx.Length > 1 ? (AxisInfo) Visit(axisCtx[1]) : new AxisInfo("Y", AxisOrientationEnum.North);
            var aa = new List<AxisInfo> {ax1, ax2};

            Authority authority = null;
            if (context.authority() != null)
            {
                authority = (Authority) Visit(context.authority());
            }

            var result = new ProjectedCoordinateSystem(gcs.HorizontalDatum, gcs, lu, p,
                    aa, name, authority?.Name, authority!=null?authority.Code:-1, string.Empty, string.Empty, string.Empty);

            return result;
        }

        public WktCrsParser.WktContext ParseAndBuild(string input)
        {
            var stream = CharStreams.fromString(input);
            var lexer = new WktCrsLexer(stream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new WktCrsParser(tokens);

            var wktContext = parser.wkt();
            this.Visit(wktContext);

            return wktContext;
        }
    }
}
