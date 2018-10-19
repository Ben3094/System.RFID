using System;
using System.Collections.Generic;
using System.Text;

namespace System.RFID
{
    public static partial class Measurements
    {
        public delegate bool ChangeFrequencyDelegate(float readerFrequency);
        public static void FrequencySweep(ref Reader targetReader, float minFrequency, float maxFrequency, float frequencyStep, ChangeFrequencyDelegate changeFrequencyProcedure, Action<float> action, Action<float> invalidFrequencyAction)
        {
            for (float currentFrequency = minFrequency; currentFrequency <= maxFrequency; currentFrequency += frequencyStep)
            {
                if (changeFrequencyProcedure(currentFrequency))
                    action.Invoke(currentFrequency);
                else
                    invalidFrequencyAction.Invoke(currentFrequency);
            }
        }
    }

    //public abstract class FrequencySweep
    //{
    //    //TODO: Create a dynamic hop table that can be linear, follow a mathematic function or a static table
    //    public float MinFrequency { get; private set; }
    //    public float MaxFrequency { get; private set; }
    //    public float FrequencyStep { get; private set; }

    //    public float CurrentFrequency { get; private set; }

    //    public Reader TargetReader { get; private set; }

    //    public FrequencySweep(Reader targetReader, float minFrequency, float maxFrequency, float frequencyStep, FrequencyPointAction frequencyPointAction, InvalidFrequencyAction invalidFrequencyAction)
    //    {
    //        this.TargetReader = targetReader;
    //        this.MinFrequency = minFrequency;
    //        this.MaxFrequency = maxFrequency;
    //        this.FrequencyStep = frequencyStep;
    //        this.CurrentFrequencyAction = frequencyPointAction;
    //        this.CurrentInvalidFrequencyAction = invalidFrequencyAction;
    //    }

    //    public void Start()
    //    {
    //        for (float freq = this.MinFrequency; freq <= this.MaxFrequency; freq += this.FrequencyStep)
    //        {
    //            this.CurrentFrequency = freq;
    //            try
    //            {
    //                this.SetFrequency();
    //                this.CurrentFrequencyAction(freq);
    //            }
    //            catch (InvalidFrequencyException)
    //            {
    //                this.CurrentInvalidFrequencyAction(this.CurrentFrequency);
    //            }
    //        }
    //    }

    //    public delegate void FrequencyPointAction(float currentFrequency);
    //    public FrequencyPointAction CurrentFrequencyAction { get; private set; }


    //    protected abstract void SetFrequency();

    //    public class InvalidFrequencyException : Exception
    //    {
    //        public double Frequency { get; private set; }

    //        public InvalidFrequencyException(double frequency) { this.Frequency = frequency; }
    //    }
    //    public delegate void InvalidFrequencyAction(float freq);
    //    public InvalidFrequencyAction CurrentInvalidFrequencyAction { get; private set; }
    //}
}
