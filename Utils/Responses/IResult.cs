using Utils.Responses;

namespace Utils.Responses
{
    /// <summary>
    /// Operasyonun başarı/başarısızlık durumunu ve sonuçlarını tutan temel interface.
    /// Tüm API yanıtlarında kullanılır.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// İşlemin başarılı olup olmadığını gösterir.
        /// true = başarılı, false = başarısız
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// İşlem hakkında bilgilendirici mesaj. Başarı veya hata açıklaması içerebilir.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Hata durumunda hata mesajlarının listesi.
        /// Validation hataları gibi birden fazla hata olabilir.
        /// </summary>
        IEnumerable<string> Errors { get; }

        /// <summary>
        /// HTTP durum kodu (200, 201, 400, 404, 500 gibi).
        /// </summary>
        int StatusCode { get; }
    }

    /// <summary>
    /// Generic IResult interface. İşlem sonucuyla birlikte veri döndürülmesi gerektiğinde kullanılır.
    /// Örneğin: kullanıcı verisini döndüren, ürün listesini döndüren işlemler.
    /// </summary>
    /// <typeparam name="T">Döndürülecek veri türü (User, Product, List<Order> gibi)</typeparam>
    public interface IResult<out T> : IResult
    {
        /// <summary>
        /// İşlem başarılı ise döndürülecek veriler. Başarısız ise null olabilir.
        /// </summary>
        T Data { get; }
    }

    /// <summary>
    /// Veri döndürmeyen basit operasyonlar için kullanılan Result sınıfı.
    /// Örneğin: Silme, güncelleme, ekleme işlemleri (sadece başarı/hata bilgisi döner).
    /// </summary>
    public class Result : IResult
    {
        /// <summary>
        /// İşlemin başarılı olup olmadığını gösterir.
        /// init: özellik sadece başlatılırken atanabilir, sonrasında değiştirilemez (immutable).
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// İşlem hakkında açıklayıcı mesaj. Boş string ile başlar.
        /// </summary>
        public string Message { get; init; } = string.Empty;

        /// <summary>
        /// Hata mesajlarının koleksiyonu. Boş liste ile başlar.
        /// </summary>
        public IEnumerable<string> Errors { get; init; } = [];

        /// <summary>
        /// HTTP durum kodu. Örneğin: 200 (OK), 201 (Created), 400 (Bad Request).
        /// </summary>
        public int StatusCode { get; init; }

        /// <summary>
        /// Result nesnesi oluşturmak için constructor.
        /// Dışarıdan doğrudan çağrılamaz, sadece static method'lar üzerinden kullanılır.
        /// </summary>
        /// <param name="isSuccess">İşlem başarılı mı?</param>
        /// <param name="statusCode">HTTP durum kodu</param>
        /// <param name="message">Açıklayıcı mesaj</param>
        /// <param name="errors">Hata mesajları listesi</param>
        protected Result(bool isSuccess, int statusCode, string? message = null, IEnumerable<string> errors = null)
        {
            IsSuccess = isSuccess;
            Message = message ?? string.Empty;
            Errors = errors ?? [];
            StatusCode = statusCode;
        }

        /// <summary>
        /// Başarılı sonuç döndüren Result oluşturur.
        /// </summary>
        /// <param name="statusCode">HTTP durum kodu (varsayılan: 200 OK)</param>
        /// <param name="message">Başarı mesajı</param>
        /// <returns>Başarılı Result nesnesi</returns>
        /// <example>
        /// Result.Success(201, "Kullanıcı başarıyla oluşturuldu");
        /// </example>
        public static Result Success(int statusCode = 200, string? message = null)
            => new(true, statusCode, message);

        /// <summary>
        /// Başarısız sonuç döndüren Result oluşturur (hata listesiyle).
        /// Validation hataları gibi birden fazla hata durumunda kullanılır.
        /// </summary>
        /// <param name="errors">Hata mesajlarının listesi</param>
        /// <param name="statusCode">HTTP durum kodu (varsayılan: 400 Bad Request)</param>
        /// <returns>Başarısız Result nesnesi</returns>
        /// <example>
        /// var errors = new[] { "Email geçersiz", "Şifre çok kısa" };
        /// Result.Failure(errors, 400);
        /// </example>
        public static Result Failure(IEnumerable<string> errors, int statusCode = 400)
            => new(false, statusCode, null, errors);

        /// <summary>
        /// Başarısız sonuç döndüren Result oluşturur (tek hata mesajıyla).
        /// </summary>
        /// <param name="message">Hata mesajı</param>
        /// <param name="statusCode">HTTP durum kodu (varsayılan: 400 Bad Request)</param>
        /// <returns>Başarısız Result nesnesi</returns>
        /// <example>
        /// Result.Failure("Sunucuda hata oluştu", 500);
        /// </example>
        public static Result Failure(string message, int statusCode = 400)
            => new(false, statusCode, message);
    }

    /// <summary>
    /// Veri döndüren operasyonlar için kullanılan generic Result sınıfı.
    /// Örneğin: Kullanıcı getirme, ürün listesi getirme, vb.
    /// </summary>
    /// <typeparam name="T">Döndürülecek veri türü</typeparam>
    public class Result<T> : Result, IResult<T>
    {
        /// <summary>
        /// İşlem başarılı ise bu özellik istenilen veriyi içerir.
        /// init: sadece başlatılırken atanabilir.
        /// </summary>
        public T Data { get; init; }

        /// <summary>
        /// Generic Result nesnesi oluşturmak için constructor.
        /// Dışarıdan doğrudan çağrılamaz, sadece static method'lar üzerinden kullanılır.
        /// </summary>
        /// <param name="data">Döndürülecek veri</param>
        /// <param name="isSuccess">İşlem başarılı mı?</param>
        /// <param name="statusCode">HTTP durum kodu</param>
        /// <param name="message">Açıklayıcı mesaj</param>
        /// <param name="errors">Hata mesajları listesi</param>
        private Result(T data, bool isSuccess, int statusCode, string? message = null, IEnumerable<string>? errors = null)
            : base(isSuccess, statusCode, message, errors)
        {
            Data = data;
        }

        /// <summary>
        /// Başarılı sonuç döndüren generic Result oluşturur.
        /// Veritabanından veri getirme, hesaplama gibi işlemlerde kullanılır.
        /// </summary>
        /// <param name="data">Döndürülecek veri</param>
        /// <param name="statusCode">HTTP durum kodu (varsayılan: 200 OK)</param>
        /// <param name="message">Başarı mesajı</param>
        /// <returns>Veriyle birlikte başarılı Result nesnesi</returns>
        /// <example>
        /// var user = new User { Id = 1, Name = "Ahmet" };
        /// Result<User>.Success(user, 200, "Kullanıcı bulundu");
        /// </example>
        public static Result<T> Success(T data, int statusCode = 200, string? message = null)
            => new(data, true, statusCode, message);

        /// <summary>
        /// Başarısız sonuç döndüren generic Result oluşturur.
        /// Veri bulunamazsa, validation hatası olursa kullanılır.
        /// </summary>
        /// <param name="errors">Hata mesajlarının listesi</param>
        /// <param name="statusCode">HTTP durum kodu (varsayılan: 400 Bad Request)</param>
        /// <param name="message">Opsiyonel hata mesajı</param>
        /// <returns>Başarısız generic Result nesnesi</returns>
        /// <example>
        /// Result<User>.Failure(["Kullanıcı bulunamadı"], 404);
        /// </example>
        public static Result<T> Failure(IEnumerable<string> errors, int statusCode = 400, string? message = null)
            => new(default, false, statusCode, message, errors);
    }
}