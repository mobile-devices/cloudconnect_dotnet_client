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
                if (!String.IsNullOrEmpty(b64_value))
                {
                    byte[] values = Convert.FromBase64String(b64_value);
                    _boolValue = BitConverter.ToBoolean(values, 0);
                }
                else _boolValue = false;
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
                if (!String.IsNullOrEmpty(b64_value))
                {
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
                }
                else _intValue = 0;

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
                if (!String.IsNullOrEmpty(b64_value))
                {
                    byte[] values = Convert.FromBase64String(b64_value);
                    _stringValue = String.Empty;
                    for (int i = 0; i < values.Length; i++)
                    {
                        _stringValue += (char)values[i];
                    }
                }
                else _stringValue = String.Empty;

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
            _type = typeof(int);
            _intValue = value;
        }

        public void SetValueAsBool(bool value)
        {
            _type = typeof(bool);
            _boolValue = value;
        }

        public void SetValueAsString(string value)
        {
            _type = typeof(string);
            _stringValue = value;
        }
    }
}
