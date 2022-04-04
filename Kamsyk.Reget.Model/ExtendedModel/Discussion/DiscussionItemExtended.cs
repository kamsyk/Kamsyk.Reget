using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.Discussion {
    public class DiscussionItemExtended {
        public int id { get; set; }
        public int author_id { get; set; }
        public string author_name { get; set; }
        public string disc_text { get; set; }
        public string author_initials { get; set; }
        public string author_photo_url { get; set; }
        public string bkg_color { get; set; }
        public string border_color { get; set; }
        public string user_color { get; set; }
        public string modif_date_text { get; set; }
    }
}
