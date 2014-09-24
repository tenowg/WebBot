using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WebBot.Converters
{
    [DataContract]
    public class DictionaryPropertyGridAdapter<T, V> : ICustomTypeDescriptor
    {
        [DataMember]
        Dictionary<T, V> _dictionary;

        Dictionary<T, string> _descriptionDictionary;

        public DictionaryPropertyGridAdapter(Dictionary<T, V> d)
        {
            _dictionary = d;
        }

        public DictionaryPropertyGridAdapter()
        {
            _dictionary = new Dictionary<T, V>();
            _descriptionDictionary = new Dictionary<T, string>();
        }

        [Browsable(false)]
        public Dictionary<T, V> Dictionary { get { return _dictionary; } }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            ArrayList properties = new ArrayList();
            foreach (KeyValuePair<T, V> e in _dictionary)
            {
                properties.Add(new DictionaryPropertyDescriptor(_dictionary, e.Key, _descriptionDictionary));
            }

            PropertyDescriptor[] props =
                (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(props);
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _dictionary;
        }

        public void AddProperty(T key, V value, string description)
        {
            if (!_dictionary.ContainsKey(key))
            {
                _dictionary.Add(key, value);
            }

            if(description != null && !_descriptionDictionary.ContainsKey(key))
            {
                _descriptionDictionary.Add(key, description);
            }
        }
        public void AddProperty(T key, V value)
        {
            AddProperty(key, value, null);
        }

        public void GetProperty<TRet>(T key, out TRet value)
        {
            V ret;
            var test = _dictionary.TryGetValue(key, out ret);

            if (ret is TRet)
            {
                value = (TRet)Convert.ChangeType(ret, typeof(TRet));
            }
            else
            {
                value = default(TRet);
            }
        }
    }
}
