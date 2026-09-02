# Uygulama Gözlem Formu — Money Manager (Realbyte)

## Oturum bilgisi

| Alan | Değer |
|---|---|
| Uygulama / geliştirici | Money Manager / Realbyte Apps |
| Sürüm | 4.12.8 (ücretsiz, "AD") |
| Test tarihi | 1 Eylül 2026 |
| ⚠ Tarih sapması | İşlemler `SENTETIK-TEST-VERISI.md`'deki Ağustos 2026 tarihleriyle (3/5/8/12/18 Ağu) değil, emülatörün o günkü tarihiyle **01.09.2026'ya, hepsi tek güne** girildi (`06-islem-listesi.png`). Tur 1 kontrol değerleri ay içi toplam olduğu için etkilenmedi ve birebir tuttu; yeniden giriş yapılmadı. **Kalan uygulamalar ve Tur 2 Ağustos 2026 + spec gün tarihleriyle yapılır**; Money Manager Tur 2'ye seçilirse doğru tarihlerle yeniden girilir. |
| Cihaz / işletim sistemi | Android emülatör `emulator-5554` (1080x2400) |
| Dil / para birimi | İngilizce arayüz / TRY (₺) — test için USD'den çevrildi |
| Hesap veya plan türü | Ücretsiz sürüm — ekranda banner reklam ("Ad") |
| Erişim kısıtı | Yok — kayıt/giriş/e-posta istemiyor, tamamen yerel |

## Görev gözlemleri

| Görev | Sonuç | Yaklaşık adım | İyi çalışan | Sürtünme/belirsizlik | Kanıt |
|---|---|---:|---|---|---|
| K00 İlk açılış ve kayıt | Tamamlandı | 0 | Kayıt/giriş/onboarding **hiç yok**; ilk açılışta doğrudan ana ekran, anında kullanılabilir | Para birimi/dil sorulmuyor; varsayılan USD | Kullanıcı doğruladı: hiçbir ekran görmedi |
| K01 Ana ekran | Tamamlandı | 0 | Daily/Calendar/Monthly/Total/Note sekmeleri; üstte Income/Expenses/Total; tek turuncu "+" | Ay gezgini + 5 sekme + 3 sütun ilk bakışta yoğun | `02-bos-ana-ekran.png`, `03-dolu-ana-ekran.png` |
| K02 Hesap/cüzdan oluşturma | Tamamlandı (uyarlanarak) | ~6/hesap | 3 hazır hesap (Cash / Accounts / Card grupları); grup + hesap iki katmanlı; Card hesabı zengin (ödeme hesabı, kesim/ödeme günü, "Balance Payable" vs "Outst. Balance") | "Hesap oluştur" butonu yok — hazır hesaplar düzenleniyor; **para birimi hesap başına**; her hesap için 2-3 onay diyaloğu | `09-ozgun-ozellik.png` (Card takvimi) |
| K03 İşletme geliri | Tamamlandı (kapsamsız) | ~7 | Segmented Income/Expense/Transfer; alanlar az ve net; "Save / Continue" (seri giriş) | Gelir kategorileri: Allowance/Salary/Petty cash/Bonus/Other — **işletme geliri kategorisi yok**; müşteri ancak "Note" alanına | `04-islem-formu.png` |
| K04 Şahsi gider | Tamamlandı (ama ayrım yok) | ~7 | Not girilince liste satırında **başlık = not**, kategori sola küçük etiket (BusinessFinance `title = açıklama ?? kategori` ile birebir) | **İşletme/şahsi (kapsam) ayrımı formda hiç yok.** Protokol gereği taklit edilmedi → bu boyut `Desteklenmiyor` | `05-siniflandirma.png` |
| K05 İşletme kart gideri | Tamamlandı | ~7 | Kart hesabı seçilince gider **aynı gün gider sayılıyor** VE kart borcu +₺1.200 oluyor (BusinessFinance CreditCardCharge kuralıyla aynı) | Kategori yine kişisel liste; "Other" seçildi | `05-siniflandirma.png`, `06-islem-listesi.png` |
| K06 Transfer | Tamamlandı | ~7 | Ayrı "Transfer" sekmesi; From/To + swap oku + isteğe bağlı "Fees"; liste satırı **nötr siyah** renkte, "Ana Hesap → Ortak Cuzdan" | — | `06-islem-listesi.png` |
| K07 Liste ve rapor | Tamamlandı | ~3 | **Transfer gelir/gidere 0 etki**; **açılış bakiyeleri (₺22.000) gelire girmiyor** (Income = tam ₺25.000, Salary %100); kart gideri gidere giriyor; Stats aylık pasta grafiği | Rapor kapsam/işletme-şahsi bölmesi yok (zaten kapsam yok) | `07-rapor.png` |
| K08 Düzeltme/iptal | Tamamlandı | ~2 | Satıra dokun → **aynı formda satır içi düzenleme**; alttan Delete / Copy / Bookmark | **Silme = tek onay → kalıcı.** "İptal/void" veya geri alma yok; BusinessFinance'in "sil yerine iptal" kuralının tersi | — |

## Kontrol değeri doğrulaması

| Değer | Beklenen (Tur 1, kart ödemesi hariç) | Money Manager | Durum |
|---|---:|---:|---|
| Ortak Cuzdan bakiyesi | 4.150,00 | ₺4.150,00 | ✓ |
| Ana Hesap bakiyesi | 42.000,00 (kart ödemesi Tur 2) | ₺42.000,00 | ✓ |
| Kart borcu (Outst.) | 1.200,00 | ₺1.200,00 | ✓ |
| Net varlık (Total) | 44.950,00 | ₺44.950,00 | ✓ |
| Gelir toplamı | 25.000,00 | ₺25.000,00 | ✓ (açılış bakiyesi kirletmedi) |
| Gider toplamı | 2.050,00 | ₺2.050,00 | ✓ |

Money Manager'ın para modeli BusinessFinance ile şaşırtıcı derecede uyumlu: transfer nötr, açılış bakiyesi gelir değil, kart harcaması harcandığı gün gider.

## Arayüz taraması (görev dışı)

| Alan | Gezildi | Kısa gözlem |
|---|---|---|
| Tüm ana sekmeler | ✓ | **Daily** (gün listesi), **Calendar** (aylık ızgara, her hücrede gelir/gider/net üç satır), **Monthly** (yıllık liste, ay→hafta açılır), **Total** (bütçe + hesap kırılımı), **Note** (işlemden ayrı memo/checklist, boş). Beşi de aynı 3 sütun başlığı + FAB. Hafta sonu başlıkları renkli (Sun kırmızı, Sat mavi) | `10-arayuz-calendar.png` |
| Rapor drill-down | ✓ | Stats pastası → kategoriye tıklanınca o kategorinin işlem listesi. Total sekmesi ayrıca **"Expenses (Cash, Accounts) ₺850" vs "Expenses (Card) ₺1.200"** diye ayırıyor + "Transfer ₺0" satırı + "Compared Expenses (Last month) %" | `11-arayuz-total-butce.png` |
| Bütçe / planlama | ✓ (yüzeysel) | Total sekmesinde "Budget" bloğu + "Budget Setting" linki; Configuration'da "Budget Setting" ve "Repeat Setting" ayrı. Kategori bazlı aylık bütçe kurgusu | — |
| Ayar derinliği | ✓ | More → Settings ızgarası: Configuration / Accounts / Passcode / CalcBox / PC Manager / Backup / Feedback / Help / Recommend. Configuration: kategori ayarları, ana+alt para birimi, başlangıç ekranı, ay/hafta başlangıcı, devir (carry-over), swipe davranışı, gelir-gider renk seti (Set A/B). Yedek ayrı; dışa aktarma "Export data to Excel" (Total sekmesinde) | — |
| Arama ve filtre | ✓ | Filtre paneli: Income/Expense donut yüzdeleri + INCOME/EXPENSES/**ACCOUNT** alt sekmeleri; hesap bazında Income/Transfer-In ve Expense/Transfer-Out sütunları, checkbox ile süzme. Ayrı büyüteç ile serbest metin arama | `12-arayuz-filtre.png` |
| Boş durum | ✓ | Her boş ekranda aynı gri kedi/robot illüstrasyonu + "No data available." Tutarlı ama yönlendirici değil (ne yapılacağını söylemiyor) | `02-bos-ana-ekran.png` |
| Hata / uç durum | ✓ | Zorunlu alan eksikse **toast** ("Please select specific category." + domuz kumbara ikonu), inline alan hatası yok. Tutar 0.00 sessizce kabul (sıfır tutar engellenmiyor). Formdan "geri" **taslak uyarısı vermeden** çıkıyor | `13-hata-toast.png` |
| Widget / hızlı giriş | Kısmen | Form içi "Continue" = kaydet + boş forma dön (seri giriş). Ana ekran widget'ı denenmedi | — |

## Arayüz incelemesi

| Başlık | Kısa gözlem |
|---|---|
| Bilgi hiyerarşisi | Zaman odaklı (ay → gün). Para özeti hep üç sütun: Income (mavi) / Expenses (turuncu) / Total (siyah) |
| Alt/üst gezinme | Alt: Trans. / Stats / Accounts / More. Üst: ay gezgini + favori + arama + filtre |
| Renklerin anlamı | Gelir mavi, gider turuncu-kırmızı, net/transfer nötr siyah; birincil eylem turuncu FAB. Tutarlı |
| Tipografi ve para | Para sağa hizalı, iki ondalık, ₺ ön ekli. Okunur. Kart borcu negatif (₺ -1.200,00) gösteriliyor |
| Kart/liste/grafik | Accounts: gruplu liste, grup alt-toplamı gri başlık. İşlem listesi: gün başlığı + satırlar. Stats: pasta + yüzdeli liste |
| Form alanları | Tek ekranda 5 alan + açıklama/fiş(kamera). Kategori/hesap alt panelden ızgara seçim. "Continue" ile arka arkaya giriş. Tutar için ayrı hesap-makinesi tuş takımı |
| Loading/boş/hata/başarı | Boş: illüstrasyon + "No data available." (yönlendirici değil). Başarı sessiz (toast yok). Hata = toast, inline değil. Silme tek onaylı |
| Erişilebilirlik | Ücretsiz sürümde kalıcı banner reklam ekranın altını yer yer kaplıyor; tuş takımı büyük ve rahat; Türkçe yok (İngilizce/başka diller var) |
| Ekran tutarlılığı | 5 ana sekme + Accounts + Stats hepsi aynı 3 sütun para başlığını taşıyor — güçlü tutarlılık. Ay gezgini her ekranda aynı yerde. Her ekranda farklı FAB ikonu (işlem +, not +, yok) — bağlama göre |

## Akış özeti

- En kısa ve güçlü akış: İşlem ekleme — "+" → tutar → kategori → hesap → Save (~7 dokunuş); "Continue" ile seri giriş
- En fazla sürtünme yaratan akış: Para birimi kurulumu — ana ayar + her hesap ayrı ayrı, hesap başına 2-3 onay diyaloğu
- Uygulamanın hedef kullanıcı varsayımı: Kişisel/gündelik bütçe takibi — işletme değil
- İşletme ve şahsi para yaklaşımı: **Yok.** Kapsam boyutu, mod veya işletme kategorisi bulunmuyor
- Transfer ve kart ödemesi yaklaşımı: Transfer ayrı segment, rapora 0 etki. Card hesabında ayrı "Pay" butonu (Tur 2'de sınanacak)
- Planlama, borç ve tahsilat yaklaşımı: "Rep/Inst." (tekrar/taksit) form içinde; Card hesabı kesim (Settlement) + ödeme (Payment) takvimi taşıyor, "Balance Payable" (kesilmiş, ödenecek) vs "Outst. Balance" (kesilmemiş) ayrımı (Tur 2)

## BusinessFinance için kararlar

| Bulgu | Karar | Gerekçe | Etkilenecek ekran/akış |
|---|---|---|---|
| İşlem listesinde başlık = not, kategori küçük etiket | Doğrudan al (zaten böyle) | BusinessFinance `title = açıklama ?? kategori` ile aynı; doğrulayıcı örnek | Aktivite feed satırı |
| ~7 dokunuşluk tek ekran işlem formu + "Continue" ile seri giriş | Uyarlayarak al | Hızlı; "Continue" (kaydet + yeni forma dön) mantığı bize de uyar; bizde ek olarak kapsam/KDV/indirilebilirlik alanları var | İşlem ekle formu |
| Gelir/gider/transfer segmented kontrolü form başında | Uyarlayarak al | Kayıt türünü en başta seçmek net; bizde transfer + kart ödemesi + cari tahsilat da var | İşlem ekle launcher |
| Hesap türü grupları (Cash/Bank/Card) + Card'da kesim/ödeme takvimi + "Balance Payable vs Outstanding" | Uyarlayarak al | Bizim kredi kartı + ekstre projeksiyon modelimizle birebir örtüşüyor; kullanıcıya iki rakamı ayrı göstermek iyi fikir | Kart ekranı, ekstre |
| Total sekmesinde ayın gideri "nakit/hesap" ve "kart" diye ayrı satır + "Transfer ₺0" satırı | Uyarlayarak al | Kullanıcıya "kart harcaman ayrı" demek bizim kart borcu modelimizi görünür kılar; transferi 0 olarak açıkça yazmak da öğretici | Özet / aylık rapor ekranı |
| Filtrede ACCOUNT alt sekmesi: hesap bazında Income / Transfer-In / Expense / Transfer-Out sütunları | Uyarlayarak al | Bizim birleşik feed'in hesap kırılımı için iyi model; transfer'i giriş/çıkış olarak ayrı göstermek net | Aktivite feed filtresi |
| Zorunlu alan hatası = toast, inline değil; sıfır tutar sessiz kabul; taslak uyarısı yok | Alma | Bizde ProblemDetails + alan bazlı hata var; toast tek başına erişilebilir değil ve hangi alan olduğunu göstermiyor | Form doğrulama |
| Boş durum ekranları yönlendirici değil (sadece "No data available.") | Alma / dikkat | Bizim proje kuralı: her ekranda görünür empty/loading/error/unauthorized/stale; boş durum ne yapılacağını söylemeli | Tüm liste ekranları |
| Para birimi hesap başına + çok adımlı değiştirme | Alma | Tek para birimi (TRY) varsayımımız var | — |
| Açılış bakiyesi = "Modified Balance / Difference" hareketi ("gelir olarak kaydet?" sorusu) | Uyarlayarak al / dikkat | Bizde `OpeningBalance` kalıcı kolon; ama MM'in çözümü de gelir raporunu kirletmiyor (K07 doğruladı). "Gelir mi?" sorusu gereksiz karmaşıklık — bizim sessiz yaklaşımımız daha iyi | Hesap oluşturma |
| Düzenleme = satır içi; silme = kalıcı, tek onay | Alma | BusinessFinance finansal geçmişi korur: sil yerine iptal, düzeltme = iptal + yeni. MM'in yaklaşımı ticari üründe veri bütünlüğünü bozar | İşlem düzenleme/iptal |
| İşletme/şahsi ayrımının hiç olmaması | Alma (kurucu kararımız tersi) | ADR 0013: ayrım bir raporlama boyutu; MM bu ihtiyacı hiç karşılamıyor — asıl farklılaşma noktamız, negatif referans | — |

## Kanıt ve güven düzeyi

- Manuel gözlem: K00–K08 emülatörde sentetik veriyle tamamlandı; kontrol değerleri birebir tuttu
- Resmî kaynak: —
- Yorum: "MM para modeli BusinessFinance ile uyumlu" — kontrol değerleriyle desteklendi
- Doğrulanamadı: Tur 2 konuları (kart ödemesi/Pay, tekrarlayan, fatura-tahsilat) henüz yapılmadı

## Tek cümlelik sonuç

Money Manager hızlı ve alışkanlık kurduran bir kişisel bütçe defteri; işlem formu, liste hiyerarşisi ve kredi kartı ekstre modeli bize örnek olur, ama işletme/şahsi ayrımı ve "sil yerine iptal" hiç olmadığı için asıl farkımızı gösteren negatif referans.
