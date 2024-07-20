
using System;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// Base class for all WktCoordinateSystem classes.
    /// </summary>
    public class WktCoordinateSystem : WktBaseObject
    {

        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; internal set; }



        /// <summary>
        /// Constructor for CoordinateSystem.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="keyword"></param>
        /// <param name="leftDelimiter"></param>
        /// <param name="rightDelimiter"></param>
        public WktCoordinateSystem(string name,
                                    string keyword, char leftDelimiter = '[', char rightDelimiter = ']')
            : base(keyword, leftDelimiter, rightDelimiter)
        {
            Name = name;
        }

    }
}
