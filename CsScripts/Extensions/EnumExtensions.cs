using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveNightsAtFrederik.CsScripts.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Gets an attribute on an enum field value
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attribute you want to retrieve</typeparam>
    /// <param name="enumValue">The enum value</param>
    /// <returns>The attribute of type T that exists on the enum value</returns>
    /// <example><![CDATA[string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;]]></example>
    public static TAttribute GetAttributeOfType<TAttribute>(this Enum enumValue) where TAttribute : System.Attribute
    {
        var type = enumValue.GetType();
        var memInfo = type.GetMember(enumValue.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(TAttribute), false);

        return (attributes.Length > 0) ? (TAttribute)attributes[0] : null;
    }

    public static string GetDescription(this Enum value) => value.GetAttributeOfType<DescriptionAttribute>().Description;
}
