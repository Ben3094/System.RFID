using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BenDotNet.RFID
{
    public class RegulationRegionAttribute : Attribute
    {
        public RegulationRegionAttribute(CultureInfo region)
        {
            this.Region = region;
        }

        public readonly CultureInfo Region;
    }
}
