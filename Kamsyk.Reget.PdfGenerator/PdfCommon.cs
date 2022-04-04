using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.PdfGenerator {
    public class PdfCommon {
        #region Constants
        private const int ORDER_TYPE_OTHER = 0;
        private const int ORDER_TYPE_CLC_BRECLAV = 1;
        private const int ORDER_TYPE_ZONA_CZ = 2;
        private const int ORDER_TYPE_ZONA_SK = 3;
        private const int ORDER_TYPE_BG = 4;
        private const int ORDER_TYPE_UNKNOWN = -1;
        #endregion

        #region Enums
        public enum OrderType {
            ClcBreclav = ORDER_TYPE_CLC_BRECLAV,
            ZonaCZ = ORDER_TYPE_ZONA_CZ,
            ZonaSK = ORDER_TYPE_ZONA_SK,
            Bg = ORDER_TYPE_BG,
            Other = ORDER_TYPE_OTHER,
            Unknown = ORDER_TYPE_UNKNOWN
        }

        #endregion

        #region Static Methods
        public static OrderType GetOrderType(int iOrderType) {
            switch (iOrderType) {
                case ORDER_TYPE_CLC_BRECLAV:
                    return OrderType.ClcBreclav;
                case ORDER_TYPE_ZONA_CZ:
                    return OrderType.ZonaCZ;
                case ORDER_TYPE_ZONA_SK:
                    return OrderType.ZonaSK;
                case ORDER_TYPE_BG:
                    return OrderType.Bg;
                case ORDER_TYPE_OTHER:
                    return OrderType.Other;
                default:
                    return OrderType.Unknown;
            }
        }
        #endregion
    }
}
