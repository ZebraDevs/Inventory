using System;
namespace Inventory
{
    /// <summary>
    /// Custom event args for use by the scanner
    /// </summary>
    public class StatusEventArgs : EventArgs
    {
        private string barcodeData;

        public StatusEventArgs(string dataIn, string barcodeTypeIn)
        {
            barcodeData = dataIn;
            barcodeType = barcodeTypeIn;
        }

        public string Data { get { return barcodeData; } }

        private string barcodeType;
        public string BarcodeType { get { return barcodeType; } }

    }
}
