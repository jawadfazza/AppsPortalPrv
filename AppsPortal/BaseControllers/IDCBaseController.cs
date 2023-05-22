using AppsPortal.Areas.PRG.Models;
using IDC_DAL.Model;

namespace AppsPortal.BaseControllers
{
    public class IDCBaseController : PortalBaseController
    {
        public IDCEntities DbIDC;
        public PRGEntities DbPRG;
        public IDCBaseController()
        {
            DbIDC = new IDCEntities();
            DbPRG = new PRGEntities();
        }
    }
}