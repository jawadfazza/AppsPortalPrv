using EMT_DAL.Model;


namespace AppsPortal.BaseControllers
{
    public class EMTBaseController : PortalBaseController
    {
        public EMTEntities DbEMT;
        public EMTBaseController()
        {
            DbEMT = new EMTEntities();
        }
    }
}