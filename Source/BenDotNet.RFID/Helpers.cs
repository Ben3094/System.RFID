using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BenDotNet.RFID
{
    public static class Helpers
    {
        internal static int ReccursiveSearchForBaseType(Type targetType, Type baseType)
        {
            int reccursion = 0;
            Type previousType = targetType;
            while (previousType != baseType)
            {
                previousType = previousType.BaseType;
                reccursion++;
            }
            return reccursion;
        }

        public const byte ONE_BIT_MASK = 0b1;

        public const byte CHAR_MAX_INDEX = 15;
        public const string CHAR_INDEX_ABOVE_LIMIT_EXCEPTION_MESSAGE = "Char index above limit";
        public static bool IsTrue(this char word, byte index)
        {
            if (index > CHAR_MAX_INDEX)
                throw new ArgumentException(CHAR_INDEX_ABOVE_LIMIT_EXCEPTION_MESSAGE);
            else
                return ((word >> index) & ONE_BIT_MASK) > 0;
        }
        public const byte BYTE_MAX_INDEX = 7;
        public const string BYTE_INDEX_ABOVE_LIMIT_EXCEPTION_MESSAGE = "Byte index above limit";
        public static bool IsTrue(this byte word, byte index)
        {
            if (index > BYTE_MAX_INDEX)
                throw new ArgumentException(BYTE_INDEX_ABOVE_LIMIT_EXCEPTION_MESSAGE);
            else
                return ((word >> index) & ONE_BIT_MASK) > 0;
        }
    }

    public class RestrictedObservableCollection<T> : ObservableCollection<T>
    {
        public readonly T[] RestrictionSource;
        public RestrictedObservableCollection(T[] restrictionSource)
        {
            this.RestrictionSource = restrictionSource;
        }
        protected override void InsertItem(int index, T item)
        {
            if (this.RestrictionSource.Contains(item))
                base.InsertItem(index, item);
            else
                throw new ArgumentException("This value is not allowed by restrictions set");
        }
        protected override void SetItem(int index, T item)
        {
            if (this.RestrictionSource.Contains(item))
                base.SetItem(index, item);
            else
                throw new ArgumentException("This value is not allowed by restrictions set");
        }
    }
}
