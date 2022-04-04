using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.ExtendedModel {
    public class CentreExtended : Centre {
        public int centre_group_id { get; set; }
        
        #region Constructor
        public CentreExtended() : base() {
            
        }
        #endregion
    }
}
