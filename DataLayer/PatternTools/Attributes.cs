using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NullableColumnAttribute : Attribute
    {
        public bool IsNullable { get; set; } = false;

        public NullableColumnAttribute(bool isNullable)
        {
            IsNullable = isNullable;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FKColumnAttribute : Attribute
    {
        public bool IsFK { get; set; } = true;

        public FKColumnAttribute(bool isFK)
        {
            IsFK = isFK;
        }
    }
}
