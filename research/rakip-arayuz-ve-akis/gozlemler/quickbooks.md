# Uygulama Gözlem Formu — QuickBooks (Intuit)

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | QuickBooks / Intuit Inc. |
| Sürüm | 30.4.7 (React Native) |
| Test tarihi | 1 Eylül 2026 |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` |
| Dil / para birimi | İngilizce |
| Erişim kısıtı | **Engelli (ödeme kapısı)** — 1 Eyl 2026'da Intuit auth webview'i siyah ekran veriyordu. 2 Eyl 2026'da kullanıcı e-posta + telefon SMS doğrulamasıyla Intuit hesabı oluşturdu ve onboarding'e girildi. Onboarding "Choose a plan" ekranında durdu: tek plan **Simple Start** (TRY 244,99/ay, 6 ay), "try free for 1 month" seçeneği bile **kredi kartı** istiyor (kullanıcı doğruladı). Ücretli plan kapısı → K01–K08 elle yapılamıyor. Ayrıca QuickBooks Solopreneur bölgesel olarak ABD dışına kapalı |

## Görev gözlemleri

| Görev | Sonuç | Not |
|---|---|---|
| K00 İlk açılış | Kısmi | 3 slaytlık onboarding ("Get paid anywhere — Send and track custom invoices" vb.), yeşil "Create account" + çerçeveli "Sign in" |
| K00 (hesap oluşturma, 1 Eyl) | Engelli | "Create account" → `com.intuit.identity.AuthorizationClientActivity` → siyah ekran |
| K00 (hesap oluşturma, 2 Eyl) | Tamamlandı | Kullanıcı e-posta + telefon SMS doğrulamasıyla Intuit hesabı açtı. Welcome ("we're glad you're here", 3 madde) → Get started |
| K00 (onboarding) | Kısmi | "Let's begin with some basic info" → tek alan **Business name** (+ "No business name? Use your name"). Girildi: "Deniz Tasarim" → Next → **"Choose a plan"** |
| K00 (plan / ödeme kapısı) | Engelli | Tek plan Simple Start (TRY 244,99/ay · 6 ay · liste TRY 819,99). Özellikler: Track income/expenses, Send custom invoices/estimates, Auto-track mileage, Create custom categories, Run reports. "try free for 1 month" bile kredi kartı istiyor → durduruldu |
| K01–K08 | **Engelli** | Ödeme kapısı; işletme/şahsi sınıflandırma dahil hepsi resmî kaynakla |

## BusinessFinance için ilgi noktası

QuickBooks Solopreneur, PRD'de "kavramsal yakın rakip" olarak seçildi:
tek kişilik işletmede **işletme/şahsi işlem sınıflandırması** yaklaşımı.
Bu, ADR 0013 (tek havuz + kapsam boyutu) ile karşılaştırılacak asıl konu.
Manuel test yapılamadığı için `Resmî kaynak` ile incelenecek:
quickbooks.intuit.com/solopreneur, ürün turu videoları, yardım merkezi
"business vs personal expenses" makaleleri.

## Kanıt ve güven düzeyi

- Manuel gözlem: Onboarding + hesap açma + "basic info" + "Choose a plan" paywall'ı
  (ekran görüntüleri `kanitlar/quickbooks/01`–`04`)
- Resmî kaynak: K01–K08 ve özellikle işletme/şahsi sınıflandırma için zorunlu
- Doğrulanamadı: Tüm asıl akışlar

## Tek cümlelik sonuç

QuickBooks Solopreneur'un işletme/şahsi sınıflandırması bizim en kritik karşılaştırma konumuz; emülatörde giriş açılmadığı için tamamen resmî kaynakla incelenecek.
