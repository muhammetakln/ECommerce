using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Utils.Responses;

namespace Utils.Generics
{
    /// <summary>
    /// Veritabanı işlemleri (CRUD) için generic repository interface.
    /// Tüm entity türleri için tekrar kullanılabilir veri erişim işlemlerini tanımlar.
    /// </summary>
    /// <typeparam name="T">Veritabanı entity'si (User, Product, Order gibi). class olmalıdır.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Tek bir entity'yi veritabanına ekler.
        /// </summary>
        /// <param name="entity">Eklenecek entity</param>
        /// <returns>Başarı/başarısızlık sonucu</returns>
        /// <example>
        /// var user = new User { Name = "Ahmet", Email = "ahmet@mail.com" };
        /// await repository.CreateAsync(user);
        /// </example>
        Task<IResult> CreateAsync(T entity);

        /// <summary>
        /// Birden fazla entity'yi veritabanına ekler.
        /// CreateAsync'den daha verimlidir çünkü toplu işlem yapar.
        /// </summary>
        /// <param name="entities">Eklenecek entity'lerin koleksiyonu</param>
        /// <returns>Başarı/başarısızlık sonucu</returns>
        Task<IResult> CreateManyAsync(IEnumerable<T> entities);

        /// <summary>
        /// İsteğe bağlı filtreyle ve related datalarla birlikte entity'leri getirir.
        /// </summary>
        /// <param name="expression">Filtreleme şartı (örn: x => x.Status == "Active")</param>
        /// <param name="includes">Yüklenecek related entity'ler (örn: "Orders", "Department")</param>
        /// <returns>Entity'lerin listesi</returns>
        /// <example>
        /// // Tüm aktif kullanıcıları ve onların siparişlerini getir
        /// await repository.ReadManyAsync(
        ///     x => x.IsActive == true, 
        ///     "Orders", "Department"
        /// );
        /// </example>
        Task<IResult<IEnumerable<T>>> ReadManyAsync(Expression<Func<T, bool>>? expression = null, params string[] includes);

        /// <summary>
        /// İsteğe bağlı filtreyle ve related datalarla birlikte entity'leri bulur.
        /// ReadManyAsync ile aynı işlevi yapar, isim farklılığıyla ağırlığa koyulan veri tabanı işlemi.
        /// </summary>
        /// <param name="expression">Filtreleme şartı</param>
        /// <param name="includes">Yüklenecek related entity'ler</param>
        /// <returns>Entity'lerin listesi</returns>
        Task<IResult<IEnumerable<T>>> FindManyAsync(Expression<Func<T, bool>>? expression = null, params string[] includes);

        /// <summary>
        /// Primary key'e göre tek bir entity'yi getirir.
        /// </summary>
        /// <param name="id">Entity'nin primary key değeri</param>
        /// <returns>Bulunan entity veya hata mesajı</returns>
        /// <example>
        /// var result = await repository.FindByIdAsync(5);
        /// if (result.IsSuccess)
        /// {
        ///     var user = result.Data;
        /// }
        /// </example>
        Task<IResult<T>> FindByIdAsync(object id);

        /// <summary>
        /// Şarta uygun ilk entity'yi getirir.
        /// Örneğin: belirli bir email'e sahip kullanıcıyı bulmak.
        /// </summary>
        /// <param name="expression">Arama şartı (örn: x => x.Email == "test@mail.com")</param>
        /// <returns>Bulunan entity veya hata mesajı</returns>
        /// <example>
        /// var result = await repository.FindFirstAsync(x => x.Email == "ahmet@mail.com");
        /// </example>
        Task<IResult<T>> FindFirstAsync(Expression<Func<T, bool>>? expression = null,params string[]includes);

        /// <summary>
        /// Tek bir entity'yi günceller.
        /// </summary>
        /// <param name="entity">Güncellenecek entity (Id'si varolan kaydın Id'siyle eşleşmelidir)</param>
        /// <returns>Başarı/başarısızlık sonucu</returns>
        /// <example>
        /// var user = new User { Id = 1, Name = "Mehmet" };
        /// await repository.UpdateAsync(user);
        /// </example>
        Task<IResult> UpdateAsync(T entity);

        /// <summary>
        /// Birden fazla entity'yi günceller.
        /// UpdateAsync'den daha verimlidir.
        /// </summary>
        /// <param name="entities">Güncellenecek entity'lerin koleksiyonu</param>
        /// <returns>Başarı/başarısızlık sonucu</returns>
        Task<IResult> UpdateManyAsync(IEnumerable<T> entities);

        /// <summary>
        /// Primary key'e göre entity'yi siler.
        /// </summary>
        /// <param name="id">Silinecek entity'nin primary key değeri</param>
        /// <returns>Başarı/başarısızlık sonucu</returns>
        /// <example>
        /// await repository.DeleteAsync(5); // Id'si 5 olan entity'yi sil
        /// </example>
        Task<IResult> DeleteAsync(object id);

        /// <summary>
        /// Verilen entity'yi siler.
        /// </summary>
        /// <param name="entity">Silinecek entity</param>
        /// <returns>Başarı/başarısızlık sonucu</returns>
        Task<IResult> DeleteAsync(T entity);

        /// <summary>
        /// Birden fazla entity'yi siler.
        /// </summary>
        /// <param name="entities">Silinecek entity'lerin koleksiyonu</param>
        /// <returns>Başarı/başarısızlık sonucu</returns>
        Task<IResult> DeleteManyAsync(IEnumerable<T> entities);

        /// <summary>
        /// Şarta uygun tüm entity'leri siler.
        /// ÖNEMLİ: Dikkatli kullanın, kritik verileri yanlışlıkla silmemeye dikkat edin.
        /// </summary>
        /// <param name="expression">Silme şartı (örn: x => x.IsInactive == true)</param>
        /// <returns>Başarı/başarısızlık sonucu</returns>
        /// <example>
        /// // Tüm silinmiş flagı işaretli kullanıcıları sil
        /// await repository.DeleteManyAsync(x => x.IsDeleted == true);
        /// </example>
        Task<IResult> DeleteManyAsync(Expression<Func<T, bool>>? expression = null);

        /// <summary>
        /// Şarta uygun entity'lerin sayısını döndürür.
        /// </summary>
        /// <param name="expression">Sayma şartı (örn: x => x.Status == "Active")</param>
        /// <returns>Entity sayısı</returns>
        /// <example>
        /// var result = await repository.CountAsync(x => x.Department == "IT");
        /// if (result.IsSuccess)
        /// {
        ///     int count = result.Data; // Aktif kullanıcı sayısı
        /// }
        /// </example>
        Task<IResult<int>> CountAsync(Expression<Func<T, bool>>? expression = null);

        /// <summary>
        /// Şarta uygun en az bir entity'nin var olup olmadığını kontrol eder.
        /// True/false döndürür.
        /// </summary>
        /// <param name="expression">Kontrol şartı (örn: x => x.Email == "test@mail.com")</param>
        /// <returns>Şarta uygun entity var ise true, yoksa false</returns>
        /// <example>
        /// var result = await repository.AnyAsync(x => x.Email == "test@mail.com");
        /// if (result.Data) // Email var mı?
        /// {
        ///     // Email daha önce kaydedilmiş
        /// }
        /// </example>
        Task<IResult<bool>> AnyAsync(Expression<Func<T, bool>>? expression = null);
    }
}