using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.RFID
{
    public static class Helpers
    {
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
