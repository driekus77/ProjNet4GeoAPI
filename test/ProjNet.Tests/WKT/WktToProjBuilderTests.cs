using NUnit.Framework;
using ProjNet.CoordinateSystems;
using ProjNet.IO.Wkt;

namespace ProjNET.Tests.WKT;

public class WktToProjBuilderTests
{

        private readonly CoordinateSystemFactory _coordinateSystemFactory = new CoordinateSystemFactory();

        private readonly WktToProjBuilder _wktBuilder = new WktToProjBuilder();


        [Test]
        public void TestProjectedCoordinateSystem_EPSG_2918()
        {
            const string wkt = "PROJCS[\"NAD83(HARN) / Texas Central (ftUS)\", "+
                                        "GEOGCS[\"NAD83(HARN)\", " +
                                                 "DATUM[\"NAD83_High_Accuracy_Regional_Network\", "+
                                                         "SPHEROID[\"GRS 1980\", 6378137, 298.257222101, AUTHORITY[\"EPSG\", \"7019\"]], "+
                                                         "TOWGS84[725, 685, 536, 0, 0, 0, 0], " +
                                                         "AUTHORITY[\"EPSG\", \"6152\"]], "+
                                                 "PRIMEM[\"Greenwich\", 0, AUTHORITY[\"EPSG\", \"8901\"]], "+
                                                 "UNIT[\"degree\", 0.0174532925199433, AUTHORITY[\"EPSG\", \"9122\"]], "+
                                                 "AUTHORITY[\"EPSG\", \"4152\"]], "+
                                        "PROJECTION[\"Lambert_Conformal_Conic_2SP\"], " +
                                        "PARAMETER[\"standard_parallel_1\", 31.883333333333], " +
                                        "PARAMETER[\"standard_parallel_2\", 30.1166666667], " +
                                        "PARAMETER[\"latitude_of_origin\", 29.6666666667], " +
                                        "PARAMETER[\"central_meridian\", -100.333333333333], " +
                                        "PARAMETER[\"false_easting\", 2296583.333], " +
                                        "PARAMETER[\"false_northing\", 9842500], " +
                                        "UNIT[\"US survey foot\", 0.304800609601219, AUTHORITY[\"EPSG\", \"9003\"]], "+
                                        "AUTHORITY[\"EPSG\", \"2918\"]]";

            _wktBuilder.ParseAndBuild(wkt);
        }


        /// <summary>
        /// This test reads in a file with 2671 pre-defined coordinate systems and projections,
        /// and tries to parse them.
        /// </summary>
        [Test]
        public void ParseAllWKTs()
        {
            int parseCount = 0;
            foreach (var wkt in SRIDReader.GetSrids())
            {
                var cs1 = _coordinateSystemFactory.CreateFromWkt(wkt.Wkt);
                Assert.IsNotNull(cs1, "Could not parse WKT: " + wkt);
                var cs2 = _coordinateSystemFactory.CreateFromWkt(wkt.Wkt.Replace("[", "(").Replace("]", ")"));
                Assert.That(cs1.EqualParams(cs2), Is.True);
                parseCount++;
            }
            Assert.That(parseCount, Is.GreaterThan(2671), "Not all WKT was parsed");
        }

        /// <summary>
        /// This test reads in a file with 2671 pre-defined coordinate systems and projections,
        /// and tries to parse them.
        /// </summary>
        [Test]
        public void ParseAllWKTs_ANTLR()
        {
            int parseCount = 0;
            foreach (var wkt in SRIDReader.GetSrids())
            {
                var cs1 = _wktBuilder.ParseAndBuild(wkt.Wkt);
                Assert.IsNotNull(cs1, "Could not parse WKT: " + wkt);
                var cs2 = _wktBuilder.ParseAndBuild(wkt.Wkt.Replace("[", "(").Replace("]", ")"));
                //Assert.That(cs1.Equals(cs2), Is.True);
                parseCount++;
            }
            Assert.That(parseCount, Is.GreaterThan(2671), "Not all WKT was parsed");
        }



        [Test]
        public void ParseProjCSWithExtension()
        {
            string wkt = "PROJCS[\"Google Maps Global Mercator\"," +
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

            var cs1 = _wktBuilder.ParseAndBuild(wkt);

            Assert.IsNotNull(cs1);
        }
}
