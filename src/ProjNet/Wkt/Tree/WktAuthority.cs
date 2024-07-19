using System;
using System.Linq;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktAuthority - Simple POCO for Authority info.
    /// </summary>
    public class WktAuthority : WktBaseObject, IEquatable<WktAuthority>
    {
        /// <summary>
        /// Name of this Authority.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Direction for this Authority.
        /// </summary>
        public long Code { get; set; }


        /// <summary>
        /// Setter for Name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WktAuthority SetName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        /// Setter for Code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public WktAuthority SetCode(string code)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                Code = long.Parse(new string(code.Where(c => char.IsDigit(c)).ToArray()));
            }

            return this;
        }


        public bool Equals(WktAuthority other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Code == other.Code;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktAuthority) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Code.GetHashCode();
            }
        }
    }
}
