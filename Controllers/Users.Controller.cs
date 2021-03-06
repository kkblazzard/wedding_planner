using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using wedding_planner.Models;
using Microsoft.AspNetCore.Identity;

namespace wedding_planner.Controllers
{
    public class UsersController : Controller
    {
        private WPContext dbContext;
        // here we can "inject" our context service into the constructor
        public UsersController(WPContext context)
        {
            dbContext = context;
        }
        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            HttpContext.Session.SetString("Login","False");

            return View();
        }

        [HttpPost]
        [Route ("register")]
        public IActionResult Register(User used )
        {System.Console.WriteLine("entered reg+++++++++++++++++++++++++++++++");
            //validating submission against models
            if(ModelState.IsValid)
            { System.Console.WriteLine("passed model validation++++++++++++++++++++++++++");
                //verifying the email address is not already in use
                if(dbContext.Users.Any(u=> u.email == used.email))
                {System.Console.WriteLine("failed email validation+++++++++++++++++++++++++++++++++++++");
                    //ading email / password error to display to user
                    ModelState.AddModelError("email", "Email already in use!");
                    return View("Index");
                }
                System.Console.WriteLine("everything passed hashing PW+++++++++++++++++++++++");
                //hasshing users password before saving to the database and saving to user instance
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                used.password = Hasher.HashPassword(used, used.password);
                //Save your user object to the database
                System.Console.WriteLine("Password Hashed adding to DB++++++++++++++");
                dbContext.Add(used);
                System.Console.WriteLine("Password Hashed saving to DB++++++++++++++");
                dbContext.SaveChanges();
                System.Console.WriteLine("created new user sending to success page++++++++++++++");
                HttpContext.Session.SetString("Login", "True");
                User user = dbContext.Users.FirstOrDefault(u => u.email == used.email);
                HttpContext.Session.SetInt32("id",user.UserId);

                return RedirectToAction("Success");
            }
            System.Console.WriteLine("modelstate is valid failed++++++++++++++++++++++++++++++++++++++++");
            return View("Index");
        }

        [HttpPost]
        [Route ("login")] //used to verify users info is valid and allow them to login to their profile
        public IActionResult Login(LoginUser userSubmission)
    {
        //validating agains models
        if(ModelState.IsValid) 
        {
            // If inital ModelState is valid, query for a user with provided email
            var userInDb = dbContext.Users.FirstOrDefault(u => u.email == userSubmission.Email);
            // If no user exists with provided email
            if(userInDb == null)
            {
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Index");
            }
            // Initialize hasher object
            var hasher = new PasswordHasher<LoginUser>();
            
            // varify provided password against hash stored in db
            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.password, userSubmission.Password);
            
            // result can be compared to 0 for failure
            if(result == 0)
            {   //adding error to display issue to user
                ModelState.AddModelError("Email", "Invalid Email/Password");
                return View("Index");
            }
            User user = dbContext.Users.FirstOrDefault(u => u.email == userSubmission.Email);
            HttpContext.Session.SetInt32("id",user.UserId);
            System.Console.WriteLine(user.UserId);
            System.Console.WriteLine("++++++++++++++++++++user.userID++++++++++++++++++++++++++++++");
            int id=(int)HttpContext.Session.GetInt32("id");
            System.Console.WriteLine(id);
            System.Console.WriteLine("++++++++++++++++++++++++++++++SessionExtensions id+++++++++++++++++++++++++++");
            HttpContext.Session.SetString("Login", "True");
            return RedirectToAction("Success");
        }
        return View("Index");
    }

        [HttpGet]
        [Route("edit")] //update form to change / correct any user info things
        public IActionResult Edit(User user)
        {   if(HttpContext.Session.GetString("Login")==null || HttpContext.Session.GetString("Login")!="True")
            {
                return RedirectToAction("Index");
            }
            
            int id=(int)HttpContext.Session.GetInt32("id");
            user.UserId=id;
            System.Console.WriteLine(user.UserId);

            return View();
        }

        [HttpPost]
        [Route("update")] //proccesses submitted updates to users profile
        public IActionResult Update(User update)
        
        { 
            int id=(int)HttpContext.Session.GetInt32("id");
            System.Console.WriteLine(id);
            update.UserId=id;
            dbContext.Users.Update(update);
            dbContext.SaveChanges(); //saves submitted updates to database
            
            return RedirectToAction("Success"); //Redirect to loggedin page
        }

        [HttpGet]
        [Route("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Route("success")]
        public IActionResult Success(int id)
        { 
            if(HttpContext.Session.GetString("Login")==null || HttpContext.Session.GetString("Login")!="True")
            {   HttpContext.Session.SetString("login","False");
                return RedirectToAction("Index");
            }
                
            return Redirect("/all_weddings");
        }
    }
}
