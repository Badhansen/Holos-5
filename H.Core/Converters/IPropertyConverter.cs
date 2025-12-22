using System.Reflection;
using H.Core.Enumerations;

namespace H.Core.Converters
{
    public interface IPropertyConverter
    {
        //all the properties in you want to work with
        List<PropertyInfo> PropertyInfos { get; set; }

        /// <summary>
        /// Allows the user to convert GUI binding value (metric/imperial) to a metric system value
        /// </summary>
        /// <param name="propertyName">the name of the property to convert</param>
        /// <returns>the converted value</returns>
        double GetSystemValueFromBinding(string propertyName);

        /// <summary>
        /// Allows the user to convert a property from system storage to binding value for the GUI in appropriate units (metric/imperial)
        /// </summary>
        /// <param name="propertyName">property to get the value for</param>
        /// <returns>metric or imperial value</returns>
        double GetBindingValueFromSystem(string propertyName);

        /// <summary>
        /// Allows the user to convert GUI binding value (metric/imperial) to a metric system value
        /// </summary>
        /// <param name="prop">the name of the property to convert</param>
        /// <returns>the converted value</returns>
        double GetSystemValueFromBinding(PropertyInfo prop);

        /// <summary>
        /// Allows the user to convert a property from system storage to binding value for the GUI in appropriate units (metric/imperial)
        /// </summary>
        /// <param name="prop">property to get the value for</param>
        /// <returns>metric or imperial value</returns>
        double GetBindingValueFromSystem(PropertyInfo prop);

        double GetBindingValueFromSystem(PropertyInfo prop, MeasurementSystemType measurementSystemType);

        /// <summary>
        /// Convert the value of a property from a DTO and convert it to metric units
        /// </summary>
        /// <param name="prop">The property that was bound to the view</param>
        /// <param name="measurementSystemType">The units of measurement being used by the view/system</param>
        /// <returns>The property value converted from one unit of measurement to another</returns>
        double GetSystemValueFromBinding(PropertyInfo prop, MeasurementSystemType measurementSystemType);
    }
}