using PPA_DAL.Model;


namespace AppsPortal.BaseControllers
{
    public class PPABaseController : PortalBaseController
    {
        public PPAEntities DbPPA;
        public PPABaseController()
        {
            DbPPA = new PPAEntities();
        }
    }
}