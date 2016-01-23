using System.Web;
using System.Web.Optimization;

namespace LechTyper
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.11.2.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapjs").Include(
                        "~/Scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/css/bootstrapcss").Include(
                        "~/Content/css/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Content/css/bootstrap-responsivecss").Include(
                        "~/Content/css/bootstrap-responsive.min.css"));
        }
    }
}