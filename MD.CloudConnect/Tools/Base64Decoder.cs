using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect.Tools
{
    public class Base64Decoder
    {
        public static bool GetValueAsBool(string b64_value)
        {
            bool result = false;
            if (!String.IsNullOrEmpty(b64_value))
            {
                byte[] values = Convert.FromBase64String(b64_value);
                result = BitConverter.ToBoolean(values, 0);
            }
            return result;
        }

        public static int GetValueAsInt(string b64_value)
        {
            int result = 0;
            if (!String.IsNullOrEmpty(b64_value))
            {
                byte[] values = Convert.FromBase64String(b64_value);

                if (values.Length > 0)
                {
                    result = 0;
                    for (int i = 0; i < values.Length; i++)
                    {
                        result = result << 8;
                        result += values[i];
                    }
                }
            }
            return result;
        }

        public static string GetValueAsString(string b64_value)
        {
            string result = String.Empty;
            if (!String.IsNullOrEmpty(b64_value))
            {
                byte[] values = Convert.FromBase64String(b64_value);
                result = String.Empty;
                for (int i = 0; i < values.Length; i++)
                {
                    result += (char)values[i];
                }
            }
            return result;
        }
    }
}
