using System;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// Base class for all WKT Objects.
    /// </summary>
    public class WktBaseObject : IWktObject
    {
        /// <inheritdoc/>
        public string Keyword { get; internal set; }

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
        /// Constructor for all Wkt Object's.
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <exception cref="NotSupportedException"></exception>
        public WktBaseObject(string keyword = null, char left = '[', char right = ']')
        {
            Keyword = keyword;

            // Check the provided delimiters and store them.
            if (!((left == '[' && right == ']') || (left == '(' && right == ')')))
                throw new NotSupportedException(
                    "Delimiters are not supported or left and right delimiters don't match!");

            LeftDelimiter = left;
            RightDelimiter = right;
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
