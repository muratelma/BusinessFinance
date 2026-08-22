namespace BusinessFinance.Domain;

/// <summary>
/// Kullanıcının kaydolurken verdiği tek cevap: işletmesi var mı.
/// </summary>
/// <remarks>
/// Bu cevap <b>hiçbir özelliği kapatmaz</b>. Yalnız iki şeyi belirler: hangi
/// varsayılan kategori setiyle başlanacağı ve kapsam boyutunun arayüzde
/// görünüp görünmeyeceği. İşletmesi olmayan kullanıcı için kapsam gerçek bir
/// soru değildir — her kaydı şahsidir ve ona bir anahtar göstermek, cevabı
/// belli olan bir soruyu her ekranda tekrar sormak olurdu.
///
/// Sunucuda durur, cihazda değil: kullanıcı uygulamayı silip yeniden
/// kurduğunda ya da ikinci bir cihazdan girdiğinde işletme sahibi olmayı
/// kaybetmemeli. Cevap sonradan değiştirilebilir; değiştirmek yalnız arayüzü
/// etkiler, çünkü varsayılan kategori seti yalnız hiç kategorisi olmayan
/// kullanıcıya bir kez uygulanır.
/// </remarks>
public sealed class UserProfile
{
    public Guid UserId { get; }
    public bool HasBusiness { get; private set; }

    private UserProfile()
    {
    }

    public UserProfile(Guid userId, bool hasBusiness)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        UserId = userId;
        HasBusiness = hasBusiness;
    }

    public void SetHasBusiness(bool hasBusiness)
    {
        HasBusiness = hasBusiness;
    }
}
