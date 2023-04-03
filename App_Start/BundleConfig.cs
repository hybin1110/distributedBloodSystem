using System.Web;
using System.Web.Optimization;

namespace FYP_Blood
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new Bundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new Bundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.bundle.min.js",
                 "~/Scripts/bootstrap.bundle.js"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                        "~/Scripts/angular.js",
                        "~/Scripts/angular.min.js",
                        "~/Scripts/AngularController/AngularHome.js",
                        "~/Scripts/AngularController/AngularAuthority.js",
                         "~/Scripts/AngularController/AngularAdmin.js"));



            bundles.Add(new ScriptBundle("~/bundles/mainscript").Include("~/Scripts/Main.js"));

            bundles.Add(new ScriptBundle("~/bundles/hosscript").Include("~/Scripts/Main_Hospital.js"));

            bundles.Add(new ScriptBundle("~/bundles/adminscript").Include("~/Scripts/Main_Admin.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/template_home.css",
                      "~/Content/template_authority.css",
                      "~/Content/template_admin.css",
                      "~/bootstrap-icons/font/bootstrap-icons",
                      "~/Content/site.css"));

        }
    }
}
