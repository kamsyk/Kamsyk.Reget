using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kamsyk.Reget.AgControls {
    public class MdInputDecimalContainer : MdInputContainer {
        #region Constructor
        public MdInputDecimalContainer(string decimalSeparator) {
            this.DataType = TextBoxType.DecimalNumber;

            if (Placeholder == null) {
                Placeholder = "#" + decimalSeparator + "##";
            }
        }
        #endregion
    }
}