/// Rota tanımıyla oraya gönderen çağrının paylaştığı yol sabitleri.
///
/// Kendi dosyasında duruyorlar çünkü `app_router.dart` özellik ekranlarını
/// içeri alıyor; sabitler orada kalsaydı bir özelliğin oraya sabit için
/// başvurması iki dosyayı birbirine bağlardı. Burası hiçbir şey import etmez.
///
/// Sabit olmalarının sebebi yaşanmış bir hata: rota taşındığında çağrı eski
/// dizgide kaldı ve uygulama cihazda "no routes for location" ile patladı.
/// Aşağıdaki yolların hepsi fiş ekranıyla **kardeş** olmak zorunda; üst
/// seviyeye alınan sayfa, shell zinciri yeniden kurulduğu için Navigator'a
/// aynı sayfa anahtarını ikinci kez kaydettiriyor ve çöküyor.
library;

/// Banka belgesi karar sayfası: "bu tutar ne?".
const bankDocumentDecisionLocation = '/transactions/new/bank-document';

/// Dekont okuma sayfası.
///
/// Fiş sayfasının kardeşi ve ondan ayrı: ikisi tek ekranda toplandığında
/// kullanıcıya `İşlem ekle` menüsünde verdiği cevap ekranda tekrar soruluyordu.
const bankSlipScanLocation = '/transactions/new/bank-slip';

/// İade karar sayfası.
const refundDecisionLocation = '/transactions/new/refund';

/// Fatura "ödendi mi?" sayfası.
const invoiceDecisionLocation = '/transactions/new/invoice';

/// Ödenmemiş faturanın tek seferlik yükümlülük formu.
const obligationCreateLocation = '/transactions/new/obligation';

/// `Kasa` ekranı: gün sonu sayımı ve POS tahsilatları.
///
/// `?tab=pos` ile açıldığında POS sekmesi seçili gelir. Sabit burada, çünkü
/// ekrana `İşlem ekle` menüsünden de gidiliyor ve rota dizgesinin iki yerde
/// elle yazılması taşındığında biri geride kalır.
const cashLocation = '/more/cash';

/// `Hesabım`: e-posta, işletme cevabı, açık oturumlar, parola ve hesabı
/// kapatma. Özet ekranının sağ üstünden ve `Diğer` menüsünden aynı yere
/// gidilir; iki kapı, gezinme kararı verilene kadar (Aşama 06.2) birlikte
/// duruyor.
const accountLocation = '/more/account';

/// Vergi takvimi: hazır kalemler ve kurdukları tekrarlayan planlar.
const taxCalendarLocation = '/more/tax-calendar';

/// Ay sonu muhasebeci paketi.
const accountantPackageLocation = '/more/accountant-package';
