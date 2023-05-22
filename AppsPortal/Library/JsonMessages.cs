using AppsPortal.Extensions;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Script.Serialization;


namespace AppsPortal.Library
{
    public static class JsonMessages
    {
        // COMMON /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static JsonReturn ErrorMessage(this DbContext context, string Message)
        {
            JsonReturn jr = new JsonReturn()
            {

                Notify = new Notify { Type = MessageTypes.Error, Message = Message }
            };
            return jr;
        }

        public static JsonReturn SuccessMessage(this DbContext context, string Message)
        {
            JsonReturn jr = new JsonReturn()
            {
                Notify = new Notify { Type = MessageTypes.Success, Message = Message },
            };
            return jr;
        }

        public static JsonReturn ConcurrencyError<T>(this DbContext context, T Model, string Container) where T : class
        {
            if (context.GetActiveValue(Model))
            {
                JsonReturn jr = new JsonReturn()
                {
                    Concurrency = true,
                    dbModel = Model,
                    Notify = new Notify { Type = MessageTypes.Error, Message = "Concurrancy Occured" },
                    RowVersions = CTX.RowVersionControls(context, new List<T>() { Model })
                };
                return jr;
            }
            else
            {
                JsonReturn jr = new JsonReturn()
                {
                    Concurrency = true,
                    dbModel = Model,
                    PartialViews = new List<PartialViewModel>()
                    {
                        new PartialViewModel { Operation = "Unload", Container = Container }
                    },
                    RowVersions = CTX.RowVersionControls<T>(context, new List<T>() { Model }),
                    NextPageMode = "Restore",
                    Notify = new Notify { Type = MessageTypes.Error, Message = "Concurrancy Occured" }
                };
                return jr;
            }
        }


        public static JsonReturn ConcurrencyErrorTest<T>(this DbContext context, T Model, bool Active) where T : class
        {
            JsonReturn jr = new JsonReturn()
            {
                Concurrency = true,
                dbModel = Model,
                NextPageMode = Active ? "" : "Restore",
                Notify = new Notify { Type = MessageTypes.Error, Message = "Concurrancy Occured" }
            };
            return jr;
        }






        public static JsonReturn PermissionError(this DbContext context)
        {
            return new JsonReturn
            {
                Notify = new Notify
                {
                    Type = MessageTypes.Error,
                    Message = "you don't have the permission to Execute This Action."
                },
            };
        }

        public static JsonReturn RecordExists(this DbContext context)
        {
            JsonReturn jr = new JsonReturn()
            {
                Notify = new Notify { Type = MessageTypes.Error, Message = "You can't restore this record because it is already exists" }
            };
            return jr;
        }


        // CREATE /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static JsonReturn SingleCreateMessage(this DbContext context)
        {
            JsonReturn jr = new JsonReturn()
            {
                Notify = new Notify { Type = MessageTypes.Success, Message = "Record Added Successfully" }
            };
            return jr;
        }
        public static JsonReturn SingleCreateMessage(this DbContext context, string Callback, bool NotUsed)
        {
            JsonReturn jr = new JsonReturn()
            {
                CallbackFunction = Callback,
                Notify = new Notify { Type = MessageTypes.Success, Message = "Record Added Successfully" }
            };
            return jr;
        }
        public static JsonReturn SingleCreateMessage(this DbContext context, string DataTableName)
        {
            JsonReturn jr = new JsonReturn()
            {
                DataTable = DataTableName,
                Notify = new Notify { Type = MessageTypes.Success, Message = "Record Added Successfully" }
            };
            return jr;
        }

        public static JsonReturn SingleCreateMessage(this DbContext context, PrimaryKeyControl PrimaryKeyControl, List<RowVersionControl> RowVersionControls, List<PartialViewModel> PartialViews, string Callback = "", List<UIButtons> UIButtons = null)
        {
            JsonReturn jr = new JsonReturn()
            {
                PrimaryKey = PrimaryKeyControl,
                PartialViews = PartialViews,
                CallbackFunction = Callback,
                RowVersions = RowVersionControls,
                NextPageMode = "Update",
                UIButtons = UIButtons,
                Notify = new Notify { Type = MessageTypes.Success, Message = "Record Added Successfully" }
            };

            return jr;
        }

        public static JsonReturn SingleCreateMessage(this DbContext context, List<PrimaryKeyControl> PrimaryKeyControls, List<RowVersionControl> RowVersionControls, List<PartialViewModel> PartialViews, string Callback = "", List<UIButtons> UIButtons = null)
        {
            JsonReturn jr = new JsonReturn()
            {
                PrimaryKeys = PrimaryKeyControls,
                PartialViews = PartialViews,
                CallbackFunction = Callback,
                RowVersions = RowVersionControls,
                NextPageMode = "Update",
                UIButtons = UIButtons,
                Notify = new Notify { Type = MessageTypes.Success, Message = "Record Added Successfully" }
            };

            return jr;
        }
        public static JsonReturn SingleCreateMessage<T>(this DbContext context, T Model)
        {
            JsonReturn jr = new JsonReturn()
            {
                dbModel = Model,
                Notify = new Notify { Type = MessageTypes.Success, Message = "Record Added Successfully" }
            };
            return jr;
        }

        // UPDATE /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static JsonReturn SingleUpdateMessage(this DbContext context, string DataTableName = null, PrimaryKeyControl PrimaryKeyControl = null, List<RowVersionControl> rowVersions = null, string CallBack = "", string Message = "Record Updated Successfully")
        {
            JsonReturn jr = new JsonReturn()
            {
                DataTable = DataTableName,
                PrimaryKey = PrimaryKeyControl,
                RowVersions = rowVersions,
                CallbackFunction = CallBack,
                Notify = new Notify { Type = MessageTypes.Success, Message = Message }
            };
            return jr;
        }

        public static JsonReturn SingleUpdateMessage(this DbContext context, bool WithMultiPKs, string DataTableName = null, List<PrimaryKeyControl> PrimaryKeyControls = null, List<RowVersionControl> rowVersions = null, string CallBack = "", string Message = "Record Updated Successfully")
        {
            JsonReturn jr = new JsonReturn()
            {
                DataTable = DataTableName,
                PrimaryKeys = PrimaryKeyControls,
                RowVersions = rowVersions,
                CallbackFunction = CallBack,
                Notify = new Notify { Type = MessageTypes.Success, Message = Message }
            };
            return jr;
        }
        // DELETE /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static JsonReturn SingleDeleteMessage<T>(this DbContext context, List<T> Model, string DataTable, string CallBack = null) where T : class
        {
            if (Model == null)
            {
                throw new DbUpdateConcurrencyException();
            }
            else
            {
                JsonReturn jr = new JsonReturn()
                {
                    RowVersions = CTX.RowVersionControls<T>(context, new List<T>() { Model.First() }),
                    NextPageMode = "Restore",
                    AffectedRecordsGuids = context.AffectedGuids(Model),
                    DataTable = DataTable,
                    CallbackFunction = CallBack,
                    Notify = new Notify { Type = MessageTypes.Success, Message = "Record Deleted Successfully" }
                };
                return jr;
            }

        }

        public static JsonReturn SingleDeleteMessage<T>(this DbContext context, int CommitedRows, T Model, string Containers, List<UIButtons> UIButtons) where T : class
        {
            List<PartialViewModel> Partials = null;
            if (Containers != null)
            {
                Partials = new List<PartialViewModel>();
                string[] Array = Containers.Split(',');
                foreach (string arr in Array)
                {
                    Partials.Add(new PartialViewModel { Operation = "Delete", Container = arr });
                }

            }

            if (Model == null)
            {
                throw new DbUpdateConcurrencyException();
            }
            else if (CommitedRows == 0)
            {
                JsonReturn jr = new JsonReturn()
                {
                    PartialViews = Partials,
                    RowVersions = CTX.RowVersionControls<T>(context, new List<T>() { Model }),
                    NextPageMode = "Restore",
                    UIButtons = UIButtons,
                    Notify = new Notify { Type = MessageTypes.Error, Message = "The record Deleted By Some else." }
                };
                return jr;
            }
            else
            {
                JsonReturn jr = new JsonReturn()
                {
                    PartialViews = Partials,
                    RowVersions = CTX.RowVersionControls<T>(context, new List<T>() { Model }),
                    NextPageMode = "Restore",
                    UIButtons = UIButtons,
                    Notify = new Notify { Type = MessageTypes.Success, Message = "Record Deleted Successfully" },
                };
                return jr;
            }
        }


        public static JsonReturn PartialDeleteMessage<T>(this DbContext context, List<T> Affected, List<T> Selected, string DataTableName) where T : class
        {
            string Type = "";
            string Message = "";

            if (Affected.Count == Selected.Count && Selected.Count > 1) //All => All (Green)
            {
                Type = MessageTypes.Success;
                Message = "Selected Records Deleted Successfully";
            }
            else if (Affected.Count < Selected.Count && Selected.Count > 1) //All => Partial (Orange)
            {
                Type = MessageTypes.Warning;
                Message = string.Format("Only {0} of {1} Record(s) Deleted Successfully, the not Deleted Records could be deleted already by someone else or you don't have the permission to delete them.", Affected.Count, Selected.Count);
            }
            else if (Selected.Count > 1 && Affected.Count == 0) //All => Zero (Red)
            {
                Type = MessageTypes.Error;
                Message = "The selected records could not be deleted because it could be deleted, modifed or you don't have permission to delete them";
            }
            else if (Affected.Count == Selected.Count && Selected.Count == 1) //One => One (Green)
            {
                Type = MessageTypes.Success;
                Message = "Selected Record Deleted Successfully";
            }
            else if (Selected.Count() == 1 && Affected.Count() == 0) //One => Zero (Red) 
            {
                Type = MessageTypes.Error;
                Message = "The selected record could be deleted by someone else already or you don't have permission to delete it";
            }

            JsonReturn jr = new JsonReturn()
            {
                Notify = new Notify
                {
                    Type = Type,
                    Message = Message
                },
                AffectedRecordsGuids = context.AffectedGuids(Affected),
                DataTable = DataTableName
            };
            return jr;

        }

        public static JsonReturn PartialDeleteMessage(this DbContext context, List<Guid> Affected)
        {

            JsonReturn jr = new JsonReturn()
            {
                Notify = new Notify
                {
                    Type = MessageTypes.Success,
                    Message = "Selected Records Deleted Successfully"
                },
                DeleteClientSide = Affected
            };
            return jr;
        }


        public static JsonReturn PartialDeleteMessage<T>(this DbContext context, List<T> Affected, List<T> Selected, string DataTableName, string Callback) where T : class
        {
            string Type = "";
            string Message = "";

            if (Affected.Count == Selected.Count && Selected.Count > 1) //All => All (Green)
            {
                Type = MessageTypes.Success;
                Message = "Selected Records Deleted Successfully";
            }
            else if (Affected.Count < Selected.Count && Selected.Count > 1) //All => Partial (Orange)
            {
                Type = MessageTypes.Warning;
                Message = string.Format("Only {0} of {1} Record(s) Deleted Successfully, the not Deleted Records could be deleted already by someone else or you don't have the permission to delete them.", Affected.Count, Selected.Count);
            }
            else if (Selected.Count > 1 && Affected.Count == 0) //All => Zero (Red)
            {
                Type = MessageTypes.Error;
                Message = "The selected records could not be deleted because it could be deleted, modifed or you don't have permission to delete them";
            }
            else if (Affected.Count == Selected.Count && Selected.Count == 1) //One => One (Green)
            {
                Type = MessageTypes.Success;
                Message = "Selected Record Deleted Successfully";
            }
            else if (Selected.Count() == 1 && Affected.Count() == 0) //One => Zero (Red) 
            {
                Type = MessageTypes.Error;
                Message = "The selected record could be deleted by someone else already or you don't have permission to delete it";
            }

            JsonReturn jr = new JsonReturn()
            {
                Notify = new Notify
                {
                    Type = Type,
                    Message = Message
                },
                CallbackFunction = Callback,
                AffectedRecordsGuids = context.AffectedGuids(Affected),
                DataTable = DataTableName
            };
            return jr;

        }

        // RESTORE /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static JsonReturn SingleRestoreMessage<T>(this DbContext context, List<T> AffectedRecord, string DataTableName) where T : class
        {
            JsonReturn jr = new JsonReturn()
            {
                NextPageMode = "Update",
                DataTable = DataTableName,
                AffectedRecordsGuids = context.AffectedGuids(AffectedRecord),
                Notify = new Notify { Type = MessageTypes.Success, Message = "Record Restored Successfully" }
            };
            return jr;
        }

        public static JsonReturn SingleRestoreMessage<T>(this DbContext context, int CommitedRows, List<T> Models, PrimaryKeyControl PrimaryKeyControl, string URL, string Container, List<UIButtons> UIButtons = null) where T : class
        {
            if (Models == null)
            {
                throw new DbUpdateConcurrencyException();
            }
            else if (CommitedRows == 0)
            {
                JsonReturn jr = new JsonReturn()
                {
                    UIButtons = UIButtons,
                    Notify = new Notify { Type = MessageTypes.Error, Message = "Record Restored By Someone Else" }
                };
                return jr;
            }
            else
            {
                List<PartialViewModel> Partials = null;
                if (Container != null)
                {
                    Partials = new List<PartialViewModel>();
                    string[] Containers = Container.Split(',');
                    string[] Urls = URL.Split(',');
                    for (int i = 0; i < Containers.Length; i++)
                    {
                        Partials.Add(new PartialViewModel { PK = PrimaryKeyControl.Value, Url = Urls[i], Container = Containers[i] });
                    }
                }
                JsonReturn jr = new JsonReturn()
                {
                    RowVersions = context.RowVersionControls(Models),
                    AffectedRecordsGuids = context.AffectedGuids(Models),
                    NextPageMode = "Update",
                    PartialViews = Partials,
                    //PartialViews = new List<PartialViewModel>(){
                    //new PartialViewModel { PK = PrimaryKeyControl.Value , Url =URL, Container = Container }
                    UIButtons = UIButtons,
                    Notify = new Notify { Type = MessageTypes.Success, Message = "Record Restored Successfully" }
                };
                return jr;
            }
        }

        public static JsonReturn PartialRestoreMessage<T>(this DbContext context, List<T> Affected, List<T> Selected, string DataTableName) where T : class
        {
            string Type = "";
            string Message = "";

            if (Affected.Count == Selected.Count && Selected.Count > 1) //All => All (Green)
            {
                Type = MessageTypes.Success;
                Message = "Selected Records Restored Successfully";
            }
            else if (Affected.Count < Selected.Count && Selected.Count > 1) //All => Partial (Orange)
            {
                Type = MessageTypes.Warning;
                Message = string.Format("Only {0} of {1} Record(s) restored successfully, the not restored selected records could be exist, restored already by someone else or you don't have the permission to restore them.", Affected.Count, Selected.Count);
            }
            else if (Selected.Count > 1 && Affected.Count == 0) //All => Zero (Red)
            {
                Type = MessageTypes.Error;
                Message = "The selected records could not be restord because it could be exists, restord or you don't have permission to restore them";
            }
            else if (Affected.Count == Selected.Count && Selected.Count == 1) //One => One (Green)
            {
                Type = MessageTypes.Success;
                Message = "Selected Record Restored Successfully";
            }
            else if (Selected.Count() == 1 && Affected.Count() == 0) //One => Zero (Red) 
            {
                Type = MessageTypes.Error;
                Message = "The selected record could be exist, restored by someone else already or you don't have permission to restore it";
            }

            JsonReturn jr = new JsonReturn()
            {
                AffectedRecordsGuids = context.AffectedGuids(Affected),
                DataTable = DataTableName,
                Notify = new Notify { Type = Type, Message = Message }
            };
            return jr;
        }
    }
}