namespace IdentityManager.Models
{
    public class TwoFactorAuthenticationViewModel
    {
        //used to login
        public string Code { set; get; }

        //used to register/sign up
        public string Token { set; get; }
        public string QRCodeUrl { get; set; }
    }
}
