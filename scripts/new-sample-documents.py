"""Fiş, fatura ve dekont örneklerini üretir (Aşama 06.1 kabul turu).

Fiş okuma akışını elle denemek için her turda telefonla belge aramak gerekiyordu;
bulunan belge de gerçek bir işletmenin gerçek verisi oluyordu. Bu betik onun
yerine sentetik ama gerçekçi belgeler üretir: tutar, tarih, KDV ve satıcı adı
uydurmadır, vergi numaraları geçersiz aralıktadır ve her belgenin altında
sentetik olduğu yazar.

Üretilen belgeler bilerek birbirinden farklıdır — okuma katmanının sınırını
göstermesi için:

  * Yazarkasa fişi     : dar, tek sütun, karışık KDV oranı
  * Akaryakıt fişi     : plaka ve litre satırı, tek kalem
  * Toptan fatura      : tablo düzeni, vade tarihi, tek KDV oranı
  * Elektrik faturası  : son ödeme tarihi taşır (yükümlülük yolu)
  * EFT dekontu        : gider değil ödeme; kategori taşımaz
  * POS dekontu        : brüt, komisyon ve bankaya geçecek tutar
  * Soluk fiş          : düşük kontrast; okunamayan belgede ne oluyor

Çalıştırma (repo kökünden):

    python scripts/new-sample-documents.py

Çıktı `samples/documents/` altına yazılır. Pillow gerekir (`pip install pillow`).
"""

from __future__ import annotations

import csv
import os
import random
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parent.parent
DOCUMENTS = ROOT / "samples" / "documents"
IMPORTS = ROOT / "samples" / "imports"

FOOTER = "SENTETIK TEST BELGESIDIR - GERCEK BIR SATISI TEMSIL ETMEZ"

MONO = "C:/Windows/Fonts/consola.ttf"
MONO_BOLD = "C:/Windows/Fonts/consolab.ttf"
SANS = "C:/Windows/Fonts/arial.ttf"
SANS_BOLD = "C:/Windows/Fonts/arialbd.ttf"


def font(path: str, size: int) -> ImageFont.FreeTypeFont:
    try:
        return ImageFont.truetype(path, size)
    except OSError:
        return ImageFont.load_default()


def money(value: float) -> str:
    return f"{value:,.2f}".replace(",", "X").replace(".", ",").replace("X", ".")


class Sheet:
    """Satır satır yazılan bir belge yüzeyi."""

    def __init__(self, width: int, height: int, background: int = 250) -> None:
        self.image = Image.new("RGB", (width, height), (background, background, background - 2))
        self.draw = ImageDraw.Draw(self.image)
        self.width = width
        self.y = 0

    def line(self, text: str, f: ImageFont.FreeTypeFont, x: int = 40,
             gap: int = 6, fill: tuple[int, int, int] = (25, 25, 25),
             center: bool = False, right: bool = False) -> None:
        if center or right:
            span = self.draw.textlength(text, font=f)
            x = int((self.width - span) / 2) if center else int(self.width - span - x)
        self.draw.text((x, self.y), text, font=f, fill=fill)
        self.y += f.size + gap

    def rule(self, char: str = "-", f: ImageFont.FreeTypeFont | None = None) -> None:
        f = f or font(MONO, 20)
        count = int((self.width - 80) / max(self.draw.textlength(char, font=f), 1))
        self.line(char * count, f)

    def space(self, pixels: int = 14) -> None:
        self.y += pixels

    def finish(self, name: str, quality: int = 88, blur: bool = False) -> Path:
        footer = font(MONO, 15)
        self.draw.text((40, self.image.height - 40), FOOTER, font=footer, fill=(150, 150, 150))
        image = self.image
        if blur:
            # Soluk fiş: düşük kontrast ve hafif gürültü. Okuma katmanının
            # "okuyamadım" yolunu denemek için bir belge gerekiyor.
            faded = Image.blend(image, Image.new("RGB", image.size, (255, 255, 255)), 0.55)
            pixels = faded.load()
            rng = random.Random(20260831)
            for _ in range(int(image.width * image.height * 0.02)):
                x = rng.randrange(image.width)
                y = rng.randrange(image.height)
                shade = rng.randrange(200, 255)
                pixels[x, y] = (shade, shade, shade)
            image = faded
        target = DOCUMENTS / name
        image.save(target, quality=quality)
        return target


def market_receipt() -> Path:
    """Yazarkasa fişi: çok kalem, iki ayrı KDV oranı, nakit ödeme."""
    sheet = Sheet(760, 1180)
    big = font(MONO_BOLD, 30)
    body = font(MONO, 22)
    small = font(MONO, 19)

    sheet.space(30)
    sheet.line("ORNEK MARKET", big, center=True)
    sheet.line("Cumhuriyet Mah. 118. Sk. No:7", small, center=True)
    sheet.line("Ornek / Test", small, center=True)
    sheet.line("VKN 0000000001", small, center=True)
    sheet.space(10)
    sheet.rule("=")
    sheet.line("TARIH 12.08.2026        SAAT 18:42", body)
    sheet.line("FIS NO 0417             KASA 02", body)
    sheet.rule("=")

    items = [
        ("EKMEK 5 AD", 2, 12.50, 25.00),
        ("SUT 1LT", 3, 34.90, 104.70),
        ("YUMURTA 15LI", 1, 89.00, 89.00),
        ("DETERJAN 2.5KG", 1, 189.90, 189.90),
        ("KAGIT HAVLU 8LI", 1, 149.50, 149.50),
        ("CAY 1KG", 1, 219.00, 219.00),
    ]
    for name, qty, unit, total in items:
        sheet.line(name, body)
        sheet.line(f"  {qty} x {money(unit)}{money(total).rjust(24)}", body)

    sheet.rule("-")
    subtotal = sum(item[3] for item in items)
    sheet.line(f"ARA TOPLAM{money(subtotal).rjust(26)}", body)
    sheet.line(f"KDV %1{money(1.19).rjust(30)}", body)
    sheet.line(f"KDV %10{money(59.30).rjust(29)}", body)
    sheet.rule("=")
    sheet.line(f"TOPLAM{money(subtotal).rjust(24)}", font(MONO_BOLD, 26))
    sheet.rule("=")
    sheet.line(f"NAKIT{money(800.00).rjust(31)}", body)
    sheet.line(f"PARA USTU{money(800.00 - subtotal).rjust(27)}", body)
    sheet.space(10)
    sheet.line("MALI DEGERI YOKTUR", small, center=True)
    return sheet.finish("fis-market-01.jpg")


def fuel_receipt() -> Path:
    """Akaryakıt fişi: tek kalem, litre ve plaka satırı."""
    sheet = Sheet(700, 900)
    big = font(MONO_BOLD, 28)
    body = font(MONO, 22)
    small = font(MONO, 19)

    sheet.space(30)
    sheet.line("ORNEK PETROL", big, center=True)
    sheet.line("Sanayi Cad. No:214", small, center=True)
    sheet.line("VKN 0000000002", small, center=True)
    sheet.rule("=")
    sheet.line("TARIH 19.08.2026   SAAT 08:15", body)
    sheet.line("POMPA 3            FIS 118204", body)
    sheet.rule("-")
    sheet.line("MOTORIN", body)
    sheet.line("  32,40 LT x 51,25", body)
    sheet.line(f"{money(1660.50).rjust(30)}", body)
    sheet.line("PLAKA 34 ABC 118", body)
    sheet.rule("-")
    sheet.line(f"KDV %20{money(276.75).rjust(23)}", body)
    sheet.rule("=")
    sheet.line(f"TOPLAM{money(1660.50).rjust(20)}", font(MONO_BOLD, 26))
    sheet.rule("=")
    sheet.line("KREDI KARTI", body)
    sheet.line("**** **** **** 4417", body)
    return sheet.finish("fis-akaryakit-02.jpg")


def wholesale_invoice() -> Path:
    """Toptan alım faturası: tablo düzeni, vade tarihi, tek KDV oranı."""
    sheet = Sheet(1000, 1320, background=253)
    title = font(SANS_BOLD, 34)
    head = font(SANS_BOLD, 22)
    body = font(SANS, 21)
    small = font(SANS, 18)

    sheet.space(36)
    sheet.line("E-ARSIV FATURA", title, center=True)
    sheet.space(10)
    sheet.line("ORNEK TOPTAN KAGIT VE KIRTASIYE LTD. STI.", head)
    sheet.line("Ticaret Sitesi 4. Blok No:12 - Ornek / Test", small)
    sheet.line("VKN 0000000003   Vergi Dairesi: Ornek", small)
    sheet.space(16)
    sheet.line("SAYIN", small)
    sheet.line("VERGI KABUL KIRTASIYE", body)
    sheet.line("VKN 0000000009", small)
    sheet.space(16)
    sheet.line("Fatura No   : ORN2026000000418", body)
    sheet.line("Duzenleme   : 18.08.2026", body)
    sheet.line("Vade Tarihi : 17.09.2026", body)
    sheet.space(18)

    sheet.draw.line((40, sheet.y, sheet.width - 40, sheet.y), fill=(120, 120, 120), width=2)
    sheet.space(10)
    sheet.line("ACIKLAMA                     MIKTAR    BIRIM F.      TUTAR", head)
    sheet.space(6)
    sheet.draw.line((40, sheet.y, sheet.width - 40, sheet.y), fill=(190, 190, 190), width=1)
    sheet.space(10)

    rows = [
        ("A4 Fotokopi Kagidi 80gr", "40 TOP", 172.50, 6900.00),
        ("Tukenmez Kalem (50li)", "12 KUTU", 245.00, 2940.00),
        ("Klasor Genis", "60 AD", 68.75, 4125.00),
        ("Post-it Blok", "24 PK", 58.00, 1392.00),
    ]
    for name, qty, unit, total in rows:
        line = f"{name[:28].ljust(29)}{qty.rjust(7)}{money(unit).rjust(13)}{money(total).rjust(13)}"
        sheet.line(line, font(SANS, 20))

    sheet.space(10)
    sheet.draw.line((40, sheet.y, sheet.width - 40, sheet.y), fill=(120, 120, 120), width=2)
    sheet.space(12)

    net = sum(row[3] for row in rows)
    vat = round(net * 0.20, 2)
    sheet.line(f"Mal Hizmet Toplam{money(net).rjust(37)}", body)
    sheet.line(f"Hesaplanan KDV %20{money(vat).rjust(36)}", body)
    sheet.space(6)
    sheet.line(f"VERGILER DAHIL TOPLAM{money(net + vat).rjust(30)}", font(SANS_BOLD, 24))
    sheet.space(20)
    sheet.line("Odeme sekli: Vadeli (cari hesaba islenecektir)", small)
    return sheet.finish("fatura-toptan-kagit-01.jpg")


def utility_invoice() -> Path:
    """Elektrik faturası: son ödeme tarihi taşır, henüz ödenmemiştir."""
    sheet = Sheet(960, 1180, background=252)
    title = font(SANS_BOLD, 32)
    head = font(SANS_BOLD, 22)
    body = font(SANS, 21)
    small = font(SANS, 18)

    sheet.space(36)
    sheet.line("ORNEK ELEKTRIK PERAKENDE SATIS A.S.", title, center=True)
    sheet.space(8)
    sheet.line("ISYERI ELEKTRIK FATURASI", head, center=True)
    sheet.space(20)
    sheet.line("Tesisat No     : 40000001180", body)
    sheet.line("Abone          : VERGI KABUL KIRTASIYE", body)
    sheet.line("Fatura No      : EL2026-0000914", body)
    sheet.line("Donem          : 01.08.2026 - 31.08.2026", body)
    sheet.line("Duzenleme      : 29.08.2026", body)
    sheet.space(18)
    sheet.draw.line((40, sheet.y, sheet.width - 40, sheet.y), fill=(120, 120, 120), width=2)
    sheet.space(12)
    sheet.line("Ilk endeks     : 118.420 kWh", body)
    sheet.line("Son endeks     : 119.164 kWh", body)
    sheet.line("Tuketim        : 744 kWh", body)
    sheet.space(12)
    sheet.line(f"Enerji bedeli{money(2604.00).rjust(38)}", body)
    sheet.line(f"Dagitim bedeli{money(688.40).rjust(36)}", body)
    sheet.line(f"KDV %20{money(658.48).rjust(44)}", body)
    sheet.space(10)
    sheet.draw.line((40, sheet.y, sheet.width - 40, sheet.y), fill=(120, 120, 120), width=2)
    sheet.space(12)
    sheet.line(f"ODENECEK TUTAR{money(3950.88).rjust(29)}", font(SANS_BOLD, 26))
    sheet.space(16)
    sheet.line("SON ODEME TARIHI : 12.09.2026", font(SANS_BOLD, 24))
    sheet.space(14)
    sheet.line("Bu fatura odenmemistir.", small)
    return sheet.finish("fatura-elektrik-02.jpg")


def transfer_slip() -> Path:
    """EFT dekontu: ödemeyi taşır, gelir/gider üretmez."""
    sheet = Sheet(920, 1080, background=254)
    title = font(SANS_BOLD, 32)
    head = font(SANS_BOLD, 22)
    body = font(SANS, 21)
    small = font(SANS, 18)

    sheet.space(36)
    sheet.line("ORNEK BANK", title, center=True)
    sheet.space(6)
    sheet.line("HAVALE / EFT DEKONTU", head, center=True)
    sheet.space(24)
    sheet.line("Islem Tarihi   : 26.08.2026 14:07", body)
    sheet.line("Dekont No      : 2026082600418812", body)
    sheet.space(16)
    sheet.line("GONDEREN", head)
    sheet.line("VERGI KABUL KIRTASIYE", body)
    sheet.line("TR00 0000 0000 0000 0000 0000 01", body)
    sheet.space(16)
    sheet.line("ALICI", head)
    sheet.line("ORNEK TOPTAN KAGIT VE KIRTASIYE LTD. STI.", body)
    sheet.line("TR00 0000 0000 0000 0000 0000 02", body)
    sheet.space(20)
    sheet.draw.line((40, sheet.y, sheet.width - 40, sheet.y), fill=(120, 120, 120), width=2)
    sheet.space(14)
    sheet.line(f"TUTAR{money(7500.00).rjust(35)} TL", font(SANS_BOLD, 28))
    sheet.line(f"Islem ucreti{money(0.00).rjust(32)} TL", body)
    sheet.space(14)
    sheet.line("Aciklama: Agustos cari hesap odemesi", body)
    sheet.space(10)
    sheet.line("Islem basariyla gerceklestirilmistir.", small)
    return sheet.finish("dekont-eft-01.jpg")


def pos_slip() -> Path:
    """POS gün sonu dekontu: brüt, komisyon ve bankaya geçecek tutar."""
    sheet = Sheet(760, 1020)
    big = font(MONO_BOLD, 28)
    body = font(MONO, 22)
    small = font(MONO, 18)

    sheet.space(30)
    sheet.line("ORNEK BANK POS", big, center=True)
    sheet.line("GUN SONU RAPORU", body, center=True)
    sheet.rule("=")
    sheet.line("UYE ISYERI 000000000001180", small)
    sheet.line("TERMINAL   OR000418", small)
    sheet.line("TARIH      29.08.2026", body)
    sheet.rule("-")
    sheet.line("PESIN SATIS      14 ADET", body)
    sheet.line(f"{money(18450.00).rjust(30)}", body)
    sheet.line("TAKSITLI SATIS    3 ADET", body)
    sheet.line(f"{money(4200.00).rjust(30)}", body)
    sheet.rule("-")
    sheet.line(f"BRUT TOPLAM{money(22650.00).rjust(19)}", font(MONO_BOLD, 24))
    sheet.line(f"KOMISYON %1,75{money(396.38).rjust(16)}", body)
    sheet.rule("=")
    sheet.line("BLOKE COZULME 02.09.2026", body)
    sheet.line(f"NET TRANSFER{money(22253.62).rjust(18)}", font(MONO_BOLD, 24))
    sheet.rule("=")
    return sheet.finish("dekont-pos-02.jpg")


def faded_receipt() -> Path:
    """Soluk fiş: okuma katmanının 'okuyamadım' yolunu denemek için."""
    sheet = Sheet(700, 820)
    body = font(MONO, 22)
    small = font(MONO, 19)

    sheet.space(40)
    sheet.line("ORNEK KIRTASIYE", font(MONO_BOLD, 26), center=True)
    sheet.line("VKN 0000000004", small, center=True)
    sheet.rule("=")
    sheet.line("TARIH 24.08.2026", body)
    sheet.line("FIS NO 0091", body)
    sheet.rule("-")
    sheet.line("TONER KARTUS", body)
    sheet.line(f"  1 x {money(1890.00)}{money(1890.00).rjust(14)}", body)
    sheet.line("ZIMBA TELI", body)
    sheet.line(f"  4 x {money(32.50)}{money(130.00).rjust(16)}", body)
    sheet.rule("-")
    sheet.line(f"KDV %20{money(336.67).rjust(23)}", body)
    sheet.rule("=")
    sheet.line(f"TOPLAM{money(2020.00).rjust(20)}", font(MONO_BOLD, 24))
    return sheet.finish("fis-kirtasiye-03-soluk.jpg", quality=45, blur=True)


def bank_statement() -> Path:
    """İçe aktarma için banka ekstresi.

    Uygulamanın kendi dışa aktarımı değildir ve kapsam kolonu taşımaz — içe
    aktarma bir banka ekstresi ayrıştırıcısıdır ve kapsamı zincirden çözer.
    Son iki satır bilerek fikstürdeki hareketlerle aynıdır: çift kayıt
    uyarısının gerçekten çıktığı görülsün.
    """
    rows = [
        ("2026-08-03", "-1250.00", "OFIS TEMIZLIK HIZMETI", "REF20260803001"),
        ("2026-08-05", "-18500.00", "ISYERI KIRA ODEMESI AGUSTOS", "REF20260805002"),
        ("2026-08-07", "9400.00", "GELEN HAVALE - OKUL KOOPERATIFI", "REF20260807003"),
        ("2026-08-11", "-2260.00", "AKARYAKIT ORNEK PETROL", "REF20260811004"),
        ("2026-08-14", "-980.50", "INTERNET VE TELEFON", "REF20260814005"),
        ("2026-08-18", "12750.00", "POS GUN SONU AKTARIMI", "REF20260818006"),
        ("2026-08-21", "-4100.00", "TOPTAN KAGIT ALIMI", "REF20260821007"),
        ("2026-08-24", "-1590.00", "SGK PRIM ODEMESI", "REF20260824008"),
        ("2026-08-26", "-7500.00", "EFT - ORNEK TOPTAN KAGIT", "REF20260826009"),
        ("2026-08-28", "3300.00", "GELEN HAVALE - MAHALLE KIRTASIYE", "REF20260828010"),
    ]

    IMPORTS.mkdir(parents=True, exist_ok=True)
    target = IMPORTS / "banka-ekstresi-agustos-2026.csv"
    with open(target, "w", encoding="utf-8-sig", newline="") as handle:
        writer = csv.writer(handle, delimiter=";")
        writer.writerow(["Tarih", "Tutar", "Aciklama", "Referans"])
        writer.writerows(rows)
    return target


def main() -> None:
    DOCUMENTS.mkdir(parents=True, exist_ok=True)
    produced = [
        market_receipt(),
        fuel_receipt(),
        wholesale_invoice(),
        utility_invoice(),
        transfer_slip(),
        pos_slip(),
        faded_receipt(),
        bank_statement(),
    ]
    for path in produced:
        size = os.path.getsize(path)
        print(f"{path.relative_to(ROOT).as_posix():<52} {size / 1024:>7.1f} KB")


if __name__ == "__main__":
    main()
