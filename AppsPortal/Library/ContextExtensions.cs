using AppsPortal.Data;
using AppsPortal.Library;
using AppsPortal.Models;
using AppsPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace AppsPortal.Extensions
{
    public static class CTX
    {
        public static string GetTableName<T>(this DbContext context) where T : class
        {
            TableAndSchema tableAndSchema = GetTableAndSchema<T>(context);
            return tableAndSchema.TableName;
            //return GetTableNameWithSchema<T>(context).Replace("["+ schema + "].[", "").Replace("]", "");
        }

        //public static string GetTableNameWithSchema<T>(this DbContext context) where T : class
        //{
        //    ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;


        //    string sql = objectContext.CreateObjectSet<T>().ToTraceString();
        //    Regex regex = new Regex("FROM (?<table>.*) AS");
        //    Match match = regex.Match(sql);


        //    string table = match.Groups["table"].Value;
        //    return table;
        //}
        public class TableAndSchema
        {
            public string TableName { get; set; }
            public string SchemaName { get; set; }
        }
        public static TableAndSchema GetTableAndSchema<T>(this DbContext context) where T : class
        {
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
            string sql = objectContext.CreateObjectSet<T>().ToTraceString();
            var startTrim = sql.LastIndexOf("FROM") + 5;
            var initialTrim = sql.Substring(startTrim);
            var endTrim = initialTrim.IndexOf(" ");
            string tableAndSchema = sql.Substring(startTrim, endTrim).Replace("[", "").Replace("]", "");
            return new TableAndSchema
            {
                TableName = tableAndSchema.Split('.')[1].Trim(),
                SchemaName = tableAndSchema.Split('.')[0].Trim()
            };
        }

        public static string GetPKFieldName<T>(this DbContext context) where T : class
        {
            //Type TA = Convert.ChangeType(T, typeof(T))

            var objectSet = ((IObjectContextAdapter)context).ObjectContext.CreateObjectSet<T>();
            return objectSet.EntitySet.ElementType.KeyMembers.Select(k => k.Name).FirstOrDefault();
        }

        public static string GetRowVersionFieldName<T>(this DbContext context) where T : class
        {
            var objectSet = ((IObjectContextAdapter)context).ObjectContext.CreateObjectSet<T>();
            return GetTableName<T>(context) + "RowVersion";
        }

        public static List<string> GetClassPropertiesList<T>(this DbContext context) where T : class
        {
            var objectSet = ((IObjectContextAdapter)context).ObjectContext.CreateObjectSet<T>();
            return objectSet.EntitySet.ElementType.Members.Where(s => s.BuiltInTypeKind == BuiltInTypeKind.EdmProperty).Select(s => s.Name).ToList();
        }

        public static Guid GetPKValue<T>(this DbContext context, T Model) where T : class
        {
            string prop = context.GetPKFieldName<T>();
            Guid PK = (Guid)Model.GetType().GetProperty(prop).GetValue(Model, null);
            return PK;
        }

        public static PrimaryKeyControl PrimaryKeyControl<T>(this DbContext context, T Model) where T : class
        {
            //0 records to restore, there is no model ==> primary key is null ==> exception (Objecet reference not set to an instance ...)

            if (Model == null)
            {
                return new PrimaryKeyControl { ControlID = context.GetPKFieldName<T>(), Value = Guid.Empty.ToString() };
            }
            else
            {
                return new PrimaryKeyControl { ControlID = context.GetPKFieldName<T>(), Value = context.GetPKValue(Model).ToString() };

            }
        }

        public static List<PrimaryKeyControl> PrimaryKeyControls<T>(this DbContext context, List<T> Models) where T : class
        {
            //0 records to restore, there is no model ==> primary key is null ==> exception (Objecet reference not set to an instance ...)

            List<PrimaryKeyControl> PrimaryKeyControls = new List<PrimaryKeyControl>();
            foreach (var m in Models)
            {
                if (m != null)
                {
                    PrimaryKeyControl primaryKeyControl = context.PrimaryKeyControl(m);
                    PrimaryKeyControls.Add(primaryKeyControl);
                }




                //if (m == null)
                //{
                //    PrimaryKeyControls.Add(new PrimaryKeyControl { ControlID = context.GetPKFieldName<T>(), Value = Guid.Empty.ToString() });
                //}
                //else
                //{
                //    PrimaryKeyControls.Add(new PrimaryKeyControl { ControlID = context.GetPKFieldName<T>(), Value = context.GetPKValue(m).ToString() });

                //}
            }

            return PrimaryKeyControls;


        }

        public static List<RowVersionControl> RowVersionControls<T>(this DbContext context, List<T> Models) where T : class
        {
            if (Models == null) throw new Exception("Model must not be null"); //return new List<RowVersionControl>();

            List<RowVersionControl> Result = new List<RowVersionControl>();
            foreach (var m in Models)
            {
                if (m != null)
                {
                    Type t = m.GetType();
                    PropertyInfo[] props = t.GetProperties();
                    List<string> propNames = new List<string>();

                    var rvs = props.Where(p => p.Name.EndsWith("RowVersion")).ToList();
                    foreach (var rv in rvs)
                    {
                        Result.Add(new RowVersionControl
                        {
                            ControlID = rv.Name,
                            Value = Convert.ToBase64String(context.GetRowVersionValue(m, rv.Name))
                        });
                    }
                }
            }
            return Result;
        }

        public static List<RowVersionControl> RowVersionControls<T, U>(this DbContext context, T Model1, U Model2) where T : class where U : class
        {
            if (Model1 == null || Model2 == null) throw new Exception("Model must not be null"); //return new List<RowVersionControl>();

            List<RowVersionControl> Result = new List<RowVersionControl>();
            PropertyInfo[] props = null;

            props = Model1.GetType().GetProperties();
            Result.Add(new RowVersionControl
            {
                ControlID = props.Where(p => p.Name.EndsWith("RowVersion")).FirstOrDefault().Name,
                Value = Convert.ToBase64String(context.GetRowVersionValue(Model1, props.Where(p => p.Name.EndsWith("RowVersion")).FirstOrDefault().Name))
            });


            props = Model2.GetType().GetProperties();
            Result.Add(new RowVersionControl
            {
                ControlID = props.Where(p => p.Name.EndsWith("RowVersion")).FirstOrDefault().Name,
                Value = Convert.ToBase64String(context.GetRowVersionValue(Model2, props.Where(p => p.Name.EndsWith("RowVersion")).FirstOrDefault().Name))
            });

            return Result;
        }

        public static List<Guid> AffectedGuids<T>(this DbContext context, List<T> Models) where T : class
        {
            if (Models == null) return new List<Guid>();
            var ObjResult = (from r in Models select r.GetType().GetProperty(context.GetPKFieldName<T>()).GetValue(r, null)).ToList();
            List<Guid> Result = ObjResult.Select(g => Guid.Parse(g.ToString())).ToList();
            return Result;
        }

        public static byte[] GetRowVersionValue<T>(this DbContext context, T Model, string PropertyName) where T : class
        {
            byte[] RowVersion = (byte[])Model.GetType().GetProperty(PropertyName).GetValue(Model, null);
            return RowVersion;
        }

        public static void SetActive<T>(this DbContext context, T Model, bool Value) where T : class
        {
            //In case there is no Active Column in the table.
            try
            {
                Model.GetType().GetProperty("Active").SetValue(Model, Value);
            }
            catch { }
        }

        public static void SetPK<T>(this DbContext context, T Model, Guid Value) where T : class
        {
            Model.GetType().GetProperty(context.GetPKFieldName<T>()).SetValue(Model, Value);
        }

        public static bool GetActiveValue<T>(this DbContext context, T Model) where T : class
        {
            return (bool)Model.GetType().GetProperty("Active").GetValue(Model, null);
        }

        public static DateTime GetDeletedOnValue<T>(this DbContext context, T Model) where T : class
        {
            return (DateTime)Model.GetType().GetProperty("DeletedOn").GetValue(Model, null);
        }

        public static void SetDeletedOn<T>(this DbContext context, T Model, DateTime Value) where T : class
        {
            Model.GetType().GetProperty("DeletedOn").SetValue(Model, Value);
        }

        public static void Create<T>(this DbContext context, T model, Guid ActionGUID, DateTime ExecutionTime, CMSEntities DbCMS = null) where T : class
        {
            if (HasLanguageID(model) && model.GetType().GetProperty("LanguageID").GetValue(model, null) == null)
            {
                model.GetType().GetProperty("LanguageID").SetValue(model, Languages.CurrentLanguage());
            }

            Guid PK = context.GetPKValue(model);

            if (PK == Guid.Empty)
            {
                PK = Guid.NewGuid();
            }

            context.SetPK(model, PK);
            context.SetActive(model, true);
            if (DbCMS != null)
            {
                new Repository<T>(context, DbCMS).SetAuditActions(ActionGUID, PK, ExecutionTime);
            }
            else
            {
                new Repository<T>(context).SetAuditActions(ActionGUID, PK, ExecutionTime);
            }
            context.Set<T>().Add(model);
        }

        public static void CreateNoAudit<T>(this DbContext context, T model) where T : class
        {
            if (HasLanguageID(model) && model.GetType().GetProperty("LanguageID").GetValue(model, null) == null)
            {
                model.GetType().GetProperty("LanguageID").SetValue(model, Languages.CurrentLanguage());
            }

            Guid PK;

            if (context.GetPKValue(model) != Guid.Empty)
            {
                PK = context.GetPKValue(model);
            }
            else
            {
                PK = Guid.NewGuid();
            }

            context.SetPK(model, PK);
            context.SetActive(model, true);
            context.Set<T>().Add(model);
        }

        public static void CreateBulk<T>(this DbContext context, List<T> models, Guid ActionGUID, DateTime ExecutionTime, CMSEntities DbCMS = null) where T : class
        {
            List<dataAuditActions> dataAuditActions = new List<Models.dataAuditActions>();
            models.ForEach(s =>
            {
                Guid PK = context.GetPKValue(s);
                if (PK == Guid.Empty)
                {
                    context.SetPK(s, Guid.NewGuid());
                }

                context.SetActive(s, true);
                if (DbCMS != null)
                {
                    new Repository<T>(context, DbCMS).SetAuditActions(ActionGUID, PK, ExecutionTime);
                }
                else
                {
                    new Repository<T>(context).SetAuditActions(ActionGUID, PK, ExecutionTime);
                }

            });
            context.Set<T>().AddRange(models);
        }

        public static void CreateBulkNoAudit<T>(this DbContext context, List<T> models, CMSEntities DbCMS = null) where T : class
        {
            models.ForEach(s =>
            {
                Guid PK = context.GetPKValue(s);
                if (PK == Guid.Empty)
                {
                    context.SetPK(s, Guid.NewGuid());
                }

                context.SetActive(s, true);


            });
            context.Set<T>().AddRange(models);
        }

        public static void Update<T>(this DbContext context, T model, Guid ActionGUID, DateTime ExecutionTime, CMSEntities DbCMS = null) where T : class
        {
            //Think about the batch update scenario. 
            context.SetActive(model, true);
            if (DbCMS != null)
            {
                new Repository<T>(context, DbCMS).SetAuditFields(model, ActionGUID, ExecutionTime);
            }
            else
            {
                new Repository<T>(context).SetAuditFields(model, ActionGUID, ExecutionTime);
            }

        }

        public static void UpdateBulk<T>(this DbContext context, List<T> model, Guid ActionGUID, DateTime ExecutionTime, CMSEntities DbCMS = null) where T : class
        {
            foreach (var item in model)
            {
                context.SetActive(model, true);
                if (DbCMS != null)
                {
                    new Repository<T>(context, DbCMS).SetAuditFields(item, ActionGUID, ExecutionTime);
                }
                else
                {
                    new Repository<T>(context).SetAuditFields(item, ActionGUID, ExecutionTime);
                }
            }
        }

        public static void UpdateNoAudit<T>(this DbContext context, T model) where T : class
        {
            context.SetActive(model, true);
            context.Entry(model).State = EntityState.Modified;
        }

        public static T Delete<T>(this DbContext context, T Model, DateTime ExecutionTime, Guid ActionGUID, CMSEntities DbCMS = null) where T : class
        {
            //this to prevent delete record deleted by someone else already
            if (context.GetActiveValue(Model))
            {
                Guid PK = context.GetPKValue(Model);
                context.SetActive(Model, false);
                context.SetDeletedOn(Model, ExecutionTime);
                context.Entry(Model).State = EntityState.Modified;
                if (DbCMS != null)
                {
                    new Repository<T>(context, DbCMS).SetAuditActions(ActionGUID, PK, ExecutionTime);
                }
                else
                {
                    new Repository<T>(context).SetAuditActions(ActionGUID, PK, ExecutionTime);
                }

            }
            return Model;
        }

        public static T DeleteNoAudit<T>(this DbContext context, T Model, DateTime ExecutionTime) where T : class
        {
            //this to prevent delete record deleted by someone else already
            if (context.GetActiveValue(Model))
            {
                Guid PK = context.GetPKValue(Model);
                context.SetActive(Model, false);
                context.SetDeletedOn(Model, ExecutionTime);
                context.Entry(Model).State = EntityState.Modified;
            }
            return Model;
        }

        public static List<T> DeleteBulk<T>(this DbContext context, List<T> Model, DateTime ExecutionTime, Guid ActionGUID, CMSEntities DbCMS = null) where T : class
        {
            foreach (var item in Model)
            {
                //this to prevent delete record deleted by someone else already
                if (context.GetActiveValue(item))
                {
                    Guid recordPK = context.GetPKValue(item);
                    context.SetActive(item, false);
                    context.SetDeletedOn(item, ExecutionTime);
                    context.Entry(item).State = EntityState.Modified;
                    if (DbCMS != null)
                    {
                        new Repository<T>(context, DbCMS).SetAuditActions(ActionGUID, recordPK, ExecutionTime);
                    }
                    else
                    {
                        new Repository<T>(context).SetAuditActions(ActionGUID, recordPK, ExecutionTime);
                    }
                }
            }
            return Model;
        }

        public static T Restore<T>(this DbContext context, T Model, Guid DeleteActionGUID, Guid RestoreActionGUID, DateTime RestoringTime, CMSEntities DbCMS = null) where T : class
        {
            //this to prevent restore record restored by someone else already
            if (context.GetActiveValue(Model) == false)
            {
                Guid RecordPK = context.GetPKValue(Model);
                context.SetActive(Model, true);
                context.Entry(Model).State = EntityState.Modified;
                if (DbCMS != null)
                {
                    new Repository<T>(context, DbCMS).SetAuditActions(RestoreActionGUID, RecordPK, RestoringTime);
                }
                else
                {
                    new Repository<T>(context).SetAuditActions(RestoreActionGUID, RecordPK, RestoringTime);
                }

            }
            return Model;
        }

        public static string QueryBuilder<T>(this DbContext context, List<T> models, Guid ActionGUID, string actionType, string BaseQuery) where T : class
        {
            //////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////// THINK ABOUT JOIN INSTEAD OF IN STATEMENT ////////////////////////
            //////////////////////////////////////////////////////////////////////////////////////////

            TableAndSchema TableAndSchema = context.GetTableAndSchema<T>();
            string Query = "";
            string RowVersionFieldName = TableAndSchema.TableName + "RowVersion";
            // string RowVersionFieldName = context.GetRowVersionFieldName<T>();
            string PKFieldName = context.GetPKFieldName<T>();

            Guid UserGUID = new Portal().UserID();

            string ActiveBit = "0";
            if (actionType == SubmitTypes.Restore) { ActiveBit = "1"; }

            List<string> PKList = (from pk in models select pk.GetType().GetProperty(PKFieldName).GetValue(pk, null).ToString()).ToList();
            List<string> RVList = new List<string>();

            var ByteRVs = (from rv in models select rv.GetType().GetProperty(RowVersionFieldName).GetValue(rv, null)).ToList();
            foreach (byte[] item in ByteRVs)
            {
                RVList.Add("0x" + BitConverter.ToString(item, 0).Replace("-", ""));
            }

            string PKs = "'" + string.Join("','", PKList) + "'";
            string RVs = string.Join(",", RVList);

            //Delete Depends on Factor(s)
            if (BaseQuery != "")
            {
                var classProperties = context.GetClassPropertiesList<T>();
                //We need to check
                //RVs.Count() > 1 Means multiple record delete, we need to check for concurrencey, or it could be child where we dont need to check the RowVersion, If we are deleting a master do we really care if a child modified?!
                Query = "SELECT DISTINCT " + TableAndSchema.SchemaName + "." + TableAndSchema.TableName + "." + string.Join("," + TableAndSchema.SchemaName + "." + TableAndSchema.TableName + ".", classProperties) + " FROM " + TableAndSchema.SchemaName + "." + TableAndSchema.TableName + " ";
                Query += " INNER JOIN ( " + BaseQuery + ") AS Result ON " + TableAndSchema.SchemaName + "." + TableAndSchema.TableName + "." + PKFieldName + " = Result." + PKFieldName;
                Query += " INNER JOIN CMS.userPermissions ON Result.C2 = CMS.userPermissions.FactorsToken";
                Query += " WHERE (( " + "Result." + PKFieldName + " IN (" + PKs + ")";
                Query += RVList.Count() > 0 ? " AND " + "Result." + RowVersionFieldName + " IN (" + RVs + ")" +
                    " and CMS.userPermissions.ActionGUID='" + ActionGUID + "' and  CMS.userPermissions.Active=1 and CMS.userPermissions.UserProfileGUID='" + HttpContext.Current.Session[SessionKeys.UserProfileGUID] + "'  )" : " )";
                Query += " OR ( " + "Result." + PKFieldName + " IN (" + PKs + ") AND " + TableAndSchema.SchemaName + "." + TableAndSchema.TableName + ".Active = " + ActiveBit + " ))";
            }
            //No Factor needed
            else
            {
                Query = "SELECT * FROM " + TableAndSchema.SchemaName + "." + TableAndSchema.TableName;
                Query += " WHERE ( " + PKFieldName + " IN (" + PKs + ")";
                Query += RVList.Count() > 0 ? " AND " + RowVersionFieldName + " IN (" + RVs + ") )" : " )";
                Query += " OR ( " + PKFieldName + " IN (" + PKs + ") AND Active = " + ActiveBit + " )";
            }
            return Query;
        }

        public static bool HasLanguageID(object obj)
        {
            return obj.GetType().GetProperty("LanguageID") != null;
        }

        //public static bool IsValueExists<T>(this DbContext context,string Column, Object Model) where T:class
        //{
        //    string TableName = GetTableName<T>(context);
        //    string Value = Convert.ToString(Model.GetType().GetProperty(Column).GetValue(Model, null));

        //    string PKName = GetPKFieldName<T>(context);
        //    string PKValue = Convert.ToString(Model.GetType().GetProperty(PKName).GetValue(Model,null));

        //    string SQL = "SELECT * FROM " + TableName + " WHERE " + Column + " = '" + Value + "' AND " + PKName + " <> '" + PKValue + "'";
        //    var Records = context.Database.SqlQuery<T>(SQL).ToList();

        //    return Records.Count() > 0;
        //}

        //public static bool IsValueExists<T, U>(this DbContext context, string column, Object Model) where T : class where U : class
        //{
        //    string TableName = GetTableName<T>(context);

        //    string PKName = GetPKFieldName<T>(context);
        //    string PKValue = Convert.ToString(Model.GetType().GetProperty(PKName).GetValue(Model, null));

        //    string FKName = GetPKFieldName<U>(context);
        //    string FKValue = Convert.ToString(Model.GetType().GetProperty(FKName).GetValue(Model, null));

        //    string LanguageIDValue = Convert.ToString(Model.GetType().GetProperty("LanguageID").GetValue(Model, null));

        //    string SQL = "SELECT * FROM " + TableName + " WHERE LanguageID = '" + LanguageIDValue + "' AND " + FKName + " = '" + FKValue + "' AND " + PKName + " <> '" + PKValue + "' AND Active =1";
        //    var Records = context.Database.SqlQuery<T>(SQL).ToList();

        //    return Records.Count() > 0;
        //}
    }
}