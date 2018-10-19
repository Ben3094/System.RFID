namespace System.RFID
{
    public static partial class Measurements
    {
        private enum PowerSweepWay { Up, Down }
        public class TagNotFoundException : Exception { }
        public delegate bool IsTagDetectedDelegate(ref Tag targetTag, float readerPower);
        public static float TurnOnPowerSweep(ref Tag targetTag, float startPower, float minPower, float maxPower, float powerStep, IsTagDetectedDelegate isTagDetectedProcedure)
        {
            float currentPower = startPower;

            if (!IsCurrentPowerCorrect())
                throw new ArgumentException("Power limits not correct");

            PowerSweepWay currentPowerSweepWay = isTagDetectedProcedure(ref targetTag, currentPower) ? PowerSweepWay.Down : PowerSweepWay.Up;

            do
            {
                switch (currentPowerSweepWay)
                {
                    case PowerSweepWay.Down:
                        if (!isTagDetectedProcedure(ref targetTag, currentPower))
                        {
                            if (currentPower + powerStep <= maxPower)
                                currentPower += powerStep;
                            return currentPower;
                        }
                        else
                            currentPower -= powerStep;
                        break;

                    case PowerSweepWay.Up:
                        if (isTagDetectedProcedure(ref targetTag, currentPower))
                            return currentPower;
                        else
                            currentPower += powerStep;
                        break;
                }
            } while (IsCurrentPowerCorrect());

            if (currentPower < minPower)
                return minPower;
            else
                throw new TagNotFoundException();

            bool IsCurrentPowerCorrect()
            {
                return (minPower <= currentPower) && (currentPower <= maxPower);
            }
        }
    }

    //public abstract class TagTurnOnPowerSweep
    //{
    //    public Tag TargetTag { get; private set; }
    //    public float StartPower { get; set; }
    //    public float MinPower { get; private set; }
    //    public float MaxPower { get; private set; }
    //    public float PowerStep { get; private set; }

    //    public TagTurnOnPowerSweep(ref Tag targetTag, float startPower, float minPower, float maxPower, float powerStep)
    //    {
    //        this.TargetTag = targetTag; this.StartPower = startPower; this.MinPower = minPower; this.MaxPower = maxPower; this.PowerStep = powerStep;
    //        this.CurrentPower = this.StartPower;
    //    }

    //    public float CurrentPower { get; private set; }

    //    public enum PowerSweepWay { Up, Down }
    //    public PowerSweepWay CurrentPowerSweepWay { get; private set; }

    //    private bool IsCurrentPowerCorrect
    //    {
    //        get { return (this.MinPower <= this.CurrentPower) && (this.CurrentPower <= this.MaxPower); }
    //    }

    //    public void Start()
    //    {
    //        if (!this.IsCurrentPowerCorrect)
    //            throw new ArgumentException("Power limits not correct");

    //        this.CurrentPowerSweepWay = this.IsTagDetected() ? PowerSweepWay.Down : PowerSweepWay.Up;

    //        do
    //        {
    //            switch (this.CurrentPowerSweepWay)
    //            {
    //                case PowerSweepWay.Down:
    //                    if (!this.IsTagDetected())
    //                    {
    //                        if (this.CurrentPower + this.PowerStep <= this.MaxPower)
    //                            this.CurrentPower += this.PowerStep;
    //                        return;
    //                    }
    //                    else
    //                        this.CurrentPower -= this.PowerStep;
    //                    break;

    //                case PowerSweepWay.Up:
    //                    if (this.IsTagDetected())
    //                        return;
    //                    else
    //                        this.CurrentPower += this.PowerStep;
    //                    break;
    //            }
    //        } while (this.IsCurrentPowerCorrect);

    //        if (this.CurrentPower < this.MinPower)
    //        {
    //            this.CurrentPower = this.MinPower;
    //            return;
    //        }
    //        else
    //        {
    //            this.CurrentPower = this.MaxPower;
    //            throw new TagNotFoundException();
    //        }
    //    }

    //    protected abstract bool IsTagDetected();

    //    public class TagNotFoundException : Exception { }
    //}
}
