using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using WebBot.Converters;

namespace WebBot.BetActions
{
    [TypeConverter(typeof(DictionaryPropertyTypeConverter))]
    public class DictionaryPropertyObject : DictionaryPropertyGridAdapter<string, object>
    {
        public DictionaryPropertyObject(Dictionary<string, object> d) : base(d)
        {
            foreach (PropertyInfo pi in typeof(DictionaryPropertyObject).GetProperties())
            {
                if (pi.Name != "Dictionary")
                {
                    d.Add(pi.Name, pi.GetValue(this, null));
                }
            }
        }
        
        public DictionaryPropertyObject() : base()
        {

        }
    }

    internal class DictionaryPropertyTypeConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context,
                             System.Globalization.CultureInfo culture,
                             object value, Type destType)
        {
            if (destType == typeof(string) && value is CActionType)
            {
                CActionType type = (CActionType)value;
                return type.GetName();
            }
            else
            {
                return "Open to Edit Properties";
            }
            //return base.ConvertTo(context, culture, value, destType);
        }
    }
}
