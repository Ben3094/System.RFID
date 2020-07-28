using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BenDotNet.RFID
{
    //TODO: Plan conversion between tag type and handle transfer between reader (a UHF tag can have a HF antenna too and the user can use multiple reader)
    public abstract class Tag : INotifyPropertyChanged
    {
        public const string NOT_GENUINE_TAG_ARGUMENT_EXCEPTION_MESSAGE_FORMAT = "Not a genuine {0} tag";

        public Tag(byte[] uid)
        {
            this.UID = uid;
        }

        public byte[] UID { get; private set; }

        public Reply Execute(Command command)
        {
            return this.DetectionSources.First().Antenna.ContainerReader.Execute(this, command);
        }

        #region Connection
        public readonly ObservableCollection<DetectionSource> DetectionSources = new ObservableCollection<DetectionSource>();
        public IOrderedEnumerable<DetectionSource> DetectionSourcesByRSSI => this.DetectionSources.OrderBy(detectionSource => detectionSource.RSSI);
        public IOrderedEnumerable<DetectionSource> LastDetectedSources => this.DetectionSources.OrderBy(detectionSource => detectionSource.Time);
        #endregion

        #region Memory
        public abstract Stream Memory { get; }
        #endregion

        #region Properties changed event handler
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    //public class MultiTypeTag : Tag
    //{
    //    internal MultiTypeTag(byte[] uid) : base(uid) { }

    //    public ObservableCollection<Tag> Value = new ObservableCollection<Tag>();
    //    //TODO: Integrate a "mini" GlobalTagCache to manage tags

    //    public override Stream Memory => throw new NotImplementedException();
    //}
}
