#!/usr/bin/env bash
# Gözlem formu / kanıt denetimi — kuralların tamamı denetim.cjs içindedir.
#
# Kullanım:
#   ./denetim.sh                          → dokuz uygulamanın hepsi
#   ./denetim.sh money-manager parasut    → seçilen uygulamalar
#   ./denetim.sh --siki                   → varsayılan sıkı denetim (uyumluluk)
# Çıkış kodu: 0 hata yok · 1 hata var ·
#             2 kullanım hatası veya olmayan gözlem formu.
# Betiğin kendi testleri: node denetim-test.cjs
#
# Kural kaynakları:
#   README.md                  → "Gözlem formlarında dil kuralı"
#   MANUEL-TEST-PROTOKOLU.md   → "Gözlem formu yazım kuralları"
# Yapısal kontroldür; içerik doğruluğu (karede yazan ile metinde yazanın aynı
# olması) elle yapılır.
#
# 14 Eyl 2026 (P1-K, B17): eski bash sürümü hata bulsa da 0 dönüyor, olmayan
# formu "temiz" sayıyor, yollu atıfları ve en tireli aralıkları göremiyordu.

set -u
command -v node >/dev/null 2>&1 || { echo "denetim.sh: node bulunamadı" >&2; exit 2; }
exec node "$(dirname "$0")/denetim.cjs" "$@"
