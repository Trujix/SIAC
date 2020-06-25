using System.Web;
using System.Web.Optimization;

namespace siacWEB
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/JQueryUI/jquery-ui.css",
                      "~/Content/bootstrap.min.css",
                      "~/iconos/css/all.css",
                      "~/Content/pnotify.min.css",
                      "~/Content/pnotify.css",
                      "~/Content/loader.css"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/JQueryUI/jquery-ui.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/popper.min.js",
                      "~/Scripts/pnotify.min.js",
                      "~/Scripts/pnotify.js",
                      "~/Scripts/loader.js",
                      "~/Scripts/errores.js"));
        }
    }
}
