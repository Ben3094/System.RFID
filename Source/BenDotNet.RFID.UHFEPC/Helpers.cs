using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.HashFunction.CRC;

namespace BenDotNet.RFID.UHFEPC
{
    public static class Helpers
    {
        #region Extensible Bit Vector
        const byte EBV_DATA_BITS_MASK = 0b01111111;
        const byte EBV_EXTENSION_BIT_MASK = ~EBV_DATA_BITS_MASK & byte.MaxValue;

        public static byte[] CompileExtensibleBitVector(int value)
        {
            const byte BYTES_IN_INT = 32 / 8;
            const int MAX_EBV_INT_VALUE = int.MaxValue >> BYTES_IN_INT; //As extension bits presence reduce value bits number in each byte to 7, 4 value bits are missing for an int value.
            if (value > MAX_EBV_INT_VALUE)
                throw new ArgumentException("");

            List<byte> results = new List<byte>();
            for (byte i = 0; i < BYTES_IN_INT; i++)
            {
                byte result = (byte)((i == 0) ? 0 : 0b10000000);
                result |= (byte)((value >> (i * 7)) & EBV_DATA_BITS_MASK);
                results.Add(result);
            }
            return results.ToArray();
        }

        public static int ParseExtensibleBitVector(byte[] value)
        {
            int result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (i != 0)
                    if (value[i] < EBV_EXTENSION_BIT_MASK)
                        throw new ArgumentException("Misformed Extensible Bit Vector");

                result += (value[i] & EBV_DATA_BITS_MASK) << (i * 7);
            }
            return result;
        }

        public static ICRCConfig DEFAULT_CRC16_METHOD = CRCConfig.CRC16_CCITTFALSE;
        public static ushort GetCRC16(byte[] value)
        {
            byte[] crcPieces = CRCFactory.Instance.Create(DEFAULT_CRC16_METHOD).ComputeHash(value).Hash;
            return (ushort)(crcPieces[1] | (crcPieces[0] >> 8));
        }
        #endregion

        #region Word conversion
        public static IEnumerable<byte> GetBytesFromWords(IEnumerable<char> words)
        {
            foreach (char word in words)
            {
                byte[] bytes = BitConverter.GetBytes(word);
                yield return bytes[0];
                yield return bytes[1];
            }
        }

        public static IEnumerable<char> GetWordsFromBytes(IEnumerable<byte> bytes, byte missingByteCompletion = 0)
        {
            if (IsMissingByteForConvertionInWords(bytes))
            {
                bytes = new List<byte>(bytes);
                ((List<byte>)bytes).Add(missingByteCompletion);
            }
            IEnumerator<byte> enumerator = bytes.GetEnumerator();
            char temp = (char)(enumerator.Current << 8);
            enumerator.MoveNext();
            yield return (char)(temp | (char)enumerator.Current);
            if (!enumerator.MoveNext())
                yield break;
        }

        public static bool IsMissingByteForConvertionInWords(IEnumerable<byte> bytes)
        {
            return (bytes.Count() & 1) != 1;
        }
        #endregion
    }
}
