using Core.Abstracts.IServices;
using Core.Concretes.Dtos;
using Core.Concretes.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Responses;

namespace Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Customer> userManager;
        private readonly SignInManager<Customer> signInManager;
        public AuthService(UserManager<Customer> userManager, SignInManager<Customer> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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

        public async Task<IResult> RegisterAsync(RegisterDto model)
        {
            try
            {
                // FirstName validasyonu
                if (string.IsNullOrWhiteSpace(model.FirstName))
                {
                    return Result.Failure("Ad gereklidir", 400);
                }

                // LastName validasyonu
                if (string.IsNullOrWhiteSpace(model.LastName))
                {
                    return Result.Failure("Soyad gereklidir", 400);
                }

                // UserName validasyonu
                if (string.IsNullOrWhiteSpace(model.UserName))
                {
                    return Result.Failure("Kullanıcı adı gereklidir", 400);
                }

                // Email validasyonu
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    return Result.Failure("Email gereklidir", 400);
                }

                // Password validasyonu
                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    return Result.Failure("Şifre gereklidir", 400);
                }

                // ConfirmPassword validasyonu
                if (string.IsNullOrWhiteSpace(model.ConfirmPassword))
                {
                    return Result.Failure("Şifre onayı gereklidir", 400);
                }

                // Şifreler eşleşiyor mu kontrol et
                if (model.Password != model.ConfirmPassword)
                {
                    return Result.Failure("Şifreler eşleşmiyor", 400);
                }

                // Şifre uzunluğu kontrol et
                if (model.Password.Length < 6)
                {
                    return Result.Failure("Şifre en az 6 karakter olmalıdır", 400);
                }

                // UserName var mı kontrol et
                var userNameExists = await userManager.FindByNameAsync(model.UserName);
                if (userNameExists != null)
                {
                    return Result.Failure("Bu kullanıcı adı zaten kullanılıyor", 409);
                }

                // Email var mı kontrol et
                var emailExists = await userManager.FindByEmailAsync(model.Email);
                if (emailExists != null)
                {
                    return Result.Failure("Bu email adresi zaten kayıtlı", 409);
                }

                // Yeni Customer oluştur
                var customer = new Customer
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Email = model.Email,
                    Address = model.Address,
                    City = model.City,
                    District = model.District,
                    EmailConfirmed = false
                };

                // Customer'ı oluştur
                var result = await userManager.CreateAsync(customer, model.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result.Failure($"Kayıt başarısız: {errors}", 400);
                }

                // Default role ekle
                await userManager.AddToRoleAsync(customer, "Customer");

                var response = new
                {
                    id = customer.Id,
                    firstName = customer.FirstName,
                    lastName = customer.LastName,
                    userName = customer.UserName,
                    email = customer.Email,
                    address = customer.Address,
                    city = customer.City,
                    district = customer.District
                };

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Kayıt sırasında hata: {ex.Message}", 500);
            }
        }
    }
}
