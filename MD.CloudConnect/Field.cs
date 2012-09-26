using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MD.CloudConnect
{
    public class Field
    {
        public string b64_value { get; set; }

        private Type _type;
        private bool _boolValue = false;
        private int _intValue = 0;
        private string _stringValue = String.Empty;

        public bool GetValueAsBool()
        {
            if (_type == null)
            {
                _type = typeof(bool);
                byte[] values = Convert.FromBase64String(b64_value);
                _boolValue = BitConverter.ToBoolean(values, 0);
                return _boolValue;
            }
            else
            {
                if (_type == typeof(bool))
                    return _boolValue;
                else throw new TypeLoadException("Not valid type for b64_value : " + b64_value);
            }
        }

        public int GetValueAsInt()
        {
            if (_type == null)
            {
                _type = typeof(int);
                byte[] values = Convert.FromBase64String(b64_value);

                if (values.Length > 0)
                {
                    _intValue = 0;
                    for (int i = 0; i < values.Length; i++)
                    {
                        _intValue = _intValue << 8;
                        _intValue += values[i];
                    }
                }

                return _intValue;
            }
            else
            {
                if (_type == typeof(int))
                    return _intValue;
                else throw new TypeLoadException("Not valid type for b64_value : " + b64_value);
            }
        }

        public string GetValueAsString()
        {
            if (_type == null)
            {
                _type = typeof(string);
                byte[] values = Convert.FromBase64String(b64_value);
                _stringValue = String.Empty;
                for (int i = 0; i < values.Length; i++)
                {
                    _stringValue += (char)values[i];
                }
                return _stringValue;
            }
            else
            {
                if (_type == typeof(string))
                    return _stringValue;
                else throw new TypeLoadException("Not valid type for b64_value : " + b64_value);
            }
        }

        public void SetValueAsInt(int value)
        {

        }

        public void SetValueAsBool(bool value)
        {

        }

        public void SetValueAsString(string value)
        {

        }
    }
}
