using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace BenDotNet.RFID
{
    public enum AntennaPortStatus { Unknown, Floating, Grounded, Connected }
    public abstract class AntennaPort : INotifyPropertyChanged
    {
        public AntennaPort(Reader container) { this.ContainerReader = container; }

        public readonly Reader ContainerReader; //TODO: Find a prettier name

        private AntennaPortStatus status;
        public AntennaPortStatus Status
        {
            get
            {
                switch (this.ConnectedLoad.Real)
                {
                    case float.NaN:
                        return AntennaPortStatus.Unknown;
                    case float.PositiveInfinity:
                        return AntennaPortStatus.Floating;
                    case 0:
                        return AntennaPortStatus.Grounded;
                    default:
                        return AntennaPortStatus.Connected;
                }
            }
        }

        private Complex connectedLoad;
        public virtual Complex ConnectedLoad
        {
            get { return this.connectedLoad; }
            protected set
            {
                this.connectedLoad = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(this.Status));
            }
        }

        public readonly ObservableCollection<Tag> ConnectedTags = new ObservableCollection<Tag>(); //Check if the collection can still be modified

        public readonly object OriginalAntennaPort;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public const float RF_LINE_DEFAULT_RESISTANCE = 50; //Ohm
        //TODO: add frequency dependent value handling
        public static Complex LoadFromReflectionCoefficient(float reflectionCoeffient, float lineResistance)
        {
            float loadResistance = Math.Abs(lineResistance * ((1 + reflectionCoeffient) / (1 - reflectionCoeffient)));
            return new Complex(loadResistance, 0);
        }
    }
}
