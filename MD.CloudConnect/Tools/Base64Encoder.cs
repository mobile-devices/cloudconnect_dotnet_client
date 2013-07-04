using System;
using System.Collections.Generic;
using System.Text;

namespace MD.CloudConnect.Tools
{
    public class Base64Encoder
    {
        public static string GetEncodedValue(string value)
        {
            string result = String.Empty;

            if (!String.IsNullOrEmpty(value))
            {
                byte[] values = new byte[value.Length];
                result = String.Empty;
                for (int i = 0; i < value.Length; i++)
                {
                    values[i] = (byte)value[i];
                }

                result = Convert.ToBase64String(values);
            }
            return result;
        }
    }
}
