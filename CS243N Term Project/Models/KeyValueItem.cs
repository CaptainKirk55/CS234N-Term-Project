using System;
using System.Collections.Generic;
using System.Text;

namespace BITSClasses.AdditionalModels
{
    public class KeyValueItem
    {
        public int Key { get; set; }
        public string Value { get; set; }

        public KeyValueItem(int key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
