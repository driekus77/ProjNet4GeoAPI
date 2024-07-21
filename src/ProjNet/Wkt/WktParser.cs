using System;
using System.Globalization;
using System.Linq;
using Pidgin;
using ProjNet.CoordinateSystems;
using ProjNet.Wkt.Tree;
using static Pidgin.Parser;


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
        internal static readonly Parser<char, char> SimpleLatinLetterParser = Letter;
            //SimpleLatinUpperCaseLetterParser.Or(SimpleLatinLowerCaseLetterParser);

        // <unsigned numeric literal> ::= <exact numeric literal> | <approximate numeric literal>
        internal static readonly Parser<char, double> UnsignedNumericLiteralParser = ExactNumericLiteralParser
            .Or(ApproximateNumericLiteralParser);

        // <signed numeric literal> ::= {<sign>}<unsigned numeric literal>
        internal static readonly Parser<char, double> SignedNumericLiteralParser = BetweenWhitespace(
            SignParser.Optional()
            .Then(UnsignedNumericLiteralParser, (sign, d) =>
            {
                int signFactor = sign.GetValueOrDefault() == '-' ? -1 : 1;
                return signFactor * d;
            } ));

        // <x> ::= <signed numeric literal>
        internal static readonly Parser<char, double> XParser = SignedNumericLiteralParser;
        // <y> ::= <signed numeric literal>
        internal static readonly Parser<char, double> YParser = SignedNumericLiteralParser;
        // <z> ::= <signed numeric literal>
        internal static readonly Parser<char, double> ZParser = SignedNumericLiteralParser;
        // <m> ::= <signed numeric literal>
        internal static readonly Parser<char, double> MParser = SignedNumericLiteralParser;

        // <letter> ::= <simple Latin letter>|<digit>|<special>
        // NOTE: This one is very slow comparing to using AnyCharExcept!
        internal static readonly Parser<char, char> LetterParser = OneOf(SimpleLatinLetterParser, DigitParser, SpecialParser);
        // <letters> ::= (<letter>)*
        internal static readonly Parser<char, string> LettersParser = AnyCharExcept('\"').ManyString();

        // <name> ::= <letters>
        internal static readonly Parser<char, string> NameParser = SimpleLatinLetterParser.Or(DigitParser).ManyString();
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


        internal readonly Parser<char, WktAuthority> AuthorityParser;

        internal readonly Parser<char, WktAxis> AxisParser;

        internal readonly Parser<char, WktExtension> ExtensionParser;

        internal readonly Parser<char, WktToWgs84> ToWgs84Parser;

        internal readonly Parser<char, WktProjection> ProjectionParser;

        internal readonly Parser<char, WktParameter> ParameterParser;

        internal readonly Parser<char, WktEllipsoid> EllipsoidParser;
        internal readonly Parser<char, WktSpheroid> SpheroidParser;

        internal readonly Parser<char, WktPrimeMeridian> PrimeMeridianParser;

        internal readonly Parser<char, WktUnit> UnitParser;

        internal readonly Parser<char, WktDatum> DatumParser;

        internal readonly Parser<char, WktParameterMathTransform> ParameterMathTransformParser;
        internal readonly Parser<char, WktFittedCoordinateSystem> FittedCsParser;

        internal readonly Parser<char, WktGeographicCoordinateSystem> GeographicCsParser;
        internal readonly Parser<char, WktGeocentricCoordinateSystem> GeocentricCsParser;
        internal readonly Parser<char, WktProjectedCoordinateSystem> ProjectedCsParser;

        internal readonly Parser<char, WktCoordinateSystem> SpatialReferenceSystemParser;


        /// <summary>
        /// Constructor initializing all parsers.
        /// </summary>
        public WktParser()
        {
            // AUTHORITY    = 'AUTHORITY' '[' string ',' string ']'
            AuthorityParser =
                Map((kw, ld, name, code, rd) => new WktAuthority(name, code, kw,ld, rd),
                    String("AUTHORITY"),
                    LeftDelimiterParser,
                    QuotedNameParser,
                    WktSeparatorParser.Then(Try(QuotedNameParser).Or(DigitParser.Many().Select(digits => new string(digits.ToArray())))),
                    RightDelimiterParser)
                .Labelled("WktAuthority");

            // AXIS         = 'AXIS' '[' string ',' string ']'
            AxisParser =
                Map((kw, ld,name, direction, rd) => new WktAxis(name, direction, kw, ld, rd),
                    String("AXIS"),
                LeftDelimiterParser,
                QuotedNameParser,
                WktSeparatorParser.Then(NameParser),
                RightDelimiterParser)
                .Labelled("WktAxis");

            ExtensionParser =
                Map((kw, ld,name, direction, rd) => new WktExtension(name, direction, kw, ld, rd),
                String("EXTENSION"),
                LeftDelimiterParser,
                QuotedNameParser,
                WktSeparatorParser.Then(LettersParser.Between(DoubleQuoteParser)),
                RightDelimiterParser)
                .Labelled("WktExtension");

            // TOWGS84      = 'TOWGS84' '[' float ',' float ',' float ',' float ',' float ',' float ',' float ']'
            // Note: Map only takes 8 parsers max. So combining sub parsers first for shifts and rotations.
           ToWgs84Parser =
               Map((kw, ld, shifts, rotations, ppm, rd) =>
                           new WktToWgs84(shifts.dx, shifts.dy, shifts.dz,
                                            rotations.ex, rotations.ey, rotations.ez, ppm,
                                            "", kw, ld, rd),
                   String("TOWGS84"),
                 LeftDelimiterParser,
                 Map((dx, dy, dz) => (dx, dy, dz),
                         ValueParser,
                         WktSeparatorParser.Then(ValueParser),
                         WktSeparatorParser.Then(ValueParser)),
                   Map((ex, ey, ez) => (ex, ey, ez),
                         WktSeparatorParser.Then(ValueParser),
                         WktSeparatorParser.Then(ValueParser),
                         WktSeparatorParser.Then(ValueParser)),
                 WktSeparatorParser.Then(ValueParser),
                 RightDelimiterParser)
                 .Labelled("WktToWgs84");

            // <projection> ::= PROJECTION <left delimiter> <projection name> <right delimiter>
            ProjectionParser =
                Map((kw, ld, name, authority, rd) =>
                            new WktProjection(name, authority.GetValueOrDefault(), kw, ld, rd),
                String("PROJECTION"),
                LeftDelimiterParser,
                ProjectionNameParser,
                WktSeparatorParser.Then(AuthorityParser).Optional(),
                RightDelimiterParser)
                .Labelled("WktProjection");

            // <parameter> ::= PARAMETER <left delimiter> <parameter name> <comma> <value> <right delimiter>
            ParameterParser =
                Map((kw, ld, name, value, rd) => new WktParameter(name, value, kw, ld, rd),
                String("PARAMETER"),
                LeftDelimiterParser,
                ParameterNameParser,
                WktSeparatorParser.Then(ValueParser),
                RightDelimiterParser)
                .Labelled("WktParameter");

            // Note: Ellipsoid and Spheroid have the same parser and object implementation. Separating them for clarity.
            // <ellipsoid> ::= ELLIPSOID <left delimiter> <ellipsoid name> <comma> <semi-major axis> <comma> <inverse flattening> <right delimiter>
            EllipsoidParser =
                Map((kw,ld, name, axis, flat, authority, rd) =>
                                new WktEllipsoid(name, axis, flat, authority.GetValueOrDefault(), kw, ld, rd),
                String("ELLIPSOID"),
                LeftDelimiterParser,
                SpheroidNameParser,
                WktSeparatorParser.Then(SemiMajorAxisParser),
                WktSeparatorParser.Then(InverseFlatteningParser),
                WktSeparatorParser.Then(AuthorityParser).Optional(),
                RightDelimiterParser)
                .Labelled("WktEllipsoid");
            // <spheroid> ::= SPHEROID <left delimiter> <spheroid name> <comma> <semi-major axis> <comma> <inverse flattening> <right delimiter>
            SpheroidParser =
                Map((kw,ld, name, axis, flat, authority, rd) =>
                                new WktSpheroid(name, axis, flat, authority.GetValueOrDefault(), kw, ld, rd),
                String("SPHEROID"),
                LeftDelimiterParser,
                SpheroidNameParser,
                WktSeparatorParser.Then(SemiMajorAxisParser),
                WktSeparatorParser.Then(InverseFlatteningParser),
                WktSeparatorParser.Then(AuthorityParser).Optional(),
                RightDelimiterParser)
                .Labelled("WktSpheroid");

            // <prime meridian> ::= PRIMEM <left delimiter> <prime meridian name> <comma> <longitude> [<comma> <authority>] <right delimiter>
            PrimeMeridianParser =
                Map((kw, ld, name, longitude, authority, rd)
                            => new WktPrimeMeridian(name, longitude, authority.GetValueOrDefault(), kw, ld, rd),
                    String("PRIMEM"),
                    LeftDelimiterParser,
                    PrimeMeridianNameParser,
                    WktSeparatorParser.Then(LongitudeParser),
                    WktSeparatorParser.Then(AuthorityParser).Optional(),
                    RightDelimiterParser)
                    .Labelled("WktPrimeMeridian");

            // <unit> ::= UNIT <left delimiter> <unit name> <comma> <conversion factor> [<comma> <authority>] <right delimiter>
            UnitParser =
                Map((kw, ld, name, factor, authority, rd) =>
                        {
                            switch (name)
                            {
                                case "degree":
                                    return new WktAngularUnit(name, factor, authority.GetValueOrDefault(), kw, ld, rd);
                                case "metre":
                                    return new WktLinearUnit(name, factor, authority.GetValueOrDefault(), kw, ld, rd);
                                default:
                                    return new WktUnit(name, factor, authority.GetValueOrDefault(), kw, ld, rd);
                            }
                        },
            String("UNIT"),
                   LeftDelimiterParser,
                   UnitNameParser,
                   WktSeparatorParser.Then(ConversionFactorParser),
                   WktSeparatorParser.Then(AuthorityParser).Optional(),
                   RightDelimiterParser)
                .Labelled("WktUnit");


            // <datum> ::= DATUM <left delimiter> <datum name> <comma> <spheroid> <right delimiter>
            // Note: UnitTests showed that TOWGS84 is usable inside a DATUM element.
            DatumParser =
                Map((kw, ld, name, spheroid, towgs84, authority, rd) =>
                                    new WktDatum(name, spheroid, towgs84.GetValueOrDefault(), authority.GetValueOrDefault(), kw, ld, rd),
                    String("DATUM"),
                    LeftDelimiterParser,
                    DatumNameParser,
                    WktSeparatorParser.Then(SpheroidParser),
                    Try(WktSeparatorParser.Then(ToWgs84Parser)).Optional(),
                    WktSeparatorParser.Then(AuthorityParser).Optional(),
                    RightDelimiterParser)
                .Labelled("WktDatum");


            // <geographic cs> ::= GEOGCS <left delimiter> <csname>
            //                              <comma> <datum> <comma> <prime meridian>
            //                              <comma> <angular unit> (<comma> <linear unit> )
            //                            <right delimiter>
            GeographicCsParser =
                Map((kw, ld,name, datum, meridian, o, rd) =>
                                new WktGeographicCoordinateSystem(name, datum, meridian, o.unit.GetValueOrDefault(), o.axes, o.authority.GetValueOrDefault(), kw, ld, rd),
                    String("GEOGCS"),
                    LeftDelimiterParser,
                    CsNameParser,
                    WktSeparatorParser.Then(DatumParser),
                    WktSeparatorParser.Then(PrimeMeridianParser),
                    Map((unit, axes,authority) =>
                                        (unit, axes, authority),
                        WktSeparatorParser.Then(UnitParser).Optional(),
                        Try(WktSeparatorParser.Then(AxisParser)).Many(),
                        WktSeparatorParser.Then(AuthorityParser).Optional()),
                    RightDelimiterParser)
                .Labelled("WktGeographicCoordinateSystem");


            // <geocentric cs> ::= GEOCCS <left delimiter> <name> <comma> <datum> <comma> <prime meridian>
            //                                             <comma> <linear unit> <right delimiter>
            GeocentricCsParser =
                Map((kw, ld, name, datum, meridian, unit, o, rd) =>
                        new WktGeocentricCoordinateSystem(name, datum, meridian, unit, o.axes, o.authority.GetValueOrDefault(),
                            kw, ld, rd ),
                String("GEOCCS").Or(String("GEOCS")),
                LeftDelimiterParser,
                QuotedNameParser,
                WktSeparatorParser.Then(DatumParser),
                WktSeparatorParser.Then(PrimeMeridianParser),
                WktSeparatorParser.Then(UnitParser),
                Map((axes, authority) => (axes, authority),
                    Try(WktSeparatorParser.Then(AxisParser)).Many(),
                    WktSeparatorParser.Then(AuthorityParser).Optional()),
                RightDelimiterParser)
                .Labelled("WktGeocentricCoordinateSystem");

            // <projected cs> ::= PROJCS <left delimiter> <csname> <comma> <geographic cs> <comma> <projection>
            //                           (<comma> <parameter> )* <comma> <linear unit> <right delimiter>
            ProjectedCsParser =
                Map((kw, ld, name, geogcs, projection, o, rd) =>
                            new WktProjectedCoordinateSystem(name, geogcs, projection,
                                                                o.parameters, o.unit.GetValueOrDefault(), o.axes, o.extension.GetValueOrDefault(), o.authority.GetValueOrDefault(),
                                                                kw, ld, rd),
                    String("PROJCS"),
                    LeftDelimiterParser,
                    CsNameParser,
                    WktSeparatorParser.Then(GeographicCsParser),
                    WktSeparatorParser.Then(ProjectionParser),
                    Map((parameters,unit,axes,extension,authority )
                                    => (parameters, unit, axes, extension, authority),
                        Try(WktSeparatorParser.Then(ParameterParser)).Many(),
                        Try(WktSeparatorParser.Then(UnitParser)).Optional(),
                        Try(WktSeparatorParser.Then(AxisParser)).Many(),
                        Try(WktSeparatorParser.Then(ExtensionParser)).Optional(),
                        WktSeparatorParser.Then(AuthorityParser).Optional()),
                    RightDelimiterParser)
                .Labelled("WktProjectedCoordinateSystem");


           // Fitted CoordinateSystem parser(s)
           ParameterMathTransformParser =
               Map((kw, ld, name, parameters, rd) => new WktParameterMathTransform(name, parameters, kw, ld, rd),
                       String("PARAM_MT"),
                       LeftDelimiterParser,
                       ParameterNameParser,
                       WktSeparatorParser.Then(ParameterParser).Many(),
                       RightDelimiterParser)
                   .Labelled("WktParameterMathTransform");

           FittedCsParser =
               Map((kw, ld,name,pmt, projcs, authority, rd) =>
                           new WktFittedCoordinateSystem(name, pmt, projcs, authority.GetValueOrDefault(), kw, ld, rd),
                       String("FITTED_CS"),
                       LeftDelimiterParser,
                       CsNameParser,
                       WktSeparatorParser.Then(ParameterMathTransformParser),
                       WktSeparatorParser.Then(ProjectedCsParser),
                       WktSeparatorParser.Then(AuthorityParser).Optional(),
                       RightDelimiterParser)
                   .Labelled("WktFittedCoordinateSystem");


            // <spatial reference system> ::= <projected cs> | <geographic cs> | <geocentric cs>
            SpatialReferenceSystemParser = OneOf(
                Try(ProjectedCsParser.Cast<WktCoordinateSystem>()),
                Try(GeographicCsParser.Cast<WktCoordinateSystem>()),
                Try(GeocentricCsParser.Cast<WktCoordinateSystem>()),
                Try(FittedCsParser.Cast<WktCoordinateSystem>())
            ).Labelled("WktCoordinateSystem");
        }
    }
}
