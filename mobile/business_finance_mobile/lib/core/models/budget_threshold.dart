/// Bütçenin ne zaman konuşmaya başladığı.
///
/// Uygulamada bütçe iki yerde okunuyor: bütçe ekranındaki satırlar ve Özet
/// ekranındaki `Bütçe durumu` kartı. İkisinin ayrı eşiği olsaydı Özet
/// "limit içinde" derken bütçe ekranı uyarıyor olurdu; eşik bu yüzden tek
/// yerde duruyor.
///
/// Değerin kendisi bir üründür, hesap değil: limitin beşte dördü harcandığında
/// ayın geri kalanı için ayrılan para gözle görülür biçimde azalmıştır.
const double budgetWarningThreshold = 0.8;
