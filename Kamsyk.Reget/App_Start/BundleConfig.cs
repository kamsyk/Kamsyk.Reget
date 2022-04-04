using System;
using System.Web;
using System.Web.Optimization;

namespace Kamsyk.Reget {
    public class BundleConfig {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles) {
            //bootstrap requires it
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.2.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));


            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            
            var angBundle = GetBasicAngularBundle("~/bundles/angular");
            bundles.Add(angBundle);

            bundles.Add(new ScriptBundle("~/bundles/regetjs").Include(
                     "~/Scripts/Reget/reget.js",
                     "~/Scripts/RegetTypeScript/Base/reget-common.js",
                     "~/Scripts/RegetTypeScript/Base/reget-data-convert.js",
                     "~/Scripts/RegetTypeScript/Base/main-page.js"));

            Bundle agHomeInfoBundle = GetBasicAngularBundle("~/bundles/home-info");
            agHomeInfoBundle.Include("~/Scripts/Reget/reget-angular-homeinfo.js");
            bundles.Add(agHomeInfoBundle);

            Bundle agAdminBundle = GetBasicAngularBundle("~/bundles/angular-admin");
            //agAdminBundle.Include("~/Scripts/Reget/reget-angular.js");
            agAdminBundle.Include("~/Scripts/RegetTypeScript/cg-admin.js");
            bundles.Add(agAdminBundle);
           

            Bundle agUserBundle = GetBasicAngularBundle("~/bundles/angular-user");
            agUserBundle.Include(
                "~/Scripts/RegetTypeScript/Base/reget-base-grid.js",
                "~/Scripts/RegetTypeScript/user.js");
            bundles.Add(agUserBundle);

            Bundle agCentreBundle = GetBasicAngularBundle("~/bundles/reget-angular-centre");
            agCentreBundle.Include(
                //!!!!!! ATTENTION order counts - parent js (reget-base-grid.js) must be added first !!!
                "~/Scripts/RegetTypeScript/Base/reget-base-grid.js",
                "~/Scripts/RegetTypeScript/centre.js");
            //agCentreBundle.Include("~/Scripts/Reget/reget-angular-centre.js");
            bundles.Add(agCentreBundle);

            Bundle agParentPgBundle = GetBasicAngularBundle("~/bundles/reget-angular-parent-pg");
            agParentPgBundle.Include(
                //!!!!!! ATTENTION order counts - parent js (reget-base-grid.js) must be added first !!!
                "~/Scripts/RegetTypeScript/Base/reget-base-grid.js",
                "~/Scripts/RegetTypeScript/Base/reget-base-grid-translation.js",
                "~/Scripts/RegetTypeScript/parent-pg.js");
            //agParentPgBundle.Include("~/Scripts/Reget/reget-angular-parent-pg.js");
            bundles.Add(agParentPgBundle);

            Bundle agUsedPgBundle = GetBasicAngularBundle("~/bundles/reget-angular-used-pg");
            //agUsedPgBundle.Include("~/Scripts/Reget/reget-angular-used-pg.js");
            agUsedPgBundle.Include(
                "~/Scripts/RegetTypeScript/Base/reget-base-grid.js",
                "~/Scripts/RegetTypeScript/Base/reget-base-grid-translation.js",
                "~/Scripts/RegetTypeScript/used-pg.js");
            bundles.Add(agUsedPgBundle);

            Bundle agAddressBundle = GetBasicAngularBundle("~/bundles/reget-angular-address");
            agAddressBundle.Include(
                //!!!!!! ATTENTION order counts - parent js (reget-base-grid.js) must be added first !!!
                "~/Scripts/RegetTypeScript/Base/reget-base-grid.js", 
                "~/Scripts/RegetTypeScript/reget-address.js");
            //agAddressBundle.Include("~/Scripts/Reget/reget-angular-address.js");
            bundles.Add(agAddressBundle);

            Bundle agSupplierBundle = GetBasicAngularBundle("~/bundles/angular-supplier");
            //agSupplierBundle.Include("~/Scripts/Reget/reget-angular-supplier.js");
            agSupplierBundle.Include(
                //!!!!!! ATTENTION order counts - parent js (reget-base-grid.js) must be added first !!!
                "~/Scripts/RegetTypeScript/Base/reget-base-grid.js",
                "~/Scripts/RegetTypeScript/supplier.js");
            bundles.Add(agSupplierBundle);

            Bundle agSupplierDetailBundle = GetBasicAngularBundle("~/bundles/angular-supplier-detail");
            agSupplierDetailBundle.Include(
                //!!!!!! ATTENTION order counts - parent js (reget-base-grid.js) must be added first !!!
                "~/Scripts/RegetTypeScript/Base/reget-base-grid.js",
                "~/Scripts/RegetTypeScript/supplier-detail.js");
            bundles.Add(agSupplierDetailBundle);

            Bundle agUserReplaceBundle = GetBasicAngularBundle("~/bundles/user-replace");
            //agUserReplaceBundle.Include("~/Scripts/Reget/reget-angular-userreplace.js");
            agUserReplaceBundle.Include("~/Scripts/RegetTypeScript/replace-user.js");
            bundles.Add(agUserReplaceBundle);

            Bundle agUserInfoBundle = GetBasicAngularBundle("~/bundles/user-info");
            agUserInfoBundle.Include("~/Scripts/RegetTypeScript/replace-user.js");
            agUserInfoBundle.Include("~/Scripts/RegetTypeScript/user-info.js");
            bundles.Add(agUserInfoBundle);

            Bundle agUserCopyBundle = GetBasicAngularBundle("~/bundles/copy-user");
            agUserCopyBundle.Include("~/Scripts/RegetTypeScript/Base/test-mod.js");
            agUserCopyBundle.Include("~/Scripts/RegetTypeScript/copy-user.js");
            bundles.Add(agUserCopyBundle);

            Bundle agUserSubstitutionBundle = GetBasicAngularBundle("~/bundles/angular-user-substitution");
            //agUserSubstitutionBundle.Include("~/Scripts/Reget/reget-angular-user-substitution.js");
            agUserSubstitutionBundle.Include("~/Scripts/RegetTypeScript/Base/reget-base-grid.js",
                "~/Scripts/RegetTypeScript/user-substitution.js",
                "~/Scripts/RegetTypeScript/Controls/reget-date-time.js",
                "~/Scripts/RegetTypeScript/Controls/reget-grid-date-time.js");
            bundles.Add(agUserSubstitutionBundle);

            Bundle agUserSubstitutionDetailBundle = GetBasicAngularBundle("~/bundles/angular-user-substitution-detail");
            agUserSubstitutionDetailBundle.Include(
                "~/Scripts/RegetTypeScript/user-substitution-detail.js",
                "~/Scripts/RegetTypeScript/Controls/reget-date-time.js");

            bundles.Add(agUserSubstitutionDetailBundle);

            //Bundle agHomeInfoBundle = GetBasicAngularBundle("~/bundles/angular-homeinfo");
            //agHomeInfoBundle.Include("~/Scripts/Reget/reget-angular-homeinfo.js");
            //bundles.Add(agHomeInfoBundle);

            Bundle agExportAppMatrixBundle = GetBasicAngularBundle("~/bundles/reget-angular-appmatrix-export");
            agExportAppMatrixBundle.Include("~/Scripts/RegetTypeScript/appmatrix-export.js");
            //agExportAppMatrixBundle.Include("~/Scripts/Reget/reget-angular-appmatrix-export.js");
            bundles.Add(agExportAppMatrixBundle);

            Bundle agAppMatrixCopyBundle = GetBasicAngularBundle("~/bundles/reget-angular-app-matrix-copy");
            //agAppMatrixCopyBundle.Include("~/Scripts/Reget/reget-angular-app-matrix-copy.js");
            agAppMatrixCopyBundle.Include("~/Scripts/RegetTypeScript/reget-app-matrix-copy.js");
            bundles.Add(agAppMatrixCopyBundle);

            Bundle agEventBundle = GetBasicAngularBundle("~/bundles/reget-angular-event");
            agEventBundle.Include(
                //!!!!!! ATTENTION order counts - parent js (reget-base-grid.js) must be added first !!!
                "~/Scripts/RegetTypeScript/Base/reget-base-grid.js",
                "~/Scripts/RegetTypeScript/event-viewer.js");
            //agEventBundle.Include("~/Scripts/Reget/reget-angular-event.js");
            bundles.Add(agEventBundle);

            Bundle agRequestBundle = GetBasicAngularBundle("~/bundles/reget-angular-request");
            //!!!!!! ATTENTION order counts - parent js (reget-base-grid.js) must be added first !!!
            agRequestBundle.Include("~/Scripts/RegetTypeScript/Base/reget-base-grid.js");
            agRequestBundle.Include("~/Scripts/RegetTypeScript/request.js");
            agRequestBundle.Include("~/Scripts/ng-file-upload.min.js");
            //agRequestBundle.Include("~/Scripts/angular-wysiwyg.js");
            bundles.Add(agRequestBundle);

            Bundle agRequestListBundle = GetBasicAngularBundle("~/bundles/request-list");
            //!!!!!! ATTENTION order counts - parent js (reget-base-grid.js) must be added first !!!
            agRequestListBundle.Include("~/Scripts/RegetTypeScript/Base/reget-base-grid.js",
                    "~/Scripts/RegetTypeScript/request-list.js",
                    "~/Scripts/RegetTypeScript/Controls/reget-date-time.js");
            bundles.Add(agRequestListBundle);

            Bundle agMultiplyAppLevels = GetBasicAngularBundle("~/bundles/angular-multiply-app-levels");
            agMultiplyAppLevels.Include("~/Scripts/RegetTypeScript/multiply-app-level.js");
            bundles.Add(agMultiplyAppLevels);

            Bundle agStatisticsBundle = GetBasicAngularBundle("~/bundles/reget-angular-statistics");
            agStatisticsBundle.Include("~/Scripts/RegetTypeScript/statistics.js");
            //agStatisticsBundle.Include("~/Scripts/Reget/reget-angular-statistics.js");
            agStatisticsBundle.Include("~/Scripts/Chart.min.js");
            agStatisticsBundle.Include("~/Scripts/Moment.min.js");
            bundles.Add(agStatisticsBundle);

            Bundle agSearchBundle = GetBasicAngularBundle("~/bundles/search");
            agSearchBundle.Include("~/Scripts/RegetTypeScript/search.js");
            bundles.Add(agSearchBundle);

            Bundle agOrderBundle = GetBasicAngularBundle("~/bundles/order");
            agOrderBundle.Include("~/Scripts/RegetTypeScript/order.js");
            agOrderBundle.Include("~/Scripts/ng-file-upload.min.js");
            bundles.Add(agOrderBundle);

            bundles.Add(new StyleBundle("~/RegetContent/css").Include(
                      "~/Content/CssStyles/bootstrap.min.css",
                     // "~/Content/less/bootstrap.min.css",
                      "~/Content/CssStyles/site.css",
                      "~/Content/CssStyles/Normalize.css",
                      //"~/Content/CssStyles/Reget.css",
                      "~/Content/CssStyles/RegetGrid.css",
                      "~/Content/angular-material.min.css",
                      "~/Content/ui-grid.min.css"));

            bundles.Add(new StyleBundle("~/RegetButtons/css").Include(
                "~/Content/CssStyles/RegetButton.css", new CssRewriteUrlTransformWrapper()));

            bundles.Add(new StyleBundle("~/RegetButtonBar/css").Include(
                "~/Content/CssStyles/RegetButtonBar.css", new CssRewriteUrlTransformWrapper()));

            bundles.Add(new StyleBundle("~/Reget/css").Include(
                "~/Content/CssStyles/Reget.css", new CssRewriteUrlTransformWrapper()));

            bundles.Add(new StyleBundle("~/RegetAngular/css").Include(
                "~/Content/CssStyles/RegetAngular.css", new CssRewriteUrlTransformWrapper()));
        }

        private static Bundle GetBasicAngularBundle(string bName) {
            Bundle agBundle = new ScriptBundle(bName).Include(
                         "~/Scripts/angular.min.js",
                         "~/Scripts/angular-animate.min.js",
                         "~/Scripts/angular-route.min.js",
                         "~/Scripts/angular-aria.min.js",
                         "~/Scripts/angular-messages.min.js",
                         "~/Scripts/angular-material.min.js",
                         "~/Scripts/angular-touch.min.js",
                         "~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
                         "~/Scripts/ui-grid.min.js",
                         //"~/Scripts/ui-grid.core.min.js",
                         //"~/Scripts/ui-grid.auto-resize.min.js",
                         "~/Scripts/angularjs-dropdown-multiselect.min.js",
                         "~/Scripts/Reget/reget-angular-grid-service.js",
                         "~/Scripts/Reget/reget-angular-user-service.js",
                         "~/Scripts/Reget/reget-angular-common-service.js",
                         "~/Scripts/RegetTypeScript/Base/reget-base.js",
                         //"~/Scripts/RegetTypeScript/Base/reget-common.js",
                         "~/Scripts/RegetTypeScript/Base/reget-entity.js",
                         //"~/Scripts/RegetTypeScript/Base/reget-base-grid.js",
                         "~/Scripts/Moment.min.js");
            return agBundle;
        }
    }

   
    public class CssRewriteUrlTransformWrapper : IItemTransform {
        public string Process(string includedVirtualPath, string input) {
            //return new CssRewriteUrlTransform().Process(includedVirtualPath, input);
            return new CssRewriteUrlTransform().Process("~" + VirtualPathUtility.ToAbsolute(includedVirtualPath), input);
        }
    }

    
}
