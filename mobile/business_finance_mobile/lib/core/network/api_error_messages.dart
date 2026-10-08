/// Sunucu hata kodunu kullanıcıya gösterilecek Türkçe cümleye çevirir.
///
/// Kural: **kullanıcıya gösterilecek cümleyi API değil istemci üretir.** API
/// kararlı bir makine kodu (`transactions.not_found`) gönderir; o kodun hangi
/// dilde, hangi tonda ve hangi yönlendirmeyle okunacağı istemcinin işidir.
/// Sunucunun `detail` alanı bu yüzden **hiçbir koşulda** ekrana yazılmaz:
/// çoğu İngilizce yazılmıştır, hepsi geliştiriciye konuşur ve hiçbiri
/// kullanıcının o ekranda ne yapabileceğini bilmez.
///
/// Çözüm üç katmanlıdır ve ilk tutan kazanır:
///
/// 1. **Tam kod** — kendi cümlesini hak eden kod ([_exact]). Kullanıcının ne
///    yapacağını söyleyebildiğimiz her yerde bu katman kullanılır.
/// 2. **Kalıp** — kodlar `<alan>.<sebep>` biçimindedir ve sebep tarafı tekrar
///    eder (`not_found`, `validation`, `scope_unresolved`, `invalid_*`).
///    Alanın Türkçe adı ([_subjects]) sebebin şablonuna konur.
/// 3. **Nötr yedek** — hiçbiri tutmazsa [neutral]. Yeni bir sunucu kodu
///    istemci güncellenmeden çıktığında kullanıcı İngilizce bir cümle değil,
///    anlamlı ve nötr bir cümle görür.
library;

abstract final class ApiErrorMessages {
  /// Karşılığı olmayan kod için gösterilen cümle.
  ///
  /// Bilmediğimiz bir hatayı tarif etmeye çalışmaz — yalnız işlemin
  /// tamamlanmadığını ve tekrar denenebileceğini söyler.
  static const String neutral = 'İşlem tamamlanamadı. Lütfen tekrar deneyin.';

  static String resolve(String code) {
    final exact = _exact[code];
    if (exact != null) return exact;

    final separator = code.indexOf('.');
    if (separator <= 0 || separator == code.length - 1) return neutral;

    return _byPattern(
          code.substring(0, separator),
          code.substring(separator + 1),
        ) ??
        neutral;
  }

  /// `<alan>.<sebep>` kalıbından cümle kurar.
  static String? _byPattern(String area, String reason) {
    final subject = _subjects[area];

    final shared = _sharedReasons[reason];
    if (shared != null) {
      return subject == null ? shared : '$subject: $shared';
    }

    final withSubject = _subjectReasons[reason];
    if (withSubject != null && subject != null) {
      return withSubject(subject);
    }

    if (reason.startsWith('invalid_')) {
      final field = _fields[reason.substring('invalid_'.length)];
      if (field != null) {
        return 'Girilen $field geçersiz. Kontrol edip tekrar deneyin.';
      }
      return subject == null
          ? neutral
          : '$subject bilgilerinde geçersiz bir alan var. '
                'Kontrol edip tekrar deneyin.';
    }

    return null;
  }

  /// Kodun alan tarafının Türkçe adı — cümlenin öznesi.
  ///
  /// `account` (kullanıcının kendi hesabı) ile `accounts` (banka/kasa hesabı)
  /// bilerek burada değil: ikisi aynı kelimeyle biten farklı şeyler, ikisi de
  /// kendi cümlesini [_exact] içinde alır.
  static const Map<String, String> _subjects = {
    'accounts': 'Hesap',
    'categories': 'Kategori',
    'transactions': 'İşlem',
    'transfers': 'Transfer',
    'budgets': 'Bütçe',
    'credit_cards': 'Kredi kartı',
    'credit_card_charges': 'Kart harcaması',
    'installments': 'Taksit planı',
    'recurring': 'Tekrarlayan plan',
    'taxes': 'Vergi',
    'tax_payments': 'Vergi ödemesi',
    'counterparties': 'Cari hesap',
    'counterparty_charges': 'Cari borçlandırma',
    'counterparty_payments': 'Cari tahsilat',
    'debt': 'Borç/alacak kaydı',
    'obligations': 'Yükümlülük',
    'goal': 'Tasarruf hedefi',
    'pos_settlements': 'POS tahsilatı',
    'pos_deposits': 'POS yatışı',
    'pos_definitions': 'POS',
    'cash_counts': 'Kasa sayımı',
    'imports': 'İçe aktarma',
    'backup': 'Yedek',
    'restore': 'Geri yükleme',
    'attachment': 'Belge',
    'reports': 'Rapor',
    'financial_activities': 'İşlem akışı',
    'planned_activities': 'Planlanan işlem',
    'upcoming_payments': 'Yaklaşan ödemeler',
    'history': 'Geçmiş',
  };

  /// Öznesiz de anlamlı olan sebepler.
  static const Map<String, String> _sharedReasons = {
    'scope_unresolved':
        'Kaydın işletme mi şahsi mi olduğu belirlenemedi. '
        'Kapsamı seçip tekrar deneyin.',
    'scope_conflict':
        'Bu kategori bu kayıtta kullanılamıyor: kategori bir tarafa özel. '
        'Başka bir kategori seçip tekrar deneyin.',
    'validation':
        'Gönderilen bilgilerde eksik veya hatalı alan var. '
        'Kontrol edip tekrar deneyin.',
    'conflict':
        'Bu işlem kaydın güncel durumuyla çelişiyor. '
        'Ekranı yenileyip tekrar deneyin.',
    'forbidden': 'Bu işlem için yetkiniz bulunmuyor.',
    'authentication_required':
        'Oturumunuz sona erdi. Lütfen yeniden giriş yapın.',
    'account_unavailable':
        'Seçilen hesap bu işlem için kullanılamıyor. '
        'Listeden aktif bir hesap seçin.',
    'category_unavailable':
        'Seçilen kategori bu işlem için kullanılamıyor. '
        'Listeden uygun türde aktif bir kategori seçin.',
    'counterparty_unavailable':
        'Seçilen cari hesap bu işlem için kullanılamıyor.',
    'card_unavailable': 'Seçilen kredi kartı bulunamadı veya pasif durumda.',
    'commission_category_unavailable':
        'Komisyon için aktif bir gider kategorisi seçmeniz gerekiyor.',
    'mapping_unavailable':
        'Seçilen hesap veya kategori artık kullanılamıyor. '
        'Eşleştirmeyi yenileyin.',
    'file_required': 'Önce bir dosya seçmeniz gerekiyor.',
    'file_too_large': 'Dosya boyut sınırını aşıyor.',
    'payload_too_large': 'Gönderilen veri boyut sınırını aşıyor.',
    'invalid_content_type': 'Dosya türü desteklenmiyor.',
    'invalid_file_type': 'Dosya türü desteklenmiyor.',
    'unsafe_file': 'Dosya güvenlik denetiminden geçemedi.',
    'entity_limit_exceeded': 'Kayıt sayısı desteklenen sınırın üzerinde.',
  };

  /// Özneyle birlikte kurulan sebepler.
  static const Map<String, String Function(String)> _subjectReasons = {
    'not_found': _notFound,
    'duplicate_name': _duplicateName,
    'inactive': _inactive,
    'in_use': _inUse,
  };

  static String _notFound(String subject) =>
      '$subject bulunamadı. Listeyi yenileyip tekrar deneyin.';

  static String _duplicateName(String subject) =>
      'Bu adı taşıyan bir $subject kaydı zaten var. Farklı bir ad seçin.';

  static String _inactive(String subject) => '$subject pasif durumda.';

  static String _inUse(String subject) =>
      '$subject başka kayıtlar tarafından kullanıldığı için silinemez.';

  /// `invalid_<alan>` kodlarının alan adı.
  static const Map<String, String> _fields = {
    'amount': 'tutar',
    'total_amount': 'toplam tutar',
    'gross_amount': 'brüt tutar',
    'counted_amount': 'sayılan tutar',
    'commission_amount': 'komisyon tutarı',
    'opening_balance': 'açılış bakiyesi',
    'limit': 'limit',
    'contribution': 'katkı tutarı',
    'currency': 'para birimi',
    'date': 'tarih',
    'charge_date': 'harcama tarihi',
    'payment_date': 'ödeme tarihi',
    'due_date': 'son ödeme tarihi',
    'issue_date': 'belge tarihi',
    'settlement_date': 'hakediş tarihi',
    'transfer_date': 'aktarma tarihi',
    'expected_transfer_date': 'beklenen aktarma tarihi',
    'count_date': 'sayım tarihi',
    'scheduled_date': 'planlanan tarih',
    'first_date': 'ilk tarih',
    'through_date': 'bitiş tarihi',
    'as_of_date': 'tarih',
    'scope': 'kapsam',
    'default_scope': 'varsayılan kapsam',
    'type': 'tür',
    'kind': 'tür',
    'direction': 'yön',
    'frequency': 'sıklık',
    'commission_rate': 'komisyon oranı',
    'minimum_payment_rate': 'asgari ödeme oranı',
    'month_end_behavior': 'ay sonu davranışı',
    'period': 'dönem',
    'date_range': 'tarih aralığı',
    'decimal_separator': 'ondalık ayracı',
    'date_format': 'tarih biçimi',
    'format': 'biçim',
  };

  /// Kendi cümlesini hak eden kodlar.
  ///
  /// Ölçüt: kalıptan çıkan cümle kullanıcıya **ne yapacağını** söyleyemiyorsa
  /// kod buraya yazılır.
  static const Map<String, String> _exact = {
    // Kimlik ve oturum
    'authentication.invalid_credentials': 'E-posta veya parola geçersiz.',
    'authentication.required':
        'Oturumunuz sona erdi. Lütfen yeniden giriş yapın.',
    'auth.authentication_required':
        'Oturumunuz sona erdi. Lütfen yeniden giriş yapın.',
    'authentication.invalid_refresh_token':
        'Oturum yenilenemedi. Lütfen yeniden giriş yapın.',
    'authentication.registration_conflict':
        'Bu e-posta adresiyle daha önce kayıt oluşturulmuş.',
    'authentication.password_policy':
        'Parola güvenlik kurallarını karşılamıyor.',
    'authentication.invalid_registration':
        'Kayıt bilgilerini kontrol edip tekrar deneyin.',
    'authentication.invalid_reset_code':
        'Sıfırlama kodu geçersiz veya süresi dolmuş. Yeni bir kod isteyin.',
    'authorization.forbidden': 'Bu işlem için yetkiniz bulunmuyor.',

    // Kullanıcının kendi hesabı
    'account.invalid_password': 'Parolanız doğrulanamadı.',
    'account.invalid_verification_code':
        'Doğrulama kodu geçersiz veya süresi dolmuş. Yeni bir kod isteyin.',
    'account.verification_code_too_soon':
        'Az önce bir kod gönderildi. Yeni kod istemek için biraz bekleyin.',
    'account.session_not_found':
        'Bu oturum artık açık değil. Listeyi yenileyin.',
    'account.delete_not_confirmed':
        'Hesap silme onaylanmadı. Silmek için onay kutusunu işaretleyin.',

    // Genel
    'request.invalid_format': 'Gönderilen bilgilerin biçimi geçersiz.',
    'server.unexpected_error':
        'Beklenmeyen bir sunucu hatası oluştu. Lütfen tekrar deneyin.',
    'server.request_failed': neutral,
    'rate_limit.exceeded':
        'Çok fazla deneme yapıldı. Bir süre sonra tekrar deneyin.',

    // Hesap, kategori
    'accounts.in_use':
        'Bu hesap finansal kayıtlarda kullanıldığı için silinemez. '
        'Pasife alabilirsiniz.',
    'accounts.forbidden': 'Bu hesap üzerinde işlem yapma yetkiniz yok.',

    // Bütçe
    'budgets.duplicate_period': 'Bu kategori için bu ayda zaten bir bütçe var.',
    'budgets.category_unavailable':
        'Bütçe yalnız aktif bir gider kategorisine konabilir.',

    // Kredi kartı
    'credit_cards.limit_exceeded':
        'Bu harcama kartın kullanılabilir limitini aşıyor.',
    'credit_cards.payment_exceeds_debt':
        'Ödeme tutarı kartın güncel borcundan büyük olamaz.',
    'credit_cards.charge_not_found':
        'Kart harcaması bulunamadı. Listeyi yenileyip tekrar deneyin.',
    'credit_cards.payment_not_found':
        'Kart ödemesi bulunamadı. Listeyi yenileyip tekrar deneyin.',

    // İptal kilidi — aynı cümle iki koddan gelir
    'transactions.cancel_origin_locked':
        'Bu hareket bir plandan doğduğu için tek başına iptal edilemez. '
        'Planı üzerinden düzenleyin.',
    'credit_card_charges.cancel_origin_locked':
        'Bu harcama bir plandan ya da taksitten doğduğu için tek başına '
        'iptal edilemez. Planı üzerinden düzenleyin.',

    // Tekrarlayan plan
    'recurring.has_realized_history':
        'Bu plan daha önce gerçekleşmiş hareket ürettiği için silinemez. '
        'Durdurmak için planı duraklatabilirsiniz.',
    'recurring.schedule_inactive':
        'Plan pasif olduğu için bu kayıt gerçekleştirilemez. '
        'Önce planı yeniden başlatın.',
    'recurring.not_due_yet': 'Bu kaydın planlanan tarihi henüz gelmedi.',
    'recurring.income_card_source_not_supported':
        'Gelir yalnız bir hesaba bağlanabilir; kaynak olarak kredi kartı '
        'seçilemez.',
    'recurring.invalid_source':
        'Kaynak olarak ya bir hesap ya bir kredi kartı seçin; ikisi birden '
        'olmaz.',
    'recurring.card_limit_insufficient':
        'Kartın kullanılabilir limiti bu kayıt için yetmiyor.',
    'recurring.occurrence_not_found':
        'Planlanan kayıt bulunamadı. Listeyi yenileyip tekrar deneyin.',
    'recurring.amount_required': 'Ödediğiniz tutarı yazın.',
    'recurring.source_required': 'Nereden ödendiğini seçin.',
    'recurring.paid_on_in_future': 'Ödeme günü ileri bir tarih olamaz.',
    'recurring.reschedule_before_history':
        'Yeni başlangıç son ödenen kalemden sonra olmalı.',
    'recurring.already_settled': 'Bu kalem zaten ödendi ya da kapatıldı.',
    'recurring.closed_by_payment':
        'Bu kalem toplu bir ödemeyle kapatıldı; geri alma o ödemeden yapılır.',
    'recurring.category_not_tax':
        'Vergi, vergi işaretli bir gider kategorisine yazılır.',
    'recurring.concurrent_change':
        'Kayıt aynı anda değişti. Ekranı yenileyip tekrar deneyin.',

    // Vergi
    'tax_payments.category_not_tax':
        'Vergi, vergi işaretli bir gider kategorisine yazılır.',
    'tax_payments.paid_on_in_future': 'Ödeme günü ileri bir tarih olamaz.',
    'tax_payments.card_limit_insufficient':
        'Kartın kullanılabilir limiti bu ödeme için yetmiyor.',
    'tax_payments.item_not_pending':
        'Seçilen kalemlerden biri zaten ödendi ya da kapatıldı. '
        'Listeyi yenileyin.',
    'tax_payments.item_inactive':
        'Duraklatılmış bir verginin kalemi kapatılamaz.',
    'tax_payments.undo_origin_locked':
        'Bu kayıt bir taksit planına ait; geri alma kendi ekranından yapılır.',
    'tax_payments.concurrent_change':
        'Kayıt aynı anda değişti. Ekranı yenileyip tekrar deneyin.',
    'taxes.plan_not_found': 'Vergi tanımı bulunamadı.',

    // Cari hesap
    'counterparties.has_history':
        'Hareketi olan cari hesap silinemez. Pasife alabilirsiniz.',
    'counterparties.inactive':
        'Pasif cari hesaba yeni borçlandırma yazılamaz. '
        'Tahsilat yazabilir ya da cariyi yeniden aktifleştirebilirsiniz.',

    // Borç/alacak
    'debt.repayment_required':
        'Ya toplam geri ödemeyi ya da yıllık faiz oranını girmeniz gerekiyor.',
    'debt.repayment_conflict':
        'Girilen toplam geri ödeme ile faiz oranı birbirini tutmuyor. '
        'Birini düzeltin.',
    'debt.invalid_contract':
        'Borç/alacak koşulları tutarlı değil. Tutar, taksit ve tarihleri '
        'kontrol edin.',

    // Hedef
    'goal.has_contributions': 'Katkı geçmişi olan tasarruf hedefi silinemez.',
    'goal.invalid_contract':
        'Hedef koşulları tutarlı değil. Tutar ve tarihleri kontrol edin.',

    // Kasa ve POS
    'cash_counts.nothing_to_adjust':
        'Sayım beklenen bakiyeyle aynı; kaydedilecek bir fark yok.',
    'pos_settlements.commission_ambiguous':
        'Komisyonu ya tutar ya oran olarak girin; ikisi birden olmaz.',
    'pos_settlements.details_required':
        'Hesap, satış kategorisi ve beklenen gün gerekli.',
    'pos_settlements.definition_unavailable':
        'Bu POS bulunamadı. Listeyi yenileyip yeniden seçin.',
    'pos_settlements.deposit_locked':
        'Bu tahsilat hesaba geçti. İptal etmek için önce yatışı geri alın.',
    'pos_settlements.day_close_locked':
        'Bu tahsilat gün sonundan geldi. İptal için gün sonunu geri alın.',
    'day_closes.invalid_date': 'Gün ileri bir tarih olamaz.',
    'day_closes.invalid_amount': 'Tutarlar eksi olamaz.',
    'day_closes.amounts_required': 'Nakit ya da kart tutarını yazın.',
    'day_closes.total_below_parts': 'Toplam, yazdığınız tutardan küçük olamaz.',
    'day_closes.pos_required': 'Kart tutarı için önce bir POS ekleyin.',
    'day_closes.pos_unavailable':
        'Seçilen POS bulunamadı. Paneli kapatıp yeniden açın.',
    'day_closes.pos_unusable':
        "POS'un hesabı ya da kategorisi kullanılamıyor. POS'u düzenleyin.",
    'day_closes.existing_exceeds_cash':
        'İşaretli kayıtlar nakit tutarını aşıyor. Tutarı düzeltin ya da '
        'işareti kaldırın.',
    'day_closes.existing_exceeds_card':
        'İşaretli kartlı kayıtlar kart tutarını aşıyor. Tutarı düzeltin ya '
        'da işareti kaldırın.',
    'day_closes.cash_account_required':
        'Nakit satışın yazılacağı kasayı seçin.',
    'day_closes.cash_account_unavailable': 'Seçilen kasa kullanılamıyor.',
    'day_closes.cash_category_required':
        'Nakit satış için bir gelir kategorisi seçin.',
    'day_closes.cash_category_unavailable': 'Seçilen kategori kullanılamıyor.',
    'day_closes.scope_unresolved':
        'Kasanın ya da kategorinin İşletme / Şahsi etiketi yok. Birini '
        'etiketleyip yeniden deneyin.',
    'day_closes.already_closed':
        'Bu günün gün sonu girildi. İkinci bir cihazın gün sonuysa ek '
        'olarak kaydedin.',
    'day_closes.not_closed_yet':
        'Bu gün henüz kapatılmadı; ek gün sonu yazılamaz.',
    'day_closes.z_number_exists': 'Bu Z raporu daha önce girildi.',
    'day_closes.additional_exists':
        'Bu günün ek gün sonu duruyor. Önce onu geri alın.',
    'transactions.day_close_counted':
        'Bu kayıt gün sonunda sayıldı. İptal için önce gün sonunu geri alın.',
    'pos_settlements.day_close_counted':
        'Bu tahsilat gün sonunda sayıldı. İptal için önce gün sonunu geri '
        'alın.',
    'counterparty_payments.day_close_counted':
        'Bu tahsilat gün sonunda sayıldı. İptal için önce gün sonunu geri '
        'alın.',
    'day_closes.deposit_locked':
        'Kart parası hesaba geçmiş. Önce yatışı geri alın.',
    'day_closes.concurrent_change':
        'Gün bu sırada değişti. Paneli kapatıp yeniden deneyin.',
    'day_closes.not_found': 'Gün sonu bulunamadı.',
    'pos_deposits.amount_exceeds_expected':
        'Yatan tutar beklenenden fazla olamaz. Komisyon fazla yazıldıysa '
        'tahsilatı iptal edip yeniden girin.',
    'pos_deposits.invalid_amount': 'Yatan tutarı sıfırdan büyük girin.',
    'pos_deposits.invalid_deposit_date':
        'Yatış günü ileri bir tarih ya da tahsilattan önce olamaz.',
    'pos_deposits.settlements_required': 'En az bir tahsilat seçin.',
    'pos_deposits.too_many_settlements':
        'Tek yatışta en çok 100 tahsilat kapatılabilir.',
    'pos_deposits.settlement_not_found':
        'Seçilen tahsilatlardan biri bulunamadı. Listeyi yenileyin.',
    'pos_deposits.settlement_not_in_transit':
        'Seçilen tahsilatlardan biri artık yolda değil. Listeyi yenileyin.',
    'pos_deposits.mixed_accounts':
        'Bir yatış tek hesaba düşer; aynı hesabın tahsilatlarını seçin.',
    'pos_deposits.account_unavailable':
        'Tahsilatların geçeceği hesap aktif değil.',
    'pos_deposits.deduction_category_required':
        'Eksik yatan tutar için bir gider kategorisi seçin.',
    'pos_deposits.deduction_category_unavailable':
        'Kesinti için aktif bir gider kategorisi seçin.',
    'pos_deposits.scope_unresolved':
        'Kesintinin işletme mi şahsi mi olduğu belirlenemedi; işletme ve '
        'şahsi tahsilatları ayrı ayrı işaretleyin.',
    'pos_deposits.concurrent_change':
        'Tahsilatlar bu sırada değişti. Listeyi yenileyip yeniden deneyin.',
    'pos_definitions.has_settlements':
        'Bu POS ile tahsilat yazıldığı için silinemez. Kullanmayacaksanız '
        'pasife alabilirsiniz.',
    'pos_definitions.inactive':
        'Bu POS pasif. Yeni tahsilat için etkinleştirin ya da başka bir POS '
        'seçin.',
    'pos_definitions.account_unavailable':
        'POS parası aktif bir banka hesabına geçer; hesabı yeniden seçin.',
    'pos_definitions.sales_category_unavailable':
        'Satış için aktif bir gelir kategorisi seçin.',
    'pos_definitions.commission_category_unavailable':
        'Komisyon için aktif bir gider kategorisi seçin.',

    // İçe aktarma, yedek, geri yükleme
    'imports.confirmation_conflict':
        'Bu içe aktarma başka bir yerde işlendi. Listeyi yenileyin.',
    'imports.invalid_csv':
        'Dosya okunamadı. Beklenen biçimde bir CSV dosyası seçin.',
    'imports.missing_column_mapping':
        'Zorunlu kolonların hepsini eşleştirmeniz gerekiyor.',
    'restore.destination_not_empty':
        'Geri yükleme yalnız boş bir hesaba yapılabilir. Önce mevcut '
        'kayıtları temizleyin.',
    'restore.unsupported_version':
        'Yedek dosyasının sürümü bu uygulama tarafından okunamıyor.',
    'restore.integrity_failed':
        'Yedek dosyası bozuk görünüyor; geri yükleme yapılmadı.',
    'restore.invalid_backup': 'Bu dosya geçerli bir yedek değil.',
    'backup.attachment_missing': 'Yedekteki bazı belgeler bulunamadı.',
    'backup.attachment_invalid': 'Yedekteki belge okunamadı.',
    'backup.attachment_storage_unavailable':
        'Belge deposuna ulaşılamadı. Daha sonra tekrar deneyin.',

    // Belge (ek)
    'attachment.transaction_not_found':
        'Belgenin bağlanacağı işlem bulunamadı.',

    // Fiş okuma — sunucu bugün Türkçe konuşuyor ama cümle yine istemcinindir
    'receipt.disabled':
        'Fiş okuma şu an kapalı. Bilgileri elle girebilirsiniz.',
    'receipt.provider_unavailable':
        'Fiş okuma servisine şu an ulaşılamıyor. '
        'Bilgileri elle girebilirsiniz.',
    'receipt.provider_rate_limited':
        'Fiş okuma servisinin kotası doldu. Bir süre sonra tekrar deneyin.',
    'receipt.unreadable':
        'Fiş okunamadı. Daha net bir fotoğraf çekebilir veya bilgileri elle '
        'girebilirsiniz.',
    'receipt.file_too_large': 'Fiş fotoğrafı 5 MiB boyut sınırını aşıyor.',
    'receipt.unsupported_file':
        'Bu dosya okunamıyor. JPEG veya PNG bir fotoğraf seçin.',
    'receipt.not_a_receipt':
        'Bu fotoğrafta alışveriş fişi görünmüyor. Fişin tamamı kadraja '
        'girecek şekilde tekrar çekebilirsiniz.',
    'receipt.not_a_bank_slip':
        'Bu bir dekont değil, alışveriş belgesi. Harcama olarak okutabilirsiniz.',
    'receipt.not_a_transfer_document':
        'Bu bir dekont değil, alışveriş belgesi. Transfer için havale, EFT '
        'veya ATM dekontu okutun.',
    'receipt.intent_mismatch':
        'Bu belge seçtiğiniz yöne uymuyor. Yönü değiştirip tekrar okutun.',
    'receipt.bank_document':
        'Bu dekont kendi hesaplarınız arasında bir aktarma. '
        'Dekont seçeneğiyle okutabilirsiniz.',
    'receipt.bank_payment_not_transfer':
        'Bu dekont başkasına yapılan bir ödeme, hesaplarınız arasında aktarma '
        'değil. Dekont seçeneğiyle okutabilirsiniz.',
    'receipt.refund_document':
        'Bu bir iade fişi. İade gider değildir; iade yolundan okutun.',

    // İstemcinin kendi ürettiği kodlar
    'network.unavailable':
        'Sunucuya ulaşılamadı. Bağlantınızı kontrol edip tekrar deneyin.',
    'network.timeout': 'İstek zaman aşımına uğradı. Lütfen tekrar deneyin.',
    'response.invalid_format': 'Sunucudan beklenmeyen bir yanıt alındı.',
  };
}
