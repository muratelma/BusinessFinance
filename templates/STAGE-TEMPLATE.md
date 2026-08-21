# Aşama <numara> — <Başlık>

## Belge durumu

- Durum: Planlandı | Aktif | Tamamlandı
- Ön koşul: Aşama <önceki numara> — <başlık>
- Sonraki aşama: Aşama <sonraki numara> — <başlık veya "henüz açılmadı">
- Dokunulacak kalıcı belgeler: `documentation/architecture.md`,
  `documentation/flows.md`, `documentation/permissions.md`,
  `documentation/tests.md`, gerekiyorsa `documentation/adr/`
- Doğrulanmış ilerleme: `docs/project-status.md`

## Amaç

Bu aşamanın çözdüğü problem, iki-üç cümle. Hangi eksik davranış tamamlanıyor?

## Kullanıcıya katkı

Uygulamayı kullanan kişi bu aşama bittiğinde neyi yapabiliyor olacak? Ekran ve
akış düzeyinde somut yaz; "altyapı iyileşir" gibi cümle kullanma.

## Değiştirilmeyecek mimari kararlar

Bu aşamada korunacak mevcut invariant'lar (ör. bakiye kalıcı kolon değildir,
kart ödemesi gider üretmez, silme yerine iptal). Yeni aşama bunları bozarsa
önce ADR yazılır.

## Kapsam

### Dahil

- …

### Açıkça kapsam dışında

- …

## Çalışma grupları

Her grup tek amaçlı, derlenebilir ve testi geçen bir checkpoint'tir.

### Grup 1 — <ad>

- Yapılacak iş
- Üretilecek/değişecek dosyalar
- Bu grubun tamamlanma ölçütü

### Grup 2 — <ad>

- …

## Zorunlu testler

### Domain / Application

- …

### API ve gerçek SQL

- Kullanıcı izolasyonu için pozitif **ve** negatif senaryo
- …

### Flutter

- Controller/repository testleri, loading-empty-error-unauthorized durumları
- …

## Kalite komutları

```bash
dotnet build BusinessFinance.slnx --configuration Release --no-restore
dotnet test BusinessFinance.slnx --configuration Release --no-restore
dotnet format BusinessFinance.slnx --verify-no-changes --no-restore
```

```bash
flutter analyze
flutter test
flutter build apk --debug --dart-define=API_BASE_URL=http://10.0.2.2:5284
```

## Belge güncellemeleri

Uygulama ilerledikçe güncellenecekler (`AGENTS.md` belge güncelleme haritası):

- `documentation/architecture.md`: <ne değişiyor>
- `documentation/flows.md`: <hangi akış>
- `documentation/permissions.md`: <yeni endpoint / sahiplik sınırı>
- `documentation/tests.md`: <test haritası>
- `documentation/adr/`: <kalıcı karar varsa>
- `docs/project-status.md`: yalnız doğrulanmış checkpoint'ler

Henüz uygulanmamış davranış, uygulanmış gibi yazılmaz.

## Güvenlik ve veri sınırları

- Bütün sorgular current-user kapsamlıdır.
- Yalnız sentetik veri kullanılır.
- Secret, connection string ve token belgeye veya loga yazılmaz.

## Riskler ve azaltımlar

| Risk | Azaltım |
|---|---|
| … | … |

## Çıkış koşulları

- [ ] Bütün çalışma grupları tamamlandı.
- [ ] Backend build, test ve format kontrolleri geçti.
- [ ] Flutter analyze, test ve build kontrolleri geçti.
- [ ] Kullanıcı izolasyonu negatif senaryolarla kanıtlandı.
- [ ] `documentation/` ve `docs/project-status.md` güncel.
- [ ] Manuel kabul adımları (gerekiyorsa) tamamlandı.
- [ ] Kullanıcı sonraki aşamayı açıkça onayladı.

## Tamamlanma kaydı

Aşama kapandığında: hangi commit'lerle bitti, hangi kontroller geçti, belge
`docs/archive/stages/` altına taşındı mı.
