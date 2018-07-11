using System;

namespace Inventory
{
    public class ZebraScannerConfig : IScannerConfig
    {
        public TriggerType TriggerType { get; set; }

        public bool IsEAN8 { get; set; }
        public bool IsEAN13 { get; set; }
        public bool IsCode39 { get; set; }
        public bool IsCode128 { get; set; }
        public bool IsContinuous { get; set; }
        public bool IsUPCA { get; set; }
        public bool IsUPCE0 { get; set; }
        public bool IsUPCE1 { get; set; }
        public bool IsD2of5 { get; set; }
        public bool IsI2of5 { get; set; }
        public bool IsAztec { get; set; }
        public bool IsPDF417 { get; set; }
        public bool IsQRCode { get; set; }

        public ZebraScannerConfig()
        {
            IsEAN8 = true;
            IsEAN13 = true;
            IsCode39 = true;
            IsCode128 = true;
            IsUPCA = true;
            IsUPCE0 = true;
            IsUPCE1 = true;
            IsD2of5 = false;
            IsI2of5 = true;
            IsAztec = false;
            IsPDF417 = true;
            IsQRCode = true;

            IsContinuous = true;
            TriggerType = TriggerType.HARD;
        }
    }

    public enum TriggerType
    {
        HARD,
        SOFT
    }
}

