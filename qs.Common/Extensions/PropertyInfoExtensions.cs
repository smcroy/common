namespace qs.Extensions.PropertyInfoExtensions
{
    using System.ComponentModel;
    using System.Reflection;

    public static class PropertyInfoExtensions
    {
        #region Methods

        /// <summary>
        /// Returns the value of the DefaultValueAttribute for the specified property.
        /// Example:
        /// using System.ComponentModel;
        /// using System.Reflection;
        /// using qs.Extensions.PropertyInfoExtensions;
        /// using qs.Extensions.SymbolExtensions;
        /// [Description("This is a public string property.")]
        /// [DefaultValue("This is the default value for the public string property.")]
        /// public string PublicStringProperty
        /// {
        /// 	get;
        /// 	set;
        /// }
        /// [Description("This is a public int property.")]
        /// [DefaultValue(222)]
        /// public int PublicIntProperty
        /// {
        /// 	get;
        /// 	set;
        /// }
        /// void getStringProp( )
        /// {
        /// 	string prop = this.GetPropertySymbol( o => o.PublicStringProperty );
        /// 	var description = this.GetType().GetProperty( prop ).GetDescription( );
        /// 	string defaultvalue = this.GetType().GetProperty( prop ).GetDefaultValue&lt;string&gt;( );
        /// }
        /// void getIntProp( )
        /// {
        /// 	string prop = this.GetPropertySymbol( o => o.PublicIntProperty );
        /// 	var description = this.GetType( ).GetProperty( prop ).GetDescription( );
        /// 	int defaultvalue = this.GetType( ).GetProperty( prop ).GetDefaultValue&lt;int&gt;( );
        /// }
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static T GetDefaultValue<T>(this PropertyInfo o)
        {
            DefaultValueAttribute[] attributes = (DefaultValueAttribute[])o.GetCustomAttributes(typeof(DefaultValueAttribute), false);
            return attributes.Length > 0 ? (T)attributes[0].Value : default(T);
        }

        /// <summary>
        /// Returns the value of the DescriptionAttribute for the specified property.
        /// Example:
        /// using System.ComponentModel;
        /// using System.Reflection;
        /// using qs.Extensions.PropertyInfoExtensions;
        /// using qs.Extensions.SymbolExtensions;
        /// [Description("This is a public string property.")]
        /// [DefaultValue("This is the default value for the public string property.")]
        /// public string PublicStringProperty
        /// {
        /// 	get;
        /// 	set;
        /// }
        /// [Description("This is a public int property.")]
        /// [DefaultValue(222)]
        /// public int PublicIntProperty
        /// {
        /// 	get;
        /// 	set;
        /// }
        /// void getStringProp( )
        /// {
        /// 	string prop = this.GetPropertySymbol( o => o.PublicStringProperty );
        /// 	var description = this.GetType().GetProperty( prop ).GetDescription( );
        /// 	string defaultvalue = this.GetType().GetProperty( prop ).GetDefaultValue&lt;string&gt;( );
        /// }
        /// void getIntProp( )
        /// {
        /// 	string prop = this.GetPropertySymbol( o => o.PublicIntProperty );
        /// 	var description = this.GetType( ).GetProperty( prop ).GetDescription( );
        /// 	int defaultvalue = this.GetType( ).GetProperty( prop ).GetDefaultValue&lt;int&gt;( );
        /// }
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static string GetDescription(this PropertyInfo o)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])o.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        #endregion Methods
    }
}