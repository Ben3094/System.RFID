using System;
using System.Collections.Generic;
using System.Text;

namespace System.RFID
{
    public class DetectionSource
    {
        public DetectionSource(ref AntennaPort antenna, ref object handle, double frequency, double rssi, double phase, DateTime time)
        {
            this.Antenna = antenna;
            this.NotifyDetection(ref handle, frequency, rssi, phase, time);
        }
        public void NotifyDetection(ref object handle, double frequency, double rssi, double phase, DateTime time)
        {
            this.Handle = handle;
            this.NotifyDetection(frequency, rssi, phase, time);
        }
        public void NotifyDetection(double frequency, double rssi, double phase, DateTime time)
        {
            this.Frequency = frequency;
            this.RSSI = rssi;
            this.Phase = phase;
            this.Time = time;
        }

        public AntennaPort Antenna { get; private set; }
        public object Handle { get; private set; }
        public double Frequency { get; private set; }
        public double RSSI { get; private set; }
        public double Phase { get; private set; }
        public DateTime Time { get; private set; }
    }
}
