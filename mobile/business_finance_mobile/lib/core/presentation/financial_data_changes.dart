import 'package:flutter/foundation.dart';

/// Tells screens that the data behind them moved, so they reread instead of
/// showing yesterday's numbers.
///
/// This is a refresh signal only. Balances, budgets and reports are always the
/// server's answer; nothing here recomputes them. Each mutation raises exactly
/// the targets it can affect, so a transfer does not make the budget screen
/// refetch work that cannot have changed.
///
/// **Kapsam anahtarının burada bir hedefi yok** ve olmamalı: anahtar veriyi
/// değiştirmez, aynı veriye başka bir soru sorar. Ekranlar onu kendi
/// kanalından (`ScopeController`) dinler. Buraya bağlansaydı her kapsam
/// dokunuşu, kapsamdan etkilenmeyen ekranları (hesaplar, kartlar) da boşuna
/// yeniden yükletirdi.
class FinancialDataChanges extends ChangeNotifier {
  int _activityFeedRevision = 0;
  int _dashboardRevision = 0;
  int _budgetsRevision = 0;
  int _accountsRevision = 0;
  int _cardsRevision = 0;
  int _planningRevision = 0;
  int _counterpartiesRevision = 0;
  int _cashRevision = 0;

  int get activityFeedRevision => _activityFeedRevision;
  int get dashboardRevision => _dashboardRevision;
  int get budgetsRevision => _budgetsRevision;
  int get accountsRevision => _accountsRevision;
  int get cardsRevision => _cardsRevision;
  int get planningRevision => _planningRevision;
  int get counterpartiesRevision => _counterpartiesRevision;

  /// Kasa ekranı: gün sonu sayımı ve POS tahsilatları.
  ///
  /// Hesapları yükselten her olay bunu da yükseltir (`_raise`): Kasa nakit
  /// hesapların bakiyesini gösterir, o bakiyeyi değiştiren her kayıt — gelir,
  /// gider, transfer, cari tahsilat, yükümlülük kapatma, tekrarlayan kalem,
  /// içe aktarım — Kasa'yı da eskitir.
  int get cashRevision => _cashRevision;

  /// Kept for the transaction list, which predates the unified feed and follows
  /// the same signal.
  int get transactionsRevision => _activityFeedRevision;

  /// Opening, renaming or closing an account. No money moved, so only the
  /// balances that list accounts need to reread.
  void accountsChanged() => _raise(accounts: true, dashboard: true);

  /// Income or expense created or cancelled: it moves a balance and counts
  /// towards a category budget.
  void transactionsChanged() =>
      _raise(feed: true, dashboard: true, budgets: true, accounts: true);

  /// Money moved between two of the user's own accounts. Budgets are untouched,
  /// because a transfer is neither income nor expense.
  void transferChanged() => _raise(feed: true, dashboard: true, accounts: true);

  /// A card purchase is an expense and raises the card debt, but leaves account
  /// balances alone.
  void cardSpendingChanged() =>
      _raise(feed: true, dashboard: true, budgets: true, cards: true);

  /// Paying the card settles debt already counted as expense when the charges
  /// happened, so it is not a second expense and budgets stay put.
  void cardPaymentChanged() =>
      _raise(feed: true, dashboard: true, accounts: true, cards: true);

  /// Realizing an installment turns a plan into a real card charge.
  void installmentRealized() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    cards: true,
    planning: true,
  );

  /// A recurring occurrence can realize into either an account transaction or a
  /// card charge, so both sides are refreshed.
  void recurringRealized() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    accounts: true,
    cards: true,
    planning: true,
  );

  /// Confirming imported rows creates real transactions.
  void importConfirmed() =>
      _raise(feed: true, dashboard: true, budgets: true, accounts: true);

  /// Paying a debt instalment or collecting a receivable moves an account
  /// balance and clears an obligation, without touching income or expense.
  /// Borç açılışı ya da taksit ödemesi.
  ///
  /// `budgets` de yükselir: gider kaynaklı bir borcun açılışı kategorili bir
  /// giderdir ve bütçeyi tüketir. Kredi kartı ödemesinin bütçeyi
  /// yükseltmemesiyle karışmasın — orada gider harcama anında yazılır,
  /// burada borcun doğduğu anda.
  void debtChanged() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    accounts: true,
    planning: true,
    // Sözleşme açarken yazılan ad karşı tarafı bulur ya da **kurar**: cari
    // listesi bunu görmeden eski hâlinde kalırdı.
    counterparties: true,
  );

  /// Veresiye satış, vadeli alım, tahsilat ya da bunların iptali.
  ///
  /// `budgets` yükselir çünkü vadeli alım kategorili bir giderdir ve bütçeyi
  /// tüketir; `accounts` yükselir çünkü tahsilat kasayı değiştirir. İkisi aynı
  /// kaydın işi değil ama tek bir sinyalde toplanıyorlar: ekran hangisinin
  /// yazıldığını bilse bile, iptal yolu ikisini birden geri alabiliyor.
  ///
  /// `planning` henüz yükselmez: cari hareket opsiyonel vade taşısa da ortak
  /// planlanan akışa Aşama 03 Grup 4'te bağlanacaktır.
  void counterpartyLedgerChanged() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    accounts: true,
    counterparties: true,
  );

  /// Bir alacak kartla (POS) tahsil edildi ya da bu tahsilat iptal edildi
  /// (ADR 0019 T5).
  ///
  /// Cari (ya da alacak) kapanır, para yola çıkar: `accounts` yükselmez çünkü
  /// hesap kıpırdamadı, Kasa'daki POS bölümü (`cash`) ve net varlık
  /// (`dashboard`) yükselir. Komisyon tahsil günü gider yazıldığı için bütçe de
  /// yenilenir; alacak kapandığı için planlanan görünüm de.
  void cardCollectionChanged() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    planning: true,
    counterparties: true,
    cash: true,
  );

  /// Tek seferlik yükümlülük ekonomik olayı şimdi tanır; kasa ödeme anına kadar
  /// değişmez. Bu yüzden hesaplar değil feed, rapor/bütçe ve planlanan görünüm
  /// yenilenir.
  void obligationRecognized() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    planning: true,
    counterparties: true,
  );

  /// Ödeme/tahsilat ekonomik olayı yeniden tanımaz; yalnız kasa, plan ve cari
  /// görünümü değişir.
  void obligationSettled() => _raise(
    feed: true,
    dashboard: true,
    accounts: true,
    planning: true,
    counterparties: true,
  );

  /// Yükümlülük iptal edildi. İptal bir bütündür: tanınan gelir/gider düşer;
  /// kapanmışsa kapanışın hesaba etkisi de geri alınır, kartla tahsil
  /// edildiyse yoldaki para (`cash`) da. Hangi hâlde iptal edildiği çağıran
  /// yerde her zaman bilinmediği için hepsi yükseltilir.
  void obligationCancelled() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    accounts: true,
    planning: true,
    counterparties: true,
    cash: true,
  );

  /// Gün sonu sayımı yazıldı.
  ///
  /// **Yalnız kasa ekranı yenilenir.** Sayım bir gözlemdir: hiçbir bakiyeyi
  /// değiştirmez ve hiçbir rapora girmez. Bütçeyi yükseltmek, sayılmış parayı
  /// harcanmış göstermek olurdu.
  void cashCountRecorded() => _raise(cash: true);

  /// Sayım farkı onaylandı: bu artık gerçek bir gelir/gider kaydıdır.
  /// Esnaf kasadan kendine para aldı (Aşama 06.3 K9). Şahsi hesaba aktarım
  /// bir transferdir ve bütçeye dokunmaz; şahsi gider olarak yazılırsa
  /// bütçeyi de etkiler.
  void ownerWithdrawalRecorded({required bool asExpense}) => _raise(
    feed: true,
    dashboard: true,
    budgets: asExpense,
    accounts: true,
    cash: true,
  );

  void cashDifferenceConfirmed() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    accounts: true,
    cash: true,
  );

  /// POS tahsilatı yazıldı: satış bugün tanınır, hesap kıpırdamaz.
  ///
  /// `accounts` yükselmez çünkü para henüz hiçbir hesapta değil; `dashboard`
  /// yükselir çünkü net varlık yoldaki parayı taşır. `feed` de yükselir:
  /// birleşik feed tahsilatı satış ve komisyon satırı olarak gösteriyor.
  void posSettlementRecognized() =>
      _raise(feed: true, dashboard: true, budgets: true, cash: true);

  /// POS yatışı yazıldı ya da geri alındı (ADR 0019 T5): para hesaba geçti
  /// ya da yeniden yola döndü. Satış yeniden tanınmaz.
  ///
  /// `feed` yükselir çünkü yatış kendi satırını doğurur. `budgets` yalnız
  /// [deduction] varsa yükselir: banka eksik yatırdıysa fark bir kesinti
  /// **gideridir** ve bütçeyi tüketir; beklendiği kadar yatan para hiçbir
  /// gider tanımaz — bütçeyi yükseltmek aynı satışı iki kez saymak olurdu.
  void posDepositChanged({required bool deduction}) => _raise(
    feed: true,
    dashboard: true,
    budgets: deduction,
    accounts: true,
    cash: true,
  );

  /// Gün sonu yazıldı ya da geri alındı (ADR 0019 T1): nakit satış kasaya
  /// bir gelir, kartlı satış bir POS tahsilatı olur.
  ///
  /// Gelir kasayı değiştirir (`accounts`) ve ikisi de satış tanır (`budgets`,
  /// `dashboard`, `feed`). Kart parası yolda kalır; Kasa'daki POS bölümü de
  /// yenilenir (`cash`).
  void dayCloseChanged() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    accounts: true,
    cash: true,
  );

  /// POS tahsilatı iptal edildi: satış ve komisyon düşer (bütçe), yoldaki
  /// tutar kalkar.
  ///
  /// `accounts` yükselmez: yalnız yoldaki tahsilat iptal edilebilir, hesaba
  /// geçmiş olan önce yatışı geri alınarak yola döner.
  void posSettlementCancelled() =>
      _raise(feed: true, dashboard: true, budgets: true, cash: true);

  /// Vergi ödendi ya da ödeme geri alındı (ADR 0018).
  ///
  /// Ödenen vergi normal bir giderdir: hesaptan ödendiyse hesap, kartla
  /// ödendiyse kart borcu değişir; bütçe ve rapor tanır. Kalem bekleyenlerden
  /// düştüğü (ya da geri döndüğü) için planlanan görünüm de yenilenir.
  void taxPaymentChanged({required bool card}) => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    accounts: !card,
    cards: card,
    planning: true,
  );

  /// Vergi tanımı eklendi, düzenlendi, duraklatıldı, silindi ya da bir
  /// kaleme tutar yazıldı. Para hareket etmedi; yalnız bekleyenler değişti.
  void taxPlansChanged() => _raise(dashboard: true, planning: true);

  /// Karşı tarafın kendisi eklendi, adı değişti, pasifleşti ya da silindi.
  /// Para hareket etmedi; yalnız kişi listesi değişti.
  void counterpartiesChanged() => _raise(counterparties: true);

  /// A restore replaces everything the user has, so every screen is stale.
  void restoreCompleted() => _raise(
    feed: true,
    dashboard: true,
    budgets: true,
    accounts: true,
    cards: true,
    planning: true,
    counterparties: true,
    cash: true,
  );

  void _raise({
    bool feed = false,
    bool dashboard = false,
    bool budgets = false,
    bool accounts = false,
    bool cards = false,
    bool planning = false,
    bool counterparties = false,
    bool cash = false,
  }) {
    if (feed) _activityFeedRevision++;
    if (dashboard) _dashboardRevision++;
    if (budgets) _budgetsRevision++;
    if (accounts) _accountsRevision++;
    if (cards) _cardsRevision++;
    if (planning) _planningRevision++;
    if (counterparties) _counterpartiesRevision++;
    // Kasa bir hesap bakiyesi görünümüdür: hesaplar değiştiyse kasa da
    // değişmiş olabilir. Kural burada tek yerde durur ki yeni bir olay onu
    // unutamasın (28 Eylül denetimi U11: nakit gider ve transfer Kasa'yı
    // yenilemiyordu).
    if (cash || accounts) _cashRevision++;
    notifyListeners();
  }
}
