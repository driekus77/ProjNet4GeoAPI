
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
        public WktCoordinateSystem(string name = null)
        {
            Name = name;
        }


        /// <summary>
        /// Setter method for Name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual WktCoordinateSystem SetName(string name)
        {
            Name = name;
            return this;
        }


    }
}
