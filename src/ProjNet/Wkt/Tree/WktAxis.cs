using System;
using ProjNet.CoordinateSystems;

namespace ProjNet.Wkt.Tree
{
    /// <summary>
    /// WktAxis - Simple POCO for Axis info.
    /// </summary>
    public class WktAxis : WktBaseObject, IEquatable<WktAxis>
    {
        /// <summary>
        /// Constructor using string for direction.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="direction"></param>
        public WktAxis(string name = null)
        {
            Name = name;
        }


        /// <summary>
        /// Name of this Axis.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Direction for this Axis.
        /// </summary>
        public AxisOrientationEnum Direction { get; set; }


        /// <summary>
        /// Name Setter method.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WktAxis SetName(string name)
        {
            Name = name;
            return this;
        }


        /// <summary>
        /// Direction Setter method.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public WktAxis SetDirection(string direction)
        {
            if (!string.IsNullOrWhiteSpace(direction))
            {
                Direction = (AxisOrientationEnum)Enum.Parse(typeof(AxisOrientationEnum), direction, true);
            }

            return this;
        }

        public bool Equals(WktAxis other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Direction == other.Direction;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WktAxis) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (int) Direction;
            }
        }
    }
}
