using System.Web;
using System.Web.Optimization;

namespace A4A.UM
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js", "~/Scripts/jquery-ui*", "~/Scripts/jquery.datatables.min.js", "~/Scripts/dataTables.buttons.min.js", "~/Scripts/buttons.html5.min.js", "~/Scripts/zozo.tabs*", "~/Scripts/multiselect*")); //"~/Scripts/jquery-{version}.js", "~/Scripts/jquery.easing.min.js"

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/reset.css", "~/Content/bootstrap.css", "~/Content/responsive.css",
                      "~/Content/site.css", "~/Content/style.css","~/Content/zozo.tabs.min.css", "~/Content/buttons.dataTables.min.css", "~/Content/jquery.datatables.min.css"));
        }
    }
}
