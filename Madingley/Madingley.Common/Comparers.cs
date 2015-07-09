using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    /// <summary>
    /// IEqualityComparer&lt;double&gt; implementation to compare two doubles by looking at their bitwise representations.
    /// See: <a href="https://msdn.microsoft.com/en-us/library/ya2zha7s(v=vs.110).aspx">https://msdn.microsoft.com/en-us/library/ya2zha7s(v=vs.110).aspx</a>
    /// </summary>
    public class TolerantDoubleComparer : IEqualityComparer<double>
    {
        /// <summary>
        /// Maximum distance between bitwise representations to be regarded as equal.
        /// </summary>
        Int64 Units { get; set; }

        /// <summary>
        /// TolerantDoubleComparer constructor.
        /// </summary>
        /// <param name="units">Maximum distance.</param>
        public TolerantDoubleComparer(Int64 units)
        {
            this.Units = units;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type double to compare.</param>
        /// <param name="y">The second object of type double to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(double x, double y)
        {
            var bx = BitConverter.DoubleToInt64Bits(x);
            var by = BitConverter.DoubleToInt64Bits(y);

            // If the signs are different, return false except for +0 and -0. 
            if (((bx >> 63) != (by >> 63))) return (x == y);
            else return Math.Abs(bx - by) <= this.Units;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The Object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(double obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// IEqualityComparer&lt;double&gt; implementation to compare two doubles by casting them to floats.
    /// </summary>
    public class FixedDoubleComparer : IEqualityComparer<double>
    {
        /// <summary>
        /// FixedDoubleComparer constructor.
        /// </summary>
        public FixedDoubleComparer()
        {
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type double to compare.</param>
        /// <param name="y">The second object of type double to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(double x, double y)
        {
            var sx = String.Format("{0:.000000}", x);
            var sy = String.Format("{0:.000000}", y);

            return sx.Equals(sy);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The Object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(double obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// IEqualityComparer&lt;IEnumerable&lt;T&gt;&gt; implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        /// <summary>
        /// Method for comparing each T object.
        /// </summary>
        IEqualityComparer<T> CS { get; set; }

        /// <summary>
        /// ArrayEqualityComparer constructor.
        /// </summary>
        /// <param name="cs">Method for comparing each T object.</param>
        public ArrayEqualityComparer(IEqualityComparer<T> cs)
        {
            this.CS = cs; 
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type double to compare.</param>
        /// <param name="y">The second object of type double to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            //Check whether the products' properties are equal. 
            return x.SequenceEqual(y, this.CS);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The Object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(IEnumerable<T> obj)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(obj, null)) return 0;

            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// IEqualityComparer&lt;IEnumerable&lt;KeyValuePair&lt;string, T&gt;&gt;&gt; implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StringMapEqualityComparer<T> : IEqualityComparer<IEnumerable<KeyValuePair<string, T>>>
    {
        /// <summary>
        /// Method for comparing each sequence element.
        /// </summary>
        IEqualityComparer<KeyValuePair<string, T>> CS { get; set; }

        /// <summary>
        /// StringMapEqualityComparer constructor.
        /// </summary>
        /// <param name="cs">Method for comparing each sequence element.</param>
        public StringMapEqualityComparer(IEqualityComparer<KeyValuePair<string, T>> cs)
        {
            this.CS = cs;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type double to compare.</param>
        /// <param name="y">The second object of type double to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(IEnumerable<KeyValuePair<string, T>> x, IEnumerable<KeyValuePair<string, T>> y)
        {

            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            //Check whether the products' properties are equal. 
            return x.SequenceEqual(y, CS);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The Object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(IEnumerable<KeyValuePair<string, T>> obj)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(obj, null)) return 0;

            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// IEqualityComparer&lt;KeyValuePair&lt;string, T&gt;&gt; implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyValuePairEqualityComparer<T> : IEqualityComparer<KeyValuePair<string, T>>
    {
        /// <summary>
        /// Method for comparing T objects.
        /// </summary>
        IEqualityComparer<T> CS { get; set; }

        /// <summary>
        /// KeyValuePairEqualityComparer constructor.
        /// </summary>
        /// <param name="cs">Method for comparing T objects.</param>
        public KeyValuePairEqualityComparer(IEqualityComparer<T> cs)
        {
            this.CS = cs;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type double to compare.</param>
        /// <param name="y">The second object of type double to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(KeyValuePair<string, T> x, KeyValuePair<string, T> y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            //Check whether the products' properties are equal. 
            var ks = x.Key.Equals(y.Key);
            var vs = CS.Equals(x.Value, y.Value);

            return ks && vs;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The Object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(KeyValuePair<string, T> obj)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(obj, null)) return 0;

            //Get hash code for the Key field.
            var hashKey = obj.Key.GetHashCode();

            //Get hash code for the Value field. 
            var hashValue = obj.Value.GetHashCode();

            //Calculate the hash code for the product. 
            return hashKey ^ hashValue;
        }
    }
}
