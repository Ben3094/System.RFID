using System;
using System.Collections.Generic;
using System.Text;

namespace BenDotNet.RFID
{
    public class DetectionSource
    {
        public DetectionSource(ref AntennaPort antenna, ref object handle, float frequency, float rssi, float phase, DateTime time)
        {
            this.Antenna = antenna;
            this.NotifyDetection(ref handle, frequency, rssi, phase, time);
        }
        public void NotifyDetection(ref object handle, float frequency, float rssi, float phase, DateTime time)
        {
            this.Handle = handle;
            this.NotifyDetection(frequency, rssi, phase, time);
        }
        public void NotifyDetection(float frequency, float rssi, float phase, DateTime time)
        {
            this.Frequency = frequency;
            this.RSSI = rssi;
            this.Phase = phase;
            this.Time = time;
        }

        public AntennaPort Antenna { get; private set; }
        public object Handle { get; private set; }
        public float Frequency { get; private set; }
        public float RSSI { get; private set; }
        public float Phase { get; private set; }
        public DateTime Time { get; private set; }
    }
}
