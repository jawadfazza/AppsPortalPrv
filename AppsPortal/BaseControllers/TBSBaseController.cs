using TBS_DAL.Model;
using Z.EntityFramework.Extensions;

namespace AppsPortal.BaseControllers
{
    public class TBSBaseController : PortalBaseController
    {
        public TBSEntities DbTBS;
        public TBSBaseController()
        {
            DbTBS = new TBSEntities();
            EntityFrameworkManager.BulkOperationBuilder = builder => builder.BatchTimeout = 1800;
            DbTBS.BulkSaveChanges(options => options.BatchTimeout = 1800);
        }

    }
}