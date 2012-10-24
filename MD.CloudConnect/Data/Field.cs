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
                _boolValue = MD.CloudConnect.Tools.Base64Decoder.GetValueAsBool(b64_value);
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
                _intValue = MD.CloudConnect.Tools.Base64Decoder.GetValueAsInt(b64_value);
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
                _stringValue = MD.CloudConnect.Tools.Base64Decoder.GetValueAsString(b64_value);
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
