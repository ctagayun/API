using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using ConfArch.Data.Repositories;
using ConfArch.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConfArch.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserRepository userRepository;

        public AccountController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
 
        //When a request is made for login. The login page is returned
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        //When the LoginForm is submitted/posted the login action in 
        //the account controller with HTTP post attribute files
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            //first check if there's a user with his username and password
            var user = userRepository.GetByUsernameAndPassword(model.Username, model.Password);
            if (user == null)
                return Unauthorized(); //user is null return a 401

            //If there is build a list of claims. Claims are basically 
            //information about the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("role", user.Role), //this user has the role "admin"
                new Claim("FavoriteColor", user.FavoriteColor)
            };

            //When I have the list of "claims" I can create a claims identity
            //object. passing it in and specify the scheme name.
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            //The identity is used to create a ClaimsPrincipal
            var principal = new ClaimsPrincipal(identity);

            //We can then signin in the Principal.
            //This line of code actually creates and sets the session cookie 
            //for us. IsPersistent makes the cookie survive between browser 
            //sessions.  model.RememberLogin is the preference of of the user
            //as how persistent the cookie should be.
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, 
                new AuthenticationProperties { IsPersistent = model.RememberLogin });

            //Redirect to the root which will display the front page of the 
            //React application
            return Redirect("/");
        }

        //Logout endpoint will delete the cookie. It has the athorized 
        //attribute which means a requet to this enpoint must have a valid
        //session cookie.
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        //Logout GetUser will return the serialized claims. It has the authorize  
        //attribute which means a requet to this endpoint must have a valid
        //session cookie. It returns a srialized claim
        [Authorize]
        public IActionResult GetUser()
        {
            return new JsonResult(User.Claims.Select(c => new { Type=c.Type, Value=c.Value }));
        }
    }
}
