using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppsPortal.Areas.PRG.Models
{
    using AppsPortal.Library;
    using AppsPortal.Models;
    using System;
    using System.Data.Entity.Core.EntityClient;
    using System.Data.SqlClient;

    public class ConnectionString
    {
        private ConnectionString() { }
        private static ConnectionString _ConsString = null;
        private String _String = null;
        public static string ConString
        {
            get
            {
                if (_ConsString == null)
                {
                    _ConsString = new ConnectionString { _String = ConnectionString.Connect() };
                    return _ConsString._String;
                }
                else return _ConsString._String;
            }
        }
        public static string Connect()
        {
            //Build an SQL connection string
            SqlConnectionStringBuilder sqlString = new SqlConnectionStringBuilder()
            {
                DataSource = "10.244.8.2", // Server name
                InitialCatalog = "ProGres",  //Database
                UserID = "reportuser",         //Username
                Password = "rptuser",  //Password

            };
            //Build an Entity Framework connection string
            EntityConnectionStringBuilder entityString = new EntityConnectionStringBuilder()
            {
                Provider = "System.Data.SqlClient",
                Metadata = "res://*/Areas.PRG.Models.PRGModel.csdl|res://*/Areas.PRG.Models.PRGModel.ssdl|res://*/Areas.PRG.Models.PRGModel.msl",
                ProviderConnectionString = sqlString.ToString()
            };
            return entityString.ConnectionString;
        }
        public static string Connect(string InitialCatalog)
        {
            if (HttpContext.Current.Session[SessionKeys.UserProfileGUID] != null)
            {
                Guid UserGUID = Guid.Parse(HttpContext.Current.Session[SessionKeys.UserGUID].ToString());
                CMSEntities DbCMS = new CMSEntities();
                var userProfiles = DbCMS.StaffCoreData.Where(x => x.UserGUID == UserGUID).FirstOrDefault();
                var connectDB = (from a in DbCMS.codeConnections.Where(x => x.InitialCatalog == InitialCatalog)
                                 join b in DbCMS.userConnections.Where(x => x.DefaultConnection) on a.ConnectionGUID equals b.ConnectionGUID
                                 select a).FirstOrDefault();
                if (connectDB == null)
                {
                    connectDB = DbCMS.codeConnections.Where(x => x.OrganizationsInstancesGUID == UNHCR.SyriaOrganizationInstanceGUID && x.DutyStationGUID == userProfiles.DutyStationGUID && x.InitialCatalog == InitialCatalog).FirstOrDefault();
                    if (connectDB == null)
                    {
                        //Defualt Connection
                        connectDB = DbCMS.codeConnections.Where(x => x.OrganizationsInstancesGUID.ToString() == "e156c022-ec72-4a5a-be09-163bd85c68ef" && x.DutyStationGUID.ToString() == "6d7397d6-3d7f-48fc-bfd2-18e69673ac92" && x.InitialCatalog == InitialCatalog).FirstOrDefault();

                    }
                }

                //Build an SQL connection string
                SqlConnectionStringBuilder sqlString = new SqlConnectionStringBuilder()
                {
                    DataSource = connectDB.DataSource, // Server name
                    InitialCatalog = connectDB.InitialCatalog,  //Database
                    UserID = connectDB.UserID,         //Username
                    Password = connectDB.Password,  //Password

                };
                //Build an Entity Framework connection string
                EntityConnectionStringBuilder entityString = new EntityConnectionStringBuilder()
                {
                    Provider = connectDB.Provider,
                    Metadata = connectDB.Metadata,
                    ProviderConnectionString = sqlString.ToString()
                };
                return entityString.ConnectionString;
            }
            else
            {
                return Connect();
            }
        }
    }
}