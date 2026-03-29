using Business.Profiles;
using Business.Services;
using Core.Abstracts;
using Core.Abstracts.IServices;
using Core.Concretes.Entities;
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Business
{
    /// <summary>
    /// IOC (Inversion of Control / Dependency Injection) konfigürasyonu için yardımcı sınıf.
    /// Uygulamanın başlangıcında tüm servisleri (DbContext, Identity, AutoMapper, Business Services vb.) 
    /// dependency injection konteynerine kaydetmek için kullanılır.
    /// 
    /// Bu sınıf extension method içerdiği için Startup.cs veya Program.cs'te şu şekilde çağrılır:
    /// builder.Services.AddCustomServices(configuration);
    /// </summary>
    public static class IOC
    {
        /// <summary>
        /// Tüm custom servisleri (veritabanı, kimlik doğrulama, mapping, business services) 
        /// Microsoft Dependency Injection konteynerine kaydeder.
        /// 
        /// Bu method IServiceCollection'a extension method olarak eklenir, böylece Startup/Program sınıfında 
        /// services.AddCustomServices(configuration) şeklinde çağrılabilir.
        /// 
        /// Kayıt edilen servisler:
        /// 1. ShopContext (DbContext) - SQLite veritabanı bağlantısı
        /// 2. ASP.NET Core Identity - Kullanıcı kimlik doğrulaması ve yetkilendirme
        /// 3. AutoMapper - DTO ve Entity mapping (dönüştürme)
        /// 4. IUnitOfWork - Veritabanı işlem yönetimi (Scoped)
        /// 5. IShowroomService - Showroom business logic (Scoped)
        /// </summary>
        /// <param name="services">IServiceCollection örneği. Servisleri bu konteynerine ekleriz</param>
        /// <param name="configuration">Uygulama konfigürasyonu. Veritabanı connection string'i burada tutulur</param>
        /// <returns>IServiceCollection örneği. Metod zincirlemesi (method chaining) için geri döndürülür</returns>
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            // ============ VERITABANI BAĞLANTISI ============
            /// <summary>
            /// Entity Framework Core ile SQLite veritabanı bağlantısını ayarlar.
            /// Connection string'i appsettings.json dosyasından "shop_db" adıyla alınır.
            /// </summary>
            services.AddDbContext<ShopContext>(opt => opt.UseSqlite(configuration.GetConnectionString("shop_db")));


            // ============ KİMLİK DOĞRULAMA (IDENTITY) ============
            /// <summary>
            /// ASP.NET Core Identity'yi yapılandırır:
            /// - Customer: Kullanıcı entity'si. "Customer" tablosunda saklanacak
            /// - IdentityRole: Rol entity'si. Kullanıcıların Admin, User vb. rollerine sahip olmasını sağlar
            /// - AddEntityFrameworkStores&lt;ShopContext&gt;: Identity veritabanı tabloları ShopContext'te saklanır
            /// - AddDefaultTokenProviders: Şifre sıfırlama, e-posta doğrulama vb. token üretimi için gerekli
            /// </summary>
            services.AddIdentity<Customer, IdentityRole>()
                    .AddEntityFrameworkStores<ShopContext>()
                    .AddDefaultTokenProviders();


            // ============ AUTOMAPPER YAPLANDIRMASI ============
            /// <summary>
            /// AutoMapper'ı başlatır ve mapping profillerini kaydetir.
            /// ShowroomProfiles: DTO ↔ Entity dönüşümleri için oluşturulmuş profile
            /// 
            /// Örn: CreateShowroomDto → Showroom entity'sine otomatik dönüştürme
            /// </summary>
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ShowroomProfiles>();
            });


            // ============ BUSINESS SERVICES KAYITLARI ============
            /// <summary>
            /// AddScoped: Her HTTP isteği için yeni bir instance oluşturur. 
            /// Aynı istek içinde kullanılıyorsa aynı instance kullanılır.
            /// DbContext ve UnitOfWork için ideal seçimdir.
            /// </summary>

            /// <summary>
            /// IUnitOfWork arayüzünü UnitOfWork sınıfına bağlar.
            /// Tüm repository işlemlerini bir transaction içinde yönetmek için kullanılır.
            /// 
            /// Kullanım: 
            /// public class ShowroomService
            /// {
            ///     private readonly IUnitOfWork _unitOfWork;
            ///     public ShowroomService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
            /// }
            /// </summary>
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            /// <summary>
            /// IShowroomService arayüzünü ShowroomService sınıfına bağlar.
            /// Showroom ile ilgili tüm business logic (iş kuralları) burada yönetilir.
            /// 
            /// Bağımlılıkları:
            /// - IUnitOfWork: Veritabanı işlemleri
            /// - IMapper (AutoMapper): DTO dönüştürmeleri
            /// </summary>
            services.AddScoped<IShowroomService, ShowroomService>();


            // ============ DÖNÜŞ ============
            /// <summary>
            /// IServiceCollection'ı geri döndürür. Bu sayede method chaining yapılabilir:
            /// builder.Services
            ///     .AddCustomServices(configuration)
            ///     .AddAuthentication(...)
            ///     .AddAuthorization(...);
            /// </summary>
            return services;
        }
    }
}