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

        [HttpPost]
        public ActionResult FillGap()
        {
            FillData fd = new FillData();
            fd.FillGapsInBD();
            return View("ShowData");
        }

        [HttpPost]
        public ActionResult FillDataBase(FormCollection collection)
        {
            FillData fd = new FillData(Convert.ToBoolean(collection["startNewDB"]));
            fd.FillStaffBD();
            return View("ShowData");
        }
    }
}