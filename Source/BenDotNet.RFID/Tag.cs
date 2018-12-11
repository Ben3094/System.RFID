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
        public Tag(byte[] uid)
        {
            this.UID = uid;
        }

        public byte[] UID { get; private set; }

        #region CONNECTION
        public readonly ObservableCollection<DetectionSource> DetectionSources = new ObservableCollection<DetectionSource>();
        //TODO: Always sort detecting antennas by best signal quality

        public Reply Execute(Command command)
        {
            return this.DetectionSources.First().Antenna.ContainerReader.Execute(this, command);
        }
        #endregion

        #region MEMORY
        public abstract Stream Memory { get; }
        #endregion

        #region PROPERTIES CHANGED EVENT HANDLER
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

    //    public override Stream Memory => throw new NotImplementedException();
    //}
}
