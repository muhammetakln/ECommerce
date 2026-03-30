using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Concretes.Dtos
{
    public class LoginDto
    {
        [Required,Display(Name = "Kullanıcı Adınız",Prompt ="Kullanıcı Adınız")]
        public string UserName { get; set; } = null!;
        [Required,DataType(DataType.Password),Display(Name = "Şifreniz",Prompt ="Şifreniz")]
        public string Password { get; set; } = null!;
        [Display(Name = "Sizi Hatırlayalım")]
        public bool RememberMe { get; set; }
    }
    public class RegisterDto
    {
        [Required, Display(Name = "Adınız", Prompt = "Adınız")]
        public string FirstName { get; set; } = null!;
        [Required, Display(Name = "Soyadınız", Prompt = "Soyadınız")]

        public string LastName { get; set; } = null!;
        [Required, Display(Name ="Adresiniz", Prompt = "Adresiniz")]
        public string Address { get; set; } = null!;
        [Required, Display(Name = "Şehir", Prompt = "Şehir")]
        public string City { get; set; } = null!;
        [Required, Display(Name = "İlçe", Prompt = "İlçe")]
        public string District { get; set; } = null!;
        [Required, Display(Name = "Kullanıcı Adınız", Prompt = "Kullanıcı Adınız")]
        public string UserName { get; set; } = null!;
        [Required, DataType(DataType.Password), Display(Name = "Şifreniz", Prompt = "Şifreniz")]

        public string Password { get; set; } = null!;
        [Required, DataType(DataType.Password), Display(Name = "Şifreniz Tekrar", Prompt = "Şifreniz Tekrar"), Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
        public string ConfirmPassword { get; set; }= null!;
        [EmailAddress,Required, Display(Name = "E-posta Adresiniz", Prompt = "E-posta Adresiniz")]
        public string Email { get; set; } = null!;

        
    }

}