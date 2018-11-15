using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace System.RFID.UHFEPC
{
    public abstract class Reader : RFID.Reader
    {
        //public const float MIN_ALLOWED_FREQUENCY = 865 * 10 ^ 6;
        //public const float MAX_ALLOWED_FREQUENCY = 928 * 10 ^ 6;
        //public override float[] AllowedFrequencies => Enumerable.Range((int)MIN_ALLOWED_FREQUENCY, (int)MAX_ALLOWED_FREQUENCY).Cast<float>().ToArray();

        //public static readonly 
    }

    /// <summary>
    /// 4 most-significant bits of the 8-bits "Cryptography Suite Indicator" (CSI) used by Challenge and Authenticate commands
    /// </summary>
    public enum CryptographicSuiteAssigningAuthority
    {
        ISO29167_1 = 0b00000000,
        ISO29167_2 = 0b00010000,
        ISO29167_3 = 0b00100000,
        ISO29167_4 = 0b00110000,
        TagManufacturer = 0b11010000,
        GS1 = 0b11100000
    }
}
