using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel.Attachment {
    public class AttachmentExtend {
        public int id { get; set; }
        public string file_name { get; set; }
        public double size_kb { get; set; }
        public string icon_url { get; set; }
        public bool is_can_be_deleted { get; set; }
    }
}
