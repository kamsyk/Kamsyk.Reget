using Kamsyk.Reget.Model.ExtendedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Kamsyk.Reget.Controllers.Interface {
    interface IParticipantController {
        ActionResult DeleteUser(ParticipantsExtended user);
    }
}
