using AppsPortal.Data;
using AppsPortal.Library;
using AppsPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RES_Repo.Globalization;

namespace AppsPortal.Extensions
{
    public static class HtmlExtension
    {
        #region UI Common Controls
        public static MvcHtmlString Image(this HtmlHelper helper, string src, string altText, string height, string width)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", src);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("height", height);
            builder.MergeAttribute("width", width);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }
        public static MvcHtmlString FormHiddenFields<TModel, TProperty>(this HtmlHelper<TModel> Helper, Expression<Func<TModel, TProperty>> expression)
        {
            //string Result = Helper.AntiForgeryToken().ToString();

            string Result = "";

            ModelMetadata Metadata = ModelMetadata.FromLambdaExpression(expression, Helper.ViewData);
            Object PKValue = Metadata.Model;
            String PKFieldName = Metadata.PropertyName;

            Result += "<input class=\"PK\" data-val=\"true\" id=\"" + PKFieldName + "\" name=\"" + PKFieldName + "\" type=\"hidden\" value=\"" + PKValue + "\">";
            string ActiveValue = "";
            if (Helper.ViewData.Model != null)
            {
                Type ModelType = Helper.ViewData.Model.GetType();
                IList<PropertyInfo> ModelProperties = new List<PropertyInfo>(ModelType.GetProperties());

                foreach (PropertyInfo prop in ModelProperties)
                {
                    if (prop.Name.EndsWith("RowVersion"))
                    {
                        //AAAAAAALTjE=
                        byte[] rv = (byte[])prop.GetValue(Helper.ViewData.Model, null);
                        string RowVersion = "";
                        if (rv != null)
                        {
                            RowVersion = Convert.ToBase64String(rv);
                        }
                        Result += "<input id=\"" + prop.Name + "\" name=\"" + prop.Name + "\" type=\"hidden\" value=\"" + RowVersion + "\">";

                    }
                    object propValue = prop.GetValue(Helper.ViewData.Model, null);

                    // Do something with propValue
                    ActiveValue = ModelType.GetProperty("Active").GetValue(Helper.ViewData.Model, null).ToString();
                }
            }
            Result += "<input data-val=\"true\" id=\"Active\" name=\"Active\" type=\"hidden\" value=\"" + ActiveValue + "\">";
            Result += ErrorSummary(Helper).ToString();

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString ErrorSummary(this HtmlHelper helper)
        {
            string Message = "";
            foreach (var key in helper.ViewData.ModelState.Keys)
            {
                foreach (var err in helper.ViewData.ModelState[key].Errors)
                {
                    Message += "<br>" + helper.Encode(err.ErrorMessage);
                }
            }

            string Result = "<div class=\"ErrorSummary alert alert-danger alert-dismissable fade in\" style=\"display: none; \">" +
                            "<a class=\"close\" data-dismiss=\"alert\" aria-label=\"close\">&times;</a>" +
                            "<strong>Validation Error: </strong>" +
                            "<span class=\"Error\">" + Message + "</span></div>";
            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString PageHeader(this HtmlHelper Helper, string PageID, string CustomeTitle = null)
        {
            string Result = "";
            string Title = "Title Not Avialable on the Current Website Language";

            var SitemapList = new CMS().GetPageSitemap(Guid.Parse(PageID));
            int Count = SitemapList.Count - 2;
            if (SitemapList.Count > 0)
            {
                Title = (CustomeTitle == null) ? SitemapList[Count + 1].Description : CustomeTitle;
            }

            Result += "<div class=\"page--header\"><div class=\"page--banner\">" +
                            "<h2 id=\"PageTitle\">" + Title + "</h2>" +
                                    "<div id=\"sitemap\" class=\"sitemap \">" +
                                            "<div class=\"nav--content row\">" +
                                                    "<ul>";
            for (int i = 0; i <= Count; i++)
            {
                Result += "<li><a href=\"" + SitemapList[i].URL + "\">" + SitemapList[i].Description + "</a></li>";
            }
            Result += "<li class=\"SitemapLastNode\">" + Title + "</li>";
            Result += "</ul>" +
             "</div>" +
     "</div>" +
"</div>" +
"</div>";

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString BackToList(this HtmlHelper Helper, string Url)
        {
            return MvcHtmlString.Create("<a class=\"btn btn-link\" href=\"" + Url + "\">" + resxUIControls.BackToListPage + "</a>");
        }
        public static MvcHtmlString CloseModalButton(this HtmlHelper Helper)
        {
            return MvcHtmlString.Create("<input type=\"button\" id=\"btnClose\" class=\"btn btn-default SLAME valid\" data-dismiss=\"modal\" value=\"" + resxUIControls.Close + "\" >");
        }
        public static MvcHtmlString DoneLink(this HtmlHelper Helper)
        {
            var input = new TagBuilder("input");
            input.MergeAttribute("type", "button");
            input.MergeAttribute("value", resxUIControls.Done);
            input.AddCssClass("btn btn-success ActionControl IgnoreAYS");
            input.MergeAttribute("data-submittype", SubmitTypes.Update);
            input.MergeAttribute("onclick", "SubmitForm(this);");

            return MvcHtmlString.Create(input.ToString());
        }
        #endregion

        #region DataTable Filter Controls
        public static MvcHtmlString DatatableFilterControls(this HtmlHelper Helper, string DataTableID)
        {
            string Result = "";

            Result += "<input type=\"button\" value=\"" + @resxUIControls.Find + "\" class=\"btnFind btn btn-success\" id=\"btnFind\" onclick=\"ApplyFilter('" + DataTableID + "');\" />&nbsp;";
            Result += "<input type=\"button\" value=\"" + @resxUIControls.Close + "\" class=\"btnClose btn btn-default\" onclick=\"ToggleFilter('" + DataTableID + "');\" />&nbsp;";
            Result += "<input type=\"button\" value=\"" + @resxUIControls.ClearFilter + "\" class=\"btnClear btn btn-default\" onclick=\"ClearFilter('" + DataTableID + "');\" />&nbsp;";

            return MvcHtmlString.Create(Result);
        }
        #endregion

        #region Index DataTable Controls
        public static MvcHtmlString IndexDatatableFilter(this HtmlHelper Helper, string DataTableName)
        {
            string Result = "<button type=\"button\" title=\"Filter\" class=\"btnFilter btn btn-success\" onclick=\"ToggleFilter('" + DataTableName + "');\"><i class=\"fa fa-filter\"></i></button>";
            return MvcHtmlString.Create(Result);
        }



        public static MvcHtmlString IndexDatatableCreate(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Create New \" class=\"btnCreate btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fa fa-plus\"></i></button>";
            }

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString FieldDataTableCreateNoPermissionWithParms(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";


            Result = "<button type=\"button\" title=\"Add New Record\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-plus\"></i></button>";

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableCreateNoPermission(this HtmlHelper Helper, string Path)
        {
            string Result = "<button type=\"button\" title=\"Create New \" class=\"btnCreate btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fa fa-plus\"></i></button>";

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableCreateVote(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";


            Result = "<input type=\"button\" title=\"Create New \" value='Start Election' class=\"btnCreate btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"></input>";


            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableDelete(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Delete Selected Records\" class=\"btnDelete btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Delete\" data-submittype=\"Delete\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-trash-o\"></i></button>";
            }

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString FieldDataTableCreateBulkReleaseItem(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Add Multi Release Item(s)\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-address-card-o\"></i></button>";
            }
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableRetrieveBulkItems(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                //Result = "<input type=\"button\" title=\"Vote Selected Records\" class=\"btnDelete btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Vote\" data-submittype=\"Vote\" data-datatable=\"" + DataTableName + "\" value=\"Vote\" ></input>";
                Result = "<button type =\"button\" title=\"Retrieve Bulk Items\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-check-circle\"></i></button>";

            }

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableExchnageBulkItems(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type =\"button\" title=\"Transfer Items\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-exchange\"></i></button>";

            }

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableChangeStatusForBulkItems(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Change item status bulk\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-cube\"></i></button>";
            }
            return MvcHtmlString.Create(Result);
        }


        public static MvcHtmlString IndexDatatableReminderPendingConfirmationBulkItems(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                //Result = "<input type=\"button\" title=\"Vote Selected Records\" class=\"btnDelete btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Vote\" data-submittype=\"Vote\" data-datatable=\"" + DataTableName + "\" value=\"Vote\" ></input>";
                Result = "<button type=\"button\" title=\"Send Reminder for pending confirmation Selected Records\" class=\"btn btn-primary Confirm\" data-mode=\"All\" data-action=\"ReminderPendingConfirmationBulkItems\" data-submittype=\"ReminderPendingConfirmationBulkItems\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-bell\"></i></button>";

            }

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableReminderPendingStaffConfirmationBulkItems(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                //Result = "<input type=\"button\" title=\"Vote Selected Records\" class=\"btnDelete btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Vote\" data-submittype=\"Vote\" data-datatable=\"" + DataTableName + "\" value=\"Vote\" ></input>";
                Result = "<button type=\"button\" title=\"Send Reminder for pending confirmation Selected Records\" class=\"btn btn-primary Confirm\" data-mode=\"All\" data-action=\"ReminderStaffForConfirmationBulkItems\" data-submittype=\"ReminderPendingStaffConfirmationBulkItems\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-bell\"></i></button>";

            }

            return MvcHtmlString.Create(Result);
        }

      
        public static MvcHtmlString IndexDatatableReminderCustodianBulkItems(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                //Result = "<input type=\"button\" title=\"Vote Selected Records\" class=\"btnDelete btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Vote\" data-submittype=\"Vote\" data-datatable=\"" + DataTableName + "\" value=\"Vote\" ></input>";
                Result = "<button type=\"button\" title=\"Send Reminder for Selected Records\" class=\"btn btn-primary Confirm\" data-mode=\"All\" data-action=\"ReminderCustodianBulkItems\" data-submittype=\"ReminderCustodianBulkItems\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-hand-grab-o\"></i></button>";

            }

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableSendReminderForDelayItemsReturn(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Send Reminder for Selected Records\" class=\"btn btn-primary Confirm\" data-mode=\"All\" data-action=\"ReminderForDelayInReturnItemsToStock\" data-submittype=\"ReminderForDelayInReturnItemsToStock\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-bell\"></i></button>";
            }

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableConfirmReceivingConumableBulkItems(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                //Result = "<input type=\"button\" title=\"Vote Selected Records\" class=\"btnDelete btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Vote\" data-submittype=\"Vote\" data-datatable=\"" + DataTableName + "\" value=\"Vote\" ></input>";
                Result = "<button type=\"button\" title=\"Confirm  Selected Records\" class=\"btn btn-primary Confirm\" data-mode=\"All\" data-action=\"ConfirmReceivingConumableBulkItems\" data-submittype=\"ConfirmReceivingConumableBulkItems\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-hand-grab-o\"></i></button>";

            }

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableReminderStaffPendingBulkItems(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                
                Result = "<button type=\"button\" title=\"Send Reminder for pending confirmation Selected Records\" class=\"btn btn-primary Confirm\" data-mode=\"All\" data-action=\"ReminderStaffPendingBulkItems\" data-submittype=\"ReminderStaffPendingBulkItems\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-bell\"></i></button>";
            }

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableCreateRelease(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Create New \" class=\"btnCreate btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fa fa-cart-plus\"></i></button>";
            }

            return MvcHtmlString.Create(Result);
        }


        public static MvcHtmlString IndexDatatableVerifyItem(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                //Result = "<input type=\"button\" title=\"Vote Selected Records\" class=\"btnDelete btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Vote\" data-submittype=\"Vote\" data-datatable=\"" + DataTableName + "\" value=\"Vote\" ></input>";
                Result = "<button type =\"button\" title=\"Verify Item(s)\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-flag-checkered\"></i></button>";

            }

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableDeleteNoPermission(this HtmlHelper Helper, string DataTableName)
        {
            string Result = "<button type=\"button\" title=\"Delete Selected Records\" class=\"btnDelete btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Delete\" data-submittype=\"Delete\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-trash-o\"></i></button>";

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableRestor(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Restore Selected Records\" class=\"btnRestore btn btn-warning Confirm\" data-mode=\"All\" data-action=\"Restore\" data-submittype=\"Restore\" style=\"display:none;\" data-datatable=" + DataTableName + "><i class=\"fa fa-history\"></i></button>";
            }

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableRestorNoPermission(this HtmlHelper Helper, string DataTableName)
        {
            string Result = "<button type=\"button\" title=\"Restore Selected Records\" class=\"btnRestore btn btn-warning Confirm\" data-mode=\"All\" data-action=\"Restore\" data-submittype=\"Restore\" style=\"display:none;\" data-datatable=" + DataTableName + "><i class=\"fa fa-history\"></i></button>";

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableRefresh(this HtmlHelper Helper, string DataTableName)
        {
            string Result = "<button type=\"button\" title=\"Refresh\" class=\"btnRefresh btn btn-primary\" onclick=\"DataTableRefresh('" + DataTableName + "');\"><i class=\"fa fa-refresh\"></i></button>";
            return MvcHtmlString.Create(Result);
        }
        #endregion

        #region Field DataTable Controls
        public static MvcHtmlString FieldDataTableFilter(this HtmlHelper Helper, string DataTableName)
        {
            string Result = "<button type=\"button\" title=\"Filter\" class=\"btnFilter btn btn-sm btn-success\" onclick=\"ToggleFilter('" + DataTableName + "');\"><i class=\"fa fa-filter\"></i></button>";
            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString FieldDataTableCreate(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Add New Record\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-plus\"></i></button>";
            }
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString FieldDataTableUploadMedical(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Search Bulk Files\" class=\" Modal-Link  btn btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fas fa-tasks\"></i></button>";
            }
            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString FieldDataTableCreateBulk(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Add Multi Record\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-address-card-o\"></i></button>";
            }
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString FieldDataTableUpload(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Search Bulk Files\" class=\" Modal-Link  btn btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fas fa-tasks\">  Search Bulk Files</i></button>";
            }
            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString FieldDataTableUploadNoPermission(this HtmlHelper Helper, string Path)
        {
            string Result = "";
            Result = "<button type=\"button\" title=\"Upload New Files\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-upload\"></i></button>";
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString FieldDataTableUploadItem(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Upload  Document\" class=\" Modal-Link  btn btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fas fa-upload\"></i></button>";
            }
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString FieldDataTableUploadIDelegationDocument(this HtmlHelper Helper, string Path, string FactorsToken = null)
        {
            string Result = "";


            Result = "<button type=\"button\" title=\"Search Bulk Files\" class=\" Modal-Link  btn btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fas fa-upload\"></i></button>";

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString FieldDataTableCreateNoPermission(this HtmlHelper Helper, string Path)
        {
            string Result = "<button type=\"button\" title=\"Add New Record\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-plus\"></i></button>";
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString FieldDataTableCreateNoActionPermission(this HtmlHelper Helper, string Path)
        {
            string Result = "<button type=\"button\" title=\"Add New Record\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-plus\"></i></button>";
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString FieldDataTableCreateNoActionPermission(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            Result = "<button type=\"button\" title=\"Add New Record\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-plus\"></i></button>";

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString FieldDataTableRestDangerPayNoActionPermission(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            Result = "<button type=\"button\" title=\"Reset data to re entered again\" class=\"btnCreate btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-plus\"></i></button>";

            return MvcHtmlString.Create(Result);
        }


        public static MvcHtmlString FieldDataTableDelete(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Delete Selected Records\" class=\"btnDelete btn btn-sm btn-primary Confirm\" data-mode=\"All\" data-action=\"Delete\" data-submittype=\"" + SubmitTypes.Delete + "\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-trash-o\"></i></button>";
            }
            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString FieldDataTableDeleteNoPermission(this HtmlHelper Helper, string DataTableName)
        {
            string Result = "<button type=\"button\" title=\"Delete Selected Records\" class=\"btnDelete btn btn-sm btn-primary Confirm\" data-mode=\"All\" data-action=\"Delete\" data-submittype=\"" + SubmitTypes.Delete + "\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-trash-o\"></i></button>";

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString FieldDataTableRestore(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Restore Selected Records\" class=\"btnRestore btn btn-sm btn-warning Confirm\" data-mode=\"All\" data-action=\"Restore\" data-submittype=\"" + SubmitTypes.Restore + "\" style=\"display:none;\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-history\"></i></button>";
            }
            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString FieldDataTableRestoreNoPermission(this HtmlHelper Helper, string DataTableName)
        {
            string Result = "<button type=\"button\" title=\"Restore Selected Records\" class=\"btnRestore btn btn-sm btn-warning Confirm\" data-mode=\"All\" data-action=\"Restore\" data-submittype=\"" + SubmitTypes.Restore + "\" style=\"display:none;\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-history\"></i></button>";
            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString FieldDataTableRefresh(this HtmlHelper Helper, string DataTableName)
        {
            string Result = "<button type=\"button\" title=\"Refresh\" class=\"btnRefresh btn btn-sm btn-primary\" onclick=\"DataTableRefresh('" + DataTableName + "');\"><i class=\"fa fa-refresh\"></i></button>";
            return MvcHtmlString.Create(Result);
        }
        #endregion


        /// Create Button ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static MvcHtmlString CreateButton(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null)
        {
            return MvcHtmlString.Create(CreateButton(ActionIndex, AppGUID));
        }
        public static string CreateButton(int? ActionIndex, Guid? AppGUID)
        {
            if (!ActionIndex.HasValue || new CMS().HasAction(ActionIndex.Value, AppGUID.Value))
            {
                string Input = "<input class=\"btn btn-primary SLAME ActionControl\" data-submittype=\"Create\" onclick=\"SubmitForm(this); \" type=\"submit\" disabled=\"disabled\" value=\"Save Changes\">";
                return Input;
            }
            else
            {
                return string.Empty;
            }
        }
        public static MvcHtmlString CreateButtonSubmitNoPermission(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null)
        {
            return MvcHtmlString.Create(CreateButtonSubmitNoPermission(ActionIndex, AppGUID));
        }
        public static string CreateButtonSubmitNoPermission(int? ActionIndex, Guid? AppGUID)
        {

            string Input = "<input class=\"btn btn-primary SLAME ActionControl\" data-submittype=\"Create\" onclick=\"SubmitForm(this); \" type=\"submit\" disabled=\"disabled\" value=\"Submit\">";
            return Input;

        }

        public static MvcHtmlString CreateButtonNoPermission(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null)
        {
            return MvcHtmlString.Create(CreateButtonNoPermission(ActionIndex, AppGUID));
        }
        public static string CreateButtonNoPermission(int? ActionIndex, Guid? AppGUID)
        {

            string Input = "<input class=\"btn btn-primary SLAME ActionControl\" data-submittype=\"Create\" onclick=\"SubmitForm(this); \" type=\"submit\" disabled=\"disabled\" value=\"" + resxUIControls.SaveChanges + "\">";
            return Input;

        }
        public static MvcHtmlString CreateButtonNoPermission(this HtmlHelper Helper)
        {
            return MvcHtmlString.Create(CreateButtonNoPermission());
        }
        public static string CreateButtonNoPermission()
        {
            string Input = "<input class=\"btn btn-primary SLAME ActionControl\" data-submittype=\"Create\" onclick=\"SubmitForm(this); \" type=\"submit\" disabled=\"disabled\" value=\"" + resxUIControls.SaveChanges + "\" > ";
            return Input;
        }

        /// Create New Button ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static MvcHtmlString CreateNewButton(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string URL)
        {
            return MvcHtmlString.Create(CreateNewButton(ActionIndex, AppGUID, URL));
        }
        public static string CreateNewButton(int ActionIndex, Guid AppGUID, string URL)
        {
            if (new CMS().HasAction(ActionIndex, AppGUID))
            {
                string input = "<a class=\"btn btn-default ActionControl btnNewRecord\" href=\"" + URL + "\"><span class=\"fa fa-plus\" style=\"color: black; \"></span></a>";
                return input;
            }
            else
            {
                return string.Empty;
            }
        }

        public static MvcHtmlString PageHeaderWithParms(this HtmlHelper Helper, string PageID, string CustomeTitle = null, string PK = null, int step = 0)
        {
            string Result = "";
            string Title = "Title Not Avialable on the Current Website Language";

            var SitemapList = new CMS().GetPageSitemap(Guid.Parse(PageID));
            int Count = SitemapList.Count - 2;
            if (SitemapList.Count > 0)
            {
                Title = (CustomeTitle == null) ? SitemapList[Count + 1].Description : CustomeTitle;
            }

            Result += "<div class=\"page--header\"><div class=\"page--banner\">" +
                            "<h2 id=\"PageTitle\">" + Title + "</h2>" +
                                    "<div id=\"sitemap\" class=\"sitemap \">" +
                                            "<div class=\"nav--content row\">" +
                                                    "<ul>";
            for (int i = 0; i <= Count; i++)
            {
                if (step == i)
                {
                    Result += "<li><a href=\"" + SitemapList[i].URL + PK + "\">" + SitemapList[i].Description + "</a></li>";
                }
                else
                    Result += "<li><a href=\"" + SitemapList[i].URL + "\">" + SitemapList[i].Description + "</a></li>";
            }
            Result += "<li class=\"SitemapLastNode\">" + Title + "</li>";
            Result += "</ul>" +
             "</div>" +
     "</div>" +
"</div>" +
"</div>";

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString CreateNewButtonNoPermission(this HtmlHelper Helper, string URL)
        {
            return MvcHtmlString.Create(CreateNewButtonNoPermission(URL));
        }
        public static string CreateNewButtonNoPermission(string URL)
        {
            string input = "<a class=\"btn btn-default ActionControl btnNewRecord\" href=\"" + URL + "\"><span class=\"fa fa-plus\" style=\"color: black; \"></span></a>";
            return input;
        }

        /// Update Button ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// 
        public static MvcHtmlString UpdateButtonNoPermission(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null, string FactorsToken = null)
        {
            return MvcHtmlString.Create(UpdateButtonNoPermission(ActionIndex, AppGUID, FactorsToken));
        }
        public static string UpdateButtonNoPermission(int? ActionIndex, Guid? AppGUID, string FactorsToken = null)
        {

            string Input = "<input class=\"btn btn-success SLAME ActionControl\" data-submittype=\"Update\" onclick=\"SubmitForm(this); \" type=\"submit\" disabled=\"disabled\" value=\"Save Changes\">";
            return Input.ToString();

        }

      

        public static MvcHtmlString UpdateButton(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null, string FactorsToken = null)
        {
            return MvcHtmlString.Create(UpdateButton(ActionIndex, AppGUID, FactorsToken));
        }
        public static string UpdateButton(int? ActionIndex, Guid? AppGUID, string FactorsToken = null)
        {
            if (!ActionIndex.HasValue || new CMS().HasAction(ActionIndex.Value, AppGUID.Value, FactorsToken))
            {
                string Input = "<input class=\"btn btn-success SLAME ActionControl\" data-submittype=\"Update\" onclick=\"SubmitForm(this); \" type=\"submit\" disabled=\"disabled\" value=\"Save Changes\">";
                return Input.ToString();
            }
            else
            {
                return string.Empty;
            }
        }


        public static MvcHtmlString UpdateButtonNoPermission(this HtmlHelper Helper)
        {
            return MvcHtmlString.Create(UpdateButtonNoPermission());
        }
        public static string UpdateButtonNoPermission()
        {
            string Input = "<input class=\"btn btn-success SLAME ActionControl\" data-submittype=\"Update\" onclick=\"SubmitForm(this); \" type=\"submit\" disabled=\"disabled\" value=\"Save Changes\">";
            return Input.ToString();
        }





        /// Delete Button /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static MvcHtmlString DeleteButton(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null, string FactorsToken = null)
        {
            return MvcHtmlString.Create((DeleteButton(ActionIndex, AppGUID, FactorsToken)));
        }
        public static string DeleteButton(int? ActionIndex, Guid? AppGUID, string FactorsToken = null)
        {
            if (!ActionIndex.HasValue || new CMS().HasAction(ActionIndex.Value, AppGUID.Value, FactorsToken))
            {
                string Input = "<a class=\"btn btn-default pull-right Confirm ActionControl\" data-mode=\"Single\" data-submittype=\"Delete\"><span class=\"fa fa-trash-o\" style=\"color: black;\"></span></a>";
                return Input;
            }
            else
            {
                return string.Empty;
            }
        }

        public static MvcHtmlString SendButton(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null, string FactorsToken = null)
        {
            return MvcHtmlString.Create((SendButton(ActionIndex, AppGUID, FactorsToken)));
        }
        public static string SendButton(int? ActionIndex, Guid? AppGUID, string FactorsToken = null)
        {
            if (!ActionIndex.HasValue || new CMS().HasAction(ActionIndex.Value, AppGUID.Value, FactorsToken))
            {
                string Input = "<input class=\"btn btn-default pull-right ActionControl\" data-submittype=\"Send\" onclick=\"SubmitForm(this); \" type=\"submit\" value=\"Share\" >&nbsp";
                return Input;
            }
            else
            {
                return string.Empty;
            }
        }


        public static MvcHtmlString DeleteButtonNoPermission(this HtmlHelper Helper)
        {
            return MvcHtmlString.Create((DeleteButtonNoPermission()));
        }
        public static string DeleteButtonNoPermission()
        {
            string Input = "<a class=\"btn btn-default pull-right Confirm ActionControl\" data-mode=\"Single\" data-submittype=\"Delete\"><span class=\"fa fa-trash-o\" style=\"color: black;\"></span></a>";
            return Input;
        }


        /// Restore Button ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static MvcHtmlString RestoreButton(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null, string FactorsToken = null)
        {
            return MvcHtmlString.Create((RestoreButton(ActionIndex, AppGUID, FactorsToken)));
        }
        public static string RestoreButton(int? ActionIndex, Guid? AppGUID, string FactorsToken = null)
        {

            if (!ActionIndex.HasValue || new CMS().HasAction(ActionIndex.Value, AppGUID.Value, FactorsToken))
            {
                //Generate Random ID and give it control and use it to convert the form to read only.
                string RandomID = Guid.NewGuid().ToString();

                string Input = "<input id=\"" + RandomID + "\" class=\"btn btn-warning SLAME Confirm ActionControl\" data-mode=\"Single\" data-submittype=\"Restore\" type=\"submit\" value=\"Restore\">" +
                               "<script>ReadOnlyForm('" + RandomID + "'); </script>";

                return Input;
            }
            else
            {
                return string.Empty;
            }
        }


        public static MvcHtmlString RestoreButtonNoPermission(this HtmlHelper Helper)
        {
            return MvcHtmlString.Create((RestoreButtonNoPermission()));
        }
        public static string RestoreButtonNoPermission()
        {

            string RandomID = Guid.NewGuid().ToString();

            string Input = "<input id=\"" + RandomID + "\" class=\"btn btn-warning SLAME Confirm ActionControl\" data-mode=\"Single\" data-submittype=\"Restore\" type=\"submit\" value=\"Restore\">" +
                           "<script>ReadOnlyForm('" + RandomID + "'); </script>";

            return Input;
        }



        /// Restore Switch Button ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static MvcHtmlString RestoreSwitchButton(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null, string FactorsToken = null)
        {
            //no need for string version, the popup will be destroyed after the action done
            if (!ActionIndex.HasValue || new CMS().HasAction(ActionIndex.Value, AppGUID.Value, FactorsToken))
            {
                string Inputs = "<input type = \"button\" value=\"" + @resxUIControls.Restore + "\" class=\"ConfirmOnModal btn btn-warning \" />" +
                "<input type = \"button\" value=\"" + @resxUIControls.ConfirmRestore + "\" class=\"btn btn-warning SLAME ActionControl\" data-submittype=\"Restore\" onclick=\"SubmitForm(this);\" style=\"display:none;\" />" +
                "<input type = \"button\" class=\"btn btn-default CancelConfirmOnModal ActionControl\" value=\"" + resxUIControls.Cancel + "\" style=\"display:none;\" />";

                return MvcHtmlString.Create(Inputs.ToString());
            }
            else
            {
                return MvcHtmlString.Create(string.Empty);
            }
        }
        public static MvcHtmlString RestoreSwitchButtonNoPermission(this HtmlHelper Helper)
        {
            string Inputs = "<input type = \"button\" value=\"" + @resxUIControls.Restore + "\" class=\"ConfirmOnModal btn btn-warning \" />" +
                           "<input type = \"button\" value=\"" + @resxUIControls.ConfirmRestore + "\" class=\"btn btn-warning SLAME ActionControl\" data-submittype=\"Restore\" onclick=\"SubmitForm(this);\" style=\"display:none;\" />" +
                           "<input type = \"button\" class=\"btn btn-default CancelConfirmOnModal ActionControl\" value=\"" + resxUIControls.Cancel + "\" style=\"display:none;\" />";

            return MvcHtmlString.Create(Inputs.ToString());
        }



        /// Delete Switch Button ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static MvcHtmlString DeleteSwitchButton(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null, string FactorsToken = null)
        {
            //no need for string version, the popup will be destroyed after the action done
            if (!ActionIndex.HasValue || new CMS().HasAction(ActionIndex.Value, AppGUID.Value, FactorsToken))
            {
                string Inputs = "<a class=\"btn btn-default ConfirmOnModal pull-right ActionControl\"><span class=\"fa fa-trash-o\" style=\"color:#333;\"></span></a>" +
                "<input type = \"button\" value=\"" + @resxUIControls.ConfirmDelete + "\" class=\"btn btn-danger SLAME ActionControl\" data-submittype=\"Delete\" onclick=\"SubmitForm(this);\" style=\"display:none;\" disabled />" +
                "<input type = \"button\" class=\"btn btn-default CancelConfirmOnModal ActionControl\" value=\"" + resxUIControls.Cancel + "\" style=\"display:none;\" />";

                return MvcHtmlString.Create(Inputs.ToString());
            }
            else
            {
                return MvcHtmlString.Create(string.Empty);
            }
        }

        public static MvcHtmlString DeleteSwitchButtonNoPermission(this HtmlHelper Helper)
        {
            string Inputs = "<a class=\"btn btn-default ConfirmOnModal pull-right ActionControl\"><span class=\"fa fa-trash-o\" style=\"color:#333;\"></span></a>" +
            "<input type = \"button\" value=\"" + @resxUIControls.ConfirmDelete + "\" class=\"btn btn-danger SLAME ActionControl\" data-submittype=\"Delete\" onclick=\"SubmitForm(this);\" style=\"display:none;\" disabled />" +
            "<input type = \"button\" class=\"btn btn-default CancelConfirmOnModal ActionControl\" value=\"" + resxUIControls.Cancel + "\" style=\"display:none;\" />";

            return MvcHtmlString.Create(Inputs.ToString());
        }

        ///Import from Excel////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static MvcHtmlString FieldImportExcel(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Import Excel\" class=\"btnExcel Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-file-excel-o\"></i></button>";
            }
            return MvcHtmlString.Create(Result);
        }


        ///Vote Inviation////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static MvcHtmlString SendVotIvitationBrodcast(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Vote Brodcast\" class=\"btnExcel Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-envelope-square\"></i></button>";
            }
            return MvcHtmlString.Create(Result);
        }

        ///Vote Index////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static MvcHtmlString IndexDataTableVote(this HtmlHelper Helper, string DataTableName)
        {
            string Result = "";

            Result = "<input type=\"button\" title=\"Vote Selected Records\" class=\"btnDelete btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Vote\" data-submittype=\"Vote\" data-datatable=\"" + DataTableName + "\" value=\"Vote\" ></input>";

            return MvcHtmlString.Create(Result);
        }


        public static MvcHtmlString IndexDatatableSubmit(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Submit Selected Records\" class=\"btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Submit\" data-submittype=\"Submit\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-check-circle\"></i></button>";

            }

            return MvcHtmlString.Create(Result);
        }
        #region STI
        public static MvcHtmlString FieldDataTableSTIReport(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"STI Reports \" class=\"btnCreate btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fa fa-file\"></i></button>";

                return MvcHtmlString.Create(Result);
            }
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString FieldDataTableDownloadSTITemplate(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Download Entry Item Template \" class=\"btnCreate btn btn-danger\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fa fa-download\"></i></button>";

                return MvcHtmlString.Create(Result);
            }
            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString FieldDataTableDelayItems(this HtmlHelper Helper, string Path)
        {
            string Result = "<button type=\"button\" title=\"Show list of item(s) delay in return \" class=\"btnCreate btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fa fa-book\"></i></button>";

            return MvcHtmlString.Create(Result);
        }
        #endregion



        //Commented By Jawad
        //public static MvcHtmlString UpdateButtonNoPermission(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null, string FactorsToken = null)
        //{
        //    return MvcHtmlString.Create(UpdateButtonNoPermission(ActionIndex, AppGUID, FactorsToken));
        //}
        //public static string UpdateButtonNoPermission(int? ActionIndex, Guid? AppGUID, string FactorsToken = null)
        //{

        //    string Input = "<input class=\"btn btn-success SLAME ActionControl\" data-submittype=\"Update\" onclick=\"SubmitForm(this); \" type=\"submit\" disabled=\"disabled\" value=\"Save Changes\">";
        //    return Input.ToString();

        //}

        #region AHD
        public static MvcHtmlString IndexDatatableAddMissionAddNewMission(this HtmlHelper Helper, string Path)
        {
            string Result = "<button type=\"button\" title=\"Add New Mission \" class=\"btnCreate btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fa fa-plus\">Add New Mission</i></button>";

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableAddMissionItineraryOutboundDate(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            Result = "<button type=\"button\" title=\"Add Outbound Travel\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-plus\">Add Outbound travel</i></button>";

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableAddMissionItineraryTravelSegmentDate(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            Result = "<button type=\"button\" title=\"Add Travel Segment\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-plus\">Add travel segment</i></button>";

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableAddMissionItineraryReturnDate(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            Result = "<button type=\"button\" title=\"Add Return Travel\" class=\"btnCreate Modal-Link btn btn-sm btn-info\" data-url=\"" + Path + "\"\"><i class=\"fa fa-plus\">Add Return Travel</i></button>";

            return MvcHtmlString.Create(Result);
        }
     
        public static MvcHtmlString FieldDataTableCreateNewConfirmation(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Reset data\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fa fa-pinterest\"></i></button>";
            }
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString UpdateButtonWithNoPermission(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null, string FactorsToken = null)
        {
            return MvcHtmlString.Create(UpdateButtonWithNoPermission(ActionIndex, AppGUID, FactorsToken));
        }
        public static string UpdateButtonWithNoPermission(int? ActionIndex, Guid? AppGUID, string FactorsToken = null)
        {

            string Input = "<input class=\"btn btn-success SLAME ActionControl\" data-submittype=\"Update\" onclick=\"SubmitForm(this); \" type=\"submit\" disabled=\"disabled\" value=\"Save Changes\">";
            return Input.ToString();

        }
    
        public static MvcHtmlString FieldTransferFile(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Add New Record\" class=\"btnCreate Modal-Link btn btn-sm btn-primary\" data-url=\"" + Path + "\"\">Transfer File</button>";
            }
            return MvcHtmlString.Create(Result);
        }
        #endregion
        #region WMS
        public static MvcHtmlString FieldDataTableBulkUploadItems(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Upload Bulk Items\" class=\" Modal-Link  btn btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fas fa-tasks\"></i></button>";
            }
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString FieldDataTableSearchStaff(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Search Staff\" class=\"btnCreate btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fa fa-search\"></i></button>";

                return MvcHtmlString.Create(Result);
            }
            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString FieldDataTableSearch(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Search \" class=\"btnCreate btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fa fa-search\"></i></button>";

                return MvcHtmlString.Create(Result);
            }
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString FieldDataTableContact(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"STI Contacts \" class=\"btnCreate btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fa fa-phone\"></i></button>";

                return MvcHtmlString.Create(Result);
            }
            return MvcHtmlString.Create(Result);
        }
        #endregion
        #region DAS
        public static MvcHtmlString DASFieldDataTableBulkUploadFromFile(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Upload Bulk Items\" class=\" Modal-Link  btn btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fas fa-tasks\"></i>Upload From File</button>";
            }
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString DASFieldDataTableBulkUploadImages(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Upload Bulk Items\" class=\" Modal-Link  btn btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fas fa-tasks\"></i>Upload Images</button>";
            }
            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableReminderPendingConfirmationBulkFiles(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                //Result = "<input type=\"button\" title=\"Vote Selected Records\" class=\"btnDelete btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Vote\" data-submittype=\"Vote\" data-datatable=\"" + DataTableName + "\" value=\"Vote\" ></input>";
                Result = "<button type=\"button\" title=\"Send Reminder for pending confirmation Selected Records\" class=\"btn btn-primary Confirm\" data-mode=\"All\" data-action=\"ReminderPendingConfirmationBulkFiles\" data-submittype=\"ReminderPendingConfirmationBulkFiles\" data-datatable=\"" + DataTableName + "\"><i class=\"fas fa-user-clock\"> Send Reminder</i>  </button>";

            }

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableReminderReturnBulkFiles(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                //Result = "<input type=\"button\" title=\"Vote Selected Records\" class=\"btnDelete btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Vote\" data-submittype=\"Vote\" data-datatable=\"" + DataTableName + "\" value=\"Vote\" ></input>";
                Result = "<button type=\"button\" title=\"Send reminder to return files\" class=\"btn btn-primary Confirm\" data-mode=\"All\" data-action=\"ReminderReturnBulkFiles\" data-submittype=\"ReminderReturnBulkFiles\" data-datatable=\"" + DataTableName + "\"><i class=\"fas fa-user-check\">  Return Bulk Files </i></button>";

            }

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableRequestBulkPhysicalFiles(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type =\"button\" title=\"Request file(s)\" class=\"btn btn-primary Modal-Link  \" data-url=\"" + Path + "\"\"><i class=\"fas fa-file-import\">   Request File(s)</i></button>";


            }

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableTransferBulkPhysicalFiles(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type =\"button\" title=\"Transfer file(s)\" class=\"Modal-Link btn  btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fas fa-file-export\">  Transfer File(s)</i></button>";

            }

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableConfirmReceivingBulkPhysicalFiles(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                //<i class="fas fa-user-check"></i>
                Result = "<button type =\"button\" title=\"Confrim receiving file(s)\" class=\" Modal-Link btn  btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fas fa-user-check\">  Confrim Receiving File(s) </i></button>";

            }

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString IndexDatatableCancelTransferPhysicalFiles(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type =\"button\" title=\"Cancel transfer file(s)\" class=\"Modal-Link btn btn-danger\" data-url=\"" + Path + "\"\"><i class=\"fas fa-times\">  Cancel Transferred File(s)</i></button>";

            }

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableDASChangeFileLocation(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type =\"button\" title=\"Change file locations\" class=\" Modal-Link btn btn-primary\" data-url=\"" + Path + "\"\"><i class=\"fas fa-location-arrow\">  Change File Location</i></button>";

            }

            return MvcHtmlString.Create(Result);
        }
        public static MvcHtmlString FieldDataTableDASSearchOwnedFiles(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Owned files \" class=\" btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fas fa-user-check\">  Owned Files</i></button>";

                return MvcHtmlString.Create(Result);
            }
            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString FieldDataTableDASUploadBulkFiles(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";
            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Search bulk files \" class=\"btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fas fa-tasks\" >  Search Bulk Files</i></button>";

                return MvcHtmlString.Create(Result);
            }
            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString SearchFiles(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null)
        {
            return MvcHtmlString.Create(SearchFiles(ActionIndex, AppGUID));
        }
        public static string SearchFiles(int? ActionIndex, Guid? AppGUID)
        {
            if (!ActionIndex.HasValue || new CMS().HasAction(ActionIndex.Value, AppGUID.Value))
            {
                string Input = "<input class=\"btn btn-primary SLAME ActionControl\" data-submittype=\"Create\" onclick=\"SubmitForm(this); \" type=\"submit\" disabled=\"disabled\" value=\"Search\">";
                return Input;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion


        #region AHD
        public static MvcHtmlString RequestPermissionButton(this HtmlHelper Helper, int? ActionIndex = null, Guid? AppGUID = null)
        {
            return MvcHtmlString.Create(RequestPermissionButton(ActionIndex, AppGUID));
        }
        public static string RequestPermissionButton(int? ActionIndex, Guid? AppGUID)
        {
           
                string Input = "<input class=\"btn btn-primary SLAME ActionControl\" data-submittype=\"Create\" onclick=\"SubmitForm(this); \" type=\"submit\"  value=\"Request Permission\">";
                return Input;
          
        }
        public static MvcHtmlString IndexDatatableCreateNewRAndRLeaveRecord(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string Result = "";

            Result = "<button type=\"button\" title=\"Add New Record\" class=\"btn btn-primary Modal-Link \" data-url=\"" + Path + "\"\"><i class=\"fas fa-plus\">Add RR/Leave Dates</i></button>";


            return MvcHtmlString.Create(Result);
        }
        #endregion

        #region PMD
        public static MvcHtmlString IndexDatatableFilterPMD(this HtmlHelper Helper, string DataTableName)
        {
            string btnName = Languages.CurrentLanguage() == "AR" ? "بحث" : "Find";
            string Result = "<button type=\"button\" title=\"Filter\" class=\"btnFilter btn btn-success\" onclick=\"ToggleFilter('" + DataTableName + "');\"><i class=\"fa fa-filter\">" + btnName + "</i></button>";
            return MvcHtmlString.Create(Result);
        }



        public static MvcHtmlString IndexDatatableCreatePMD(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string Path, string FactorsToken = null)
        {
            string btnName = Languages.CurrentLanguage() == "AR" ? "إضافة" : "Add";

            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Create New \" class=\"btnCreate btn btn-primary\" onclick=\"window.location.href='" + Path + "'\"><i class=\"fa fa-plus\">" + btnName + "</i></button>";
            }

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableDeletePMD(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string btnName = Languages.CurrentLanguage() == "AR" ? "حذف" : "Delete";

            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Delete Selected Records\" class=\"btnDelete btn btn-primary Confirm\" data-mode=\"All\" data-action=\"Delete\" data-submittype=\"Delete\" data-datatable=\"" + DataTableName + "\"><i class=\"fa fa-trash-o\">" + btnName + "</i></button>";
            }

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableRestorPMD(this HtmlHelper Helper, int ActionIndex, Guid AppGUID, string DataTableName, string FactorsToken = null)
        {
            string btnName = Languages.CurrentLanguage() == "AR" ? "استعادة" : "Restore";
            string Result = "";

            if (new CMS().HasAction(ActionIndex, AppGUID, FactorsToken))
            {
                Result = "<button type=\"button\" title=\"Restore Selected Records\" class=\"btnRestore btn btn-warning Confirm\" data-mode=\"All\" data-action=\"Restore\" data-submittype=\"Restore\" style=\"display:none;\" data-datatable=" + DataTableName + "><i class=\"fa fa-history\">" + btnName + "</i></button>";
            }

            return MvcHtmlString.Create(Result);
        }

        public static MvcHtmlString IndexDatatableRefreshPMD(this HtmlHelper Helper, string DataTableName)
        {
            string btnName = Languages.CurrentLanguage() == "AR" ? "تحديث" : "Refresh";

            string Result = "<button type=\"button\" title=\"Refresh\" class=\"btnRefresh btn btn-primary\" onclick=\"DataTableRefresh('" + DataTableName + "');\"><i class=\"fa fa-refresh\">" + btnName + "</i></button>";
            return MvcHtmlString.Create(Result);
        }
        #endregion
    }
}