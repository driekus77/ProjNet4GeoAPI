using System;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktEllipsoid class.
    /// </summary>
    public class WktEllipsoid : WktBaseObject, IEquatable<WktEllipsoid>
    {
        /// <summary>
        /// Name property.
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        /// SemiMajorAxis property.
        /// </summary>
        public double SemiMajorAxis { get; internal set; }
        /// <summary>
        /// InverseFlattening property.
        /// </summary>
        public double InverseFlattening { get; internal set; }
        /// <summary>
        /// Authority property.
        /// </summary>
        public WktAuthority Authority { get; internal set; }

        /// <summary>
        /// Constructor for a WktEllipsoid.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="semiMajorAxis"></param>
        /// <param name="inverseFlattening"></param>
        /// <param name="authority"></param>
        /// <param name="keyword"></param>
        /// <param name="leftDelimiter"></param>
        /// <param name="rightDelimiter"></param>
        public WktEllipsoid(string name, double semiMajorAxis, double inverseFlattening, WktAuthority authority,
                            string keyword = "ELLIPSOID", char leftDelimiter = '[', char rightDelimiter = ']')
            : base(keyword, leftDelimiter, rightDelimiter)
        {
            Name = name;
            SemiMajorAxis = semiMajorAxis;
            InverseFlattening = inverseFlattening;
            Authority = authority;
        }



        /// <summary>
        /// IEquatable.Equals implementation checking the whole tree.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(WktEllipsoid other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && SemiMajorAxis.Equals(other.SemiMajorAxis) && InverseFlattening.Equals(other.InverseFlattening) && Equals(Authority, other.Authority);
        }

        /// <summary>
        /// Override of basic Equals.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktEllipsoid) obj);
        }

        /// <summary>
        /// Override of basic GetHashCode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ SemiMajorAxis.GetHashCode();
                hashCode = (hashCode * 397) ^ InverseFlattening.GetHashCode();
                hashCode = (hashCode * 397) ^ (Authority != null ? Authority.GetHashCode() : 0);
                return hashCode;
            }
        }


        /// <inheritdoc/>
        public override void Traverse(IWktTraverseHandler handler)
        {
            if (Authority!=null)
                Authority.Traverse(handler);

            handler.Handle(this);
        }
    }
}
