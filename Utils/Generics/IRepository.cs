using Microsoft.EntityFrameworkCore;
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
        Task<IResult<T>> FindFirstAsync(Expression<Func<T, bool>>? expression = null);

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

    /// <summary>
    /// IRepository interface'ini uygulayan abstract sınıf.
    /// Entity Framework Core kullanarak veritabanı işlemlerini gerçekleştirir.
    /// Bu sınıftan türetilerek her entity türü için kendi repository'si oluşturulur.
    /// </summary>
    /// <typeparam name="T">Veritabanı entity'si</typeparam>
    /// <example>
    /// // Kullanıcılar için repository oluşturma
    /// public class UserRepository : Repository<User>
    /// {
    ///     public UserRepository(AppDbContext context) : base(context) { }
    /// }
    /// 
    /// // Kullanım
    /// var userRepository = new UserRepository(dbContext);
    /// var result = await userRepository.FindByIdAsync(1);
    /// </example>
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// Veritabanı bağlantısı. Tüm işlemler bu context üzerinden yapılır.
        /// </summary>
        protected readonly DbContext _db;

        /// <summary>
        /// Belirli entity türü için DbSet. Doğrudan SQL sorgusu yapmadan entity işlemleri yapar.
        /// </summary>
        protected readonly DbSet<T> _table;

        /// <summary>
        /// Repository'yi başlatır. DbContext ve DbSet'i ayarlar.
        /// </summary>
        /// <param name="db">Entity Framework Core DbContext nesnesi</param>
        protected Repository(DbContext db)
        {
            _db = db;
            _table = _db.Set<T>();
        }


        /// <summary>
        /// Şarta uygun entity'nin var olup olmadığını kontrol eder.
        /// </summary>
        public async Task<IResult<bool>> AnyAsync(Expression<Func<T, bool>>? expression = null)
        {
            try
            {
                // Eğer şart null ise tüm entity'leri kontrol et, değilse şarta uygun olanları kontrol et
                bool any = expression != null ? await _table.AnyAsync(expression) : await _table.AnyAsync();
                return Result<bool>.Success(any);
            }
            catch (Exception ex)
            {
                // Hata durumunda hata mesajıyla başarısız sonuç döndür
                return Result<bool>.Failure([ex.Message], 500);
            }
        }


        /// <summary>
        /// Şarta uygun entity'lerin toplam sayısını döndürür.
        /// </summary>
        public async Task<IResult<int>> CountAsync(Expression<Func<T, bool>>? expression = null)
        {
            try
            {
                // Eğer şart null ise tüm entity'leri say, değilse şarta uygun olanları say
                int count = expression != null ? await _table.CountAsync(expression) : await _table.CountAsync();
                return Result<int>.Success(count);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure([ex.Message], 500);
            }
        }

        /// <summary>
        /// Yeni bir entity'yi veritabanına ekler.
        /// SaveChangesAsync() çağrılmadığını dikkat edin - manuel olarak çağırılmalı!
        /// </summary>
        public async Task<IResult> CreateAsync(T entity)
        {
            try
            {
                // Entity'yi veritabanına ekle (henüz kaydedilmedi)
                await _table.AddAsync(entity);
                // Not: SaveChangesAsync() çağrılmıyor, transaction yönetimi için parent servisde yapılmalı

                return Result.Success(201); // 201 Created
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message, 500);
            }
        }

        /// <summary>
        /// Birden fazla entity'yi veritabanına ekler.
        /// Veritabanı işlemini tamamlamak için SaveChangesAsync() çağırılmalı.
        /// </summary>
        public async Task<IResult> CreateManyAsync(IEnumerable<T> entities)
        {
            try
            {
                // Tüm entity'leri toplu olarak ekle
                await _table.AddRangeAsync(entities);

                return Result.Success(201); // 201 Created
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message, 500);
            }
        }

        /// <summary>
        /// Primary key'e göre entity'yi siler.
        /// Önce FindByIdAsync ile entity'yi bulur, sonra siler.
        /// </summary>
        public async Task<IResult> DeleteAsync(object id)
        {
            // Önce ID'ye göre entity'yi bul
            var result = await FindByIdAsync(id);

            // Entity bulunduysa sil
            if (result.IsSuccess)
            {
                return await DeleteAsync(result.Data);
            }

            // Entity bulunamadıysa hata döndür
            return result;
        }

        /// <summary>
        /// Verilen entity'yi veritabanından siler.
        /// SaveChangesAsync() manuel olarak çağırılmalı.
        /// </summary>
        public async Task<IResult> DeleteAsync(T entity)
        {
            try
            {
                // Entity'yi sil (veritabanından hemen silinmez, SaveChanges gerekir)
                await Task.Run(() => _table.Remove(entity));
                return Result.Success(204); // 204 No Content
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message, 500);
            }
        }

        /// <summary>
        /// Birden fazla entity'yi veritabanından siler.
        /// </summary>
        public async Task<IResult> DeleteManyAsync(IEnumerable<T> entities)
        {
            try
            {
                // Tüm entity'leri toplu olarak sil
                await Task.Run(() => _table.RemoveRange(entities));
                return Result.Success(204); // 204 No Content
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message, 500);
            }
        }

        /// <summary>
        /// Şarta uygun tüm entity'leri siler.
        /// Örneğin: Silinmek için işaretlenmiş tüm kayıtları sil.
        /// </summary>
        public async Task<IResult> DeleteManyAsync(Expression<Func<T, bool>>? expression = null)
        {
            // Şarta uygun entity'leri seç (null ise hepsi)
            var entities = expression == null ? _table : _table.Where(expression);

            if (entities == null)
            {
                return Result.Failure("Silinecek kayıt bulunamadı!", 404);
            }

            // Bulunan entity'leri sil
            return await DeleteManyAsync(entities);
        }


        /// <summary>
        /// Primary key'e göre entity'yi bulur.
        /// Veritabanında en hızlı arama yöntemidir.
        /// </summary>
        public async Task<IResult<T>> FindByIdAsync(object id)
        {
            // ID'ye göre doğrudan ara
            var entity = await _table.FindAsync(id);

            if (entity == null)
            {
                return Result<T>.Failure(["Entity not found"], 404);
            }
            return Result<T>.Success(entity);
        }

        /// <summary>
        /// Şarta uygun ilk entity'yi bulur.
        /// Örneğin: Belirli bir email'e sahip kullanıcı ara.
        /// </summary>
        public async Task<IResult<T>> FindFirstAsync(Expression<Func<T, bool>>? expression = null)
        {
            // Şart yoksa ilk entity'yi al, yoksa şarta uygun ilk entity'yi al
            var entity = expression == null ? await _table.FirstOrDefaultAsync() : await _table.FirstOrDefaultAsync(expression);

            if (entity == null)
            {
                return Result<T>.Failure(["Entity not found"], 404);
            }
            return Result<T>.Success(entity);
        }

        /// <summary>
        /// Şarta uygun tüm entity'leri bulur. İsteğe bağlı olarak related data'yı yükler.
        /// </summary>
        public async Task<IResult<IEnumerable<T>>> FindManyAsync(Expression<Func<T, bool>>? expression = null, params string[] includes)
        {
            // Şart yoksa tüm entity'leri al, yoksa şarta uygun olanları al
            var entities = expression == null ? _table : _table.Where(expression);

            if (entities == null)
            {
                return Result<IEnumerable<T>>.Failure(["No entities found"], 404);
            }

            // Related data'yı (Orders, Department gibi) yükle
            foreach (var include in includes)
            {
                entities = entities.Include(include);
            }

            return Result<IEnumerable<T>>.Success(await entities.ToListAsync());
        }


        /// <summary>
        /// Tüm entity'leri okur. İsteğe bağlı filtre ve related data yükleme yapılabilir.
        /// FindManyAsync ile aynı işlevi yapar.
        /// </summary>
        public async Task<IResult<IEnumerable<T>>> ReadManyAsync(Expression<Func<T, bool>>? expression = null, params string[] includes)
        {
            // Şart yoksa tüm entity'leri al, yoksa şarta uygun olanları al
            var entities = expression == null ? _table : _table.Where(expression);

            if (entities == null)
            {
                return Result<IEnumerable<T>>.Failure(["No entities found"], 404);
            }

            // Related data'yı yükle
            foreach (var include in includes)
            {
                entities = entities.Include(include);
            }

            return Result<IEnumerable<T>>.Success(await entities.ToListAsync());
        }

        /// <summary>
        /// Tek bir entity'yi günceller.
        /// Entity'nin Id'si varolan kaydın Id'siyle eşleşmelidir.
        /// SaveChangesAsync() manuel olarak çağırılmalı.
        /// </summary>
        public async Task<IResult> UpdateAsync(T entity)
        {
            try
            {
                // Entity'yi güncelle (veritabanında henüz değiştirilmez)
                await Task.Run(() => _table.Update(entity));
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message, 500);
            }
        }

        /// <summary>
        /// Birden fazla entity'yi günceller.
        /// UpdateAsync'den daha verimlidir.
        /// SaveChangesAsync() manuel olarak çağırılmalı.
        /// </summary>
        public async Task<IResult> UpdateManyAsync(IEnumerable<T> entities)
        {
            try
            {
                // Tüm entity'leri toplu olarak güncelle
                await Task.Run(() => _table.UpdateRange(entities));
                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message, 500);
            }
        }
    }
}