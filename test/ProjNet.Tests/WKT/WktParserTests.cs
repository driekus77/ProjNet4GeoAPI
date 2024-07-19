using NUnit.Framework;
using Pidgin;
using ProjNet.Wkt;

namespace ProjNET.Tests.WKT;

public class WktParserTests
{

    [Test]
    public void TestUnsignedIntegerParser()
    {
        // Arrange
        string parserText01 = $@"210677";

        // Act
        uint parserResult01 = WktParser.UnsignedIntegerParser.ParseOrThrow(parserText01);

        // Assert
        Assert.AreEqual(210677, parserResult01);
    }

    [Test]
    public void TestSignedIntegerParser()
    {
        // Arrange
        string parserText01 = $@"+210677";
        string parserText02 = $@"-210677";
        string parserText03 = $@"210677";

        // Act
        int parserResult01 = WktParser.SignedIntegerParser.ParseOrThrow(parserText01);
        int parserResult02 = WktParser.SignedIntegerParser.ParseOrThrow(parserText02);
        int parserResult03 = WktParser.SignedIntegerParser.ParseOrThrow(parserText03);

        // Assert
        Assert.AreEqual(210677, parserResult01);
        Assert.AreEqual(-210677, parserResult02);
        Assert.AreEqual(210677, parserResult03);
    }


    [Test]
    public void TestSignedNumericLiteralParser()
    {
        // Arrange
        string parserText01 = "-100.333333333333";
        string parserText02 = "+100.333333333333";
        string parserText03 = "100.333333333333";
        string parserText04 = ".333333333333";

        // Act
        double parserResult01 = WktParser.SignedNumericLiteralParser.ParseOrThrow(parserText01);
        double parserResult02 = WktParser.SignedNumericLiteralParser.ParseOrThrow(parserText02);
        double parserResult03 = WktParser.SignedNumericLiteralParser.ParseOrThrow(parserText03);
        double parserResult04 = WktParser.SignedNumericLiteralParser.ParseOrThrow(parserText04);

        // Assert
        Assert.AreEqual(-100.333333333333, parserResult01);
        Assert.AreEqual(100.333333333333, parserResult02);
        Assert.AreEqual(100.333333333333, parserResult03);
        Assert.AreEqual(0.333333333333, parserResult04);
    }

    [Test]
    public void TestExactNumericLiteralParser()
    {
        // Arrange
        string parserText01 = $@"21.043";
        string parserText02 = $@"0.043";
        string parserText03 = $@".043";

        // Act
        double parserResult01 = WktParser.ExactNumericLiteralParser.ParseOrThrow(parserText01);
        double parserResult02 = WktParser.ExactNumericLiteralParser.ParseOrThrow(parserText02);
        double parserResult03 = WktParser.ExactNumericLiteralParser.ParseOrThrow(parserText03);

        // Assert
        Assert.AreEqual(21.043d, parserResult01);
        Assert.AreEqual(0.043d, parserResult02);
        Assert.AreEqual(0.043d, parserResult03);
    }

    [Test]
    public void TestApproximateNumericLiteralParser()
    {
        // Arrange
        string parserText01 = $@"21.04E-3";
        string parserText02= $@"21.04E+3";
        string parserText03 = $@"21.04E3";
        string parserText04 = $@"0.04E3";
        string parserText05 = $@".04E3";

        // Act
        double parserResult01 = WktParser.ApproximateNumericLiteralParser.ParseOrThrow(parserText01);
        double parserResult02 = WktParser.ApproximateNumericLiteralParser.ParseOrThrow(parserText02);
        double parserResult03 = WktParser.ApproximateNumericLiteralParser.ParseOrThrow(parserText03);
        double parserResult04 = WktParser.ApproximateNumericLiteralParser.ParseOrThrow(parserText04);
        double parserResult05 = WktParser.ApproximateNumericLiteralParser.ParseOrThrow(parserText05);

        // Assert
        Assert.AreEqual(0.02104d, parserResult01);
        Assert.AreEqual(21040d, parserResult02);
        Assert.AreEqual(21040d, parserResult03);
        Assert.AreEqual(40d, parserResult04);
        Assert.AreEqual(40d, parserResult05);
    }

    [Test]
    public void TestQuotedNameParser()
    {
        // Arrange
        string str01 = "MyTextString";
        string parserText01 = $"\"{str01}\"";

        // Act
        string parserResult01 = WktParser.QuotedNameParser.ParseOrThrow(parserText01);

        // Assert
        Assert.AreEqual(str01, parserResult01);
    }


}
