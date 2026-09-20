using Dommel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace AppRuteoFactuSys.Service
{
    public class SnakeCaseColumnNameResolverImpl : IColumnNameResolver
    {
        public string ResolveColumnName(PropertyInfo propertyInfo)
        {
            // 1. Si la propiedad tiene el atributo [Column], lo respetamos
            var columnAttr = propertyInfo.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>();
            if (columnAttr != null) return columnAttr.Name;

            // 2. Si no lo tiene, convertimos PascalCase (TipoCedula) a snake_case (tipo_cedula)
            return Regex.Replace(propertyInfo.Name, "(?<!^)([A-Z])", "_$1").ToLower();
        }
    }
}
