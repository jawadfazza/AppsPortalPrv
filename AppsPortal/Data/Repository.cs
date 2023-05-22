using AppsPortal.Data;
using AppsPortal.Extensions;
using AppsPortal.Library;
using AppsPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;

namespace AppsPortal.Data
{
    public class Repository<T> where T : class
    {
        private CMSEntities DbCMS;

        private DbContext DbAPP;

        private Guid UserGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.UserGUID].ToString());

        public Repository(DbContext DbCMS)
        {
            this.DbCMS = (CMSEntities)DbCMS;
            this.DbAPP = DbCMS;
        }

        public Repository(DbContext DbAPP, DbContext DbCMS)
        {
            this.DbCMS = (CMSEntities)DbCMS;
            this.DbAPP = DbAPP;
        }
        public Guid SetAuditActions(Guid ActionGUID, Guid PK, DateTime ExecutionTime)
        {
            string TableName = CTX.GetTableName<T>(DbAPP);

            dataAuditActions AuditAction = new dataAuditActions
            {
                AuditGUID = Guid.NewGuid(),
                TableName = TableName,
                UserProfileGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.UserProfileGUID].ToString()),
                RecordGUID = PK,
                ActionGUID = ActionGUID,
                ExecutionTime = ExecutionTime
            };

            DbCMS.dataAuditActions.Add(AuditAction);
            return AuditAction.AuditGUID;
        }

        public void SetAuditFields(T Model, Guid ActionGUID, DateTime ExecutionTime)
        {
            Guid PK = DbAPP.GetPKValue(Model);
            Guid ActionAuditGUID = Guid.Empty;
            string TableName = CTX.GetTableName<T>(DbAPP);

            List<dataAuditFields> AuditFields = new List<dataAuditFields>();
            var original = DbAPP.Set<T>().Find(PK);
            DbAPP.Entry(original).CurrentValues.SetValues(Model);
            bool Changed = false;
            var entry = DbAPP.Entry(original);
            var namesOfChangedProperties = entry.CurrentValues.PropertyNames.Where(p => entry.Property(p).IsModified && entry.Property(p).Name.EndsWith("RowVersion") == false && entry.Property(p).Name != "DeletedOn").ToList();

            //Check if at least there is one prop has been changed, so add action audit otherwise, don't do anything.
            if (namesOfChangedProperties.Count > 0)
            {
                ActionAuditGUID = SetAuditActions(ActionGUID, PK, ExecutionTime);
            }

            foreach (string prop in namesOfChangedProperties)
            {
                object originalValue = entry.GetDatabaseValues().GetValue<object>(prop);
                object currentValue = Model.GetType().GetProperty(prop).GetValue(Model, null);

                if (currentValue == null)
                {
                    currentValue = "";
                }

                AuditFields.Add(new dataAuditFields
                {
                    AuditFieldGUID = Guid.NewGuid(),
                    AuditGUID = ActionAuditGUID,
                    TableName = TableName,
                    FieldName = prop,
                    BeforeChange = Convert.ToString(originalValue),
                    AfterChange = Convert.ToString(currentValue),
                });
                Changed = true;
            }
            if (original != null)
            {
                DbAPP.Entry(original).State = EntityState.Detached;
                DbAPP.Entry(Model).State = EntityState.Modified;
            }
            if (Changed)//Means there is at least one field changed
            {
                DbCMS.dataAuditFields.AddRange(AuditFields);
            }
        }
    }
}
