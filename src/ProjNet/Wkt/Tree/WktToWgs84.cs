using System;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktToWgs84 - Simple POCO for ToWgs84.
    /// </summary>
    public class WktToWgs84 : WktBaseObject, IEquatable<WktToWgs84>
    {

        /// <summary>
        /// DxShift property.
        /// </summary>
        public double DxShift { get; internal set; }

        /// <summary>
        /// DyShift property.
        /// </summary>
        public double DyShift { get; internal set; }

        /// <summary>
        /// DzShift Property.
        /// </summary>
        public double DzShift { get; internal set; }

        /// <summary>
        /// ExRotation property.
        /// </summary>
        public double ExRotation { get; internal set; }
        /// <summary>
        /// EyRotation property.
        /// </summary>
        public double EyRotation { get; internal set; }
        /// <summary>
        /// EzRotation property.
        /// </summary>
        public double EzRotation { get; internal set; }
        /// <summary>
        /// PpmScaling property.
        /// </summary>
        public double PpmScaling { get; internal set; }

        /// <summary>
        /// Description property.
        /// </summary>
        public string Description { get; internal set; }


        /// <summary>
        /// Constructor optionally setting the Description.
        /// </summary>
        /// <param name="description"></param>
        public WktToWgs84(double dxShift, double dyShift, double dzShift,
                            double exRotation, double eyRotation, double ezRotation,
                            double ppmScaling, string description = default,
                            string keyword = "TOWGS84", char leftDelimiter = '[', char rightDelimiter = ']')
            : base(keyword, leftDelimiter, rightDelimiter)
        {
            DxShift = dxShift;
            DyShift = dyShift;
            DzShift = dzShift;

            ExRotation = exRotation;
            EyRotation = eyRotation;
            EzRotation = ezRotation;

            PpmScaling = ppmScaling;

            Description = description;
        }


        /// <summary>
        /// IEquatable.Equals implementation checking the whole tree.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(WktToWgs84 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DxShift.Equals(other.DxShift) && DyShift.Equals(other.DyShift) && DzShift.Equals(other.DzShift) && ExRotation.Equals(other.ExRotation) && EyRotation.Equals(other.EyRotation) && EzRotation.Equals(other.EzRotation) && PpmScaling.Equals(other.PpmScaling) && Description == other.Description;
        }

        /// <summary>
        /// Basic override of Equals.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktToWgs84) obj);
        }

        /// <summary>
        /// Basic override of GetHashCode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = DxShift.GetHashCode();
                hashCode = (hashCode * 397) ^ DyShift.GetHashCode();
                hashCode = (hashCode * 397) ^ DzShift.GetHashCode();
                hashCode = (hashCode * 397) ^ ExRotation.GetHashCode();
                hashCode = (hashCode * 397) ^ EyRotation.GetHashCode();
                hashCode = (hashCode * 397) ^ EzRotation.GetHashCode();
                hashCode = (hashCode * 397) ^ PpmScaling.GetHashCode();
                hashCode = (hashCode * 397) ^ (Description != null ? Description.GetHashCode() : 0);
                return hashCode;
            }
        }

        /// <inheritdoc/>
        public override void Traverse(IWktTraverseHandler handler)
        {
            handler.Handle(this);
        }
    }
}
