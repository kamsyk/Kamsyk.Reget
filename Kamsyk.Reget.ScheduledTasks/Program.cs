using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kamsyk.Reget.ScheduledTasks {
    class Program {
        static void Main(string[] args) {

            //Non Active Users
//#if !DEBUG
            //new User().CheckNonActiveUsers();
//#endif


            ////Empty Roles
            //new User().CheckEmptyRoles();

            //Load User Photos from OWA
            //new User().LoadUserPhotos();
            //new User().LoadUserPhotosSelenium();
//#if DEBUG
            new User().LoadUserPhotosGraph();
//#endif

            DateTime dDate = new DateTime(2021, 1, 1);
            while (dDate < DateTime.Now) {
                Console.WriteLine("Loading requests from " + dDate.Year);
                new Request().AddRequestTextFullIndex(dDate, dDate.AddYears(1));
                dDate = dDate.AddYears(1);
            }
        }
    }
}
