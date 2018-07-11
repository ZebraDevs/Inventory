using System;
namespace Inventory
{
    public class Barcode
    {
        private string data;
        public string Data
        {
            get { return data; }
            set { data = value; }
        }

        private string type;
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string info;
        public string Info
        {
            get { return $"{data} / {type}"; }
        }

        public Barcode() { }

        public Barcode(string a_data, string a_type)
        {
            data = a_data;
            type = a_type;
        }
    }
}
