﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebTroChuyen.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/HomeController
        public ActionResult Index()
        {
            return View();
        }
    }
}