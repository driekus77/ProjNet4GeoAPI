using System;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// Base class for all WKT Objects.
    /// </summary>
    public class WktBaseObject : IWktObject
    {
        /// <summary>
        /// LeftDelimiter used after keyword to surround content.
        /// </summary>
        public char LeftDelimiter { get; set; }

        /// <summary>
        /// RightDelimiter used after content.
        /// </summary>
        public char RightDelimiter { get; set; }


        /// <inheritdoc/>
        public virtual IWktObject SetLeftDelimiter(char leftDelimiter)
        {
            LeftDelimiter = leftDelimiter;
            return this;
        }

        /// <inheritdoc/>
        public virtual IWktObject SetRightDelimiter(char rightDelimiter)
        {
            RightDelimiter = rightDelimiter;
            return this;
        }



        /// <summary>
        /// Cast function for all IWktObject"s.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual T As<T>() where T : IWktObject
        {
            return (T) Convert.ChangeType(this, typeof(T));
        }


    }
}
