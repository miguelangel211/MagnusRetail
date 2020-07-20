using System;
namespace CheckstoresMagnusRetail.ViewModels
{
    public class LoginModel:BaseViewModel
    {
        public string user;
        public string Usuario { get { return user; } set { user = value; if (string.IsNullOrEmpty(value)) { Errorusuario = true; }
                else { Errorusuario = false; }
                OnPropertyChanged(); } }
        public string pass;
        public string Password { get { return pass; } set { pass = value; if (string.IsNullOrEmpty(value)) { Errorpassword = true; }
                else { Errorpassword = false; }
                OnPropertyChanged(); }
        }

        public bool erroruser;
        public bool Errorusuario { get { return erroruser; } set { erroruser = value;OnPropertyChanged(); } }

        public bool errorpass;
        public bool Errorpassword { get { return errorpass; } set { errorpass = value; OnPropertyChanged(); } }
        public LoginModel()
        {
        }
    }
}
