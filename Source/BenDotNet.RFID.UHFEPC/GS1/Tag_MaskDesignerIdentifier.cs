using System;
using System.Collections.Generic;
using System.Text;

namespace BenDotNet.RFID.UHFEPC.GS1
{
    public partial class Tag : UHFEPC.Tag
    {
        public enum MaskDesignerIdentifier
        {
            Impinj = 0b000000001,
            Texas_Instruments = 0b000000010,
            Alien_Technology = 0b000000011,
            Intelleflex = 0b000000100,
            Atmel = 0b000000101,
            NXP_Semiconductors = 0b000000110,
            ST_Microelectronics = 0b000000111,
            EP_Microelectronics = 0b000001000,
            Motorola = 0b000001001,
            Sentech_Snd_Bhd = 0b000001010,
            EM_Microelectronics = 0b000001011,
            Renesas_Technology = 0b000001100,
            Mstar = 0b000001101,
            Tyco_International = 0b000001110,
            Quanray_Electronics = 0b000001111,
            Fujitsu = 0b000010000,
            LSIS = 0b000010001,
            CAEN_RFID = 0b000010010,
            Productivity_Engineering = 0b000010011,
            Federal_Electric = 0b000010100,
            ON_Semiconductor = 0b000010101,
            Ramtron = 0b000010110,
            Tego = 0b000010111,
            Ceitec_S_A = 0b000011000,
            CPA_Wernher_von_Braun = 0b000011001,
            TransCore = 0b000011010,
            Nationz = 0b000011011,
            Invengo = 0b000011100,
            Kiloway = 000011101,
            Longjing_Microelectronics = 0b000011110,
            Chipus_Microelectronics = 0b000011111,
            ORIDAO = 0b000100000,
            Maintag = 0b000100001,
            Yangzhou_Daoyuan_Microelectronics = 0b000100010,
            Gate_Elektronik = 0b000100011,
            RFMicron = 0b000100100,
            RST_Invent_LLC = 0b000100101,
            Crystone_Technology = 0b000100110,
            Shanghai_Fudan_Microelectronics_Group = 0b000100111,
            Farsens = 0b000101000,
            Giesecke_and_Devrient = 0b000101001,
            AWID = 0b000101010,
            Unitec_Semicondutores = 0b000101011,
            Q_Free_ASA = 0b000101100,
            Valid = 0b000101101,
            Fraunhofer_IPMS = 0b000101110,
            AMS = 0b000101111,
            Angstrem = 0b000110000,
            Honeywell = 0b000110001,
            Huada_Semiconductor = 0b000110010,
            Lapis_Semiconductor = 0b000110011,
            PJSC_Mikron = 0b000110100,
            Hangzhou_Landa_Microelectronics = 0b000110101,
            Nanjing_NARI_Micro_Electronic_Technology = 0b000110110,
            Southwest_Integrated_Circuit_Design = 0b000110111,
            Silictec = 0b000111000,
            Nation_RFID = 0b00111001
        }
    }
}