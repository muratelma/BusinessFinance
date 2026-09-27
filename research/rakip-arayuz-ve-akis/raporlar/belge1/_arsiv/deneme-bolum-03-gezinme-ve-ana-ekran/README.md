# Belge 1 · Bölüm 3 · Gezinme ve ana ekran

16 Eylül 2026 · Taslak v2 · Kullanıcı değerlendirmesi bekleniyor.

- Ana metin: [bolum-03.md](bolum-03.md).
- Düzenlenebilir çıktı: [bolum-03.docx](bolum-03.docx).
- Okuma çıktısı: [bolum-03.pdf](bolum-03.pdf). PDF doğrulama durumu aşağıdadır.
- Denetim: [iddia-tablosu.md](iddia-tablosu.md), [eksik-listesi.md](eksik-listesi.md).
- Kanıt: 26 görsel; [kaynaklar.md](kaynaklar.md) ve kanit-manifest.json.

## v2 düzeni

Beş soru: 3.1 ilk karşılaşma, 3.2 gezinme, 3.3 ekleme, 3.4 bilgi sırası, 3.5 boş ekran. Her soruda Ürün ve platform / Görünen düzen / Dayanak ve sınır tablosu kullanılır; görseller ve gerekli ayrıntılar ardından gelir. Aynı görsel gerektiğinde yeniden basılmak yerine şekil numarasıyla anılır.

Eski 3.6 kaynak bölümü ilgili sorulara dağıtıldı. Kaynak destek görseli, video ve temsili çizim ayrı etiketlenir; canlı koşum da kare/kayıt olarak ayrılır. Görülmedi, özellik yokluğu değildir. Kazandırdığı/Bedeli tabloları kaldırıldı; ürün tercihi Belge 3'e bırakıldı.

Money Manager'ın dört ana bölümündeki sekmeler E0228, E0231, E0236, E0254 ile desteklenir; bütün alt ekranlara genellenmez. Ek üç kare bu turda açılarak doğrulandı. Tür seçeneğinin zorunlu ek seçim olduğu ve Goodbudget kurulumunun zorunluluğu iddiaları kaldırıldı. İddia tablosu, özet ve altyazıları da kapsar.

## Gizlilik ve kapsam

Özgün kanıtlar değişmedi. E0275 ve E0115 bölüm kopyalarındaki mevcut kişisel ad karartmaları korundu. Yeni emülatör koşumu yapılmadı. Ortak içindekiler ve diğer bölümler bu düzenleme kapsamında değiştirilmedi; tam rapor kabulü anlamına gelmez.

## Üretim

raporlar/ içinden `.pilot-tools/venv/Scripts/python belge1/bolum-03-gezinme-ve-ana-ekran/uret.py`, ardından `powershell -NoProfile -File belge1/bolum-03-gezinme-ve-ana-ekran/export-word.ps1` çalıştırılır. Mevcut Python/python-docx/PyMuPDF ve Microsoft Word yolu kullanılır; yeni bağımlılık eklenmedi. Uzun tablolar satırlar arasında bölünebilir; başlık ilk veri satırıyla tutulur.

## Doğrulama

- Markdown, manifest ve Word içinde 26 görsel eşleşti.
- Özgün ve bölüm kopyası SHA-256 değerleri kontrol edildi.
- Metinde kaldırılan başlıklar ve eski 3.6 bölümü yok; bozuk karakter bulunmadı.
- PDF yeniden üretildi: 9 sayfa, 26 görsel. Sayfa önizlemeleri incelendi; tek notluk taşma giderildi. Metin sınırları sayfa içinde, bozuk karakter yok. Görsel kimlikleri PDF metninde mevcut; özgün hashler araştırma envanteriyle eşleşiyor.
- Word dışa aktarımı mutlak dosya yolu kullanacak şekilde güncellendi; üretim betiğiyle yeniden çalıştırılarak doğrulandı.
- Uygulama kodu değişmedi; backend/Flutter kontrolleri çalıştırılmadı.
