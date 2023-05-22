using PCR_DAL.Model;


namespace AppsPortal.BaseControllers
{
    public class PCRBaseController : PortalBaseController
    {
        public PCREntities DbPCR;
        public PCRBaseController()
        {
            DbPCR = new PCREntities();
        }
    }
}