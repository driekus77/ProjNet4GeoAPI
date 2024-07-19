using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal.Commands;
using Pidgin;
using ProjNet.CoordinateSystems;
using ProjNet.Wkt;
using ProjNet.Wkt.Tree;

namespace ProjNET.Tests.WKT;

public class WktTextReaderTests
{
    [Test]
    public void TestAxisParser()
    {
        // Arrange
        string parseText01 = @"AXIS[""Geodetic latitude"", NORTH]";

        // Act
        using var sr = new StringReader(parseText01);
        using var reader = new WktTextReader(sr);
        var axisInfo = reader.Parser.AxisParser.ParseOrThrow(parseText01);

        // Assert
        Assert.AreEqual("Geodetic latitude", axisInfo.Name);
        Assert.AreEqual(AxisOrientationEnum.North, axisInfo.Direction);
    }

    [Test]
    public void TestAuthorityParser()
    {
        // Arrange
        string parseText01 = "AUTHORITY[\"EPSG\", \"4152\"]";

        // Act
        using var sr = new StringReader(parseText01);
        using var reader = new WktTextReader(sr);
        var auth = reader.Parser.AuthorityParser.ParseOrThrow(parseText01);

        // Assert
        Assert.AreEqual("EPSG", auth.Name);
        Assert.AreEqual(4152, auth.Code);
    }

    [Test]
    public void TestUnitParser()
    {
        // Arrange
        string parseText01 = "UNIT[\"degree\", 0.0174532925199433, AUTHORITY[\"EPSG\", \"9122\"]]";
        //string parseText01 = "UNIT[\"US survey foot\", 0.304800609601219, AUTHORITY[\"EPSG\", \"9003\"]]";

        // Act
        using var sr = new StringReader(parseText01);
        using var reader = new WktTextReader(sr);
        var unit = reader.Parser.UnitParser.ParseOrThrow(parseText01);

        // Assert
        Assert.AreEqual("degree", unit.Name);
        Assert.AreEqual(0.0174532925199433, unit.ConversionFactor);
        Assert.AreEqual("EPSG", unit.Authority.Name);
        Assert.AreEqual(9122, unit.Authority.Code);
    }

    [Test]
    public void TestDatumParser()
    {
        // Arrange
        string parseText01 = "DATUM[\"NAD83_High_Accuracy_Regional_Network\", \n" +
                             "SPHEROID[\"GRS 1980\", 6378137, 298.257222101, AUTHORITY[\"EPSG\", \"7019\"]], \n" +
                             "TOWGS84[725, 685, 536, 0, 0, 0, 0], AUTHORITY[\"EPSG\", \"6152\"]]";

        // Act
        using var sr = new StringReader(parseText01);
        using var reader = new WktTextReader(sr);
        var datum = reader.Parser.DatumParser.ParseOrThrow(parseText01);

        // Assert
        Assert.AreEqual("NAD83_High_Accuracy_Regional_Network", datum.Name);
        Assert.AreEqual(6152, datum.Authority.Code);
    }

    [Test]
    public void TestGeogCSParser()
    {
        // Arrange
        string parseText01 = "GEOGCS[\"NAD83(HARN)\", \n" +
                             "DATUM[\"NAD83_High_Accuracy_Regional_Network\", \n" +
                             "SPHEROID[\"GRS 1980\", 6378137, 298.257222101, AUTHORITY[\"EPSG\", \"7019\"]], \n" +
                             "TOWGS84[725, 685, 536, 0, 0, 0, 0], AUTHORITY[\"EPSG\", \"6152\"]], \n" +
                             "PRIMEM[\"Greenwich\", 0, AUTHORITY[\"EPSG\", \"8901\"]], \n" +
                             "UNIT[\"degree\", 0.0174532925199433, AUTHORITY[\"EPSG\", \"9122\"]], \n" +
                             "AUTHORITY[\"EPSG\", \"4152\"]]";

        // Act
        using var sr = new StringReader(parseText01);
        using var reader = new WktTextReader(sr);
        var cs = reader.Parser.GeographicCsParser.ParseOrThrow(parseText01);

        // Assert
        Assert.AreEqual("NAD83(HARN)", cs.Name);
    }


    [Test]
    public void TestProjectedCoordinateSystem_EPSG_2918()
    {
        string parseText01 =
            "PROJCS[\"NAD83(HARN) / Texas Central (ftUS)\", \n" +
            "GEOGCS[\"NAD83(HARN)\", \n" +
            "DATUM[\"NAD83_High_Accuracy_Regional_Network\", \n" +
            "SPHEROID[\"GRS 1980\", 6378137, 298.257222101, AUTHORITY[\"EPSG\", \"7019\"]], \n" +
            "TOWGS84[725, 685, 536, 0, 0, 0, 0], AUTHORITY[\"EPSG\", \"6152\"]], \n" +
            "PRIMEM[\"Greenwich\", 0, AUTHORITY[\"EPSG\", \"8901\"]], \n" +
            "UNIT[\"degree\", 0.0174532925199433, AUTHORITY[\"EPSG\", \"9122\"]], \n" +
            "AUTHORITY[\"EPSG\", \"4152\"]], \n" +
            "PROJECTION[\"Lambert_Conformal_Conic_2SP\"], \n" +
            "PARAMETER[\"standard_parallel_1\", 31.883333333333], \n" +
            "PARAMETER[\"standard_parallel_2\", 30.1166666667], \n" +
            "PARAMETER[\"latitude_of_origin\", 29.6666666667], \n" +
            "PARAMETER[\"central_meridian\", -100.333333333333], \n" +
            "PARAMETER[\"false_easting\", 2296583.333], \n" +
            "PARAMETER[\"false_northing\", 9842500], \n" +
            "UNIT[\"US survey foot\", 0.304800609601219, AUTHORITY[\"EPSG\", \"9003\"]], \n" +
            "AUTHORITY[\"EPSG\", \"2918\"]]";

        using var sr = new StringReader(parseText01);
        using var reader = new WktTextReader(sr);

        // Act
        var result = reader.Parser.ProjectedCsParser.ParseOrThrow(parseText01);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual("NAD83(HARN) / Texas Central (ftUS)", result.Name);

        Assert.AreEqual("US survey foot", result.Unit.Name);
        Assert.AreEqual(0.304800609601219, result.Unit.ConversionFactor);
        Assert.AreEqual("EPSG", result.Unit.Authority.Name);
        Assert.AreEqual(9003, result.Unit.Authority.Code);

        var parameter = result.Parameters.First(p => p.Name.Equals("central_meridian"));
        Assert.AreEqual(-100.333333333333, parameter.Value);

        Assert.AreEqual("EPSG", result.Authority.Name);
        Assert.AreEqual(2918, result.Authority.Code);
    }


    [Test]
    public void TestProjectedCoordinateSystem_EPSG_3067()
    {
        string parseText01 =
            "PROJCS[\"ETRS89 / TM35FIN(E,N)\"," +
            "   GEOGCS[\"ETRS89\"," +
            "       DATUM[\"European_Terrestrial_Reference_System_1989\"," +
            "           SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]]," +
            "           TOWGS84[0,0,0,0,0,0,0]," +
            "           AUTHORITY[\"EPSG\",\"6258\"]]," +
            "       PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]]," +
            "       UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]]," +
            "       AUTHORITY[\"EPSG\",\"4258\"]]," +
            "   PROJECTION[\"Transverse_Mercator\"]," +
            "   PARAMETER[\"latitude_of_origin\",0]," +
            "   PARAMETER[\"central_meridian\",27]," +
            "   PARAMETER[\"scale_factor\",0.9996]," +
            "   PARAMETER[\"false_easting\",500000]," +
            "   PARAMETER[\"false_northing\",0]," +
            "   UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]]," +
            "   AXIS[\"Easting\",EAST]," +
            "   AXIS[\"Northing\",NORTH]," +
            "   AUTHORITY[\"EPSG\",\"3067\"]]";

        using var sr = new StringReader(parseText01);
        using var reader = new WktTextReader(sr);

        // Act
        var result = reader.Parser.ProjectedCsParser.ParseOrThrow(parseText01);

        // Assert
        Assert.NotNull(result);
    }


    [Test]
    public void TestCoordinateSystem_EPSG_3822()
    {
        string parseText01 =
            "GEOCCS[\"TWD97\",\n" +
            "   DATUM[\"Taiwan_Datum_1997\",\n" +
            "       SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],\n" +
            "       AUTHORITY[\"EPSG\",\"1026\"]],\n" +
            "   PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],\n" +
            "   UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],\n" +
            "   AXIS[\"Geocentric X\",OTHER],\n" +
            "   AXIS[\"Geocentric Y\",OTHER],\n" +
            "   AXIS[\"Geocentric Z\",NORTH],\n" +
            "   AUTHORITY[\"EPSG\",\"3822\"]]";

        using var sr = new StringReader(parseText01);
        using var reader = new WktTextReader(sr);

        // Act
        var result = reader.Parser.GeocentricCsParser.ParseOrThrow(parseText01);

        // Assert
        Assert.NotNull(result);
    }

    [Test]
    public void TestCoordinateSystem_EPSG_8351()
    {
        string parseText01 =
            "GEOGCS[\"S-JTSK [JTSK03]\",DATUM[\"System_of_the_Unified_Trigonometrical_Cadastral_Network_JTSK03\",SPHEROID[\"Bessel 1841\",6377397.155,299.1528128,AUTHORITY[\"EPSG\",\"7004\"]],TOWGS84[485.021,169.465,483.839,7.786342,4.397554,4.102655,0],AUTHORITY[\"EPSG\",\"1201\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AXIS[\"Latitude\",NORTH],AXIS[\"Longitude\",EAST],AUTHORITY[\"EPSG\",\"8351\"]]";

        using var sr = new StringReader(parseText01);
        using var reader = new WktTextReader(sr);

        // Act
        var result = reader.Parser.SpatialReferenceSystemParser.ParseOrThrow(parseText01);

        // Assert
        Assert.NotNull(result);
    }

    [Test]
    public void TestCoordinateSystem_EPSG_32161()
    {
        string parseText01 =
            "PROJCS[\"NAD83 / Puerto Rico & Virgin Is.\",\n" +
            "   GEOGCS[\"NAD83\",\n" +
            "       DATUM[\"North_American_Datum_1983\",\n" +
            "           SPHEROID[\"GRS 1980\",6378137,298.257222101,AUTHORITY[\"EPSG\",\"7019\"]],\n" +
            "           TOWGS84[0,0,0,0,0,0,0],\n" +
            "           AUTHORITY[\"EPSG\",\"6269\"]],\n" +
            "       PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],\n" +
            "       UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],\n" +
            "       AUTHORITY[\"EPSG\",\"4269\"]],\n" +
            "   PROJECTION[\"Lambert_Conformal_Conic_2SP\"],\n" +
            "   PARAMETER[\"standard_parallel_1\",18.43333333333333],\n" +
            "   PARAMETER[\"standard_parallel_2\",18.03333333333333],\n" +
            "   PARAMETER[\"latitude_of_origin\",17.83333333333333],\n" +
            "   PARAMETER[\"central_meridian\",-66.43333333333334],\n" +
            "   PARAMETER[\"false_easting\",200000],\n" +
            "   PARAMETER[\"false_northing\",200000],\n" +
            "   UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],\n" +
            "   AXIS[\"X\",EAST],\n" +
            "   AXIS[\"Y\",NORTH],\n" +
            "   AUTHORITY[\"EPSG\",\"32161\"]]";

        using var sr = new StringReader(parseText01);
        using var reader = new WktTextReader(sr);

        // Act
        var result = reader.Parser.SpatialReferenceSystemParser.ParseOrThrow(parseText01);

        // Assert
        Assert.NotNull(result);
    }

    [Test]
    public void TestCoordinateSystem_EPSG_102100()
    {
        string parseText01 =
            "PROJCS[\"WGS_1984_Web_Mercator_Auxiliary_Sphere (deprecated)\",\n" +
            "   GEOGCS[\"WGS 84\",\n" +
            "       DATUM[\"WGS_1984\",\n" +
            "           SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],\n" +
            "           AUTHORITY[\"EPSG\",\"6326\"]],\n" +
            "   PRIMEM[\"Greenwich\",0],\n" +
            "   UNIT[\"Degree\",0.0174532925199433]],\n" +
            "   PROJECTION[\"Mercator_1SP\"],\n" +
            "   PARAMETER[\"central_meridian\",0],\n" +
            "PARAMETER[\"scale_factor\",1],\n" +
            "PARAMETER[\"false_easting\",0],\n" +
            "PARAMETER[\"false_northing\",0],\n" +
            "UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],\n" +
            "AXIS[\"Easting\",EAST],\n" +
            "AXIS[\"Northing\",NORTH],\n" +
            "EXTENSION[\"PROJ4\",\"+proj=merc +a=6378137 +b=6378137 +lat_ts=0 +lon_0=0 +x_0=0 +y_0=0 +k=1 +units=m +nadgrids=@null +wktext +no_defs\"],\n" +
            "AUTHORITY[\"ESRI\",\"102100\"]]";

        using var sr = new StringReader(parseText01);
        using var reader = new WktTextReader(sr);

        // Act
        var result = reader.Parser.SpatialReferenceSystemParser.ParseOrThrow(parseText01);

        // Assert
        Assert.NotNull(result);
    }

    [Test]
    public void TestParseAllWKTs_in_CSV()
    {
        int parseCount = 0;
        foreach (var wkt in SRIDReader.GetSrids())
        {
            var reader01 = new WktTextReader(new StringReader(wkt.Wkt));
            var result01 = reader01.ReadToEnd();
            var cs01 = result01.GetValueOrDefault();
            Assert.IsNotNull(cs01, "Could not parse WKT: " + wkt.Wkt);
            Assert.That(result01.Success, Is.True);

            using var reader02 = new WktTextReader(new StringReader(wkt.Wkt));//.Replace("[", "(").Replace("]", ")")));
            var result02 = reader02.ReadToEnd();
            Assert.That(result02.Success, Is.True);
            var cs02 = result02.GetValueOrDefault();

            //Assert.That(cs01.Equals(cs02), Is.True);
            parseCount++;
        }
        Assert.That(parseCount, Is.GreaterThan(2671), "Not all WKT was parsed");
    }

    /// <summary>
    /// Test parsing of a <see cref="ProjectedCoordinateSystem"/> from WKT
    /// </summary>
    [Test]
    public void TestProjectedCoordinateSystem_EPSG27700_UnitBeforeProjection()
    {
        const string wkt = "PROJCS[\"OSGB 1936 / British National Grid\",\n" +
                           "    GEOGCS[\"OSGB 1936\",\n" +
                           "        DATUM[\"OSGB_1936\",\n" +
                           "            SPHEROID[\"Airy 1830\",6377563.396,299.3249646,AUTHORITY[\"EPSG\",\"7001\"]],\n" +
                           "            AUTHORITY[\"EPSG\",\"6277\"]],\n" +
                           "        PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],\n" +
                           "        UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],\n" +
                           "        AUTHORITY[\"EPSG\",\"4277\"]],\n" +
                           "        PROJECTION[\"Transverse_Mercator\"],\n" +
                           "        PARAMETER[\"latitude_of_origin\",49],\n" +
                           "        PARAMETER[\"central_meridian\",-2],\n" +
                           "        PARAMETER[\"scale_factor\",0.9996012717],\n" +
                           "        PARAMETER[\"false_easting\",400000],\n" +
                           "        PARAMETER[\"false_northing\",-100000],\n" +
                           "        UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],\n" +
                           "        AXIS[\"Easting\",EAST],\n" +
                           "        AXIS[\"Northing\",NORTH],\n" +
                           "        AUTHORITY[\"EPSG\",\"27700\"]]";

        WktProjectedCoordinateSystem pcs = null;
        var reader01 = new WktTextReader(new StringReader(wkt));
        var result01 = reader01.ReadToEnd();
        Assert.IsTrue(result01.Success, !result01.Success ? result01.Error.RenderErrorMessage() : "Error!");
        Assert.That(() => pcs = result01.Value as WktProjectedCoordinateSystem, Throws.Nothing);

        WktProjectedCoordinateSystem pcs2 = null;
        Assert.That(
            () =>
            {
                var reader01 = new WktTextReader(new StringReader(wkt.Replace("[", "(").Replace("]", ")")));
                var result01 = reader01.ReadToEnd();
                pcs2 = result01.Value as WktProjectedCoordinateSystem;
            }, Throws.Nothing);
        Assert.That(pcs.Equals(pcs2), Is.True);


        Assert.AreEqual("OSGB 1936 / British National Grid", pcs.Name);
        Assert.AreEqual("EPSG", pcs.Authority.Name);
        Assert.AreEqual(27700, pcs.Authority.Code);

        var gcs = pcs.GeographicCoordinateSystem;
        Assert.AreEqual("OSGB 1936", gcs.Name);
        Assert.AreEqual("EPSG", gcs.Authority.Name);
        Assert.AreEqual(4277, gcs.Authority.Code);

        //CheckDatum(gcs.HorizontalDatum, "OSGB_1936", "EPSG", 6277);
        Assert.AreEqual("OSGB_1936", gcs.HorizontalDatum.Name);
        Assert.AreEqual("EPSG", gcs.HorizontalDatum.Authority.Name);
        Assert.AreEqual(6277, gcs.HorizontalDatum.Authority.Code);

        //CheckEllipsoid(gcs.HorizontalDatum.Ellipsoid, "Airy 1830", 6377563.396, 299.3249646, "EPSG", 7001);
        Assert.AreEqual("Airy 1830", gcs.HorizontalDatum.Spheroid.Name);
        Assert.AreEqual(6377563.396, gcs.HorizontalDatum.Spheroid.SemiMajorAxis);
        Assert.AreEqual(299.3249646, gcs.HorizontalDatum.Spheroid.InverseFlattening);
        Assert.AreEqual("EPSG", gcs.HorizontalDatum.Spheroid.Authority.Name);
        Assert.AreEqual(7001, gcs.HorizontalDatum.Spheroid.Authority.Code);

        //CheckPrimem(gcs.PrimeMeridian, "Greenwich", 0, "EPSG", 8901);
        Assert.AreEqual("Greenwich", gcs.PrimeMeridian.Name);
        Assert.AreEqual(0, gcs.PrimeMeridian.Longitude);
        Assert.AreEqual("EPSG", gcs.PrimeMeridian.Authority.Name);
        Assert.AreEqual(8901, gcs.PrimeMeridian.Authority.Code);

        //CheckUnit(gcs.AngularUnit, "degree", 0.0174532925199433, "EPSG", 9122);
        Assert.AreEqual("degree", gcs.Unit.Name);
        Assert.AreEqual(0.0174532925199433, gcs.Unit.ConversionFactor);
        Assert.AreEqual("EPSG", gcs.Unit.Authority.Name);
        Assert.AreEqual(9122, gcs.Unit.Authority.Code);

        //Assert.AreEqual("Transverse_Mercator", pcs.Projection.ClassName, "Projection Classname");
        Assert.AreEqual("Transverse_Mercator", pcs.Projection.Name);
        //Assert.AreEqual("Projection Classname", pcs.Projection.Name); // <= Wkt related?

       /*
        CheckProjection(pcs.Projection, "Transverse_Mercator", new[]
        {
            Tuple.Create("latitude_of_origin", 49d),
            Tuple.Create("central_meridian", -2d),
            Tuple.Create("scale_factor", 0.9996012717),
            Tuple.Create("false_easting", 400000d),
            Tuple.Create("false_northing", -100000d)
        });
        */

        //CheckUnit(pcs.LinearUnit, "metre", 1d, "EPSG", 9001);
        Assert.AreEqual("metre", pcs.Unit.Name);
        Assert.AreEqual(1d, pcs.Unit.ConversionFactor);
        Assert.AreEqual("EPSG", pcs.Unit.Authority.Name);
        Assert.AreEqual(9001, pcs.Unit.Authority.Code);

        //string newWkt = pcs.WKT.Replace(", ", ",");
        //Assert.AreEqual(wkt, newWkt);
    }


    [Test]
    public void TestCoordinateSystem_EPSG28992()
    {
        // Arrange
        string text = @"PROJCS[""Amersfoort / RD New"", GEOGCS[""Amersfoort"",
    DATUM[""Amersfoort"",SPHEROID[""Bessel 1841"", 6377397.155, 299.1528128, AUTHORITY[""EPSG"",""7004""]],
      TOWGS84[565.2369, 50.0087, 465.658, -0.40685733032239757, -0.3507326765425626, 1.8703473836067956, 4.0812],
      AUTHORITY[""EPSG"",""6289""]],
    PRIMEM[""Greenwich"", 0.0, AUTHORITY[""EPSG"",""8901""]],
    UNIT[""degree"", 0.017453292519943295],
    AXIS[""Geodetic latitude"", NORTH],
    AXIS[""Geodetic longitude"", EAST],
    AUTHORITY[""EPSG"",""4289""]],
  PROJECTION[""Oblique_Stereographic"", AUTHORITY[""EPSG"",""9809""]],
  PARAMETER[""central_meridian"", 5.387638888888891],
  PARAMETER[""latitude_of_origin"", 52.15616055555556],
  PARAMETER[""scale_factor"", 0.9999079],
  PARAMETER[""false_easting"", 155000.0],
  PARAMETER[""false_northing"", 463000.0],
  UNIT[""m"", 1.0],
  AXIS[""Easting"", EAST],
  AXIS[""Northing"", NORTH],
  AUTHORITY[""EPSG"",""28992""]]";
        using var sr = new StringReader(text);
        using var reader = new WktTextReader(sr);

        // Act
        var resultCs = reader.ReadToEnd();

        // Assert
        Assert.NotNull(resultCs);
        Assert.IsTrue(resultCs.Success);//, resultCs.Error.RenderErrorMessage());
    }


    [Test]
    public void TestParseSrOrg()
    {
        // Arrange
        string text = "PROJCS[\"WGS 84 / Pseudo-Mercator\",GEOGCS[\"Popular Visualisation CRS\"," +
                      "DATUM[\"Popular_Visualisation_Datum\",SPHEROID[\"Popular Visualisation Sphere\"," +
                      "6378137,0,AUTHORITY[\"EPSG\",\"7059\"]],TOWGS84[0,0,0,0,0,0,0],AUTHORITY[\"EPSG\"," +
                      "\"6055\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\"," +
                      "0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4055\"]]," +
                      "PROJECTION[\"Mercator_1SP\"]," +
                      "PARAMETER[\"central_meridian\",0],PARAMETER[\"scale_factor\",1],PARAMETER[" +
                      "\"false_easting\",0],PARAMETER[\"false_northing\",0],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"X\",EAST],AXIS[\"Y\",NORTH]],AUTHORITY[\"EPSG\",\"3785\"]"
            ;
        using var sr = new StringReader(text);
        using var reader = new WktTextReader(sr);

        // Act
        var resultCs = reader.ReadToEnd();

        // Assert
        Assert.NotNull(resultCs);
        Assert.IsTrue(resultCs.Success);
    }


    [Test]
    public void TestProjNetIssues()
    {
        // Arrange
        string text =  "PROJCS[\"International_Terrestrial_Reference_Frame_1992Lambert_Conformal_Conic_2SP\"," +
                       "GEOGCS[\"GCS_International_Terrestrial_Reference_Frame_1992\"," +
                       "DATUM[\"International_Terrestrial_Reference_Frame_1992\"," +
                       "SPHEROID[\"GRS_1980\",6378137,298.257222101]," +
                       "TOWGS84[0,0,0,0,0,0,0]]," +
                       "PRIMEM[\"Greenwich\",0]," +
                       "UNIT[\"Degree\",0.0174532925199433]]," +
                       "PROJECTION[\"Lambert_Conformal_Conic_2SP\",AUTHORITY[\"EPSG\",\"9802\"]]," +
                       "PARAMETER[\"Central_Meridian\",-102]," +
                       "PARAMETER[\"Latitude_Of_Origin\",12]," +
                       "PARAMETER[\"False_Easting\",2500000]," +
                       "PARAMETER[\"False_Northing\",0]," +
                       "PARAMETER[\"Standard_Parallel_1\",17.5]," +
                       "PARAMETER[\"Standard_Parallel_2\",29.5]," +
                       "PARAMETER[\"Scale_Factor\",1]," +
                       "UNIT[\"Meter\",1,AUTHORITY[\"EPSG\",\"9001\"]]]";
        using var sr = new StringReader(text);
        using var reader = new WktTextReader(sr);

        // Act
        var resultCs = reader.ReadToEnd();

        // Assert
        Assert.NotNull(resultCs);
        Assert.IsTrue(resultCs.Success);


        text = "PROJCS[\"Google Maps Global Mercator\"," +
               "GEOGCS[\"WGS 84\"," +
               "DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]]," +
               "AUTHORITY[\"EPSG\",\"6326\"]]," +
               "PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]]," +
               "UNIT[\"degree\",0.01745329251994328,AUTHORITY[\"EPSG\",\"9122\"]]," +
               "AUTHORITY[\"EPSG\",\"4326\"]]," +
               "PROJECTION[\"Mercator_2SP\"]," +
               "PARAMETER[\"standard_parallel_1\",0]," +
               "PARAMETER[\"latitude_of_origin\",0]," +
               "PARAMETER[\"central_meridian\",0]," +
               "PARAMETER[\"false_easting\",0]," +
               "PARAMETER[\"false_northing\",0]," +
               "UNIT[\"Meter\",1]," +
               "EXTENSION[\"PROJ4\",\"+proj=merc +a=6378137 +b=6378137 +lat_ts=0.0 +lon_0=0.0 +x_0=0.0 +y_0=0 +k=1.0 +units=m +nadgrids=@null +wktext  +no_defs\"]," +
               "AUTHORITY[\"EPSG\",\"900913\"]]";

        using var sr2 = new StringReader(text);
        using var reader2 = new WktTextReader(sr2);

        // Act
        var resultCs2 = reader2.ReadToEnd();

        // Assert
        Assert.NotNull(resultCs2);
        Assert.IsTrue(resultCs2.Success);
    }

    [Test]
    public void TestFittedCoordinateSystemWkt()
    {
        Assert.Ignore("Is FITTED_CS Support still needed?");

        string wkt = "FITTED_CS[\"Local coordinate system MNAU (based on Gauss-Krueger)\"," +
                     "PARAM_MT[\"Affine\"," +
                     "PARAMETER[\"num_row\",3],PARAMETER[\"num_col\",3],PARAMETER[\"elt_0_0\", 0.883485346527455],PARAMETER[\"elt_0_1\", -0.468458794848877],PARAMETER[\"elt_0_2\", 3455869.17937689],PARAMETER[\"elt_1_0\", 0.468458794848877],PARAMETER[\"elt_1_1\", 0.883485346527455],PARAMETER[\"elt_1_2\", 5478710.88035753],PARAMETER[\"elt_2_2\", 1]]," +
                     "PROJCS[\"DHDN / Gauss-Kruger zone 3\"," +
                     "GEOGCS[\"DHDN\"," +
                     "DATUM[\"Deutsches_Hauptdreiecksnetz\"," +
                     "SPHEROID[\"Bessel 1841\", 6377397.155, 299.1528128, AUTHORITY[\"EPSG\", \"7004\"]]," +
                     "TOWGS84[612.4, 77, 440.2, -0.054, 0.057, -2.797, 0.525975255930096]," +
                     "AUTHORITY[\"EPSG\", \"6314\"]]," +
                     "PRIMEM[\"Greenwich\", 0, AUTHORITY[\"EPSG\", \"8901\"]]," +
                     "UNIT[\"degree\", 0.0174532925199433, AUTHORITY[\"EPSG\", \"9122\"]]," +
                     "AUTHORITY[\"EPSG\", \"4314\"]]," +
                     "PROJECTION[\"Transverse_Mercator\"]," +
                     "PARAMETER[\"latitude_of_origin\", 0]," +
                     "PARAMETER[\"central_meridian\", 9]," +
                     "PARAMETER[\"scale_factor\", 1]," +
                     "PARAMETER[\"false_easting\", 3500000]," +
                     "PARAMETER[\"false_northing\", 0]," +
                     "UNIT[\"metre\", 1, AUTHORITY[\"EPSG\", \"9001\"]]," +
                     "AUTHORITY[\"EPSG\", \"31467\"]]" +
                     "]";

        using var sr = new StringReader(wkt);
        using var reader = new WktTextReader(sr);

        // Act
        var resultCs = reader.ReadToEnd();

        // Assert
        Assert.NotNull(resultCs);
        Assert.IsTrue(resultCs.Success, resultCs.Error.RenderErrorMessage());
    }

    [Test]
    public void TestGeocentricCoordinateSystem()
    {
        var fac = new CoordinateSystemFactory();
        GeocentricCoordinateSystem fcs = null;

        const string wkt = "GEOCCS[\"TUREF\", " +
                           "DATUM[\"Turkish_National_Reference_Frame\", " +
                           "SPHEROID[\"GRS 1980\", 6378137, 298.257222101, AUTHORITY[\"EPSG\", \"7019\"]], " +
                           "AUTHORITY[\"EPSG\", \"1057\"]], " +
                           "PRIMEM[\"Greenwich\", 0, AUTHORITY[\"EPSG\", \"8901\"]], " +
                           "UNIT[\"metre\", 1, AUTHORITY[\"EPSG\", \"9001\"]], " +
                           "AXIS[\"Geocentric X\", OTHER], AXIS[\"Geocentric Y\", OTHER], AXIS[\"Geocentric Z\", NORTH], " +
                           "AUTHORITY[\"EPSG\", \"5250\"]]";


        using var sr = new StringReader(wkt);
        using var reader = new WktTextReader(sr);

        // Act
        var resultCs = reader.ReadToEnd();

        // Assert
        Assert.NotNull(resultCs);
        Assert.IsTrue(resultCs.Success);
    }



    [Test]
    public void ParseWktCreatedByCoordinateSystem()
    {
        // Sample WKT from an external source.
        string sampleWKT =
            "PROJCS[\"\", " +
            "GEOGCS[\"\", " +
            "DATUM[\"\", " +
            "SPHEROID[\"GRS_1980\", 6378137, 298.2572221010042] " +
            "], " +
            "PRIMEM[\"Greenwich\", 0], " +
            "UNIT[\"Degree\", 0.017453292519943295]" +
            "], " +
            "PROJECTION[\"Transverse_Mercator\"], " +
            "PARAMETER[\"False_Easting\", 500000], " +
            "PARAMETER[\"False_Northing\", 0], " +
            "PARAMETER[\"Central_Meridian\", -75], " +
            "PARAMETER[\"Scale_Factor\", 0.9996], " +
            "UNIT[\"Meter\", 1]" +
            "]";

        using var sr = new StringReader(sampleWKT);
        using var reader = new WktTextReader(sr);

        // Act
        var resultCs = reader.ReadToEnd();

        // Assert
        Assert.NotNull(resultCs);
        Assert.IsTrue(resultCs.Success);
    }

}
