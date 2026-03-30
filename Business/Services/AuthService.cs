using AutoMapper;
using Core.Abstracts.IServices;
using Core.Concretes.Dtos;
using Core.Concretes.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Utils.Responses;

namespace Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Customer> userManager;
        private readonly SignInManager<Customer> signInManager;
        private readonly IMapper mapper;

        public AuthService(UserManager<Customer> userManager, SignInManager<Customer> signInManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }

        public async Task<IResult> LoginAsync(LoginDto model)
        {
            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                var user = await userManager.FindByNameAsync(model.UserName);
               

                return Result.Success();
            }
            else if (result.IsLockedOut)
            {
                return Result.Failure("Hesabınız kilitlenmiş. Lütfen daha sonra tekrar deneyin", 429);
            }
            else if (result.IsNotAllowed)
            {
                return Result.Failure("Giriş Başarısız", 400);
            }
            else if (result.RequiresTwoFactor)
            {
                return Result.Failure("İki faktörlü doğrulama gerekli", 401);
            }
            else
            {
                return Result.Failure("Kullanıcı adı veya şifre yanlış", 401);
            }
        }

        public Task LogoutAsync()
        {
            return signInManager.SignOutAsync();
        }

        /// <summary>
        /// Kullanıcı kayıt işlemini gerçekleştiren asenkron metod
        /// </summary>
        public async Task<IResult> RegisterAsync(RegisterDto model)
        {
            // RegisterDto nesnesini Customer entity'sine çevirme (mapping)
            var costumer = mapper.Map<Customer>(model);

            // Yeni kullanıcıyı veritabanına şifre ile birlikte ekle
            var result = await userManager.CreateAsync(costumer, model.Password);

            // Kayıt işlemi başarılı olduysa
            if (result.Succeeded)
            {
                // Başarı yanıtı döndür
                return Result.Success();
            }
            else
            {
                // Hata listesini çıkarıp hata yanıtı döndür
                return Result.Failure(result.Errors.Select(e => e.Description));
            }
        }
    }
}
