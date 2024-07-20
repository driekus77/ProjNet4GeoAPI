using System;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktDatum class.
    /// </summary>
    public class WktDatum : WktBaseObject, IEquatable<WktDatum>
    {
        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Spheroid property.
        /// </summary>
        public WktSpheroid Spheroid { get; internal set; }

        /// <summary>
        /// ToWgs84 property.
        /// </summary>
        public WktToWgs84 ToWgs84 { get; internal set; }

        /// <summary>
        /// Authority property.
        /// </summary>
        public WktAuthority Authority { get; internal set; }


        /// <summary>
        /// Constructor for WKT Datum.
        /// </summary>
        /// <param name="name"></param>
        public WktDatum(string name, WktSpheroid spheroid, WktToWgs84 toWgs84, WktAuthority authority,
                        string keyword = "DATUM", char leftDelimiter = '[', char rightDelimiter = ']')
            : base(keyword, leftDelimiter, rightDelimiter)
        {
            Name = name;
            Spheroid = spheroid;
            ToWgs84 = toWgs84;
            Authority = authority;
        }


        /// <summary>
        /// IEquatable.Equals implementation checking the whole tree.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(WktDatum other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Equals(Spheroid, other.Spheroid) && Equals(ToWgs84, other.ToWgs84) && Equals(Authority, other.Authority);
        }

        /// <summary>
        /// Override basic Equals method.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktDatum) obj);
        }

        /// <summary>
        /// Override basic GetHashCode method.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Spheroid != null ? Spheroid.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ToWgs84 != null ? ToWgs84.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Authority != null ? Authority.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public override void Traverse(IWktTraverseHandler handler)
        {
            if (Spheroid!=null)
                this.Spheroid.Traverse(handler);
            if (ToWgs84!=null)
                this.ToWgs84.Traverse(handler);
            if (Authority!=null)
                this.Authority.Traverse(handler);

            handler.Handle(this);
        }
    }
}
