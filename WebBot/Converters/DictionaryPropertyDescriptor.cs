using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WebBot.Converters
{
    public class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        IDictionary _dictionary;
        object _key;

        internal DictionaryPropertyDescriptor(IDictionary d, object key, IDictionary desc)
            : base(key.ToString(), null)
        {
            _dictionary = d;
            _key = key;

            List<Attribute> a = new List<Attribute>();
            if (desc.Contains(_key))
            {
                a.Add(new DescriptionAttribute((string)desc[_key]));
            }

            this.AttributeArray = a.ToArray();
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override object GetValue(object component)
        {
            return _dictionary[_key];
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return _dictionary[_key].GetType(); }
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            _dictionary[_key] = value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
