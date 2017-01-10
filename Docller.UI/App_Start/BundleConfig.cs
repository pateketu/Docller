using System.Web;
using System.Web.Optimization;
using Docller.Common;

namespace Docller
{

    public class ScriptBundles
    {
        public const string JQueryBundle = "~/bundles/jquery";
        public const string BootStrapBundle = "~/bundles/bootstrap";
        public const string DocllerCommonBundle = "~/bundles/docllercommon";
        public const string ValidationBundle = "~/bundles/plugins/validation";
        public const string FileRegisterBundle = "~/bundles/fileregister";
        public const string FolderTreeBundle = "~/bundles/foldertree";
        public const string FilePickerFolderTreeBundle = "~/bundles/filepickerfoldertree";
        public const string EditFilesBundle = "~/bundles/editfiles";
        public const string TransmittalBundle = "~/bundles/transmittal";
        public const string KnockoutBundle = "~/bundles/knockout";
        public const string ManagePermissionsBundle = "~/bundles/ManagePermissions";

    }

    public class StyleBundles
    {
        
        public const string ThemesBundle = "~/Content/themes/base/css";
        public const string BootstrapBundle = "~/Content/bootstrap/css";
        public const string SiteBundle = "~/Content/css";
        public const string DynaTreeBundle = "~/Content/DynaTree/skin-vista/css";
        public const string PluUploadBundle = "~/Content/plupload/css";
        public const string FileRegisterBundle = "~/Content/fileregister/css";
        public const string FileRegisterResponsiveBundle = "~/Content/FileRegister-Responsive";
        public const string HandsonTableBundle = "~/Content/handsontable/css";
        public const string Select2Bundle = "~/Content/select2/css";
    }
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            ScriptBundle jqueryBundle = new ScriptBundle(ScriptBundles.JQueryBundle) { Orderer = new AsIsBundleOrderer() };

            jqueryBundle.Include("~/Scripts/jquery/jquery-{version}.js",
                                 "~/Scripts/jquery/jquery-ui-{version}.js");

            bundles.Add(jqueryBundle);


            bundles.Add(new ScriptBundle(ScriptBundles.BootStrapBundle){ Orderer =  new AsIsBundleOrderer()}
                            .Include("~/Scripts/bootstrap/bootstrap.js",
                                    "~/Scripts/bootstrap/bootstrap-transition.js"));

            bundles.Add(new ScriptBundle(ScriptBundles.ValidationBundle)
                        {
                            Orderer = new AsIsBundleOrderer()
                        }.Include("~/Scripts/plugins/jquery.validate*",
                                  "~/Scripts/plugins/jquery.jquery.validate.unobtrusive*")
                );
            bundles.Add(new ScriptBundle(ScriptBundles.DocllerCommonBundle)
                            .Include("~/Scripts/Docller.js", "~/Scripts/plugins/jquery.blockUI.js", "~/Scripts/plugins/holder.js"));

            bundles.Add(new ScriptBundle(ScriptBundles.KnockoutBundle)
                            .Include("~/Scripts/plugins/knockout-2.1.0.js", "~/Scripts/plugins/knockout.mapping-latest.js"));

            ScriptBundle managePermissionsBundle = new ScriptBundle(ScriptBundles.ManagePermissionsBundle)
            {
                Orderer = new AsIsBundleOrderer()
            };

            managePermissionsBundle.Include("~/Scripts/plugins/jquery.handsontable.js");
            managePermissionsBundle.Include("~/Scripts/plugins/bootstrap-typeahead.js");
            managePermissionsBundle.Include("~/Scripts/ManagePermissions.js");
            bundles.Add(managePermissionsBundle);


            RegisterFileRegisterScriptBundles(bundles);

            RegisterStyleBundles(bundles);
            /* bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));*/


        }

        private static void RegisterStyleBundles(BundleCollection bundles)
        {
            //to fix the issues with wrong image paths in css when running Release version
            //see http://stackoverflow.com/questions/11355935/mvc4-stylebundle-not-resolving-images

            bundles.Add(new StyleBundle(StyleBundles.ThemesBundle) { Orderer = new AsIsBundleOrderer() }
                            .Include("~/Content/themes/base/*.css"));


            bundles.Add(new StyleBundle(StyleBundles.BootstrapBundle) { Orderer = new AsIsBundleOrderer() }
                            .Include("~/Content/bootstrap/bootstrap.css",
                                     "~/Content/bootstrap/bootstrap-responsive.css"));

            bundles.Add(new StyleBundle(StyleBundles.SiteBundle)
            .Include("~/Content/Site.css"));

            bundles.Add(
                new StyleBundle(StyleBundles.DynaTreeBundle).Include("~/Content/DynaTree/skin-vista/ui.dynatree.css"));


            bundles.Add(
                new StyleBundle(StyleBundles.PluUploadBundle).Include("~/Content/plupload/jquery.plupload.queue.css"));


            bundles.Add(new StyleBundle(StyleBundles.FileRegisterBundle).Include("~/Content/fileregister/fileregister.css"));



            bundles.Add(
                new StyleBundle(StyleBundles.FileRegisterResponsiveBundle).Include(
                    "~/Content/fileregister/fileregister-responsive.css"));

            
            bundles.Add(new StyleBundle(StyleBundles.HandsonTableBundle)
                   .Include("~/Content/handsontable/jquery.handsontable.css"));
            
            bundles.Add(new StyleBundle(StyleBundles.Select2Bundle)
                            .Include("~/Content/select2/select2.css"));
        }

        private static void RegisterFileRegisterScriptBundles(BundleCollection bundles)
        {
            ScriptBundle scriptBundle = new ScriptBundle(ScriptBundles.FileRegisterBundle)
                {
                    Orderer = new AsIsBundleOrderer()
                };
            
            scriptBundle.Include("~/Scripts/plugins/plupload.full.js");
            scriptBundle.Include("~/Scripts/plugins/jquery.plupload.queue.js");
            scriptBundle.Include("~/Scripts/openseadragon/openseadragon*");
            
            scriptBundle.Include("~/Scripts/plugins/jquery.unobtrusive*");
            scriptBundle.Include("~/Scripts/plugins/jquery.fileDownload.js");
            scriptBundle.Include("~/Scripts/fileregister/*.js");
            scriptBundle.Include("~/Scripts/InviteUsers.js");


            bundles.Add(scriptBundle);  

            ScriptBundle editFileBundle = new ScriptBundle(ScriptBundles.EditFilesBundle)
                {
                    Orderer = new AsIsBundleOrderer()
                };
            editFileBundle.Include("~/Scripts/plugins/jquery.handsontable.js");
            editFileBundle.Include("~/Scripts/plugins/bootstrap-typeahead.js");
            editFileBundle.Include("~/Scripts/EditFiles.js");
            bundles.Add(editFileBundle);

            ScriptBundle transmittalBundle = new ScriptBundle(ScriptBundles.TransmittalBundle)   
                {
                    Orderer = new AsIsBundleOrderer()
                };
            transmittalBundle.Include("~/Scripts/plugins/select2.js");
            transmittalBundle.Include("~/Scripts/fileregister/filepicker.js");
            transmittalBundle.Include("~/Scripts/CreateTransmittal.js");
            transmittalBundle.Include("~/Scripts/InviteUsers.js");
            bundles.Add(transmittalBundle);

            bundles.Add(new ScriptBundle(ScriptBundles.FolderTreeBundle)     {
                    Orderer = new AsIsBundleOrderer()
                }.Include("~/Scripts/plugins/jquery.dynatree.js","~/Scripts/Tree.js"));

            bundles.Add(new ScriptBundle(ScriptBundles.FilePickerFolderTreeBundle)
                {
                    Orderer = new AsIsBundleOrderer()
                }.Include("~/Scripts/plugins/jquery.dynatree.js", "~/Scripts/FilePickerTree.js"));
        }
    }
}