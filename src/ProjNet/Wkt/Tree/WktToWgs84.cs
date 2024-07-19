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
        /// Constructor optionaly setting the Description.
        /// </summary>
        /// <param name="description"></param>
        public WktToWgs84(string description = default)
        {
            Description = description;
        }

        /// <summary>
        /// SetDxShift setter method.
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        public WktToWgs84 SetDxShift(double shift)
        {
            DxShift = shift;
            return this;
        }

        /// <summary>
        /// SetDyShift setter method.
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        public WktToWgs84 SetDyShift(double shift)
        {
            DyShift = shift;
            return this;
        }

        /// <summary>
        /// SetDzShift setter method.
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        public WktToWgs84 SetDzShift(double shift)
        {
            DzShift = shift;
            return this;
        }

        /// <summary>
        /// SetExRotation setter method.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public WktToWgs84 SetExRotation(double rotation)
        {
            ExRotation = rotation;
            return this;
        }

        /// <summary>
        /// SetEyRotation setter method.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public WktToWgs84 SetEyRotation(double rotation)
        {
            EyRotation = rotation;
            return this;
        }

        /// <summary>
        /// SetEzRotation setter method.
        /// </summary>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public WktToWgs84 SetEzRotation(double rotation)
        {
            EzRotation = rotation;
            return this;
        }

        /// <summary>
        /// SetPpmScaling setter method.
        /// </summary>
        /// <param name="ppm"></param>
        /// <returns></returns>
        public WktToWgs84 SetPpmScaling(double ppm)
        {
            PpmScaling = ppm;
            return this;
        }

        /// <summary>
        /// SetDescription setter method.
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public WktToWgs84 SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public bool Equals(WktToWgs84 other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DxShift.Equals(other.DxShift) && DyShift.Equals(other.DyShift) && DzShift.Equals(other.DzShift) && ExRotation.Equals(other.ExRotation) && EyRotation.Equals(other.EyRotation) && EzRotation.Equals(other.EzRotation) && PpmScaling.Equals(other.PpmScaling) && Description == other.Description;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktToWgs84) obj);
        }

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
    }
}
