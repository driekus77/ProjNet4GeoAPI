using System;
using System.Collections.Generic;
using System.Globalization;
using Pidgin;
using ProjNet.CoordinateSystems;
using ProjNet.Wkt.Builders;
using ProjNet.Wkt.Tree;
using static Pidgin.Parser;
using Unit = ProjNet.CoordinateSystems.Unit;


namespace ProjNet.Wkt
{
    /// <summary>
    /// WktParser using the Pidgin Parser Combinator library.
    /// </summary>
    public class WktParser
    {
        internal static Parser<char, T> BetweenWhitespace<T>(Parser<char, T> p) => p.Between(SkipWhitespaces);

        // 7.2 Component description
        // 7.2.1 BNF Introduction

        // <minus sign> ::= -
        internal static readonly Parser<char, char> MinusSignParser = Char('-');
        // <plus sign> ::= +
        internal static readonly Parser<char, char> PlusSignParser = Char('+');

        // <ampersand> ::= &
        internal static readonly Parser<char, char> AmpersandParser = Char('&');

        // <at sign> ::= @
        internal static readonly Parser<char, char> AtSignParser = Char('@');

        // <equal sign> ::= =
        internal static readonly Parser<char, char> EqualSignParser = Char('=');

        // <left paren> ::= (
        internal static readonly Parser<char, char> LeftParenParser = Char('(');
        // <right paren> ::= )
        internal static readonly Parser<char, char> RightParenParser = Char(')');

        // <left bracket> ::= [
        internal static readonly Parser<char, char> LeftBracketParser = Char('[');
        // <right bracket> ::= ]
        internal static readonly Parser<char, char> RightBracketParser = Char(']');

        // <forward slash> ::= /
        internal static readonly Parser<char, char> ForwardSlashParser = Char('/');
        // <backward slash> ::= \
        internal static readonly Parser<char, char> BackwardSlashParser = Char('\\');

        // <period> ::= .
        internal static readonly Parser<char, char> PeriodParser = Char('.');

        // <double quote> ::= "
        internal static readonly Parser<char, char> DoubleQuoteParser = Char('"');

        // <quote> ::= '
        internal static readonly Parser<char, char> QuoteParser = Char('\'');

        // <comma> ,
        internal static readonly Parser<char, char> CommaParser = Char(',');

        // <underscore> ::= _
        internal static readonly Parser<char, char> UnderScoreParser = Char('_');

        // <digit> ::= 0|1|2|3|4|5|6|7|8|9
        internal static readonly Parser<char, char> DigitParser = Digit;

        // <simple Latin lower case letter> ::= a|b|c|d|e|f|g|h|i|j|k|l|m|n|o|p|q|r|s|t|u|v|w|x|y|z
        internal static readonly Parser<char, char> SimpleLatinLowerCaseLetterParser = OneOf(
            Char('a'), Char('b'), Char('c'),
            Char('d'), Char('e'), Char('f'), Char('g'),
            Char('h'), Char('i'), Char('j'), Char('k'),
            Char('l'), Char('m'), Char('n'), Char('o'),
            Char('p'), Char('q'), Char('r'), Char('s'),
            Char('t'), Char('u'), Char('v'), Char('w'),
            Char('x'), Char('y'), Char('z')
        );

        // <simple Latin upper case letter> ::= A|B|C|D|E|F|G|H|I|J|K|L|M|N|O|P|Q|R|S|T|U|V|W|X|Y|Z
        internal static readonly Parser<char, char> SimpleLatinUpperCaseLetterParser = OneOf(
            Char('A'), Char('B'), Char('C'),
            Char('D'), Char('E'), Char('F'), Char('G'),
            Char('H'), Char('I'), Char('J'), Char('K'),
            Char('L'), Char('M'), Char('N'), Char('O'),
            Char('P'), Char('Q'), Char('R'), Char('S'),
            Char('T'), Char('U'), Char('V'), Char('W'),
            Char('X'), Char('Y'), Char('Z')
        );

        // <space>= " " // unicode "U+0020" (space)
        internal static readonly Parser<char, char> SpaceParser = Char(' ');

        // <left delimiter> ::= <left paren>|<left bracket> // must match balancing right delimiter
        internal static readonly Parser<char, char> LeftDelimiterParser = BetweenWhitespace(LeftParenParser.Or(LeftBracketParser));
        // <right delimiter> ::= <right paren>|<right bracket> // must match balancing left delimiter
        internal static readonly Parser<char, char> RightDelimiterParser = BetweenWhitespace(RightParenParser.Or(RightBracketParser));

        // <special> ::= <right paren>|<left paren>|<minus sign> |<underscore>|<period>|<quote>|<space>
        internal static readonly Parser<char, char> SpecialParser = OneOf(
            RightParenParser,
            LeftParenParser,
            MinusSignParser,
            PlusSignParser,
            UnderScoreParser,
            PeriodParser,
            CommaParser,
            EqualSignParser,
            AtSignParser,
            QuoteParser,
            SpaceParser,
            ForwardSlashParser,
            BackwardSlashParser,
            LeftBracketParser,
            RightBracketParser,
            AmpersandParser
            );

        // <sign> ::= <plus sign> | <minus sign>
        internal static readonly Parser<char, char> SignParser = PlusSignParser.Or(MinusSignParser);

        // <decimal point> ::= <period> | <comma> // <== This is what definition states but comma doesn't work in example(s).
        internal static readonly Parser<char, char> DecimalPointParser = PeriodParser;//.Or(CommaParser);

        // <empty set> ::= EMPTY
        internal static readonly Parser<char, string> EmptySetParser = String("EMPTY");

        // <wkt separator> ::= <comma>
        internal static readonly Parser<char, char> WktSeparatorParser = BetweenWhitespace(CommaParser);


        // <unsigned integer> ::= (<digit>)*
        internal static readonly Parser<char, string> UnsignedIntegerStringParser = DigitParser.AtLeastOnceString();
        internal static readonly Parser<char, uint> UnsignedIntegerParser = UnsignedIntegerStringParser
            .Select(uint.Parse);


        // <signed integer> ::= {<sign>}<unsigned integer>
        internal static readonly Parser<char, string> SignedIntegerStringParser = SignParser.Optional()
            .Then(DigitParser.ManyString(), (sign, str) => sign.HasValue && sign.Value == '-' ? $"-{str}" : str);
        internal static readonly Parser<char, int> SignedIntegerParser = SignedIntegerStringParser.Select(int.Parse);


        // <exact numeric literal> ::= <unsigned integer>{<decimal point>{<unsigned integer>}} |<decimal point><unsigned integer>
        internal static readonly Parser<char, double> ExactNumericLiteralDottedParser =
            DecimalPointParser
                .Then(UnsignedIntegerStringParser, (c, ui) => Utils.CalcAsFractionOf(0, ui));

        internal static readonly Parser<char, double> ExactNumericLiteralParser =
            UnsignedIntegerStringParser.Optional()
                .Then(ExactNumericLiteralDottedParser.Optional(), (ui, d) => (ui.HasValue ? uint.Parse(ui.GetValueOrDefault()) : 0) + d.GetValueOrDefault());

        // <mantissa> ::= <exact numeric literal>
        internal static readonly Parser<char, double> MantissaParser = ExactNumericLiteralParser;

        // <exponent> ::= <signed integer>
        internal static readonly Parser<char, string> ExponentStringParser = SignedIntegerStringParser;
        internal static readonly Parser<char, int> ExponentParser = SignedIntegerParser;

        // <approximate numeric literal> ::= <mantissa>E<exponent>
        internal static readonly Parser<char, double> ApproximateNumericLiteralExpParser =
                OneOf(Char('e'),Char('E'))
                .Then(ExponentParser)
                .Select(e => Math.Pow(10, e));
        internal static readonly Parser<char, double> ApproximateNumericLiteralParser =
            MantissaParser.Then(ApproximateNumericLiteralExpParser.Optional(), (m, e) => m * (e.HasValue ? e.Value : 1));

        // <simple Latin letter> ::= <simple Latin upper case letter>|<simple Latin lower case letter>
        internal static readonly Parser<char, char> SimpleLatinLetterParser =
            SimpleLatinUpperCaseLetterParser.Or(SimpleLatinLowerCaseLetterParser);

        // <unsigned numeric literal> ::= <exact numeric literal> | <approximate numeric literal>
        internal static readonly Parser<char, double> UnsignedNumericLiteralParser = ExactNumericLiteralParser
            .Or(ApproximateNumericLiteralParser);

        // <signed numeric literal> ::= {<sign>}<unsigned numeric literal>
        internal static readonly Parser<char, double> SignedNumericLiteralParser = SignParser.Optional()
            .Then(UnsignedNumericLiteralParser, (sign, d) =>
            {
                int signFactor = sign.GetValueOrDefault() == '-' ? -1 : 1;
                return signFactor * d;
            } );

        // <x> ::= <signed numeric literal>
        internal static readonly Parser<char, double> XParser = SignedNumericLiteralParser;
        // <y> ::= <signed numeric literal>
        internal static readonly Parser<char, double> YParser = SignedNumericLiteralParser;
        // <z> ::= <signed numeric literal>
        internal static readonly Parser<char, double> ZParser = SignedNumericLiteralParser;
        // <m> ::= <signed numeric literal>
        internal static readonly Parser<char, double> MParser = SignedNumericLiteralParser;

        // <letter> ::= <simple Latin letter>|<digit>|<special>
        internal static readonly Parser<char, char> LetterParser = OneOf(SimpleLatinLetterParser, DigitParser, SpecialParser);
        // <letters> ::= (<letter>)*
        internal static readonly Parser<char, string> LettersParser = LetterParser.ManyString();
        // <name> ::= <letters>
        internal static readonly Parser<char, string> NameParser = OneOf(SimpleLatinLetterParser, DigitParser).ManyString();
        // <quoted name> ::= <double quote> <name> <double quote>
        internal static readonly Parser<char, string> QuotedNameParser =
            LettersParser.Between(DoubleQuoteParser, DoubleQuoteParser);



        // 7.2.2 BNF Productions for Two-Dimension Geometry WKT

        // The following BNF defines two-dimensional geometries in (x, y) coordinate spaces. With the exception of the
        // addition of polyhedral surfaces, these structures are unchanged from earlier editions of this standard.
        // <point> ::= <x> <y>
        // <geometry tagged text> ::= <point tagged text>
        // | <linestring tagged text>
        // | <polygon tagged text>
        // | <triangle tagged text>
        // | <polyhedralsurface tagged text>
        // | <tin tagged text>
        // | <multipoint tagged text>
        // | <multilinestring tagged text>
        // | <multipolygon tagged text>
        // | <geometrycollection tagged text>
        // <point tagged text> ::= point <point text>
        // <linestring tagged text> ::= linestring <linestring text>
        // <polygon tagged text> ::= polygon <polygon text>
        // <polyhedralsurface tagged text> ::= polyhedralsurface
        // <polyhedralsurface text>
        // <triangle tagged text> ::= triangle <polygon text>
        // <tin tagged text> tin <polyhedralsurface text>
        // <multipoint tagged text> ::= multipoint <multipoint text>
        // <multilinestring tagged text> ::= multilinestring <multilinestring text>
        // <multipolygon tagged text> ::= multipolygon <multipolygon text>
        // <geometrycollection tagged text> ::= geometrycollection
        // <geometrycollection text>
        // <point text> ::= <empty set> | <left paren> <point> <right paren>
        // <linestring text> ::= <empty set> | <left paren>
        // <point>
        // {<comma> <point>}*
        // <right paren>
        // <polygon text> ::= <empty set> | <left paren>
        // <linestring text>
        // {<comma> <linestring text>}*
        // <right paren>
        // <polyhedralsurface text> ::= <empty set> | <left paren>
        // <polygon text>
        // {<comma> <polygon text>}*
        // <right paren>
        // <multipoint text> ::= <empty set> | <left paren>
        // <point text>
        // {<comma> <point text>}*
        // <right paren>
        // <multilinestring text> ::= <empty set> | <left paren>
        // <linestring text>
        // {<comma> <linestring text>}*
        // <right paren>
        // <multipolygon text> ::= <empty set> | <left paren>
        // <polygon text>
        // {<comma> <polygon text>}*
        // <right paren>
        // <geometrycollection text> ::= <empty set> | <left paren>
        // <geometry tagged text>
        // {<comma> <geometry tagged text>}*
        // <right paren>


        // 7.2.3 BNF Productions for Three-Dimension Geometry WKT

        // The following BNF defines geometries in 3 dimensional (x, y, z) coordinates.
        // <point z> ::= <x> <y> <z>
        // <geometry z tagged text> ::= <point z tagged text>
        // |<linestring z tagged text>
        // |<polygon z tagged text>
        // |<polyhedronsurface z tagged text>
        // |<triangle tagged text>
        // |<tin tagged text>
        // |<multipoint z tagged text>
        // |<multilinestring z tagged text>
        // |<multipolygon z tagged text>
        // |<geometrycollection z tagged text>
        // <point z tagged text> ::= point z <point z text>
        // <linestring z tagged text> ::= linestring z <linestring z text>
        // <polygon z tagged text> ::= polygon z <polygon z text>
        // <polyhedralsurface z tagged text> ::= polyhedralsurface z
        // <polyhedralsurface z text>
        // <triangle z tagged text> ::= triangle z <polygon z text>
        // <tin z tagged text> tin z <polyhedralsurface z text>
        // <multipoint z tagged text> ::= multipoint z <multipoint z text>
        // <multilinestring z tagged text> ::= multilinestring z <multilinestring z text>
        // <multipolygon z tagged text> ::= multipolygon z <multipolygon z text>
        // <geometrycollection z tagged
        // text> ::=
        // geometrycollection z
        // <geometrycollection z text>
        // <point z text> ::= <empty set> | <left paren> <point z>
        // <right paren>
        // <linestring z text> ::= <empty set> | <left paren> <point z>
        // {<comma> <point z>}*
        // <right paren>
        // <polygon z text> ::= <empty set> | <left paren>
        // <linestring z text>
        // {<comma> <linestring z text>}*
        // <right paren>
        // <polyhedralsurface z text> ::= <empty set>|<left paren>
        // <polygon z text>
        // {<comma> <polygon z text>}*
        // <right paren>
        // <multipoint z text> ::= <empty set> | <left paren>
        // <point z text>
        // {<comma> <point z text>}*
        // <right paren>
        // <multilinestring z text> ::= <empty set> | <left paren>
        // <linestring z text>
        // {<comma> <linestring z text>}*
        // <right paren>
        // <multipolygon z text> ::= <empty set> | <left paren>
        // <polygon z text>
        // {<comma> <polygon z text>}*
        // <right paren>
        // <geometrycollection z text> ::= <empty set> | <left paren>
        // <geometry tagged z text>
        // {<comma> <geometry tagged z text>}*
        // <right paren>


        // 7.2.4 BNF Productions for Two-Dimension Measured Geometry WKT

        // The following BNF defines two-dimensional geometries in (x, y) coordinate spaces. In addition, each coordinate
        // carries an "m" ordinate value that is part of some linear reference system.
        // <point m> ::= <x> <y> <m>
        // <geometry m tagged text> ::= <point m tagged text>
        // |<linestring m tagged text>
        // |<polygon m tagged text>
        // |<polyhedralsurface m tagged text>
        // |<triangle tagged m text>
        // |<tin tagged m text>
        // |<multipoint m tagged text>
        // |<multilinestring m tagged text>
        // |<multipolygon m tagged text>
        // |<geometrycollection m tagged text>
        // <point m tagged text> ::= point m <point m text>
        // <linestring m tagged text> ::= linestring m <linestring m text>
        // <polygon m tagged text> ::= polygon m <polygon m text>
        // <polyhedralsurface m tagged text> ::= polyhedralsurface m
        // <polyhedralsurface m text>
        // <triangle m tagged text> ::= triangle m <polygon m text>
        // <tin m tagged text> tin m <polyhedralsurface m text>
        // <multipoint m tagged text> ::= multipoint m <multipoint m text>
        // <multilinestring m tagged text> ::= multilinestring m <multilinestring m text>
        // <multipolygon m tagged text> ::= multipolygon m <multipolygon m text>
        // <geometrycollection m tagged
        // text> ::=
        // geometrycollection m
        // <geometrycollection m text>
        // <point m text> ::= <empty set> | <left paren>
        // <point m>
        // <right paren>
        // <linestring m text> ::= <empty set> | <left paren>
        // <point m>
        // {{<comma> <point m>}+
        // <right paren>
        // <polygon m text> ::= <empty set> | <left paren>
        // <linestring m text>
        // {<comma> <linestring m text>}*
        // <right paren>
        // <polyhedralsurface m text> ::= <empty set> | <left paren>
        // <polygon m text>
        // {<comma> <polygon m text>}*
        // <right paren>
        // <multipoint m text> ::= <empty set> | <left paren> <point m text>
        // {<comma> <point m text>}*
        // <right paren>
        // <multilinestring m text> ::= <empty set> | <left paren>
        // <linestring m text>
        // {<comma> <linestring m text>}*
        // <right paren>
        // <multipolygon m text> ::= <empty set> | <left paren>
        // <polygon m text>
        // {<comma> <polygon m text>}*
        // <right paren>
        // <geometrycollection m text> ::= <empty set> | <left paren>
        // <geometry tagged m text>
        // {<comma> <geometry tagged m text>}*
        // <right paren>


        // 7.2.5 BNF Productions for Three-Dimension Measured Geometry WKT

        // The following BNF defines three-dimensional geometries in (x, y, z) coordinate spaces. In addition, each
        // coordinate carries an "m" ordinate value that is part of some linear reference system.
        // <point zm> ::= <x> <y> <z> <m>
        // <geometry zm tagged text> ::= <point zm tagged text>
        // |<linestring zm tagged text>
        // |<polygon zm tagged text>
        // |<polyhedralsurface zm tagged text>
        // |<triangle zm tagged text>
        // |<tin zm tagged text>
        // |<multipoint zm tagged text>
        // |<multilinestring zm tagged text>
        // |<multipolygon zm tagged text>
        // |<geometrycollection zm tagged text>
        // <point zm tagged text> ::= point zm <point zm text>
        // <linestring zm tagged text> ::= linestring zm <linestring zm text>
        // <polygon zm tagged text> ::= polygon zm <polygon zm text>
        // <polyhedralsurface zm tagged
        // text> ::=
        // polyhedralsurface zm
        // <polyhedralsurface zm text>
        // <triangle zm tagged text> ::= triangle zm <polygon zm text>
        // <tin zm tagged text> tin zm <polyhedralsurface zm text>
        // <multipoint zm tagged text> ::= multipoint zm <multipoint zm text>
        // <multipoint zm tagged text> ::= multipoint zm
        // <multipoint zm text>
        // <multilinestring zm tagged text> ::= multilinestring zm
        // <multilinestring zm text>
        // <multipolygon zm tagged text> ::= multipolygon zm
        // <MultiPolygon zm text>
        // <geometrycollection zm tagged
        // text> ::=
        // geometrycollection zm
        // <geometrycollection zm text>
        // <point zm text> ::= <empty set> | <left paren> <point zm>
        // <right paren>
        // <linestring zm text> ::= <empty set> | <left paren>
        // <point z>
        // {<comma> <point z>}*
        // <right paren>
        // <polygon zm text> ::= <empty set> | <left paren>
        // <linestring zm text>
        // {<comma> <linestring zm text>}*
        // <right paren>
        // <polyhedralsurface zm text> ::= <empty set> | <left paren> {
        // <polygon zm text
        // {<comma> <polygon zm text>}*)
        // <right paren>
        // <multipoint zm text> ::= <empty set> | <left paren>
        // <point zm text>
        // {<comma> <point zm text>}*
        // <right paren>
        // <multilinestring zm text> ::= <empty set> | <left paren>
        // <linestring zm text>
        // {<comma> <linestring zm text>}*
        // <right paren>
        // <multipolygon zm text> ::= <empty set> | <left paren>
        // <polygon zm text>
        // {<comma> <polygon zm text>}* <right paren>
        // <geometrycollection zm text> ::= <empty set> | <left paren>
        // <geometry tagged zm text>
        // {<comma> <geometry tagged zm text>}*  <right paren>



        // 9 Well-known Text Representation of Spatial Reference System


        // <value> ::= <signed numeric literal>
        internal static readonly Parser<char, double> ValueParser = SignedNumericLiteralParser;

        // <semi-major axis> ::= <signed numeric literal>
        internal static readonly Parser<char, double> SemiMajorAxisParser = SignedNumericLiteralParser;

        // <longitude> ::= <signed numeric literal>
        internal static readonly Parser<char, double> LongitudeParser = SignedNumericLiteralParser;

        // <inverse flattening> ::= <signed numeric literal>
        internal static readonly Parser<char, double> InverseFlatteningParser = SignedNumericLiteralParser;

        // <conversion factor> ::= <signed numeric literal>
        internal static readonly Parser<char, double> ConversionFactorParser = SignedNumericLiteralParser;


        // <unit name> ::= <quoted name>
        internal static readonly Parser<char, string> UnitNameParser = QuotedNameParser;

        // <spheroid name> ::= <quoted name>
        internal static readonly Parser<char, string> SpheroidNameParser = QuotedNameParser;

        // <projection name> ::= <quoted name>
        internal static readonly Parser<char, string> ProjectionNameParser = QuotedNameParser;

        // <prime meridian name> ::= <quoted name>
        internal static readonly Parser<char, string> PrimeMeridianNameParser = QuotedNameParser;

        // <parameter name> ::= <quoted name>
        internal static readonly Parser<char, string> ParameterNameParser = QuotedNameParser;

        // <datum name> ::= <quoted name>
        internal static readonly Parser<char, string> DatumNameParser = QuotedNameParser;

        // <csname> ::= <quoted name>
        internal static readonly Parser<char, string> CsNameParser = QuotedNameParser;



        // Extra grammar from elements outside the specification document. (Borrowed from WKT 2.x ?)

        // AUTHORITY    = 'AUTHORITY' '[' string ',' string ']'
        internal Parser<char, WktAuthority> AuthorityParser => String("AUTHORITY")
            .Then(LeftDelimiterParser)
            .Then(QuotedNameParser, (c, name) => new WktAuthority().SetLeftDelimiter(c).As<WktAuthority>().SetName(name))
            .Then(WktSeparatorParser.Then(QuotedNameParser), (wktObject, code) => wktObject.SetCode(code))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktAuthority>())
            .Labelled("WktAuthority");

        // AXIS         = 'AXIS' '[' string ',' string ']'
        internal Parser<char, WktAxis> AxisParser => String("AXIS")
            .Then(LeftDelimiterParser)
            .Then(QuotedNameParser, (c, name) => new WktAxis(name).SetLeftDelimiter(c))
            .Then(WktSeparatorParser.Then(NameParser), (wktObject, direction) => wktObject.As<WktAxis>().SetDirection(direction))
            .Before(RightDelimiterParser)
            .Labelled("WktAxis");

        internal Parser<char, WktExtension> ExtensionParser => String("EXTENSION")
            .Then(LeftDelimiterParser)
            .Then(QuotedNameParser, (c, name) => new WktExtension(name).SetLeftDelimiter(c).As<WktExtension>())
            .Then(WktSeparatorParser.Then(LettersParser.Between(DoubleQuoteParser)), (wktObject, value) => wktObject.SetValue(value))
            .Before(RightDelimiterParser)
            .Labelled("WktExtension");

        // TOWGS84      = 'TOWGS84' '[' float ',' float ',' float ',' float ',' float ',' float ',' float ']'
        internal Parser<char, WktToWgs84> ToWgs84Parser => String("TOWGS84")
            .Then(LeftDelimiterParser)
            .Then(ValueParser, (c, d) => new WktToWgs84().SetDxShift(d).SetLeftDelimiter(c).As<WktToWgs84>() )
            .Then(WktSeparatorParser.Then(ValueParser), (wktObject, d) => wktObject.SetDyShift(d))
            .Then(WktSeparatorParser.Then(ValueParser), (wktObject, d) => wktObject.SetDzShift(d))
            .Then(WktSeparatorParser.Then(ValueParser), (wktObject, d) => wktObject.SetExRotation(d))
            .Then(WktSeparatorParser.Then(ValueParser), (wktObject, d) => wktObject.SetEyRotation(d))
            .Then(WktSeparatorParser.Then(ValueParser), (wktObject, d) => wktObject.SetEzRotation(d))
            .Then(WktSeparatorParser.Then(ValueParser), (wktObject, d) => wktObject.SetPpmScaling(d))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktToWgs84>())
            .Labelled("WktToWgs84");




        // <projection> ::= PROJECTION <left delimiter> <projection name> <right delimiter>
        internal Parser<char, WktProjection> ProjectionParser => String("PROJECTION")
            .Then(LeftDelimiterParser)
            .Then(ProjectionNameParser, ((c, name) => new WktProjection(name).SetLeftDelimiter(c).As<WktProjection>()))
            .Then(WktSeparatorParser.Then(AuthorityParser).Optional(), (wktObject, authority) => wktObject.SetAuthority(authority.GetValueOrDefault()))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktProjection>() )
            .Labelled("WktProjection");

        // <parameter> ::= PARAMETER <left delimiter> <parameter name> <comma> <value> <right delimiter>
        internal Parser<char, WktParameter> ParameterParser => String("PARAMETER")
            .Then(LeftDelimiterParser)
            .Then(ParameterNameParser, (c, name) => new WktParameter(name).SetLeftDelimiter(c).As<WktParameter>() )
            .Then(WktSeparatorParser.Then(ValueParser), (wktObject, value) => wktObject.SetValue(value))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktParameter>() )
            .Labelled("WktParameter");

        // <ellipsoid> ::= ELLIPSOID <left delimiter> <ellipsoid name> <comma> <semi-major axis> <comma> <inverse flattening> <right delimiter>
        internal Parser<char, WktEllipsoid> ElipsoidParser => String("ELLIPSOID")
            .Then(LeftDelimiterParser)
            .Then(SpheroidNameParser, (c, name) => new WktEllipsoid(name).SetLeftDelimiter(c).As<WktEllipsoid>() )
            .Then(WktSeparatorParser.Then(SemiMajorAxisParser), (wktObject, axis) => wktObject.SetSemiMajorAxis(axis))
            .Then(WktSeparatorParser.Then(InverseFlatteningParser), (wktObject, inverse) => wktObject.SetInverseFlattening(inverse))
            .Then(WktSeparatorParser.Then(AuthorityParser).Optional(), (wktObject, authority) => wktObject.SetAuthority(authority.GetValueOrDefault()))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktEllipsoid>())
            .Labelled("WktEllipsoid");
        // <spheroid> ::= SPHEROID <left delimiter> <spheroid name> <comma> <semi-major axis> <comma> <inverse flattening> <right delimiter>
        internal Parser<char, WktSpheroid> SpheroidParser => String("SPHEROID")
            .Then(LeftDelimiterParser)
            .Then(SpheroidNameParser, (c, name) => new WktSpheroid(name).SetLeftDelimiter(c).As<WktSpheroid>() )
            .Then(WktSeparatorParser.Then(SemiMajorAxisParser), (wktObject, axis) => wktObject.SetSemiMajorAxis(axis))
            .Then(WktSeparatorParser.Then(InverseFlatteningParser), (wktObject, inverse) => wktObject.SetInverseFlattening(inverse))
            .Then(WktSeparatorParser.Then(AuthorityParser).Optional(), (wktObject, authority) => wktObject.SetAuthority(authority.GetValueOrDefault()))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktSpheroid>())
            .Labelled("WktSpheroid");

        // <prime meridian> ::= PRIMEM <left delimiter> <prime meridian name> <comma> <longitude> [<comma> <authority>] <right delimiter>
        internal Parser<char, WktPrimeMeridian> PrimeMeridianParser => String("PRIMEM")
            .Then(LeftDelimiterParser)
            .Then(PrimeMeridianNameParser, (c, name) => new WktPrimeMeridian(name).SetLeftDelimiter(c).As<WktPrimeMeridian>() )
            .Then(WktSeparatorParser.Then(LongitudeParser), (wktObject, longitude) => wktObject.SetLongitude(longitude))
            .Then(WktSeparatorParser.Then(AuthorityParser).Optional(), (wktObject, authority) => wktObject.SetAuthority(authority.GetValueOrDefault()))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktPrimeMeridian>())
            .Labelled("WktPrimeMeridian");


        // <linear unit> ::= <unit>
        internal Parser<char, WktLinearUnit> LinearUnitParser => String("UNIT")
            .Then(LeftDelimiterParser)
            .Then(CIString("metre").Between(DoubleQuoteParser), (c, name) => new WktLinearUnit(name).SetLeftDelimiter(c).As<WktLinearUnit>() )
            .Then(WktSeparatorParser.Then(ConversionFactorParser), (wktObject, factor)  => wktObject.SetConversionFactor(factor))
            .Then(WktSeparatorParser.Then(AuthorityParser).Optional(), (wktObject, authority) => wktObject.SetAuthority(authority.GetValueOrDefault()))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktLinearUnit>())
                .Labelled("WktLinearUnit");

        // <angular unit> ::= <unit>
        internal Parser<char, WktAngularUnit> AngularUnitParser => String("UNIT")
            .Then(LeftDelimiterParser)
            .Then(CIString("degree").Between(DoubleQuoteParser), (c, name) => new WktAngularUnit(name).SetLeftDelimiter(c).As<WktAngularUnit>() )
            .Then(WktSeparatorParser.Then(ConversionFactorParser), (wktObject, factor)  => wktObject.SetConversionFactor(factor))
            .Then(WktSeparatorParser.Then(AuthorityParser).Optional(), (wktObject, authority) => wktObject.SetAuthority(authority.GetValueOrDefault()))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktAngularUnit>())
            .Labelled("WktAngularUnit");

        // <unit> ::= UNIT <left delimiter> <unit name> <comma> <conversion factor> [<comma> <authority>] <right delimiter>
        internal Parser<char, WktUnit> GenericUnitParser => String("UNIT")
            .Then(LeftDelimiterParser)
            .Then(UnitNameParser, (c, name) => new WktUnit(name).SetLeftDelimiter(c).As<WktUnit>() )
            .Then(WktSeparatorParser.Then(ConversionFactorParser), (wktObject, factor)  => wktObject.SetConversionFactor(factor))
            .Then(WktSeparatorParser.Then(AuthorityParser).Optional(), (wktObject, authority) => wktObject.SetAuthority(authority.GetValueOrDefault()))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktUnit>())
            .Labelled("WktUnit");
        internal Parser<char, WktUnit> UnitParser => OneOf(
            Try(AngularUnitParser.Cast<WktUnit>()),
            Try(LinearUnitParser.Cast<WktUnit>()),
            Try(GenericUnitParser));

        // <datum> ::= DATUM <left delimiter> <datum name> <comma> <spheroid> <right delimiter>
        // Note: UnitTests showed that TOWGS84 is usable inside a DATUM element.
        internal Parser<char, WktDatum> DatumParser => String("DATUM")
            .Then(LeftDelimiterParser)
            .Then(DatumNameParser, (c, name) => new WktDatum(name).SetLeftDelimiter(c).As<WktDatum>() )
            .Then(WktSeparatorParser.Then(SpheroidParser), (wktObject, spheroid) => wktObject.SetSpheroid(spheroid))
            .Then(Try(WktSeparatorParser.Then(ToWgs84Parser)).Optional(), (wktObject, towgs84) => wktObject.SetToWgs84(towgs84.GetValueOrDefault()))
            .Then(WktSeparatorParser.Then(AuthorityParser).Optional(), (wktObject, authority) => wktObject.SetAuthority(authority.GetValueOrDefault()))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktDatum>() )
            .Labelled("WktDatum");


        // <geographic cs> ::= GEOGCS <left delimiter> <csname>
        //                              <comma> <datum> <comma> <prime meridian>
        //                              <comma> <angular unit> (<comma> <linear unit> )
        //                            <right delimiter>
        internal Parser<char, WktGeographicCoordinateSystem> GeographicCsParser => String("GEOGCS")
            .Then(LeftDelimiterParser)
            .Then(CsNameParser, (c, name) => new WktGeographicCoordinateSystem(name).SetLeftDelimiter(c).As<WktGeographicCoordinateSystem>())
            .Then(WktSeparatorParser.Then(DatumParser), (wktObject, datum) => wktObject.SetDatum(datum))
            .Then(WktSeparatorParser.Then(PrimeMeridianParser),(wktObject, meridian) => wktObject.SetPrimeMeridian(meridian))
            .Then(WktSeparatorParser.Then(UnitParser).Optional(), (wktObject, unit) => wktObject.SetUnit(unit.GetValueOrDefault()))
            .Then(Try(WktSeparatorParser.Then(AxisParser)).Many(), (wktObject, axis) => wktObject.SetAxes(axis))
            .Then(WktSeparatorParser.Then(AuthorityParser).Optional(), ((wktObject, authority) => wktObject.SetAuthority(authority.GetValueOrDefault())))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktGeographicCoordinateSystem>())
            .Labelled("WktGeographicCoordinateSystem");


        // <geocentric cs> ::= GEOCCS <left delimiter> <name> <comma> <datum> <comma> <prime meridian>
        //                                             <comma> <linear unit> <right delimiter>
        internal Parser<char, WktGeocentricCoordinateSystem> GeocentricCsParser => String("GEOCCS").Or(String("GEOCS"))
            .Then(LeftDelimiterParser)
            .Then(QuotedNameParser, (c, name) => new WktGeocentricCoordinateSystem(name).SetLeftDelimiter(c).As<WktGeocentricCoordinateSystem>() )
            .Then(WktSeparatorParser.Then(DatumParser), (wktObject, datum) => wktObject.SetDatum(datum))
            .Then(WktSeparatorParser.Then(PrimeMeridianParser), (wktObject, meridian) => wktObject.SetPrimeMeridian(meridian))
            .Then(WktSeparatorParser.Then(LinearUnitParser), (wktObject, unit) => wktObject.SetLinearUnit(unit))
            .Then(Try(WktSeparatorParser.Then(AxisParser)).Many(), (wktObject, axes) => wktObject.SetAxes(axes))
            .Then(WktSeparatorParser.Then(AuthorityParser).Optional(), (wktObject, authority) => wktObject.SetAuthority(authority.GetValueOrDefault()))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktGeocentricCoordinateSystem>() )
            .Labelled("WktGeocentricCoordinateSystem");

        // <projected cs> ::= PROJCS <left delimiter> <csname> <comma> <geographic cs> <comma> <projection>
        //                           (<comma> <parameter> )* <comma> <linear unit> <right delimiter>
        internal Parser<char, WktProjectedCoordinateSystem> ProjectedCsParser => String("PROJCS")
            .Then(LeftDelimiterParser)
            .Then(CsNameParser, (c, name) => new WktProjectedCoordinateSystem(name).SetLeftDelimiter(c).As<WktProjectedCoordinateSystem>())
            .Then(WktSeparatorParser.Then(GeographicCsParser), (wktObject, csSystem) => wktObject.SetGeographicCoordinateSystem(csSystem))
            .Then(WktSeparatorParser.Then(ProjectionParser), (wktObject, projection) => wktObject.SetProjection(projection))
            .Then(Try(WktSeparatorParser.Then(ParameterParser)).Many(), (wktObject, parameters) => wktObject.SetParameters(parameters))
            .Then(Try(WktSeparatorParser.Then(UnitParser)).Optional(), (wktObject, unit) => wktObject.SetUnit(unit.GetValueOrDefault()))
            .Then(Try(WktSeparatorParser.Then(AxisParser)).Many(), (wktObject, axes) => wktObject.SetAxes(axes))
            .Then(Try(WktSeparatorParser.Then(ExtensionParser)).Optional(), (wktObject, extension) => wktObject.SetExtension(extension.GetValueOrDefault()))
            .Then(WktSeparatorParser.Then(AuthorityParser).Optional(), (wktObject, authority) => wktObject.SetAuthority(authority.GetValueOrDefault()))
            .Then(RightDelimiterParser, (wktObject, c) => wktObject.SetRightDelimiter(c).As<WktProjectedCoordinateSystem>() )
            .Labelled("WktProjectedCoordinateSystem");

        // <spatial reference system> ::= <projected cs> | <geographic cs> | <geocentric cs>
        internal Parser<char, WktCoordinateSystem> SpatialReferenceSystemParser => OneOf(
            Try(ProjectedCsParser.Cast<WktCoordinateSystem>()),
            Try(GeographicCsParser.Cast<WktCoordinateSystem>()),
            Try(GeocentricCsParser.Cast<WktCoordinateSystem>())
        ).Labelled("WktCoordinateSystem");

    }
}
