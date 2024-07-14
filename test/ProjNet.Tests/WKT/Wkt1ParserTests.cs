using System;
using System.Globalization;
using System.Linq;
using NUnit.Framework;
using Pidgin;
using ProjNet.IO.CoordinateSystems;
using ProjNet.Wkt;
using ProjNet.Wkt.v1.tree;
using static Pidgin.Parser;


namespace ProjNET.Tests.WKT;

[TestFixture]
public class Wkt1ParserTests
{

    [Test]
    public void TestParsing_UnsignedIntegers_Ok()
    {
        // Arrange
        string input01 = "210677";
        string input02 = "210677.24";
        string input03 = "0.210677";
        string input04 = ".210677";

        // Act
        uint pres01 = Wkt1Parser.UnsignedInteger.ParseOrThrow(input01);
        var pres02 = Wkt1Parser.UnsignedInteger.Parse(input02);
        var pres03 = Wkt1Parser.UnsignedInteger.Parse(input03);
        var pres04 = Wkt1Parser.UnsignedInteger.Parse(input04);

        // Assert
        Assert.AreEqual(210677, pres01);

        Assert.True(pres02.Success);
        Assert.AreEqual(210677, pres02.Value);

        Assert.True(pres03.Success);
        Assert.AreEqual(0, pres03.Value);

        Assert.False(pres04.Success);
    }

    [Test]
    public void TestParsing_ExactNumericLiterals_Ok()
    {
        // Arrange
        string input01 = "210677";
        string input02 = "24.210677";
        string input03 = ".210677";
        string input04= "21.5";

        // Act
        double pres01 = Wkt1Parser.ExactNumericLiteral.ParseOrThrow(input01);
        double pres02 = Wkt1Parser.ExactNumericLiteral.ParseOrThrow(input02);
        double pres03 = Wkt1Parser.ExactNumericLiteral.ParseOrThrow(input03);
        double pres04 = Wkt1Parser.ExactNumericLiteral.ParseOrThrow(input04);

        // Asserts
        Assert.AreEqual(210677d, pres01);
        Assert.AreEqual(24.210677d, pres02);
        Assert.AreEqual(0.210677d, pres03);
        Assert.AreEqual(21.5, pres04);
    }

    [Test]
    public void TestParsing_SignedIntegers_Ok()
    {
        // Arrange
        string input01 = "-210677";
        string input02 = "-24.210677";
        string input03 = "-.210677";
        string input04 = "21.5";

        // Act
        double pres01 = Wkt1Parser.SignedInteger.ParseOrThrow(input01);
        double pres02 = Wkt1Parser.SignedInteger.ParseOrThrow(input02);
        var pres03 = Wkt1Parser.SignedInteger.Parse(input03);
        double pres04 = Wkt1Parser.SignedInteger.ParseOrThrow(input04);

        // Asserts
        Assert.AreEqual(-210677.0d, pres01);
        Assert.False(pres03.Success);
    }

    [Test]
    public void TestParsing_ApproximateNumericLiterals_Ok()
    {
        // Arrange
        string input01 = "21.5E3";
        string input02 = "21.5E-3";
        string input03 = "21.5e3";
        string input04 = "21.5e-3";

        // Act
        double pres01 = Wkt1Parser.ApproximateNumericLiteral.ParseOrThrow(input01);
        double pres02 = Wkt1Parser.ApproximateNumericLiteral.ParseOrThrow(input02);
        double pres03 = Wkt1Parser.ApproximateNumericLiteral.ParseOrThrow(input03);
        double pres04 = Wkt1Parser.ApproximateNumericLiteral.ParseOrThrow(input04);

        // Asserts
        Assert.AreEqual(21.5e3d, pres01);
        Assert.AreEqual(21.5e-3d, pres02, 0.01);
        Assert.AreEqual(21.5e3d, pres03);
        Assert.AreEqual(21.5e-3d, pres04, delta:0.01);
    }

    [Test]
    public void TestParsing_SignedNumericLiterals_Ok()
    {
        // Arrange
        string input01 = "+210677";
        string input02 = "-24.210677";
        string input03 = "-.210677";
        string input04 = "+21.5";
        string input05 = "-21.5";
        string input06 = "21.5";

        // Act
        double pres01 = Wkt1Parser.SignedNumericLiteral.ParseOrThrow(input01);
        double pres02 = Wkt1Parser.SignedNumericLiteral.ParseOrThrow(input02);
        double pres03 = Wkt1Parser.SignedNumericLiteral.ParseOrThrow(input03);
        double pres04 = Wkt1Parser.SignedNumericLiteral.ParseOrThrow(input04);
        double pres05 = Wkt1Parser.SignedNumericLiteral.ParseOrThrow(input05);
        double pres06 = Wkt1Parser.SignedNumericLiteral.ParseOrThrow(input06);

        // Asserts
        Assert.AreEqual(210677.0d, pres01);
        Assert.AreEqual(-24.210677, pres02);
        Assert.AreEqual(-0.210677, pres03);
        Assert.AreEqual(21.5, pres04);
        Assert.AreEqual(-21.5, pres05);
        Assert.AreEqual(21.5, pres06);
    }


    [Test]
    public void TestParsing_Numbers_Ok()
    {
        // Arrange
        string input01 = "+210677";
        string input02 = "-24.210677";
        string input03 = "-.210677";
        string input04 = "+21.5";
        string input05 = "-21.5";
        string input06 = "21.5";
        string input07 = "21.5E3";
        string input08 = "-21.5E-3";
        string input09 = "21.5e3";

        // Act
        double pres01 = Wkt1Parser.Number.ParseOrThrow(input01);
        double pres02 = Wkt1Parser.Number.ParseOrThrow(input02);
        double pres03 = Wkt1Parser.Number.ParseOrThrow(input03);
        double pres04 = Wkt1Parser.Number.ParseOrThrow(input04);
        double pres05 = Wkt1Parser.Number.ParseOrThrow(input05);
        double pres06 = Wkt1Parser.Number.ParseOrThrow(input06);
        double pres07 = Wkt1Parser.Number.ParseOrThrow(input07);
        double pres08 = Wkt1Parser.Number.ParseOrThrow(input08);
        double pres09 = Wkt1Parser.Number.ParseOrThrow(input09);

        // Asserts
        Assert.AreEqual(210677.0d, pres01);
        Assert.AreEqual(-24.210677, pres02);
        Assert.AreEqual(-0.210677, pres03);
        Assert.AreEqual(21.5, pres04);
        Assert.AreEqual(-21.5, pres05);
        Assert.AreEqual(21.5, pres06);
        Assert.AreEqual(21.5E3, pres07);
        Assert.AreEqual(-21.5e-3, pres08,0.1);
        Assert.AreEqual(21.5e3, pres09);
    }





    [Test]
    public void TestParser_24HourClock()
    {
        // Arrange
        string strTime = "T00:34:56.789Z";

        // Act
        var dtb = Wkt1Parser._24HourClock.ParseOrThrow(strTime);

        // Assert
        Assert.AreEqual(0, dtb.Hour);
        Assert.AreEqual(34, dtb.Minutes);
        Assert.AreEqual(56, dtb.Seconds);
        Assert.AreEqual(789, dtb.Milliseconds);
    }

    [Test]
    public void TestParser_DateTime_UTC()
    {
        // Arrange
        string strDate = "2014-11-23T00:34:56.789Z";

        // Act
        var dtOffset = Wkt1Parser.DateTimeParser.ParseOrThrow(strDate);

        // Assert
        Assert.AreEqual(2014, dtOffset.Year);
        Assert.AreEqual(11, dtOffset.Month);
        Assert.AreEqual(23, dtOffset.Day);
        Assert.AreEqual(0, dtOffset.Hour);
        Assert.AreEqual(34, dtOffset.Minute);
        Assert.AreEqual(56, dtOffset.Second);
        Assert.AreEqual(789, dtOffset.Millisecond);
        // Assert.AreEqual(DateTimeKind.Utc, dtOffset.DateTime.Kind);

        Assert.AreEqual(TimeSpan.Zero, dtOffset.Offset);
    }

    [Test]
    public void TestParser_DateTime_Local()
    {
        // Arrange
        var ts = new TimeSpan(0, 2, 0, 0);
        var dt = new DateTime(2014, 7, 12, 17, 0, 0, 0, new GregorianCalendar(), DateTimeKind.Local);
        var offset = TimeZoneInfo.Local.GetUtcOffset(dt);
        string strDate = $"2014-07-12T17:00+0{offset.Hours}";

        // Act
        var dtOffset = Wkt1Parser.DateTimeParser.ParseOrThrow(strDate);

        // Assert
        Assert.AreEqual(2014, dtOffset.Year);
        Assert.AreEqual(07, dtOffset.Month);
        Assert.AreEqual(12, dtOffset.Day);
        Assert.AreEqual(17, dtOffset.Hour);
        Assert.AreEqual(0, dtOffset.Minute);
        Assert.AreEqual(0, dtOffset.Second);
        Assert.AreEqual(0, dtOffset.Millisecond);
        // Assert.AreEqual(DateTimeKind.Utc, dtOffset.DateTime.Kind);

        Assert.AreEqual(ts, dtOffset.Offset);
    }

    [Test]
    public void TestParser_Date()
    {
        // Arrange
        string strDate = "2014-03-01";

        // Act
        var dtOffset = Wkt1Parser.DateTimeParser.ParseOrThrow(strDate);

        // Assert
        Assert.AreEqual(2014, dtOffset.Year);
        Assert.AreEqual(03, dtOffset.Month);
        Assert.AreEqual(01, dtOffset.Day);
        Assert.AreEqual(0, dtOffset.Hour);
        Assert.AreEqual(0, dtOffset.Minute);
        Assert.AreEqual(0, dtOffset.Second);
        Assert.AreEqual(0, dtOffset.Millisecond);

        Assert.AreEqual(TimeSpan.Zero, dtOffset.Offset);
    }

    [Test]
    public void TestParser_WktLatinTextCharacter()
    {
        // Arrange
        string str = @"Datum origin is 30°25'20""""N, 130°25'20""""E.";

        // Act
        string pres = Wkt1Parser.WktLatinTextCharacters.ParseOrThrow(str);

        // Assert
        Assert.AreEqual(str, pres);
    }

    [Test]
    public void TestParser_QuotedLatinText()
    {
        // Arrange
        string str = "Datum origin is 30°25'20\"\"N, 130°25'20\"\"E.";
        string quoted = $"\"{str}\"";

        // Act
        string pres = Wkt1Parser.QuotedLatinText.ParseOrThrow(quoted);

        // Assert
        Assert.AreEqual(str, pres);
    }



    [Test]
    public void TestParser_Scope()
    {
        // Arrange
        string str = @"Large scale topographic mapping and cadastre.";
        string scopeText = $@"SCOPE[""{str}""]";

        // Act
        var pres = Wkt1Parser.ScopeParser.ParseOrThrow(scopeText);
        string toWkt = pres.ToWKT();

        // Assert
        Assert.AreEqual(str, pres.Description);

        Assert.AreEqual(scopeText, toWkt);
    }

    [Test]
    public void TestParser_Area()
    {
        // Arrange
        string str = @"Netherlands offshore.";
        string parserText = $@"AREA[""{str}""]";

        // Act
        var pres = Wkt1Parser.AreaDescriptionParser.ParseOrThrow(parserText);
        string toWkt = pres.ToWKT();

        // Assert
        Assert.AreEqual(str, pres.Description);

        Assert.AreEqual(parserText, toWkt);

    }


    [Test]
    public void TestParser_BBox()
    {
        // Arrange
        string parserText = $@"BBOX[51.43,2.54,55.77,6.4]";

        // Act
        var pres = Wkt1Parser.GeographicBoundingBoxParser.ParseOrThrow(parserText);
        string toWkt = pres.ToWKT();

        // Assert
        Assert.AreEqual(51.43, pres.LowerLeftLatitude);
        Assert.AreEqual(2.54, pres.LowerLeftLongitude);
        Assert.AreEqual(55.77, pres.UpperRightLatitude);
        Assert.AreEqual(6.40, pres.UpperRightLongitude);

        Assert.AreEqual(parserText, toWkt);
    }


    [Test]
    public void TestParser_VerticalExtent()
    {
        // Arrange
        string parserText = $@"VERTICALEXTENT[-1000,0]";

        // Act
        var pres = Wkt1Parser.VerticalExtentParser.ParseOrThrow(parserText);
        string toWkt = pres.ToWKT();

        // Assert
        Assert.AreEqual(-1000, pres.MinimumHeight);
        Assert.AreEqual(0, pres.MaximumHeight);

        Assert.AreEqual(parserText, toWkt);
    }

    [Test]
    public void Test_TemporalExtent_Parser()
    {
        // Arrange
        var dt01 = DateTime.Parse("01-01-2013");
        var dt02 = DateTime.Parse("31-12-2013");
        string parserText01 = $@"TIMEEXTENT[2013-01-01,2013-12-31]";
        string parserText02 = $@"TIMEEXTENT[""Jurassic"",""Quaternary""]";

        // Act
        var pres01 = Wkt1Parser.TemporalExtentParser.ParseOrThrow(parserText01);
        string toWkt01 = pres01.ToWKT();
        var pres02 = Wkt1Parser.TemporalExtentParser.ParseOrThrow(parserText02);
        string toWkt02 = pres02.ToWKT();

        // Assert
        Assert.AreEqual(dt01, pres01.StartDateTime.Value.DateTime);
        Assert.AreEqual(dt02, pres01.EndDateTime.Value.DateTime);

        Assert.AreEqual("Jurassic", pres02.StartText);
        Assert.AreEqual("Quaternary", pres02.EndText);

        Assert.AreEqual(parserText01, toWkt01);
        Assert.AreEqual(parserText02, toWkt02);
    }

    [Test]
    public void Test_IdUriParser()
    {
        // Arrange
        string strUri = @"urn:ogc:def:crs:EPSG::4326";
        string parserText01 = $@"URI[""{strUri}""]";

        // Act
        var pres01 = Wkt1Parser.IdUriParser.ParseOrThrow(parserText01);
        string toWkt01 = pres01.ToWKT();

        // Assert
        Assert.AreEqual(strUri, pres01.ToString());
        Assert.AreEqual(parserText01, toWkt01);
    }

    [Test]
    public void Test_IdentifierParser()
    {
        // Arrange
        string parserText01 = $@"ID[""Authority name"",""Abcd_Ef"",7.1]";
        string parserText02 = $@"ID[""EPSG"",4326]";
        string parserText03 = $@"ID[""EPSG"",4326,URI[""urn:ogc:def:crs:EPSG::4326""]]";
        string parserText04 = $@"ID[""EuroGeographics"",""ES_ED50 (BAL99) to ETRS89"",""2001-04-20""]";

        // Act
        var pres01 = Wkt1Parser.IdentifierParser.ParseOrThrow(parserText01);
        string toWkt01 = pres01.ToWKT();

        var pres02 = Wkt1Parser.IdentifierParser.ParseOrThrow(parserText02);
        string toWkt02 = pres02.ToWKT();

        var pres03 = Wkt1Parser.IdentifierParser.ParseOrThrow(parserText03);
        string toWkt03 = pres03.ToWKT();

        var pres04 = Wkt1Parser.IdentifierParser.ParseOrThrow(parserText04);
        string toWkt04 = pres04.ToWKT();

        // Assert
        Assert.AreEqual(toWkt01, pres01);
        Assert.AreEqual(toWkt02, pres02);
        Assert.AreEqual(toWkt03, pres03);
        Assert.AreEqual(toWkt04, pres04);
    }


}
