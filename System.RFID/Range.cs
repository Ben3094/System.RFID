using System;
using System.Collections.Generic;
using System.Text;

namespace System.RFID
{
    public abstract class Range<T> where T : IComparable
    {
        public Range(T minValue, T maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }
        public T MinValue;
        public T MaxValue;

        public virtual bool IsInRange(T value)
        {
            return (MinValue.CompareTo(value) < 0) & (value.CompareTo(MaxValue) < 0);
        }
        public abstract T FindClosestValue(T value);
        public bool IsCorrect(T value)
        {
            return this.IsInRange(value) & (this.FindClosestValue(value).Equals(value));
        }
    }

    public class ContinuousRange<T> : Range<T> where T : IComparable
    {
        public ContinuousRange(T minValue, T maxValue) : base(minValue, maxValue) { } 

        public override T FindClosestValue(T value)
        {
            if (value.CompareTo(MinValue) < 0)
                return MinValue;
            else if (value.CompareTo(MaxValue) > 0)
                return MaxValue;
            else
                return value;
        }
    }

    public class LinearDiscreteRange : Range<float>
    {
        public LinearDiscreteRange(float minValue, float maxValue, float valueStep) : base(minValue, maxValue)
        {
            this.ValueStep = valueStep;
        }
        public readonly float ValueStep;

        public override float FindClosestValue(float value)
        {
            if (value.CompareTo(MinValue) < 0)
                return MinValue;
            else if (value.CompareTo(MaxValue) > 0)
                return MaxValue;
            else
                return Math.Abs((value - this.MinValue) / this.ValueStep) * this.ValueStep;
        }
    }

    //public class CompositeRange<T> : Range<T> where T : IComparable
    //{
    //    public CompositeRange(IEnumerable<Range<T>> ranges) : base()
    //    {

    //    }

    //    public override T FindClosestValue(T value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override bool IsInRange(T value)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
