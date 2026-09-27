# Bölüm 3 — bağımsız anlatım

16 Eylül 2026 · Kullanıcı değerlendirmesi için yeni taslak.

- [PDF](bolum-03.pdf) · [Word](bolum-03.docx) · [Markdown](bolum-03.md)
- [Önceki çıktının değerlendirmesi](degerlendirme.md)
- [İddia ve sınır kaydı](iddia-tablosu.md)
- [Kaynak dizini](kaynaklar.md) · kanit-manifest.json

## Nasıl üretildi?

Metin sıfırdan yazıldı. Önceki bölümün paragrafları dönüştürülmedi; ham gözlem formları, tema 02, envanter ve seçilmiş kanıt kareleri kullanıldı. Altı sorulu bir anlatı kuruldu: ilk bilgi, gezinme, kayıt başlatma, bekleyen işler, ilk kullanım, boş ekran. Kaynak panoları bekleyen işler sorusunun devamındadır.

18 şekil yeniden seçildi; 6 ek görsel dayanak ayrıca dizine alındı. Görseller mevcut korpustandır, yeni emülatör koşumu yapılmadı. İki karartılmış teslim kopyası aynen kullanıldı; özgün görseller değişmedi. Önceki bölüm klasörü bu çalışmada değiştirilmedi.

## Üretim

raporlar/ içinden `.pilot-tools/venv/Scripts/python belge1/bolum-03-bagimsiz/uret.py`, sonra `powershell -NoProfile -File belge1/bolum-03-bagimsiz/export-word.ps1`.

Mevcut python-docx/PyMuPDF ve Microsoft Word kullanılır; yeni kütüphane eklenmedi. Üretici yalnız bu klasörün çıktısını yazar. Kaynak kopyaların hashleri kopyalamadan önce doğrulanır.

## Kontrol

Doğrulama sonucu dogrulama.json içindedir. Sayfa önizlemeleri kontrol edilir; görsel kimlikleri, şekil sayısı, bağlantılar, envanter ve dosya hashleri karşılaştırılır. Uygulama kodu değişmediği için backend/Flutter build ve testleri çalıştırılmadı.

Sıradaki tek iş: bu bağımsız örneğin anlatımını ve görsel yoğunluğunu değerlendirmek. Ortak içindekiler ve diğer bölümler henüz bu örneğe göre değiştirilmedi.
