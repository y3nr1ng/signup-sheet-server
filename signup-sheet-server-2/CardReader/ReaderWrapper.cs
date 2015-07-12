using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace signup_sheet_server.CardReader
{
    class ReaderWrapper
    {
        #region DLL imports.

        class NativeMethods
        {
            [DllImport("dcrf32.dll")]
            public static extern short dc_init(Int16 port, uint baud);
            [DllImport("dcrf32.dll")]
            public static extern short dc_exit(int icdev);
            [DllImport("dcrf32.dll")]
            public static extern short dc_card(int icdev, char _Mode, ref ulong Snr);
            [DllImport("dcrf32.dll")]
            public static extern short dc_beep(int icdev, uint _Msec);
        }

        #endregion

        #region Card reader private variables.

        // Hardcoded to USB port.
        private const Int16 readerPort = 100;
        private int cardReaderId = -1;

        #endregion

        public bool Open()
        {
            // Initialize the card reader.
            this.cardReaderId = NativeMethods.dc_init(readerPort, 115200);

            // Check the status of the card reader.
            return (cardReaderId >= 0);
        }

        public bool Close()
        {
            short status = NativeMethods.dc_exit(this.cardReaderId);

            // Reset the handler.
            this.cardReaderId = -1;

            // Check the status of the card reader.
            return (status == 0);
        }

        public bool Scan(out string cardIdStr)
        {
            // Default value.
            cardIdStr = string.Empty;

            ulong cardId = 0;
            // Operate in MODE1, ALL mode.
            short status = NativeMethods.dc_card(this.cardReaderId, (char)0, ref cardId);
            if(status == 0)
            {
                cardIdStr = cardId.ToString();

                // Beep to notify the user that the reader has acknowledge the card.
                NativeMethods.dc_beep(this.cardReaderId, 10);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
