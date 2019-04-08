using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using BenDotNet.Numerics;

namespace BenDotNet.RFID
{
    public enum ReaderStatus { Unknown, Error, Disconnected, Connected } 

    public abstract class Reader : INotifyPropertyChanged
    {
        public Reader()
        {
            foreach (AntennaPort antennaPort in this.AntennaPorts)
                antennaPort.ConnectedTags.CollectionChanged += ConnectedTags_CollectionChanged;
        }

        public ReaderStatus status;
        public ReaderStatus Status
        {
            get { return this.status; }
            protected set
            {
                this.status = value;
                OnPropertyChanged();
            }
        }

        private void ConnectedTags_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach (Tag addedTag in e.NewItems)
                this.ConnectedTags.Add(addedTag);
            foreach (Tag removedTag in e.OldItems)
                this.ConnectedTags.Remove(removedTag);
        }
        public readonly ObservableCollection<Tag> ConnectedTags = new ObservableCollection<Tag>();

        public object OriginalReader;

        public void Connect()
        {
            this.ConnectionMethod();
            this.Status = ReaderStatus.Connected;
        }
        protected abstract void ConnectionMethod();

        public void Disconnect()
        {
            this.DisconnectionMethod();
            this.Status = ReaderStatus.Disconnected;
        }
        protected abstract void DisconnectionMethod();

        public abstract Reply Execute(Tag targetTag, Command command);

        public abstract IEnumerable<AntennaPort> AntennaPorts { get; }

        public const ushort DEFAULT_DELAY_BETWEEN_INVENTORY_ms = 500;
        public static TimeSpan DEFAULT_DELAY_BETWEEN_INVENTORY = TimeSpan.FromMilliseconds(DEFAULT_DELAY_BETWEEN_INVENTORY_ms);
        protected Timer AutoInventoryTimer = new Timer() { AutoReset = true };
        internal TimeSpan autoInventorydelay = DEFAULT_INVENTORY_DELAY;
        private void AutoInventoryTimer_Tick(object sender, ElapsedEventArgs e) { this.Inventory(this.autoInventorydelay); }
        public virtual void StartContinousInventory(TimeSpan interval, TimeSpan delay)
        {
            this.autoInventorydelay = delay;
            this.AutoInventoryTimer.Elapsed += AutoInventoryTimer_Tick;
            this.AutoInventoryTimer.Interval = interval.TotalMilliseconds;
            this.AutoInventoryTimer.Start();
        }
        public void StartContinousInventory(TimeSpan delay) { this.StartContinousInventory(DEFAULT_DELAY_BETWEEN_INVENTORY, delay); }
        public void StartContinousInventory() { this.StartContinousInventory(DEFAULT_DELAY_BETWEEN_INVENTORY, DEFAULT_INVENTORY_DELAY); }
        public virtual void StopContinuousInventory() 
        {
            this.AutoInventoryTimer.Elapsed -= AutoInventoryTimer_Tick;
            this.AutoInventoryTimer.Stop();
        }
        public const ushort DEFAULT_INVENTORY_DELAY_ms = 500;
        public static TimeSpan DEFAULT_INVENTORY_DELAY = new TimeSpan(0, 0, 0, 0, DEFAULT_INVENTORY_DELAY_ms);
        public abstract IEnumerable<Tag> Inventory(TimeSpan delay);
        public IEnumerable<Tag> Inventory() { return this.Inventory(DEFAULT_INVENTORY_DELAY); }
        public virtual Tag Detect(ref Tag tag, TimeSpan delay)
        {
            Tag targetTag = tag;
            try { return this.Inventory(delay).First(detectedTag => detectedTag == targetTag); }
            catch (InvalidOperationException) { return null; }
        }
        public Tag Detect(ref Tag tag)
        {
            return this.Detect(ref tag, DEFAULT_INVENTORY_DELAY);
        }
        
        /// <summary>
        /// Frequency used by the reader to operate
        /// </summary>
        /// <remarks>
        /// 0 for automatic
        /// Always used the closest value in the allowed frequencies range
        /// </remarks>
        public abstract float Frequency { get; set; }
        public abstract Range<float> AllowedFrequencies { get; }
        
        /// <summary>
        /// Power used by the reader to operate
        /// </summary>
        /// <remarks>
        /// 0 for automatic
        /// Always used the closest value in the allowed power range
        /// </remarks>
        public abstract float Power { get; set; }
        public abstract Range<float> AllowedPowers { get; }

        #region Properties changes event handler
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
