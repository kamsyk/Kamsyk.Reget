using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.Common {
    public sealed class DataNulls {
        public const int INT_NULL = Int32.MinValue;
        public const long LONG_NULL = long.MinValue;
        public const decimal DECIMAL_NULL = decimal.MinValue;
        public const double DOUBLE_NULL = double.MinValue;
        public static DateTime DATETIME_NULL = DateTime.MinValue;
        public static string STRING_NULL = "#null#";
    }
}
