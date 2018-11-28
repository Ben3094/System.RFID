using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Numerics.Range;

namespace System.RFID
{
    public enum ReaderStatus { Unknown, Error, Disconnected, Connected } 

    public abstract class Reader : INotifyPropertyChanged
    {
        public Reader()
        {
            foreach (AntennaPort antennaPort in this.AntennaPorts)
                antennaPort.ConnectedTags.CollectionChanged += ConnectedTags_CollectionChanged;
            //this.OperationFrequencies = new List<float>(this.AllowedFrequencies);
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

        private void ConnectedTags_CollectionChanged(object sender, Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

        public static TimeSpan DEFAULT_DELAY_BETWEEN_INVENTORY = TimeSpan.FromMilliseconds(500);
        protected Timers.Timer AutoInventoryTimer = new Timers.Timer() { AutoReset = true };
        private void AutoInventoryTimer_Tick(object sender, Timers.ElapsedEventArgs e) { this.Inventory(); }
        public virtual void StartContinousInventory()
        {
            this.AutoInventoryTimer.Elapsed += AutoInventoryTimer_Tick;
            this.AutoInventoryTimer.Interval = DEFAULT_DELAY_BETWEEN_INVENTORY.TotalMilliseconds;
            this.AutoInventoryTimer.Start();
        }
        public virtual void StopContinuousInventory() 
        {
            this.AutoInventoryTimer.Elapsed -= AutoInventoryTimer_Tick;
            this.AutoInventoryTimer.Stop();
        }
        public abstract IEnumerable<Tag> Inventory();
        
        public abstract List<Range<float>> AllowedFrequencies { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
