using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Utils.Responses;

namespace Utils.Generics
{
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
        /// <summary>
        /// Şarta uygun ilk entity'yi bulur. İsteğe bağlı olarak related data'yı yükler.
        /// Örneğin: belirli bir email'e sahip kullanıcıyı ve onun siparişlerini bulmak.
        /// </summary>
        public async Task<IResult<T>> FindFirstAsync(Expression<Func<T, bool>>? expression = null, params string[] includes)
        {
            // Şart yoksa ilk entity'yi al, yoksa şarta uygun ilk entity'yi al
            var query = expression == null ? _table : _table.Where(expression);

            // Related data'yı (Orders, Department gibi) yükle
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            var entity = await query.FirstOrDefaultAsync();

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