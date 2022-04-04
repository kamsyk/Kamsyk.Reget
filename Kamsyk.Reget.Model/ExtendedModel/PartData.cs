using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class PartData<T> where T : class {
        public int rows_count = 0;
        public List<T> db_data;
    }
}
