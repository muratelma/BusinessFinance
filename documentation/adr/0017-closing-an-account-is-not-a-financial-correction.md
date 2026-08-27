# ADR 0017 — Hesabı kapatmak bir finansal düzeltme değildir: veri gerçekten silinir

- Durum: **Kabul edildi** (27 Ağustos 2026, Aşama 06 Grup 1)
- Bağlam: Kullanıcının uygulama içinden kendi hesabını silebilmesi
- İlgili: ADR 0013 (işletme ve şahsi tek havuzdur), ADR 0014 (ekonomik olay
  tanır, ödeme taşır), `AGENTS.md` "Veri ve güvenlik"

## Bağlam

Ürünün en eski davranış kurallarından biri **silme yerine iptal**'dir: hesap,
kategori ve hareket fiziksel olarak silinmez; pasifleştirilir veya UTC zaman
damgalı, idempotent bir iptalle kapatılır. Gerekçesi finansaldır — silinen bir
hareket geçmiş bir ayın raporunu bugün değiştirir ve kullanıcının kendi
defterine güveni biter.

Kullanıcının **kendi hesabını** kapatması bu kuralın kapsamına giriyormuş gibi
görünür, çünkü ikisi de "silme" kelimesini kullanır. Aynı şey değildir ve
karıştırılması iki yönde de zarar verir: kuralı hesap kapatmaya uygularsak
kullanıcı verisinden hiçbir zaman kurtulamaz; hesap kapatmanın serbestliğini
deftere taşırsak çifte sayım yasağını kaybederiz.

## Karar

### 1. İki ayrı konu, tek cümleyle ayrılır

- **Defterin içindeki hareket** silinmez. Düzeltme = eski hareketi iptal et +
  doğrusunu yaz. Bu kural aynen durur.
- **Defterin kendisini kapatan kullanıcı** için silme gerçek silmedir.
  Kapatılan şey bir hareket değil, hesabın tamamıdır; korunacak bir rapor
  tutarlılığı kalmaz çünkü raporu okuyacak kimse kalmaz.

### 2. Silme gerçek silmedir, anonimleştirme değildir

Kullanıcıya ait bütün satırlar veritabanından kalkar; kimlik kaydı da silinir.

Reddedilenler ve gerekçeleri:

| Seçenek | Neden reddedildi |
|---|---|
| Anonimleştirme (kimliği sil, finansal satırları sahipsiz bırak) | Veri tek kullanıcıya aittir ve karşı tarafı yoktur: sahipsiz kalan satır kimseye bir şey anlatmaz, kimsenin defterini tamamlamaz. Taşıdığı tek şey, kullanıcının sildiğini sandığı veridir |
| Pasife alma + 30 gün sonra silme | Temizliği yapacak zamanlanmış bir iş ister. Sunucu bugün yalnız geliştirme makinesi açıkken çalışıyor; "30 gün sonra" hiç gelmeyebilir. Bu yolu ancak sunucu sürekli ayakta olduğunda (Aşama 07) yeniden açmak dürüst olur |
| Yalnız kimlik kaydını silmek | Foreign key'ler `Restrict` davranışındadır: istek zaten reddedilir. Reddi aşmak için `Cascade`'e geçmek, yanlış bir silmenin bütün defteri götürmesi demektir |

### 3. Silmenin iki kapısı vardır

Geri dönüşü olmadığı için istek tek bir dokunuşla tamamlanmaz:

1. **Yeniden kimlik doğrulama.** Kullanıcı parolasını yeniden yazar; elinde
   açık kalmış bir telefonla karşılaşan biri hesabı silemez.
2. **Açık onay.** İstek `confirmed` alanını taşımak zorundadır; taşımayan istek
   `account.delete_not_confirmed` ile reddedilir ve hiçbir şey silinmez.

Uygulama silmeden önce kullanıcıya **yedeğini almasını** önerir. Yedek
kullanıcının elindedir; sunucu silinen veriden bir kopya saklamaz — sakladığı
anda "silindi" demek yalan olurdu.

### 4. Sıra ve bütünlük

- Silme **tek transaction** içinde yapılır. Yarıda kalan bir silme, yarısı
  kaybolmuş bir defter bırakırdı.
- Sıra çocuktan ebeveyne doğru **açıkça yazılıdır** (`EfUserAccountEraser`);
  şemadaki `Restrict` davranışı korunur. Cascade'e geçilmez: bugün silmeyi
  reddeden kısıt, yarın yanlış bir silmeyi de reddedecek olan kısıttır.
- Veritabanı dışında yaşayan **dosyalar** (fiş/dekont ekleri) satırlarla
  birlikte gider: anahtarlar silmeden önce okunur, dosyalar transaction
  başarıyla kapandıktan sonra silinir.

## Sonuçlar

- Kullanıcı kendi verisinden gerçekten kurtulabilir; "sildim" dediğinde silinir.
- Yeni bir kalıcı tablo veya şema değişikliği gerekmez — bu karar migration
  üretmez.
- Yeni bir satır tipi eklendiğinde `EfUserAccountEraser`'a da eklenmesi
  gerekir; gerçek SQL üzerinde çalışan silme testi bunu unutulduğunda kırılacak
  şekilde yazılmıştır (bütün tablolar için sıfır satır iddiası).
- Bu ADR yalnız **kullanıcının kendi hesabını kapatması** hakkındadır. Yönetici
  eliyle silme, toplu silme veya başka bir kullanıcının verisine dokunan hiçbir
  yol yoktur ve açılmaz.
