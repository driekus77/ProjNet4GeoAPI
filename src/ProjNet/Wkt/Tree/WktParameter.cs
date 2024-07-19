using System;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktParameter
    /// </summary>
    public class WktParameter : WktBaseObject, IEquatable<WktParameter>
    {

        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// Value property.
        /// </summary>
        public double Value { get; internal set; }


        /// <summary>
        /// Constructor with optional name.
        /// </summary>
        /// <param name="name"></param>
        public WktParameter(string name = null)
        {
            Name = name;
        }


        /// <summary>
        /// Name setter method.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WktParameter SetName(string name)
        {
            Name = name;
            return this;
        }


        /// <summary>
        /// Value setter method.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public WktParameter SetValue(double d)
        {
            Value = d;
            return this;
        }

        public bool Equals(WktParameter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktParameter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Value.GetHashCode();
            }
        }
    }
}
