//using dbc_Dave.Data;
//using dbc_Dave.Data.Models;
//using Microsoft.AspNetCore.Identity;

//namespace dbc_Dave.Services
//{
//    public class GetToken
//    {
//        private SignInManager<IdentityUser> _signInManager;

//        public GetToken(SignInManager<ApplicationUser> signinManager)
//        {
//            _signInManager = signinManager;
//        }

//        public async Task<object> Token(string username, string password)
//        {
//            // This doesn't count login failures towards account lockout
//            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
//            var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

//            if (result.Succeeded)
//            {
//                Console.WriteLine("hi");
//            }
//            return result;
//        }
//    }
//}
