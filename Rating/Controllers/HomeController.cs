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
            return View();
        }
    }
}