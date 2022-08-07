using System;

namespace Broadcast.Configuration
{
    /// <summary>
    /// Base for enumerations
    /// </summary>
    public abstract class Enumeration : IComparable
    {
        /// <summary>
        /// Gets the Id
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the Name
        /// </summary>
        public string Name { get; }

        protected Enumeration(string name)
        {
            Id = 0;
            Name = name;
        }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// The string value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Gets the lower string value
        /// </summary>
        /// <returns></returns>
        public string ToLower()
        {
            return Name.ToLower();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (Id.CompareTo(((Enumeration)obj).Id) > 0 || string.Compare(Name, ((Enumeration)obj).Name, StringComparison.Ordinal) > 0)
            {
                return 1;
            }

            if (Id.CompareTo(((Enumeration)obj).Id) == 0 && string.Compare(Name, ((Enumeration)obj).Name, StringComparison.Ordinal) == 0)
            {
                return 0;
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static int Compare(Enumeration left, Enumeration right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return 0;
            }

            if (left is null)
            {
                return -1;
            }

            return left.CompareTo(right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is string v)
            {
                return ToString() == v;
            }

            return obj is Enumeration en && en.Id == Id && en.ToLower() == ToLower();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return new { Id, Name }.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="en"></param>
        public static implicit operator string(Enumeration en)
        {
            return en?.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Enumeration left, Enumeration right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Enumeration left, Enumeration right)
        {
            return !(left == right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(Enumeration left, Enumeration right)
        {
            return Compare(left, right) < 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(Enumeration left, Enumeration right)
        {
            return Compare(left, right) <= 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(Enumeration left, Enumeration right)
        {
            return Compare(left, right) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(Enumeration left, Enumeration right)
        {
            return Compare(left, right) >= 0;
        }
    }
}
