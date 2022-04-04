using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WcfReget {
    [ServiceContract]
    interface IOrder {
        [OperationContract]
        ActionResult GenerateOrder(int companyId);
    }
}
