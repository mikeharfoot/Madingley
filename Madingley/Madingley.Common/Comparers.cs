using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    //
    // Compare two doubles by looking at their bitwise representation
    //
    public class TolerantDoubleComparer : IEqualityComparer<double>
    {
        Int64 Units { get; set; }

        public TolerantDoubleComparer(Int64 units)
        {
            this.Units = units;
        }

        public bool Equals(double x, double y)
        {
            var fx = (float)x;
            var fy = (float)y;

            //Math.Abs(((float32)value1 - ((float32)value2 < 
            var bx = BitConverter.DoubleToInt64Bits((double)fx);
            var by = BitConverter.DoubleToInt64Bits((double)fy);

            // If the signs are different, return false except for +0 and -0. 
            if (((bx >> 63) != (by >> 63))) return (x == y);
            else return Math.Abs(bx - by) <= this.Units;
        }

        public int GetHashCode(double x)
        {
            return x.GetHashCode();
        }
    }

    public class FixedDoubleComparer : IEqualityComparer<double>
    {
        public FixedDoubleComparer()
        {
        }

        public bool Equals(double x, double y)
        {
            var sx = String.Format("{0:.000000}", x);
            var sy = String.Format("{0:.000000}", y);

            return sx.Equals(sy);
        }

        public int GetHashCode(double x)
        {
            return x.GetHashCode();
        }
    }

    //
    // Compare two arrays
    //
    public class ArrayEqualityComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        IEqualityComparer<T> CS { get; set; }

        public ArrayEqualityComparer(IEqualityComparer<T> cs)
        {
            this.CS = cs; 
        }

        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            //Check whether the products' properties are equal. 
            return x.SequenceEqual(y, this.CS);
        }

        public int GetHashCode(IEnumerable<T> x)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(x, null)) return 0;

            return x.GetHashCode();
        }
    }

    //
    // Compare two string maps
    //
    public class StringMapEqualityComparer<T> : IEqualityComparer<IEnumerable<KeyValuePair<string, T>>>
    {
        IEqualityComparer<KeyValuePair<string, T>> cs { get; set; }

        public StringMapEqualityComparer(IEqualityComparer<KeyValuePair<string, T>> cs)
        {
            this.cs = cs;
        }

        public bool Equals(IEnumerable<KeyValuePair<string, T>> x, IEnumerable<KeyValuePair<string, T>> y)
        {

            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            //Check whether the products' properties are equal. 
            return x.SequenceEqual(y, cs);
        }

        public int GetHashCode(IEnumerable<KeyValuePair<string, T>> x)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(x, null)) return 0;

            return x.GetHashCode();
        }
    }

    //
    // Compare two KeyValuePair s
    //
    public class KeyValuePairEqualityComparer<T> : IEqualityComparer<KeyValuePair<string, T>>
    {
        IEqualityComparer<T> cs { get; set; }

        public KeyValuePairEqualityComparer(IEqualityComparer<T> cs)
        {
            this.cs = cs;
        }

        public bool Equals(KeyValuePair<string, T> x, KeyValuePair<string, T> y)
        {
            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            //Check whether the products' properties are equal. 
            var ks = x.Key.Equals(y.Key);
            var vs = cs.Equals(x.Value, y.Value);

            return ks && vs;
        }

        public int GetHashCode(KeyValuePair<string, T> x)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(x, null)) return 0;

            //Get hash code for the Key field.
            var hashKey = x.Key.GetHashCode();

            //Get hash code for the Value field. 
            var hashValue = x.Value.GetHashCode();

            //Calculate the hash code for the product. 
            return hashKey ^ hashValue;
        }
    }
}
