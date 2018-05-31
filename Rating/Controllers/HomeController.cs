using Rating.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Rating.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            //FillData fd = new FillData();
            return View();
        }

        public ActionResult ShowData()
        {
            //FillData fd = new FillData();
            using ( var context = new IndividualContext())
            {
                var allProff = context.Professors.ToList();
                return View(allProff);
            }
            
        }

        public ActionResult FillGap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FillDataBase(FormCollection collection)
        {
            FillData fd = new FillData(Convert.ToBoolean(collection["startNewDB"]));
            return View("ShowData");
        }
    }
}