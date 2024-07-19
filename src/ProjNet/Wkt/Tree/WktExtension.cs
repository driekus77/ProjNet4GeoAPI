using System;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktExtension
    /// </summary>
    public class WktExtension : WktBaseObject, IEquatable<WktExtension>
    {
        /// <summary>
        /// Name of this Extension.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Direction for this Authority.
        /// </summary>
        public string Value { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        public WktExtension(string name = null)
        {
            Name = name;
        }

        /// <summary>
        /// Setter for Name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WktExtension SetName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        /// Setter for Code.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public WktExtension SetValue(string value)
        {
            Value = value;
            return this;
        }


        public bool Equals(WktExtension other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktExtension) obj);
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
