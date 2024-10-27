using Fair_Recruitment_Web_Result.Handlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Fair_Recruitment_Web_Result.Data;
using Microsoft.Extensions.Configuration;
using Fair_Recruitment_Web_Result.Auth;
using static Fair_Recruitment_Web_Result.Models.Database;
using System.Diagnostics;

namespace Fair_Recruitment_Web_Result.Controllers
{
    public class LoginController : Controller
    {
        private IConfiguration _configuration;
        private readonly CustomAuthentication _auth;
        private readonly ApplicationDbContext _db;
        public LoginController(IConfiguration configuration, CustomAuthentication auth, ApplicationDbContext db)
        {
            _configuration = configuration;
            _auth = auth;
            _db = db;
        }

        public class LoginModel
        {
            [Required(ErrorMessage = "Email is required")]
            [DataType(DataType.EmailAddress)]
            public string id { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public string password { get; set; }
        }

        [Route("/login")]
        [HttpPost]
        [AllowAnonymous] // Allow anonymous access to the login page
        public async Task<IActionResult> Login(string id, string pass)
        {
            //IActionResult response = Unauthorized();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            LoginAttempt loginAttempt = new LoginAttempt();
            loginAttempt.email = id;
            loginAttempt.password = pass;

            LoginOutput? loginOutput = new LoginOutput();
            if (!String.IsNullOrWhiteSpace(id) && !String.IsNullOrWhiteSpace(pass))
            {
                loginOutput = await _auth.Auth420(id, pass, HttpContext);
                loginAttempt.remarks = loginOutput.msg;
            }
            
            loginAttempt.created_at = DateTime.Now;
            loginAttempt.request_time = stopwatch.Elapsed.Milliseconds;
            stopwatch.Stop();

            if (loginOutput.loginResult != null && loginOutput.loginResult.success)
            {
                await StoreLoginAttempt(true, loginAttempt);
                return Ok(loginOutput.loginResult);
            }
            else
            {
                await StoreLoginAttempt(false, loginAttempt);
                return Unauthorized();
            }
        }

        private async Task StoreLoginAttempt(bool status , LoginAttempt loginAttempt)
        {
            try
            {
                loginAttempt.status = status;
                _db.LoginAttempts.Add(loginAttempt);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogHandler.WriteErrorLog(ex);
            }
        }


        [HttpPost("/logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/login");
            }
            catch (Exception ex)
            {
                LogHandler.WriteErrorLog(ex);
            }

            return Problem();
        }
    }
}
