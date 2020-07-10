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
                      "~/Content/dataTables.bootstrap4.min.css",
                      "~/Content/pnotify.min.css",
                      "~/Content/cropper.css",
                      "~/Content/pnotify.css",
                      "~/Content/loader.css"));

            bundles.Add(new ScriptBundle("~/bundles/scripts").Include(
                      "~/JQueryUI/jquery-ui.js",
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/popper.min.js",
                      "~/Scripts/jquery.dataTables.min.js",
                      "~/Scripts/dataTables.bootstrap4.min.js",
                      "~/Scripts/cropper.js",
                      "~/Scripts/pnotify.min.js",
                      "~/Scripts/pnotify.js",
                      "~/Scripts/attrchange.js",
                      "~/Scripts/loader.js",
                      "~/Scripts/errores.js",
                      "~/Scripts/auxfuns.js"));

            bundles.Add(new StyleBundle("~/Content/login").Include(
                      "~/Content/login.css"));

            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                      "~/Scripts/login.js"));

            bundles.Add(new StyleBundle("~/Content/menu").Include(
                      "~/Content/menuscroll.min.css",
                      "~/Content/menu.min.css",
                      "~/Content/estilospers.css"));

            bundles.Add(new ScriptBundle("~/bundles/menu").Include(
                      "~/Scripts/menuscroll.min.js",
                      "~/Scripts/menu.min.js",
                      "~/Scripts/dinamicos.js",
                      "~/Scripts/index.js",
                      "~/Scripts/citas.js",
                      "~/Scripts/consultas.js",
                      "~/Scripts/administracion.js"));
        }
    }
}
