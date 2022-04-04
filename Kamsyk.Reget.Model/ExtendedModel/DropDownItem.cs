using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class DropDownItem {
        public int id { get; set; }
        public string name { get; set; }
        public bool is_disabled { get; set; }

        public DropDownItem(int itemId, string itemName) {
            id = itemId;
            name = itemName;
            is_disabled = false;
        }

        public DropDownItem(int itemId, string itemName, bool isDisabled) {
            id = itemId;
            name = itemName;
            is_disabled = isDisabled;
        }
    }
}
