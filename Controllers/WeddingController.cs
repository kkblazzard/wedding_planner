using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using wedding_planner.Models;
using System.Linq;

namespace wedding_planner.Controllers
{
    public class WeddingController : Controller
    {
        private WPContext dbContext;
     
        // here we can "inject" our context service into the constructor
        public WeddingController(WPContext context)
        {
            dbContext = context;
        }
        // GET: /Home/
        [HttpGet]
        [Route("all_weddings")]
        public IActionResult All_Weddings()
        {
            // Validate login session
            if(HttpContext.Session.GetString("Login")==null || HttpContext.Session.GetString("Login")!="True")
            {   HttpContext.Session.SetString("login","False");
                return Redirect("/");
            }
            List<Wedding> allWeddings=dbContext.Wedding.ToList();
            foreach(var w in allWeddings){
                if(w.date < DateTime.Now.Date){
                    dbContext.Wedding.Remove(w);
                    dbContext.SaveChanges();
                }
            }
            ViewBag.allWeddings=dbContext.Wedding.Include(a=>a.RSVP).ThenInclude(u => u.User).ToList();
            ViewBag.id=HttpContext.Session.GetInt32("id");
            System.Console.WriteLine(ViewBag.id);
            foreach(var w in ViewBag.allWeddings){
                foreach(var e in w.RSVP){
            System.Console.WriteLine(e.UserId);}}
            return View("all_weddings");
        }
        [HttpGet]
        [Route("wedding/add_wedding")]
        public IActionResult Add_Wedding()
        {
            // Validate login session
            if(HttpContext.Session.GetString("Login")==null || HttpContext.Session.GetString("Login")!="True")
            {   HttpContext.Session.SetString("login","False");
                return Redirect("/");
            }
            ViewBag.id=HttpContext.Session.GetInt32("id");
            return View("add_wedding");
        }
        [HttpPost]
        [Route("wedding/Add")]
        public IActionResult Add(Wedding wedding)
        {
            // Validate login session
            if(HttpContext.Session.GetString("Login")==null || HttpContext.Session.GetString("Login")!="True")
            {   HttpContext.Session.SetString("login","False");
                return Redirect("/");
            }
            if(ModelState.IsValid)
            {
                dbContext.Wedding.Add(wedding);
                dbContext.SaveChanges();
                var a =dbContext.Wedding.Last();
                ViewBag.id=HttpContext.Session.GetInt32("id");
                return Redirect($"/wedding/{a.WeddingId}");
            }
            return View("Add_Wedding");
        }
        [HttpGet]
        [Route("wedding/{id}")]
        public IActionResult Wedding_Profile(int id)
            {
                // Validate login session
                if(HttpContext.Session.GetString("Login")==null || HttpContext.Session.GetString("Login")!="True")
            {   HttpContext.Session.SetString("login","False");
                return Redirect("/");
            }
            System.Console.WriteLine("entered profile+++++++++++++++++++++++++");
            System.Console.WriteLine(id);
            ViewBag.id=HttpContext.Session.GetInt32("id");
            var wedding=dbContext.Wedding.Include(a=>a.RSVP).ThenInclude(u => u.User).FirstOrDefault(w =>w.WeddingId==id);
            foreach(var i in wedding.RSVP){
            System.Console.WriteLine(i.UserId);}
            System.Console.WriteLine("User ID up++++++++++++++++++++++++++++");
            System.Console.WriteLine("CreatorID down++++++++++++++++++++++++++");
            System.Console.WriteLine(wedding.creatorId);
            System.Console.WriteLine(id);
            System.Console.WriteLine("++++++++wed id++++++++++++++++++++++++");
            ViewBag.wedid=id;

            return View("wedding_profile",wedding);
        }
        [HttpGet]
        [Route("delete/{id}")]
        public IActionResult Delete(int id)
        {
            // Validate login session
            if(HttpContext.Session.GetString("Login")==null || HttpContext.Session.GetString("Login")!="True")
            {   HttpContext.Session.SetString("login","False");
                return Redirect("/");
            }
            System.Console.WriteLine(id);
            System.Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++");
            var wedding=dbContext.Wedding.FirstOrDefault(w =>w.WeddingId==id);
            System.Console.WriteLine("");
            dbContext.Wedding.Remove(wedding);
            dbContext.SaveChanges();
            return RedirectToAction("All_Weddings");
        }

        [HttpPost]
        [Route("rsvp/{id}")]
        public IActionResult RSVP(RSVP rsvp, int id)
        {
            // Validate login session
            if(HttpContext.Session.GetString("Login")==null || HttpContext.Session.GetString("Login")!="True")
            {   HttpContext.Session.SetString("login","False");
                return Redirect("/");
            }
            System.Console.WriteLine("entered RSVP ADD+++++++++++++++++++++++++++++++++");
            System.Console.WriteLine(rsvp.WeddingId);
            System.Console.WriteLine(rsvp.UserId);
            dbContext.RSVP.Add(rsvp);
            dbContext.SaveChanges();
            return Redirect($"/Wedding/{id}");
        }
        [HttpPost]
        [Route("unrsvp/{id}")]
        public IActionResult UNRSVP(RSVP rsvp, int id)
        { 
            // Validate login session
            if(HttpContext.Session.GetString("Login")==null || HttpContext.Session.GetString("Login")!="True")
            {   HttpContext.Session.SetString("login","False");
                return Redirect("/");
            }

            System.Console.WriteLine("entered UNRSVP REMOVE+++++++++++++++++++++++++++++++++");
            RSVP unrsvp=dbContext.RSVP.Where(w =>w.WeddingId==id && w.UserId==rsvp.UserId).FirstOrDefault();
            rsvp.RsvpId=unrsvp.RsvpId;
            System.Console.WriteLine(rsvp);
            System.Console.WriteLine(unrsvp);
            System.Console.WriteLine("+++++++++++++++++++++++++++++++++++");
            dbContext.RSVP.Remove(unrsvp);
            dbContext.SaveChanges();
            return Redirect($"/Wedding/{id}");
        }
    }
}
