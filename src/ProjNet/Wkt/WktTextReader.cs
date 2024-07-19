using System;
using System.Collections.Generic;
using System.IO;
using Pidgin;
using ProjNet.CoordinateSystems;
using ProjNet.Wkt.Tree;
using Unit = ProjNet.CoordinateSystems.Unit;

namespace ProjNet.Wkt
{
    /// <summary>
    /// Reader for WKT.
    /// </summary>
    public class WktTextReader : IDisposable
    {
        internal WktParser Parser;

        /// <summary>
        /// TextStream full of WKT characters.
        /// </summary>
        protected TextReader Reader;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="reader"></param>
        public WktTextReader(TextReader reader)
        {
            Parser = new WktParser();
            Reader = reader;
        }

        /// <summary>
        /// Dispose the Reader.
        /// </summary>
        public void Dispose()
        {
            Reader?.Dispose();
        }



        /// <summary>
        /// Initiate the read from this Wkt Text reader.
        /// </summary>
        /// <returns></returns>
        public Result<char, WktCoordinateSystem> ReadToEnd()
        {
            return Parser.SpatialReferenceSystemParser.Parse(Reader);
        }
    }
}
