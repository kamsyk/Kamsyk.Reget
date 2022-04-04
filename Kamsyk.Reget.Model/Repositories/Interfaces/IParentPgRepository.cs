using Kamsyk.Reget.Model.ExtendedModel.PurchaseGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.Repositories.Interfaces {
    public interface IParentPgRepository {
        void SaveParentPg(ParentPgExtended parentPg);
    }
}
