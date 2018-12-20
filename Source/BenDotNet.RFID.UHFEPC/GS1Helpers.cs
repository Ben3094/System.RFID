using System;
using System.Collections.Generic;
using System.Text;

namespace BenDotNet.RFID.UHFEPC
{
    public static class GS1Helpers
    {
        public static void FallbackBlockWrite(Tag tag, GS1Tag.MemoryBank memoryBank, ref char[] data, int offset = 0)
        {
            for (int index = 0; index < data.Length; index++)
            {
                char dataPiece = data[index];
                tag.Execute(new WriteCommand(memoryBank, ref dataPiece, offset + index));
            }
        }
    }
}
