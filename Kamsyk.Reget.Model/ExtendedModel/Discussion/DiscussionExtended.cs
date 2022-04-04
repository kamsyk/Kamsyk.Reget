using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.Discussion {
    public class DiscussionExtended {
        public string discussion_bkg_color { get; set; }
        public string discussion_border_color { get; set; }
        public string discussion_user_color { get; set; }

        public List<DiscussionItemExtended> discussion_items { get; set; }
    }
}
