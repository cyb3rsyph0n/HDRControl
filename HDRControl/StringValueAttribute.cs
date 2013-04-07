using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace HDRControl
{
    public static class Extensions
    {
        public class StringValueAttribute : Attribute
        {

            #region Properties
            public string StringValue { get; protected set; }
            #endregion

            #region Constructor
            public StringValueAttribute(string value)
            {
                this.StringValue = value;
            }
            #endregion
        }
        public static string GetStringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            try
            {
                StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                    typeof(StringValueAttribute), false) as StringValueAttribute[];

                // Return the first if there was a match.
                return attribs.Length > 0 ? attribs[0].StringValue : null;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}