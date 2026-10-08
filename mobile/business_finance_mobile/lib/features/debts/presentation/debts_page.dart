import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_input.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/data_choice.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_date_field.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_row_action.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../activities/data/receipt_fee_writer.dart';
import '../data/debt_models.dart';
import '../data/debt_repository.dart';
import 'debts_controller.dart';
import 'lending_prefill.dart';

/// Borç ve alacaklar.
///
/// Bu ekran Veri Araçları'nın bir sekmesiydi; CSV içe aktarma, belgeler ve
/// yedekleme ile aynı çekmecede duruyordu. Borç artık kendi başına finansal
/// bir kavram: açılışı para hareket ettiriyor ya da gider/gelir yazıyor.
/// Kendi ekranı olunca yalnız kendi verisini de çekiyor — borç listesi
/// açılırken belge listesi yüklenmesi için bir sebep yok.
class DebtsPage extends StatefulWidget {
  const DebtsPage({
    required this.repository,
    this.changes,
    this.lendingPrefill,
    this.recordFee,
    super.key,
  });

  final DebtRepositoryContract repository;
  final FinancialDataChanges? changes;

  /// Dekont okunarak gelindiyse alacak formu önerilerle açılır. Kullanıcı
  /// karar sayfasında "Geri bekliyorum" dedi; listeye bırakıp formu kendisinin
  /// açmasını beklemek istediği işi bir adım uzatırdı.
  final LendingPrefill? lendingPrefill;

  /// Dekonttaki işlem ücretini yazan dar imza; alacak açıldıktan sonra
  /// **açılış hesabından** çalışır.
  final ReceiptFeeRecorder? recordFee;

  @override
  State<DebtsPage> createState() => _DebtsPageState();
}

class _DebtsPageState extends State<DebtsPage> {
  late final DebtsController controller;

  @override
  void initState() {
    super.initState();
    controller = DebtsController(
      widget.repository,
      financialDataChanges: widget.changes,
    )..addListener(_changed);
    controller.load().then((_) => _openPrefilledForm());
  }

  /// Dekontun önerdiği alacağı **bir kez** açar.
  ///
  /// Kart ödeme yolunda öğrenilen ders: öneri her tazelemede yeniden
  /// kullanılırsa form kendini yeniden açar ve ücret ikinci kez yazılır.
  bool _prefillUsed = false;

  void _openPrefilledForm() {
    final prefill = widget.lendingPrefill;
    if (_prefillUsed || prefill == null || prefill.isEmpty) return;
    if (!mounted || controller.snapshot == null) return;
    _prefillUsed = true;
    _showForm(prefill: prefill);
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  void dispose() {
    controller.removeListener(_changed);
    controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(title: const Text('Borç ve alacaklar')),
    body: _body(),
  );

  Widget _body() {
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (controller.isLoading && controller.snapshot == null) {
      return const AppLoadingView(message: 'Borçlar yükleniyor');
    }
    if (controller.snapshot == null) {
      return AppErrorView(
        message: controller.errorMessage ?? 'Borçlar alınamadı.',
        onRetry: controller.load,
      );
    }

    final debts = controller.snapshot!.debts;
    return Column(
      children: [
        if (controller.isLoading || controller.isSubmitting)
          const LinearProgressIndicator(),
        if (controller.errorMessage != null ||
            controller.successMessage != null)
          MaterialBanner(
            content: Text(
              controller.errorMessage ?? controller.successMessage!,
            ),
            leading: Icon(
              controller.errorMessage == null
                  ? Icons.check_circle_outline
                  : Icons.error_outline,
            ),
            actions: [
              TextButton(
                onPressed: controller.clearMessage,
                child: const Text('Kapat'),
              ),
            ],
          ),
        Expanded(
          child: RefreshIndicator(
            onRefresh: controller.load,
            child: ListView(
              padding: const EdgeInsets.all(AppSpacing.medium),
              children: [
                FilledButton.icon(
                  onPressed: controller.isSubmitting ? null : _showForm,
                  icon: const Icon(Icons.add),
                  label: const Text('Borç / alacak ekle'),
                ),
                const SizedBox(height: AppSpacing.medium),
                if (debts.isEmpty)
                  const SizedBox(
                    height: 220,
                    child: AppEmptyView(
                      icon: Icons.handshake_outlined,
                      title: 'Borç planı yok',
                      message: 'Borç veya alacak planı yok.',
                    ),
                  ),
                for (final debt in debts) _debtCard(debt),
              ],
            ),
          ),
        ),
      ],
    );
  }

  /// Bir borcun ya da alacağın kartı.
  ///
  /// Üst satır tek bakışta üç soruyu cevaplar: kim, hangi yön, ne kadar kaldı.
  /// Faiz ve taksit sayacı bunun altında ikinci sıradadır — faiz sıfırsa hiç
  /// yazılmaz, çünkü "Faiz 0,00 ₺ (%0,00)" kartı doldurup hiçbir şey söylemez
  /// (kişiler arası borçlar tipik olarak faizsizdir).
  ///
  /// Taksitler kapalı başlar: kalan tutar ve kaçının ödendiği başlıkta zaten
  /// yazıyor, satırlar ancak biri ödenecekken gerekli.
  Widget _debtCard(DebtItem debt) {
    final theme = Theme.of(context);
    final paid = debt.installments
        .where((item) => item.status == 'paid')
        .length;
    final effect = debt.isReceivable
        ? AppMoneyEffect.income
        : AppMoneyEffect.expense;
    final hasInterest = _isNonZero(debt.totalInterest);

    return Padding(
      // Boşluk `small`: `medium` kartları birbirinden koparmıştı ve liste tek
      // bir liste gibi değil, ayrı ayrı duyurular gibi okunuyordu.
      padding: const EdgeInsets.only(bottom: AppSpacing.small),
      child: AppCard(
        padding: EdgeInsets.zero,
        child: ExpansionTile(
          // Kartın kenarlığı zaten var; `ExpansionTile`'ın kendi çizgileri
          // ikinci bir çerçeve çiziyor ve kartlar birbirine yapışmış
          // görünüyordu.
          shape: const Border(),
          collapsedShape: const Border(),
          tilePadding: const EdgeInsets.symmetric(
            horizontal: AppSpacing.medium,
            vertical: AppSpacing.xSmall,
          ),
          title: Row(
            children: [
              Expanded(
                child: Text(
                  debt.name,
                  style: theme.textTheme.titleMedium,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                ),
              ),
              const SizedBox(width: AppSpacing.small),
              AppStatusChip(
                label: debt.isReceivable ? 'Alacak' : 'Borç',
                icon: debt.isReceivable ? Icons.south_west : Icons.north_east,
                tone: debt.isReceivable
                    ? AppStatusTone.income
                    : AppStatusTone.expense,
              ),
            ],
          ),
          subtitle: Padding(
            padding: const EdgeInsets.only(top: AppSpacing.small),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    Text(
                      'Kalan',
                      style: theme.textTheme.labelMedium?.copyWith(
                        color: theme.colorScheme.onSurfaceVariant,
                      ),
                    ),
                    const SizedBox(width: AppSpacing.small),
                    AppMoneyText(
                      amount: debt.remaining,
                      currency: 'TRY',
                      effect: effect,
                      style: theme.textTheme.titleMedium,
                    ),
                  ],
                ),
                const SizedBox(height: AppSpacing.xSmall),
                Text(
                  [
                    '${debt.installments.length} taksitin $paid tanesi ödendi',
                    if (hasInterest)
                      'Faiz ${MoneyText.format(debt.totalInterest, 'TRY')} '
                          '(%${MoneyText.percent(debt.annualInterestRate)})',
                  ].join(' · '),
                  style: theme.textTheme.bodySmall,
                ),
              ],
            ),
          ),
          children: [
            // Eksik gizlenmiyor: açılışı kayıtsız borç ne para hareketi ne
            // gider üretir ve bunu kullanıcıya söylemek gerekir. Gerçeği
            // yalnız o biliyor.
            if (debt.hasUnrecordedOpening)
              Padding(
                padding: const EdgeInsets.fromLTRB(
                  AppSpacing.medium,
                  0,
                  AppSpacing.medium,
                  AppSpacing.small,
                ),
                child: AppInlineNotice(
                  icon: Icons.help_outline,
                  message:
                      'Bu borcun açılışı kayıtlı değil, bu yüzden hesap ve '
                      'raporlara hiç girmiyor. Para hesabınıza mı girdi, '
                      'yoksa bir şey mi aldınız?',
                  actionLabel: 'Tamamla',
                  onAction: controller.isSubmitting
                      ? null
                      : () => _recordOpening(debt),
                ),
              ),
            for (final item in debt.installments)
              _installmentRow(debt, item, effect),
            const SizedBox(height: AppSpacing.small),
          ],
        ),
      ),
    );
  }

  /// Tek taksit satırı.
  ///
  /// Durum rozeti ve eylem **aynı yerde ve aynı boyda** duruyor: eskiden
  /// ödenmiş taksitte küçük bir onay ikonu, ödenmemişte bir buton vardı ve iki
  /// satır aynı listede farklı yüksekliklerde görünüyordu. Tutar her satırda
  /// sağda; göz sütunu takip edebiliyor.
  Widget _installmentRow(
    DebtItem debt,
    DebtInstallmentItem item,
    AppMoneyEffect effect,
  ) {
    final isPaid = item.status == 'paid';
    return AppListRow(
      icon: isPaid ? Icons.check_circle_outline : Icons.schedule,
      title: '${item.sequence}. taksit',
      subtitle: DateText.dayMonthYear(item.dueDate),
      // Durum solda, eylem sağda: önceden ikisi yan yana akıyor ve eylem
      // sığmadığında alt satıra düşüp satırı dağıtıyordu. `spaceBetween` ile
      // yerleri sabit; sığmadığı durumda hâlâ alt satıra iniyor, kırpılmıyor.
      badge: Wrap(
        alignment: WrapAlignment.spaceBetween,
        spacing: AppSpacing.small,
        runSpacing: AppSpacing.small,
        crossAxisAlignment: WrapCrossAlignment.center,
        children: [
          AppStatusChip(
            label: _statusLabel(item.status),
            icon: _statusIcon(item.status),
            tone: _statusTone(item.status),
          ),
          // İkonsuz: rozet zaten bir ikon taşıyor ve yan yana iki küçük ikon
          // ikisini de okunmaz yapıyordu. Oklar ayrıca kartın başındaki
          // `Borç`/`Alacak` rozetinin oklarıyla karışıyordu.
          if (!isPaid)
            AppRowAction(
              label: debt.isReceivable ? 'Tahsil et' : 'Öde',
              onPressed: controller.isSubmitting
                  ? null
                  : () => _pay(debt, item),
            ),
        ],
      ),
      trailing: AppMoneyText(
        amount: item.amount,
        currency: 'TRY',
        effect: isPaid ? AppMoneyEffect.neutral : effect,
      ),
    );
  }

  Future<void> _showForm({LendingPrefill? prefill}) async {
    final payload = await AppFormSheet.show<Map<String, dynamic>>(
      context: context,
      builder: (_) => _DebtForm(
        today: controller.today,
        accounts: controller.snapshot!.accounts,
        categories: controller.snapshot!.categories,
        prefill: prefill,
      ),
    );
    if (payload == null) return;
    await controller.create(payload);

    // Ücret alacaktan **sonra** ve alacağın açılış hesabından: karşı taraf o
    // ücreti sana borçlanmadı, bankaya sen ödedin. Anaparaya eklemek geri
    // beklediğin tutarı şişirirdi.
    final fee = prefill?.feeAmount;
    final account = payload['openingAccountId'];
    if (!mounted ||
        fee == null ||
        account is! String ||
        controller.errorMessage != null) {
      return;
    }
    final record = widget.recordFee;
    if (record == null) return;
    final result = await record(
      sourceId: account,
      amount: fee,
      date: payload['startDate'] as String? ?? controller.today,
      description: prefill?.feeDescription ?? 'İşlem ücreti',
    );
    if (!mounted) return;
    ScaffoldMessenger.of(
      context,
    ).showSnackBar(SnackBar(content: Text(result.message)));
  }

  Future<void> _recordOpening(DebtItem debt) async {
    final payload = await AppFormSheet.show<Map<String, dynamic>>(
      context: context,
      builder: (_) => _DebtOpeningForm(
        direction: debt.direction,
        accounts: controller.snapshot!.accounts,
        categories: controller.snapshot!.categories,
      ),
    );
    if (payload == null) return;
    await controller.recordOpening(debt.id, payload);
  }

  Future<void> _pay(DebtItem debt, DebtInstallmentItem item) async {
    final account = await _chooseAccount(
      debt.isReceivable ? 'Tahsilat hesabı' : 'Ödeme hesabı',
    );
    if (account == null) return;
    await controller.pay(
      debt.id,
      item.sequence,
      account,
      isReceivable: debt.isReceivable,
    );
  }

  /// Taksitin hangi hesaptan ödeneceğini (ya da hangisine tahsil edileceğini)
  /// sorar.
  ///
  /// **Hiçbir hesap önceden seçili değil.** Önceden listenin ilki seçiliydi ve
  /// `Seç` tek dokunuşla o hesabı kabul ediyordu: dalgın bir dokunuş parayı
  /// yanlış hesaptan çıkarırdı. Formlardaki "ödeme kaynağı boş başlar ve
  /// zorunludur" kuralının aynısı; bu da bir ödeme kaynağı sorusu.
  Future<String?> _chooseAccount(String title) async {
    final accounts = controller.snapshot!.accounts;
    if (accounts.isEmpty) return null;
    String? selected;
    return showDialog<String>(
      context: context,
      builder: (dialogContext) => StatefulBuilder(
        builder: (context, setDialogState) => AlertDialog(
          title: Text(title),
          content: DropdownButtonFormField<String>(
            initialValue: selected,
            hint: const Text('Hesap seçin'),
            items: accounts
                .map(
                  (item) =>
                      DropdownMenuItem(value: item.id, child: Text(item.name)),
                )
                .toList(),
            onChanged: (value) => setDialogState(() => selected = value),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.pop(dialogContext),
              child: const Text('Vazgeç'),
            ),
            FilledButton(
              onPressed: selected == null
                  ? null
                  : () => Navigator.pop(dialogContext, selected),
              child: const Text('Seç'),
            ),
          ],
        ),
      ),
    );
  }

  static String _statusLabel(String status) => switch (status) {
    'paid' => 'Ödendi',
    'overdue' => 'Gecikmiş',
    'due-today' => 'Bugün vadeli',
    'upcoming' => 'Yaklaşan',
    _ => 'Bilinmeyen durum',
  };

  static IconData _statusIcon(String status) => switch (status) {
    'paid' => Icons.check,
    'overdue' => Icons.priority_high,
    'due-today' => Icons.today,
    'upcoming' => Icons.schedule,
    _ => Icons.help_outline,
  };

  /// Gecikmiş taksit uyarı tonunda, ödenmiş olan nötr.
  ///
  /// Ton tek başına bilgi taşımıyor: rozet zaten ikon ve etiket zorunlu
  /// kılıyor, renk yalnız aynı şeyi ikinci kez söylüyor.
  static AppStatusTone _statusTone(String status) => switch (status) {
    'paid' => AppStatusTone.neutral,
    'overdue' => AppStatusTone.expense,
    'due-today' => AppStatusTone.planned,
    _ => AppStatusTone.planned,
  };

  /// Sıfır faiz, faizin yokluğunun başka bir yazılışı (`ReceiptDraft.hasFee`
  /// ile aynı gerekçe). Bu bir finansal hesap değil, satırın ekranda yeri
  /// olup olmadığı kararı; toplamlar hep sunucudan geliyor.
  static bool _isNonZero(String amount) {
    final value = double.tryParse(amount);
    return value != null && value != 0;
  }
}

/// Borcun maliyetini kullanıcının hangi ucundan bildiği.
enum _DebtCostInput { total, rate }

/// Borcu ya da alacağı neyin doğurduğunu soran alanlar.
///
/// Hem yeni kayıt formunda hem de açılışı kayıtsız bir borcu tamamlama
/// panelinde aynı üç kural geçerli, o yüzden tek yerde duruyor: kaynak
/// nakitse hesap, kategoriliyse kategori sorulur; kategori tipi yöne bağlıdır
/// (borç tüketir → gider, alacak satar → gelir).
class _DebtSourceFields extends StatelessWidget {
  const _DebtSourceFields({
    required this.direction,
    required this.source,
    required this.accounts,
    required this.categories,
    required this.openingAccountId,
    required this.categoryId,
    required this.onSourceChanged,
    required this.onAccountChanged,
    required this.onCategoryChanged,
    this.sourceHelperText,
  });

  final String direction;
  final String source;
  final List<DataChoice> accounts;
  final List<DataChoice> categories;
  final String? openingAccountId;
  final String? categoryId;
  final ValueChanged<String> onSourceChanged;
  final ValueChanged<String?> onAccountChanged;
  final ValueChanged<String?> onCategoryChanged;
  final String? sourceHelperText;

  static bool isPayable(String direction) => direction == 'payable';

  /// Yöne karşılık gelen kategorili kaynak.
  static String categoricalSourceOf(String direction) =>
      isPayable(direction) ? 'expense' : 'income';

  static String _categoryTypeOf(String direction) =>
      isPayable(direction) ? 'expense' : 'income';

  /// Yönün kabul ettiği kategoriler. Yön değişince liste de değişir; eski
  /// seçim başka tipte kalırsa sunucu isteği reddeder.
  static List<DataChoice> categoriesFor(
    String direction,
    List<DataChoice> categories,
  ) => categories
      .where((item) => item.type == _categoryTypeOf(direction))
      .toList(growable: false);

  @override
  Widget build(BuildContext context) {
    final payable = isPayable(direction);
    final categorical = source != 'cash';
    final options = categoriesFor(direction, categories);

    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        AppFormField(
          child: DropdownButtonFormField<String>(
            // Testin doğru alanı sürükleyebilmesi için: sırayla bulmak alan
            // eklendiğinde sessizce başka bir alanı ölçmeye başlar.
            key: const Key('debt-source'),
            initialValue: source,
            isExpanded: true,
            decoration: InputDecoration(
              labelText: payable ? 'Borcu ne doğurdu' : 'Alacağı ne doğurdu',
              helperText: sourceHelperText,
            ),
            items: [
              DropdownMenuItem(
                value: 'cash',
                child: Text(
                  payable ? 'Para hesabıma girdi' : 'Borç para verdim',
                ),
              ),
              DropdownMenuItem(
                value: categoricalSourceOf(direction),
                child: Text(
                  payable
                      ? 'Bir şey aldım / tükettim'
                      : 'Bir şey sattım / hizmet verdim',
                ),
              ),
            ],
            onChanged: (value) => onSourceChanged(value!),
          ),
        ),
        if (categorical)
          AppFormField(
            child: DropdownButtonFormField<String>(
              initialValue: options.any((item) => item.id == categoryId)
                  ? categoryId
                  : null,
              isExpanded: true,
              decoration: InputDecoration(
                labelText: payable ? 'Gider kategorisi' : 'Gelir kategorisi',
              ),
              items: [
                for (final item in options)
                  DropdownMenuItem(value: item.id, child: Text(item.name)),
              ],
              onChanged: onCategoryChanged,
              validator: (value) => value == null
                  ? (payable
                        ? 'Bir gider kategorisi seçin.'
                        : 'Bir gelir kategorisi seçin.')
                  : null,
            ),
          )
        else
          AppFormField(
            child: DropdownButtonFormField<String>(
              initialValue: openingAccountId,
              isExpanded: true,
              decoration: InputDecoration(
                labelText: payable
                    ? 'Paranın girdiği hesap'
                    : 'Paranın çıktığı hesap',
              ),
              items: [
                for (final item in accounts)
                  DropdownMenuItem(value: item.id, child: Text(item.name)),
              ],
              onChanged: onAccountChanged,
              validator: (value) => value == null ? 'Bir hesap seçin.' : null,
            ),
          ),
      ],
    );
  }
}

/// Borç / alacak planı.
///
/// İki şey soruyor ve ikisi de kayıtsız kaldığı için defter kapanmıyordu:
///
/// 1. **Borcu ne doğurdu.** Para hesaba girdiyse gider yoktur, bir şey
///    tüketildiyse tam o gün gider vardır ve kategori taşır. Alacakta bu
///    soru sorulmaz: alacak her zaman "para çıktı, geri gelecek"tir.
/// 2. **Borç ne kadara mal olacak.** Kullanıcı elinde ne varsa onu girer:
///    toplam geri ödemeyi ya da yıllık faizi. Diğerini sunucu hesaplar.
///
/// Hesabı istemci yapmaz. Anüite formülünü burada tekrarlamak, aynı finansal
/// kuralın iki yerde yaşaması ve zamanla ayrışması demek olurdu; proje kuralı
/// da finansal tutarın istemcide ikinci kez hesaplanmamasını söylüyor.
class _DebtForm extends StatefulWidget {
  const _DebtForm({
    required this.today,
    required this.accounts,
    required this.categories,
    this.prefill,
  });

  final String today;
  final List<DataChoice> accounts;
  final List<DataChoice> categories;

  /// Dekonttan gelen öneriler. Hepsi değiştirilebilir: dekont paranın ne zaman
  /// geri geleceğini söylemez, biz de uydurmuyoruz — bir varsayım koyup
  /// kullanıcıya bırakıyoruz.
  final LendingPrefill? prefill;

  @override
  State<_DebtForm> createState() => _DebtFormState();
}

class _DebtFormState extends State<_DebtForm> {
  final _formKey = GlobalKey<FormState>();
  final _name = TextEditingController();
  final _nameFocus = FocusNode();

  /// Kullanıcı "bu değil" dedi mi. Reddedilen öneri geri gelmez; aksi hâlde
  /// kullanıcı aynı rozeti her harfte yeniden görürdü.
  var _counterpartyMatchRejected = false;
  final _principal = TextEditingController();
  final _total = TextEditingController();
  final _count = TextEditingController(text: '1');
  final _interest = TextEditingController();
  final _description = TextEditingController();
  var _direction = 'payable';
  var _source = 'cash';
  var _costInput = _DebtCostInput.total;
  String? _openingAccountId;
  String? _categoryId;
  late var _startDate = widget.today;
  late var _firstDueDate = widget.today;

  bool get _sourceIsCategorical => _source != 'cash';

  /// Kullanıcının açık kapsam seçimi; boşsa zincir karar verir.
  ///
  /// `DebtAgreement` gelir/gider raporunu etkiler ve **kapsam taşımak
  /// zorundadır**. Bu alan Aşama 01'de atlanmıştı: zincir çözülemeyen
  /// kullanıcıda sunucu isteği reddediyor ve borç planı hiç kurulamıyordu
  /// (Aşama 06 Grup 6 cihaz kabul turu).
  TransactionScope? _scope;

  /// Zincir çözülemedi ve kullanıcı da seçmedi.
  String? _scopeError;

  /// Kapsam boyutunu gören kullanıcı mı. Görmeyen kullanıcıda alan hiç
  /// çizilmez ve istekte `scope` gitmez.
  ///
  /// `read`, `watch` değil: bu getter gönderim geri çağrısından da okunuyor ve
  /// `watch` yalnız `build` içinde çağrılabilir.
  bool get _scopeIsVisible =>
      context.read<ScopeController?>()?.isVisible ?? false;

  /// Formun gösterdiği taraf: kategori → açık seçim → hesabın etiketi.
  /// Kategorisi olmayan borç (nakit) iki tarafa açık kategori gibi davranır.
  ///
  /// Sunucudaki kuralın **önizlemesi** (`TransactionScopeResolution`); karar
  /// sunucunundur, burası yalnız ne yazılacağını gösterip onu gönderir.
  TransactionScope? get _resolvedScope => previewResolvedScope(
    explicit: _scope,
    source: _sourceScope,
    category: _categoryScope,
  );

  TransactionScope? get _sourceScope => _sourceIsCategorical
      ? null
      : _defaultScopeOf(widget.accounts, _openingAccountId);

  TransactionScope? get _categoryScope => _sourceIsCategorical
      ? _defaultScopeOf(widget.categories, _categoryId)
      : null;

  static TransactionScope? _defaultScopeOf(
    List<DataChoice> choices,
    String? id,
  ) {
    if (id == null) return null;
    for (final choice in choices) {
      if (choice.id == id) return choice.defaultScope;
    }
    return null;
  }

  @override
  void initState() {
    super.initState();
    _openingAccountId = widget.accounts.isEmpty
        ? null
        : widget.accounts.first.id;
    _applyPrefill();
    _syncCategory();
    _name.addListener(_onNameChanged);
  }

  /// Ad değişince rozetin görünürlüğü değişebilir; yalnız değiştiğinde çizer.
  void _onNameChanged() {
    if (_showsCounterpartyMatch == _matchWasVisible) return;
    setState(() => _matchWasVisible = _showsCounterpartyMatch);
  }

  var _matchWasVisible = false;

  /// Rozet yalnız **fişten gelen ad hâlâ yerindeyken** görünür.
  ///
  /// Kullanıcı adı değiştirdiyse artık başka birinden söz ediyor ve eşleşme
  /// hükümsüzdür; rozeti bırakmak, olmayan bir bağı varmış gibi gösterirdi.
  bool get _showsCounterpartyMatch {
    final prefill = widget.prefill;
    if (prefill == null ||
        !prefill.hasCounterpartyMatch ||
        _counterpartyMatchRejected) {
      return false;
    }
    return _name.text.trim().toLowerCase() ==
        prefill.counterpartyName!.trim().toLowerCase();
  }

  /// Yanlış eşleşmeyi tek dokunuşla reddeder.
  ///
  /// Ad **siliniyor**: eşleşmeyi reddedip aynı adı bırakmak, sunucunun aynı
  /// karşı tarafı yeniden bulmasıyla sonuçlanırdı — reddetme hiçbir şeyi
  /// değiştirmemiş olurdu.
  void _rejectCounterpartyMatch() {
    setState(() {
      _counterpartyMatchRejected = true;
      _matchWasVisible = false;
      _name.clear();
    });
    _nameFocus.requestFocus();
  }

  /// Dekontun söylediğini yazar, söylemediğini varsayar.
  ///
  /// Belgede yazan: karşı taraf, tutar, tarih. Belgede **yazmayan**: vade ve
  /// taksit sayısı — bir dekont paranın ne zaman geri geleceğini söylemez.
  /// Varsayım tek taksit ve bir ay sonrası vade; kişiler arası borç tipik
  /// olarak faizsiz ve tek seferde geri gelir. Toplam geri ödeme anaparaya
  /// eşitleniyor, yani faiz sıfır: uydurulmuş bir faiz, kullanıcının hiç
  /// konuşmadığı bir maliyeti deftere yazardı.
  ///
  /// Üçü de formda duruyor ve değiştirilebilir. Vadeyi boş bırakıp zorunlu
  /// kılmak da olurdu ama yanlış vade düzeltilebilir, hiç kaydedilmemiş bir
  /// alacak düzeltilemez.
  void _applyPrefill() {
    final prefill = widget.prefill;
    if (prefill == null || prefill.isEmpty) return;
    _matchWasVisible = prefill.hasCounterpartyMatch;

    // Para çıktı ve geri bekleniyor: bu bir alacak.
    _direction = 'receivable';
    _source = 'cash';
    if (prefill.counterpartyName case final name?) _name.text = name;
    if (prefill.amount case final amount?) {
      _principal.text = MoneyText.editable(amount);
      _total.text = MoneyText.editable(amount);
    }
    _costInput = _DebtCostInput.total;
    _count.text = '1';
    if (prefill.date case final date?) {
      _startDate = date;
      _firstDueDate = _oneMonthAfter(date);
    }
  }

  /// `yyyy-MM-dd` + bir ay. Ayın son günleri o ayın uzunluğuna kırpılır —
  /// 31 Ocak'ın bir ay sonrası 3 Mart değil, şubatın son günüdür.
  static String _oneMonthAfter(String date) {
    final parsed = DateTime.tryParse(date);
    if (parsed == null) return date;
    final month = parsed.month == 12 ? 1 : parsed.month + 1;
    final year = parsed.month == 12 ? parsed.year + 1 : parsed.year;
    final lastDay = DateTime(year, month + 1, 0).day;
    final day = parsed.day < lastDay ? parsed.day : lastDay;
    final next = DateTime(year, month, day);
    return '${next.year}-${next.month.toString().padLeft(2, '0')}-'
        '${next.day.toString().padLeft(2, '0')}';
  }

  /// Yön değişince kabul edilen kategori tipi de değişir; eski seçim başka
  /// tipte kalırsa sunucu isteği reddeder.
  void _syncCategory() {
    final options = _DebtSourceFields.categoriesFor(
      _direction,
      widget.categories,
    );
    if (options.any((item) => item.id == _categoryId)) return;
    _categoryId = options.isEmpty ? null : options.first.id;
  }

  @override
  void dispose() {
    _name.removeListener(_onNameChanged);
    _name.dispose();
    _nameFocus.dispose();
    _principal.dispose();
    _total.dispose();
    _count.dispose();
    _interest.dispose();
    _description.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Form(
    key: _formKey,
    child: AppFormSheet<Map<String, dynamic>>(
      title: 'Borç / alacak planı',
      description:
          'Taksitler ilk vade tarihinden başlayarak aylık ilerler. Toplam '
          'geri ödemeyi ya da yıllık faizi girin; diğerini hesaplarız.',
      submitLabel: 'Oluştur',
      onSubmit: () async {
        if (!(_formKey.currentState?.validate() ?? false)) return null;
        // Kapsamı gören kullanıcıda zincir çözülemiyorsa istek **gitmeden**
        // alanın yanında söylenir; sunucunun reddi ekranda ham hata olurdu.
        final scope = _resolvedScope;
        if (_scopeIsVisible && scope == null) {
          setState(
            () => _scopeError =
                'Kaynak ve kategori kapsam taşımıyor; bu kayıt için seçin.',
          );
          return null;
        }
        final byTotal = _costInput == _DebtCostInput.total;
        return {
          'scope': scope?.apiValue,
          'counterpartyName': _name.text.trim(),
          'direction': _direction,
          'principal': MoneyInput.wire(_principal.text),
          // Yalnız kullanıcının girdiği alan gönderilir. İkisini birden
          // göndermek, çelişirlerse isteğin reddedilmesi demekti.
          'totalRepayment': byTotal ? MoneyInput.wire(_total.text) : null,
          'annualInterestRate': byTotal
              ? null
              : MoneyInput.wire(_interest.text),
          'currency': 'TRY',
          'sourceType': _source,
          'openingAccountId': _sourceIsCategorical ? null : _openingAccountId,
          'categoryId': _sourceIsCategorical ? _categoryId : null,
          'startDate': _startDate,
          'firstDueDate': _firstDueDate,
          'installmentCount': int.tryParse(_count.text.trim()) ?? 1,
          'description': _description.text.trim().isEmpty
              ? null
              : _description.text.trim(),
        };
      },
      children: [
        AppFormField(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              TextFormField(
                controller: _name,
                focusNode: _nameFocus,
                hintLocales: const [Locale('tr', 'TR')],
                decoration: const InputDecoration(labelText: 'Kişi / kurum'),
                validator: (value) => value == null || value.trim().isEmpty
                    ? 'Kişi veya kurum adı zorunludur.'
                    : null,
              ),
              // Fişten okunan ad defterdeki bir kişiyle eşleştiyse bunu
              // **söylemek** zorundayız: kayıt o kişinin açık bakiyesine
              // eklenecek ve yanlış eşleşme iki müşterinin hesabını
              // birbirine karıştırır. Model önerir, kullanıcı onaylar
              // (ADR 0011); reddi tek dokunuş.
              if (_showsCounterpartyMatch)
                AppInlineNotice(
                  icon: Icons.person_search_outlined,
                  message:
                      '“${_name.text.trim()}” defterinizde kayıtlı. Bu kayıt '
                      'aynı karşı tarafa bağlanacak ve onun bakiyesine '
                      'eklenecek.',
                  actionLabel: 'Bu kişi değil',
                  onAction: _rejectCounterpartyMatch,
                ),
            ],
          ),
        ),
        AppFormField(
          child: DropdownButtonFormField<String>(
            initialValue: _direction,
            isExpanded: true,
            decoration: const InputDecoration(labelText: 'Yön'),
            items: const [
              DropdownMenuItem(value: 'payable', child: Text('Ödenecek borç')),
              DropdownMenuItem(value: 'receivable', child: Text('Alınacak')),
            ],
            onChanged: (value) => setState(() {
              _direction = value!;
              // Yön değişince kategorili kaynağın anlamı da değişir: borç
              // tüketir, alacak satar. Nakit dışı seçim yeni yönün kaynağına
              // taşınır, kategori de o tipe göre tazelenir.
              if (_source != 'cash') {
                _source = _DebtSourceFields.categoricalSourceOf(_direction);
              }
              _syncCategory();
            }),
          ),
        ),
        _DebtSourceFields(
          direction: _direction,
          source: _source,
          accounts: widget.accounts,
          categories: widget.categories,
          openingAccountId: _openingAccountId,
          categoryId: _categoryId,
          sourceHelperText: _direction == 'payable'
              ? 'Para aldıysanız gider yazılmaz; bir şey tükettiyseniz '
                    'anapara o gün gidere yazılır.'
              : 'Borç para verdiyseniz gelir yazılmaz; bir şey sattıysanız '
                    'anapara o gün gelire yazılır.',
          onSourceChanged: (value) => setState(() => _source = value),
          onAccountChanged: (value) =>
              setState(() => _openingAccountId = value),
          onCategoryChanged: (value) => setState(() => _categoryId = value),
        ),
        // Nakit borcun kategorisi yoktur ve taraf hep sorulur; kategorili
        // kaynakta kategori seçilmeden çizilecek bir şey yoktur.
        if ((context.watch<ScopeController?>()?.isVisible ?? false) &&
            (!_sourceIsCategorical || _categoryId != null))
          AppFormField(
            child: AppScopeSection(
              explicit: _scope,
              source: _sourceScope,
              category: _categoryScope,
              onChanged: (value) => setState(() {
                _scope = value;
                _scopeError = null;
              }),
              errorText: _scopeError,
            ),
          ),
        AppFormField(
          child: TextFormField(
            controller: _principal,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: const InputDecoration(
              labelText: 'Anapara',
              suffixText: 'TRY',
            ),
            validator: MoneyInput.positiveError,
          ),
        ),
        AppFormField(
          child: DropdownButtonFormField<_DebtCostInput>(
            initialValue: _costInput,
            isExpanded: true,
            decoration: const InputDecoration(
              labelText: 'Elinizde hangisi var',
              helperText: 'Girmediğinizi hesaplayıp size gösteririz.',
            ),
            items: const [
              DropdownMenuItem(
                value: _DebtCostInput.total,
                child: Text('Toplam geri ödeme'),
              ),
              DropdownMenuItem(
                value: _DebtCostInput.rate,
                child: Text('Yıllık faiz oranı'),
              ),
            ],
            onChanged: (value) => setState(() => _costInput = value!),
          ),
        ),
        if (_costInput == _DebtCostInput.total)
          AppFormField(
            child: TextFormField(
              controller: _total,
              keyboardType: const TextInputType.numberWithOptions(
                decimal: true,
              ),
              decoration: const InputDecoration(
                labelText: 'Toplam geri ödeme',
                suffixText: 'TRY',
                helperText: 'Faiz dahil, gerçekte ödenecek toplam tutar.',
              ),
              validator: (value) {
                final error = MoneyInput.positiveError(value);
                if (error != null) return error;
                final principalAmount = MoneyInput.parse(_principal.text);
                final totalAmount = MoneyInput.parse(value!);
                return principalAmount != null &&
                        totalAmount != null &&
                        totalAmount < principalAmount
                    ? 'Toplam geri ödeme anaparadan küçük olamaz.'
                    : null;
              },
            ),
          )
        else
          AppFormField(
            child: TextFormField(
              controller: _interest,
              keyboardType: const TextInputType.numberWithOptions(
                decimal: true,
              ),
              decoration: const InputDecoration(
                labelText: 'Yıllık faiz %',
                helperText:
                    'Azalan bakiyeye işler; toplam geri ödemeyi bundan '
                    'hesaplarız.',
              ),
              validator: (value) {
                final rate = MoneyInput.parse(value ?? '');
                return rate == null || rate < 0 || rate > 1000
                    ? '0 ile 1000 arasında faiz girin.'
                    : null;
              },
            ),
          ),
        AppFormField(
          child: TextFormField(
            controller: _count,
            keyboardType: TextInputType.number,
            decoration: const InputDecoration(labelText: 'Taksit sayısı'),
            validator: (value) {
              final parsed = int.tryParse(value?.trim() ?? '');
              return parsed == null || parsed < 1 || parsed > 360
                  ? '1 ile 360 arasında taksit sayısı girin.'
                  : null;
            },
          ),
        ),
        AppFormField(
          child: AppDateField(
            label: 'Başlangıç tarihi',
            value: _startDate,
            helperText: 'Borcun doğduğu gün; geçmiş bir tarih olabilir.',
            onChanged: (value) => setState(() {
              _startDate = value;
              // İlk vade başlangıçtan önce olamaz (sunucu da reddeder);
              // kullanıcıyı reddedilecek bir kombinasyona bırakmak yerine
              // vade tarihi başlangıca çekilir.
              if (AppDateField.parse(
                _firstDueDate,
              )!.isBefore(AppDateField.parse(value)!)) {
                _firstDueDate = value;
              }
            }),
          ),
        ),
        AppFormField(
          child: AppDateField(
            label: 'İlk taksit vadesi',
            value: _firstDueDate,
            helperText: 'Sonraki taksitler bu günden aylık ilerler.',
            onChanged: (value) => setState(() => _firstDueDate = value),
          ),
        ),
        AppFormField(
          child: TextFormField(
            controller: _description,
            hintLocales: const [Locale('tr', 'TR')],
            maxLines: 2,
            decoration: const InputDecoration(labelText: 'Not (isteğe bağlı)'),
          ),
        ),
      ],
    ),
  );
}

/// Açılışı kayıtsız bir borcun kaynağını tamamlar.
///
/// Yalnız tek soru sorar, çünkü eksik olan tek şey odur: borcu ne doğurdu.
/// Tutarlar, tarihler ve taksitler zaten kayıtlı ve değişmiyor.
class _DebtOpeningForm extends StatefulWidget {
  const _DebtOpeningForm({
    required this.direction,
    required this.accounts,
    required this.categories,
  });

  final String direction;
  final List<DataChoice> accounts;
  final List<DataChoice> categories;

  @override
  State<_DebtOpeningForm> createState() => _DebtOpeningFormState();
}

class _DebtOpeningFormState extends State<_DebtOpeningForm> {
  final _formKey = GlobalKey<FormState>();
  var _source = 'cash';
  String? _openingAccountId;
  String? _categoryId;

  bool get _sourceIsCategorical => _source != 'cash';

  @override
  void initState() {
    super.initState();
    _openingAccountId = widget.accounts.isEmpty
        ? null
        : widget.accounts.first.id;
    final options = _DebtSourceFields.categoriesFor(
      widget.direction,
      widget.categories,
    );
    _categoryId = options.isEmpty ? null : options.first.id;
  }

  @override
  Widget build(BuildContext context) => Form(
    key: _formKey,
    child: AppFormSheet<Map<String, dynamic>>(
      title: 'Borcun açılışı',
      description:
          'Bu borç, kaynağı sorulmadan önce kaydedilmiş. Kaydettiğiniz anda '
          'hesap ve raporlara girmeye başlar; geçmiş tutarlar değişmez.',
      submitLabel: 'Kaydet',
      onSubmit: () async {
        if (!(_formKey.currentState?.validate() ?? false)) return null;
        return {
          'sourceType': _source,
          'openingAccountId': _sourceIsCategorical ? null : _openingAccountId,
          'categoryId': _sourceIsCategorical ? _categoryId : null,
        };
      },
      children: [
        _DebtSourceFields(
          direction: widget.direction,
          source: _source,
          accounts: widget.accounts,
          categories: widget.categories,
          openingAccountId: _openingAccountId,
          categoryId: _categoryId,
          onSourceChanged: (value) => setState(() => _source = value),
          onAccountChanged: (value) =>
              setState(() => _openingAccountId = value),
          onCategoryChanged: (value) => setState(() => _categoryId = value),
        ),
      ],
    ),
  );
}
