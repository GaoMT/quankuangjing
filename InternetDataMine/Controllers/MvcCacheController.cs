using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InternetDataMine.Controllers
{
    public class MvcCacheController : Controller
    {
        //
        // GET: /MvcCache/

        [OutputCache(CacheProfile = "SqlDependencyCache")]
        public ActionResult Index()
        {
            ViewBag.CurrentTime = System.DateTime.Now;
            return View();
        }

    }
}
