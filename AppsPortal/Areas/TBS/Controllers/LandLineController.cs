using AppsPortal.BaseControllers;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TBS_DAL.Model;
using AppsPortal.Extensions;
using AppsPortal.Library;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace AppsPortal.Areas.TBS.Controllers
{
    public class LandLineController : TBSBaseController
    {




        public ActionResult CalculateLandLineBillsS()
        {
            return PartialView();
        }


    }
}