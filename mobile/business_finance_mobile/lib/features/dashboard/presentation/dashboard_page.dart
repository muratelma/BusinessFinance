import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../account/presentation/account_status_controller.dart';
import '../../../core/routing/app_locations.dart';
import '../../../core/formatters/date_text.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/models/transaction_scope.dart';
import '../../../core/theme/app_breakpoints.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_finance_icons.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_donut_chart.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_metric_tile.dart';
import '../../../core/widgets/app_money_text.dart';
import '../../../core/widgets/app_responsive_grid.dart';
import '../../../core/widgets/app_scope_selector.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_trend_chart.dart';
import '../../activities/data/planned_activity_models.dart';
import '../../planning/data/planning_models.dart';
import '../data/dashboard_models.dart';
import 'dashboard_view_model.dart';

class DashboardPage extends StatelessWidget {
  const DashboardPage({super.key});

  @override
  Widget build(BuildContext context) {
    final viewModel = context.watch<DashboardViewModel>();
    final state = _stateView(viewModel);
    return Scaffold(
      // Hesabın kapısı sağ üstte: Özet ana sekme olduğu için hesap her
      // yerden bir dokunuş uzakta kalıyor ve `Diğer` menüsünden bir satır
      // düşüyor. İkon bir profil fotoğrafı değil — uygulamada avatar yok.
      appBar: AppBar(
        title: const Text('Özet'),
        actions: [
          // Doğrulanmamış e-postanın kalıcı uyarısı ikonun üstünde küçük bir
          // nokta: uyarının doğal yeri, gidip düzelteceği sayfanın kapısıdır.
          Badge(
            isLabelVisible:
                context
                    .watch<AccountStatusController?>()
                    ?.needsEmailVerification ??
                false,
            smallSize: 8,
            child: IconButton(
              icon: const Icon(Icons.account_circle_outlined),
              tooltip: 'Hesabım',
              onPressed: () => context.push(accountLocation),
            ),
          ),
        ],
      ),
      // Anahtar kaydırılan gövdenin **dışında**: uygulamanın tek kapsam
      // denetimi bu ve yükleme, hata ya da boş durumda da yerinde durmalı —
      // kullanıcı listeyi boş görüp anahtarın nerede olduğunu aramamalı.
      body: Column(
        children: [
          if (viewModel.isScopeVisible)
            _ScopeBar(
              value: viewModel.scope,
              onChanged: (value) =>
                  context.read<DashboardViewModel>().selectScope(value),
            ),
          Expanded(child: state ?? _buildReport(context, viewModel)),
        ],
      ),
    );
  }

  Widget _buildReport(BuildContext context, DashboardViewModel viewModel) {
    final report = viewModel.report!;
    final advanced = viewModel.advanced;

    return RefreshIndicator(
      onRefresh: viewModel.load,
      child: ListView(
        padding: const EdgeInsets.fromLTRB(
          AppSpacing.medium,
          AppSpacing.small,
          AppSpacing.medium,
          AppSpacing.fabClearance,
        ),
        children: [
          _MonthSelector(
            // Ekranın en üstündeki bağlam makine biçimindeydi (`08.2026`).
            // Ay adları zaten tek bir yerde duruyor.
            label: DateText.monthYear(report.year, report.month),
            onPrevious: viewModel.previousMonth,
            onNext: viewModel.nextMonth,
          ),
          const SizedBox(height: AppSpacing.medium),

          // Uyarı hero metriğin **üstünde**: "ekran başına tek mesaj"
          // kuralının istisnası değil, sırası. Bir şey gecikmişse ekranın
          // söylemesi gereken ilk şey odur; gecikme yokken bant hiç
          // çizilmez ve sıra normale döner.
          if (viewModel.overdue.isNotEmpty) ...[
            _OverdueBand(items: viewModel.overdue),
            const SizedBox(height: AppSpacing.medium),
          ],
          // Ekranın söylediği tek şey en üstte ve en büyük.
          //
          // İşletmesi olan kullanıcıda bu tek şey **işletme netidir**: "bu ay
          // ne kaldı" sorusunun dükkân tarafı. Şahsi taraf ve ikisinin toplamı
          // hemen altında durur, çünkü kapsam varken tek bir "net" hangi neti
          // sorduğunu söylemiyordu.
          ..._buildHero(viewModel, report, advanced),
          AppResponsiveGrid(
            minItemWidth: 150,
            spacing: AppSpacing.small,
            children: [
              AppMetricTile(
                key: const ValueKey('dashboard-summary-Gelir'),
                label: 'Gelir',
                amount: report.totalIncome,
                currency: report.currency,
                icon: Icons.south_west,
                effect: AppMoneyEffect.income,
                tinted: true,
              ),
              AppMetricTile(
                key: const ValueKey('dashboard-summary-Gider'),
                label: 'Gider',
                amount: report.totalExpense,
                currency: report.currency,
                icon: Icons.north_east,
                effect: AppMoneyEffect.expense,
                tinted: true,
              ),
            ],
          ),
          // Ekran iki soruya ayrı ayrı yanıt verir ve sıra bunu izler:
          // önce **bu ay nasıl geçti** (akış), sonra **şu an nerede
          // duruyorum** (durum). Önceden ikisi iç içeydi — varlık durumu
          // kategori ve bütçenin üstündeydi — ve okuyucu akıştan duruma,
          // oradan tekrar akışa geçiyordu. İki blok arasındaki dikiş aynı
          // zamanda yeni bölümlerin gireceği yerdir.

          // — Bu ay —
          const SizedBox(height: AppSpacing.large),
          AppSectionHeader(
            title: 'Kategori giderleri',
            trailing: report.categoryExpenses.isEmpty
                ? null
                : Text(
                    '${report.categoryExpenses.length} kategori',
                    style: Theme.of(context).textTheme.labelMedium,
                  ),
          ),
          _CategoryBreakdown(report: report),
          // Gelişmiş rapor ikincil okumadır; düşerse yalnız bu bölümler
          // gizlenir, ay özeti tek başına geçerli bir ekrandır.
          if (advanced != null && advanced.budgetVariances.isNotEmpty) ...[
            const SizedBox(height: AppSpacing.large),
            const AppSectionHeader(title: 'Bütçe durumu'),
            _BudgetStatus(
              items: advanced.budgetVariances,
              currency: advanced.currency,
            ),
          ],

          // — Önümde ne var —
          //
          // Üçüncü bir zaman dilimi: geçmiş ("bu ay") ile şimdi ("şu an")
          // arasında değil, ikisinin arasındaki dikişte duruyor. Ekran
          // bugüne kadar yalnız olan biteni anlatıyordu; gecikmiş bandı da
          // yalnız **kaçırılmış** olanı söylüyor, henüz gelmemişi değil.
          const SizedBox(height: AppSpacing.large),
          AppSectionHeader(
            title: 'Yaklaşanlar',
            // Pencere düz yazı değil rozet: `11 kategori` bir **sayım**,
            // bu ise listenin **sınırı**. Bölüm 7 günü gösteriyor ve bunun
            // görünmesi zorunlu — ekranda 30 günlük başka toplamlar da
            // olabiliyor, iki pencere etiketsiz yan yana durursa kullanıcı
            // ikisini karşılaştırıp tutturamaz. Rozetin kendi iç boşluğu
            // yazıyı sağ kenardan içeri alıyor ve zemini onu düz sayaçtan
            // ayırıyor.
            trailing: _HorizonBadge(
              label: DashboardViewModel.upcomingHorizon.label,
            ),
          ),
          _Upcoming(
            items: viewModel.upcoming,
            hiddenCount: viewModel.upcomingHiddenCount,
          ),

          // — Şu an —
          if (advanced != null) ...[
            const SizedBox(height: AppSpacing.large),
            const AppSectionHeader(title: 'Varlık durumu'),
            // Kapsam anahtarı açıkken bu bölümün **değişmediğini** ekran
            // söyler. Sessizce aynı kalsaydı kullanıcı filtrelenmiş sanır ve
            // iki tarafın net varlığını toplamaya çalışırdı.
            if (viewModel.scope != null) const _UnsplitNote(),
            _NetWorthCard(report: advanced),
            // `Bu ay nasıl bölündü` (halka) ve `Son 6 ay` (eğilim) geçici
            // olarak kapalı: iki grafik de biçim olarak henüz yerine
            // oturmadı ve ekranı taşıyacakları bilgiden fazla yer
            // kaplıyorlardı. Bileşenler (`AppDonutChart`, `AppTrendChart`)
            // ve testleri duruyor; biçim kararı verildiğinde bu iki satır
            // geri açılır.
          ],
          const SizedBox(height: AppSpacing.large),
          const AppSectionHeader(title: 'Hesap bakiyeleri'),
          if (viewModel.scope != null) const _UnsplitNote(),
          _AccountBalances(report: report),
        ],
      ),
    );
  }

  /// Ayın netini soran blok.
  ///
  /// Üç biçimi var ve hepsi aynı soruyu farklı bilinenlerle yanıtlıyor:
  ///
  /// - Kapsam boyutu görünmüyorsa ekran bugünkü davranışını korur: tek `Bu
  ///   ayın neti`. Kapsamı olmayan kullanıcı için ikinci bir sayı yok.
  /// - Anahtar `Hepsi` konumundaysa üç sayı: işletme neti (hero), şahsi taraf
  ///   ve ikisinin toplamı. Üçü de sunucudan gelir; istemci çıkarma yapmaz.
  /// - Anahtar bir tarafı seçtiyse rapor zaten o taraftır; hero o tarafın
  ///   netini, adını yazarak gösterir.
  List<Widget> _buildHero(
    DashboardViewModel viewModel,
    DashboardReport report,
    AdvancedReport? advanced,
  ) {
    final breakdown = report.scopeBreakdown;
    if (!viewModel.isScopeVisible || breakdown == null) {
      return [
        AppMetricTile(
          key: const ValueKey('dashboard-summary-Net'),
          size: AppMetricSize.hero,
          label: _singleNetLabel(viewModel.scope),
          amount: report.net,
          currency: report.currency,
          caption: viewModel.scope == null
              ? _netCaption(advanced)
              : 'Yalnız ${viewModel.scope!.label.toLowerCase()} tarafı',
        ),
        const SizedBox(height: AppSpacing.small),
      ];
    }

    return [
      AppMetricTile(
        key: const ValueKey('dashboard-summary-Net'),
        size: AppMetricSize.hero,
        label: 'İşletme neti',
        amount: breakdown.business.net,
        currency: report.currency,
        // "Kâr" değil: muhasebe kârı satılan malın maliyetini ister ve ürün
        // sınırının dışındadır. Yanlış kelime kullanıcıyı vergi beyanında
        // yanıltır.
        caption: 'İşletme geliri eksi işletme gideri',
      ),
      const SizedBox(height: AppSpacing.small),
      AppResponsiveGrid(
        minItemWidth: 150,
        spacing: AppSpacing.small,
        children: [
          AppMetricTile(
            key: const ValueKey('dashboard-summary-Personal'),
            label: _personalLabel(breakdown.personal.net),
            amount: breakdown.personal.net,
            currency: report.currency,
            caption: 'Şahsi gelir eksi şahsi gider',
          ),
          AppMetricTile(
            key: const ValueKey('dashboard-summary-Total'),
            label: 'Bu ayın neti',
            amount: report.net,
            currency: report.currency,
            caption: _netCaption(advanced),
          ),
        ],
      ),
      const SizedBox(height: AppSpacing.small),
    ];
  }

  /// Kapsam boyutu görünmeyen ya da tek tarafı okuyan ekranın hero etiketi.
  static String _singleNetLabel(TransactionScope? scope) => switch (scope) {
    null => 'Bu ayın neti',
    TransactionScope.business => 'İşletme neti',
    TransactionScope.personal => 'Şahsi net',
  };

  /// Şahsi tarafın adı sayının yönüne göre değişir.
  ///
  /// Çoğu esnafta şahsi taraf yalnız harcamadır ve doğru kelime **çekim**:
  /// havuzdan cebe geçen para. Ama şahsi bir gelir de girilebiliyor; o ayda
  /// "çekim" demek artı bir sayıyı eksi gibi okuturdu.
  static String _personalLabel(String personalNet) =>
      personalNet.startsWith('-') ? 'Şahsi çekim' : 'Şahsi net';

  /// Hero kartın altındaki bağlam satırı: mümkünse önceki dönemle
  /// karşılaştırır, gelişmiş rapor yoksa ne olduğunu açıklar.
  static String _netCaption(AdvancedReport? advanced) {
    if (advanced == null) return 'Gelir eksi gider';
    final current = double.tryParse(advanced.currentPeriod.net);
    final previous = double.tryParse(advanced.previousPeriod.net);
    if (current == null || previous == null) return 'Gelir eksi gider';
    final difference = current - previous;
    if (difference == 0) return 'Geçen ayla aynı';
    final direction = difference > 0 ? 'daha iyi' : 'daha düşük';
    final amount = MoneyText.format(
      difference.abs().toStringAsFixed(4),
      advanced.currency,
    );
    return 'Geçen aya göre $amount $direction';
  }
}

extension on DashboardPage {
  Widget? _stateView(DashboardViewModel viewModel) {
    if (viewModel.isLoading && !viewModel.hasLoaded) {
      return const AppLoadingView(message: 'Finansal özet yükleniyor');
    }
    if (viewModel.error case final error?) {
      if (error.isUnauthorized) return const AppUnauthorizedView();
      return AppErrorView(message: error.message, onRetry: viewModel.load);
    }
    if (viewModel.report == null) {
      return const AppEmptyView(
        icon: Icons.account_balance_wallet_outlined,
        title: 'Finansal özet bulunamadı',
        message: 'Özetinizi yenilemek için tekrar deneyin.',
      );
    }
    return null;
  }
}

/// Kapsam anahtarının ekrandaki yeri: başlığın hemen altında, kaydırılan
/// gövdenin dışında.
class _ScopeBar extends StatelessWidget {
  const _ScopeBar({required this.value, required this.onChanged});

  final TransactionScope? value;
  final ValueChanged<TransactionScope?> onChanged;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.medium,
        vertical: AppSpacing.small,
      ),
      child: Align(
        alignment: AlignmentDirectional.centerStart,
        child: AppScopeSwitch(value: value, onChanged: onChanged),
      ),
    );
  }
}

/// Kapsam filtresinden **etkilenmeyen** bölümün altındaki not.
///
/// Bakiye, kart borcu ve net varlık tek havuzdur (ADR 0013); anahtarın
/// konumuna göre değişselerdi "ne kadar param var" sorusunun aynı anda iki
/// farklı doğru cevabı olurdu. Bunu yazmak zorunlu: filtre açıkken sessizce
/// aynı kalan bir sayı, filtrelenmiş sanılır.
class _UnsplitNote extends StatelessWidget {
  const _UnsplitNote();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Padding(
      padding: const EdgeInsets.only(bottom: AppSpacing.small),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(
            Icons.info_outline,
            size: AppSpacing.medium,
            color: theme.colorScheme.onSurfaceVariant,
          ),
          const SizedBox(width: AppSpacing.xSmall),
          Expanded(
            child: Text(
              'Kapsam filtresinden etkilenmez: işletme ve şahsi toplamıdır.',
              style: theme.textTheme.bodySmall?.copyWith(
                color: theme.colorScheme.onSurfaceVariant,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// Ay seçici: iki ok ve ortada dönem. Kart yüzeyine oturur, böylece sayfa
/// zemininde yüzen üç ayrı öğe gibi durmaz.
class _MonthSelector extends StatelessWidget {
  const _MonthSelector({
    required this.label,
    required this.onPrevious,
    required this.onNext,
  });

  final String label;
  final VoidCallback onPrevious;
  final VoidCallback onNext;

  @override
  Widget build(BuildContext context) {
    final surfaces = AppSurfaces.of(context);
    return Container(
      decoration: BoxDecoration(
        color: surfaces.card,
        borderRadius: BorderRadius.circular(AppRadius.field),
        border: Border.all(color: surfaces.border),
      ),
      child: Row(
        children: [
          IconButton(
            tooltip: 'Önceki ay',
            onPressed: onPrevious,
            icon: const Icon(Icons.chevron_left),
          ),
          // Esnek: en büyük yazı ölçeğinde iki ok arasına sığmalı.
          Expanded(
            child: Semantics(
              header: true,
              child: Text(
                label,
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.titleMedium,
              ),
            ),
          ),
          IconButton(
            tooltip: 'Sonraki ay',
            onPressed: onNext,
            icon: const Icon(Icons.chevron_right),
          ),
        ],
      ),
    );
  }
}

/// Net varlık: likit varlık eksi kart borcu. Ay özetinden farklı bir soruyu
/// yanıtlar — "bu ay nasıl geçti" değil, "şu an nerede duruyorum".
class _NetWorthCard extends StatelessWidget {
  const _NetWorthCard({required this.report});

  final AdvancedReport report;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Semantics(
            container: true,
            label:
                'Net varlık: '
                '${MoneyText.format(report.netWorth, report.currency)}',
            child: ExcludeSemantics(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Net varlık', style: theme.textTheme.labelMedium),
                  const SizedBox(height: AppSpacing.xSmall),
                  AppMoneyText(
                    amount: report.netWorth,
                    currency: report.currency,
                    style: theme.textTheme.titleLarge,
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: AppSpacing.medium),
          const Divider(height: 1),
          const SizedBox(height: AppSpacing.medium),
          _NetWorthLine(
            icon: Icons.savings_outlined,
            label: 'Likit varlık',
            amount: report.liquidAssets,
            currency: report.currency,
            effect: null,
            meaning: 'net varlığa eklenir',
          ),
          const SizedBox(height: AppSpacing.small),
          // Kart borcu negatif olabilir (Aşama 06 Grup 5): fazla ödenmiş
          // kartta duran para kullanıcınındır ve net varlığı **artırır**.
          // Satır o zaman hem adını hem yönünü değiştirir; "Kart borcu −₺500,
          // net varlığı düşürür" iki kez yanlış olurdu.
          if (MoneyText.isNegative(report.creditCardDebt))
            _NetWorthLine(
              icon: Icons.credit_card,
              label: 'Kart alacağı',
              amount: MoneyText.unsigned(report.creditCardDebt),
              currency: report.currency,
              effect: AppMoneyEffect.income,
              meaning: 'net varlığa eklenir',
            )
          else
            _NetWorthLine(
              icon: Icons.credit_card,
              label: 'Kart borcu',
              amount: report.creditCardDebt,
              currency: report.currency,
              effect: AppMoneyEffect.expense,
              meaning: 'net varlığı düşürür',
            ),
          // Net varlık dört terimden hesaplanıyor:
          //   likit varlık − kart borcu + alacak − borç
          // Kart uzun süre ilk ikisini gösterdi; açık bir borcu ya da alacağı
          // olan kullanıcıda alttaki döküm üstteki toplamı açıklamıyordu.
          // Sayı yanlış değildi, dökümü eksikti.
          //
          // Sıfırken çizilmiyorlar: borcu olmayan kullanıcı için kart eskisi
          // gibi kalıyor ve iki satır boş yer kaplamıyor. Bu bir para hesabı
          // değil, "gösterilecek bir şey var mı" kontrolü.
          // Yoldaki para likit varlığın **dışındadır** ve net varlığın
          // içindedir (ADR 0015). Likit varlığın hemen altında duruyor çünkü
          // açıkladığı şey o iki sayının farkı: "neden harcayabildiğimden
          // fazla param var?" sorusunun cevabı burada.
          if (_hasAmount(report.moneyInTransit)) ...[
            const SizedBox(height: AppSpacing.small),
            _NetWorthLine(
              icon: Icons.schedule_outlined,
              label: 'Yolda',
              amount: report.moneyInTransit,
              currency: report.currency,
              effect: AppMoneyEffect.neutral,
              meaning: 'net varlığa eklenir, henüz harcanamaz',
              subtitle: 'POS tahsilatı',
            ),
          ],
          if (_hasAmount(report.receivableDebt)) ...[
            const SizedBox(height: AppSpacing.small),
            // Alacak **nötr** tonda: gelir yeşili değil. Alacak gelir değil,
            // bir varlık kalemi — likit varlık gibi. Yeşile boyansaydı hem
            // "yeşil yalnız gelirdir" kuralı kırılırdı hem de tahsil
            // edilmemiş bir para kazanılmış gibi okunurdu.
            _NetWorthLine(
              icon: Icons.handshake_outlined,
              label: 'Alacak',
              amount: report.receivableDebt,
              currency: report.currency,
              effect: AppMoneyEffect.neutral,
              meaning: 'sana girecek, gelir değil',
              subtitle: 'faiz hariç',
            ),
          ],
          if (_hasAmount(report.payableDebt)) ...[
            const SizedBox(height: AppSpacing.small),
            // Borç, kart borcuyla aynı rolde: net varlığı düşüren yükümlülük.
            _NetWorthLine(
              icon: Icons.account_balance_outlined,
              label: 'Borç',
              amount: report.payableDebt,
              currency: report.currency,
              effect: AppMoneyEffect.neutral,
              meaning: 'senden çıkacak, gider değil',
              subtitle: 'faiz hariç',
            ),
          ],
        ],
      ),
    );
  }

  /// Tutarın gösterilecek bir değeri var mı.
  ///
  /// Metni sayıya çevirmek burada para aritmetiği değil, varlık kontrolü:
  /// üretilen hiçbir yeni tutar yok, gösterilen değer yine sunucunun
  /// gönderdiği string. Çözümlenemeyen bir değer gizlenmiyor — gizlenseydi
  /// biçim değişikliği satırı sessizce yok ederdi.
  static bool _hasAmount(String amount) => double.tryParse(amount) != 0;
}

/// Net varlığın bir terimi: ikon, ad ve tutar.
///
/// Renk **yönü** söyler, işareti değil:
///
/// - **Likit varlık** nötr mürekkep. Duran paranın bir yönü yok.
/// - **Kart borcu** gider kırmızısı. Birikmiş `CreditCardCharge`'lardır ve
///   **her biri gider olarak yazılmıştır**; kırmızı burada gerçekten gideri
///   gösteriyor.
/// - **Borç** ve **alacak** nötr mavi, ve ikisi **simetriktir**. Taksit
///   ödemesi gider yazmaz, tahsilat gelir yazmaz; ikisi de saf bakiye
///   hareketidir — `neutral` rolünün sözleşmesi ("gelir de gider de değil")
///   tam olarak budur. Bir ara borç kırmızıydı: kart borcuyla aynı kefeye
///   konmuştu, oysa kart borcu birikmiş **giderdir**, borç değildir.
///
/// Bir ara satırların başına `+`/`−` konmuştu. Gereksizdi: yönü zaten renk ve
/// satırın **adı** söylüyor (`Alacak` ile `Borç` karışmıyor), yani bilgi
/// yalnız renge bağlı değil. İşaret üçüncü bir kanal açıp hiçbir şey
/// eklemiyordu.
class _NetWorthLine extends StatelessWidget {
  const _NetWorthLine({
    required this.icon,
    required this.label,
    required this.amount,
    required this.currency,
    required this.effect,
    required this.meaning,
    this.subtitle,
  });

  final IconData icon;
  final String label;
  final String amount;
  final String currency;

  /// Tutarın tonu; `null` ise nötr mürekkep.
  final AppMoneyEffect? effect;

  /// Ekran okuyucuya satırın net varlıktaki rolü. Renk bir konuşma kanalı
  /// değildir; bu cümle onun yerine geçer.
  final String meaning;

  /// Sayının ne olduğunu söyleyen alt satır.
  ///
  /// Borç ve alacakta `faiz hariç` yazıyor. Sayının faizi içerip içermediği
  /// ekrandan okunamıyordu ve bu gerçek bir soru: 1.000 anapara / 1.200
  /// toplam bir kredide hangisinin yazdığı belli değildi.
  final String? subtitle;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Semantics(
      container: true,
      label:
          '$label: ${MoneyText.format(amount, currency)}, $meaning'
          '${subtitle == null ? '' : ', $subtitle'}',
      child: ExcludeSemantics(
        child: Row(
          children: [
            Icon(icon, size: 18, color: theme.colorScheme.onSurfaceVariant),
            const SizedBox(width: AppSpacing.small),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(label, style: theme.textTheme.bodyMedium),
                  if (subtitle != null)
                    Text(subtitle!, style: theme.textTheme.bodySmall),
                ],
              ),
            ),
            const SizedBox(width: AppSpacing.small),
            AppMoneyText(
              amount: amount,
              currency: currency,
              effect: effect,
              style: theme.textTheme.titleSmall,
            ),
          ],
        ),
      ),
    );
  }
}

/// Bu ayın gelir/gider bölünmesi. Halka, iki parçalı tek bir dönem için
/// sütun grafiğinden daha doğru bir biçim: soru "hangisi daha büyük ve ne
/// kadarı" ve halka bunu tek bakışta yanıtlıyor.
// Özet ekranında geçici olarak kapalı (biçim kararı bekliyor); sınıf
// silinmedi çünkü geri açılacak.
// ignore: unused_element
class _MonthSplit extends StatelessWidget {
  const _MonthSplit({required this.report});

  final DashboardReport report;

  @override
  Widget build(BuildContext context) {
    final colors = AppFinanceColors.of(context);
    return AppDonutChart(
      centerLabel: 'Net',
      centerValue: MoneyText.format(report.net, report.currency),
      slices: [
        AppDonutSlice(
          label: 'Gelir',
          value: double.tryParse(report.totalIncome) ?? 0,
          color: colors.incomeFill,
          formattedValue: MoneyText.format(report.totalIncome, report.currency),
        ),
        AppDonutSlice(
          label: 'Gider',
          value: double.tryParse(report.totalExpense) ?? 0,
          color: colors.expenseFill,
          formattedValue: MoneyText.format(
            report.totalExpense,
            report.currency,
          ),
        ),
      ],
    );
  }
}

// Özet ekranında geçici olarak kapalı (biçim kararı bekliyor); sınıf
// silinmedi çünkü geri açılacak.
// ignore: unused_element
class _CashFlow extends StatelessWidget {
  const _CashFlow({required this.report});

  final AdvancedReport report;

  @override
  Widget build(BuildContext context) {
    return AppTrendChart(
      currency: report.currency,
      points: [
        for (final point in report.cashFlowTrend)
          AppTrendPoint(
            label: point.month.toString().padLeft(2, '0'),
            net: point.net,
            // Geliri de gideri de sıfır olan ay için çubuk çizilmez: boş bir
            // sütun "sıfır net" ile "hiç hareket yok"u aynı gösteriyordu.
            hasData:
                (double.tryParse(point.income) ?? 0) != 0 ||
                (double.tryParse(point.expense) ?? 0) != 0,
          ),
      ],
    );
  }
}

/// Kategori giderleri: tutarın yanında toplam içindeki payı da gösterir.
/// "Market 1.100 TL" tek başına çok mu az mı belli değildir; pay bunu bir
/// bakışta yanıtlar.
/// Kategori dağılımı: halka ve yanında ikonlu efsane.
///
/// Önce her kategori için bir pay çubuğu vardı. Çubuk oranı doğru okutur ama
/// "bu ayın parası nasıl bölündü" sorusunu tek bakışta yanıtlamaz; kategori
/// sayısı arttıkça liste de uzayıp gidiyordu.
///
/// Halka en büyük dört kategoriyi ve "Diğer"i gösteriyor. Gruplamayı **sunucu**
/// yapıyor: "Diğer" bir finansal toplam ve istemci parayı ikinci kez
/// hesaplamaz.
///
/// Efsanede nokta yerine kategorinin **ikonu** var, dilimin rengine boyalı.
/// Böylece satır iki kanaldan birden okunuyor: renk dilimle eşleşiyor, ikon
/// kategoriyi renkten bağımsız söylüyor.
class _CategoryBreakdown extends StatelessWidget {
  const _CategoryBreakdown({required this.report});

  final DashboardReport report;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final slices = report.categoryExpenseSlices;

    if (slices.isEmpty) {
      return AppCard(
        child: Text(
          'Bu ay kategori gideri yok.',
          style: theme.textTheme.bodySmall,
        ),
      );
    }

    return AppCard(
      child: LayoutBuilder(
        builder: (context, constraints) {
          final legend = _legend(context, slices);
          final chart = _chart(context, slices);

          // Yan yana durabilmeleri iki şeye bağlı: kabın genişliği ve yazı
          // ölçeği. Yalnız genişliğe bakmak yetmiyordu — kart yeterince geniş
          // olsa bile büyütülmüş yazıda efsane satırı taşıyordu.
          final scaled = MediaQuery.textScalerOf(context).scale(1) > 1.3;
          if (scaled || constraints.maxWidth < AppBreakpoints.donutWithLegend) {
            return Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              children: [
                Center(child: chart),
                const SizedBox(height: AppSpacing.medium),
                legend,
              ],
            );
          }
          return Row(
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              chart,
              const SizedBox(width: AppSpacing.medium),
              Expanded(child: legend),
            ],
          );
        },
      ),
    );
  }

  Widget _chart(BuildContext context, List<CategoryExpenseSlice> slices) =>
      AppDonutChart(
        size: 132,
        thickness: 24,
        showLegend: false,
        slices: [
          for (var i = 0; i < slices.length; i++)
            AppDonutSlice(
              label: slices[i].categoryName,
              value: double.tryParse(slices[i].amount) ?? 0,
              color: _sliceColor(context, slices, i),
              formattedValue: MoneyText.format(
                slices[i].amount,
                report.currency,
              ),
            ),
        ],
      );

  Widget _legend(BuildContext context, List<CategoryExpenseSlice> slices) =>
      Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        mainAxisSize: MainAxisSize.min,
        children: [
          for (var i = 0; i < slices.length; i++) ...[
            if (i > 0) const SizedBox(height: AppSpacing.small),
            _LegendRow(
              slice: slices[i],
              currency: report.currency,
              color: _sliceColor(context, slices, i),
            ),
          ],
        ],
      );

  /// "Diğer" paletin dışında kendi grisini alır: o dilim bir kategori değil,
  /// kalanın toplamı ve adı olan kategorilerle renk yarışına girmemeli.
  static Color _sliceColor(
    BuildContext context,
    List<CategoryExpenseSlice> slices,
    int index,
  ) {
    final colors = AppFinanceColors.of(context);
    return slices[index].isOther
        ? colors.categoryOtherSlice
        : colors.categorySlice(index);
  }
}

class _LegendRow extends StatelessWidget {
  const _LegendRow({
    required this.slice,
    required this.currency,
    required this.color,
  });

  final CategoryExpenseSlice slice;
  final String currency;
  final Color color;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Semantics(
      container: true,
      label:
          '${slice.categoryName}: '
          '${MoneyText.format(slice.amount, currency)}',
      child: ExcludeSemantics(
        child: Row(
          children: [
            Icon(
              slice.isOther
                  ? Icons.more_horiz
                  : AppFinanceIcons.forCategory(
                      slice.canonicalName,
                      displayName: slice.categoryName,
                    ),
              size: 18,
              color: color,
            ),
            const SizedBox(width: AppSpacing.small),
            // Ad `bodyMedium`; `bodySmall`'da bırakılmıştı ve küçük
            // görünüyordu — sebebi punto değil kademeydi: `bodySmall` ölçekte
            // **yardımcı metin** (alt satır, tarih, kaynak) kademesidir,
            // efsane satırı ise birincil içeriktir.
            //
            // Boş alanın tamamını **ad** yutar, tutar esnek değil: satırın
            // düzeni `_NetWorthLine` ve bütçe satırıyla aynı olsun diye.
            // Tutar da `Flexible` olsaydı ikisi flex 1'le boşluğu paylaşırdı;
            // ölçüldüğünde bu tek başına kaymaya yol açmıyor ama üç satır
            // tipinin üç ayrı düzeni olması için de sebep yok.
            Expanded(
              child: Text(
                slice.categoryName,
                style: theme.textTheme.bodyMedium,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ),
            const SizedBox(width: AppSpacing.small),
            // Tutar adla aynı kademede ve **kalın değil**. Efsanenin işi
            // dilimi adlandırmak; büyüklük karşılaştırmasını halkanın kendisi
            // zaten yapıyor, tutarı ayrıca kalınlaştırmak satırda ikinci bir
            // vurgu açıyordu.
            AppMoneyText(
              amount: slice.amount,
              currency: currency,
              style: theme.textTheme.bodyMedium,
            ),
          ],
        ),
      ),
    );
  }
}

/// Bütçe durumu: limiti aşan kategoriler ve ne kadar aştıkları.
///
/// Önce yalnız aşan kategorilerin **adları** alt alta yazılıyordu. İki şeyi
/// birden söylemiyordu: aşımın büyüklüğünü (5 TL ile 500 TL aynı görünüyordu)
/// ve aşmayan bütçelerin var olduğunu — beş bütçenin ikisi aşmışsa diğer üçü
/// ekrandan tamamen kayboluyor, bölüm de olduğundan boş duruyordu.
///
/// Aşım tutarı `remaining`'dir ve **negatif gelir**; istemci `limit − spent`
/// çıkarmasını kendisi yapmaz.
class _BudgetStatus extends StatelessWidget {
  const _BudgetStatus({required this.items, required this.currency});

  final List<BudgetVarianceItem> items;
  final String currency;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final exceeded = items.where((item) => item.isExceeded).toList();
    final withinCount = items.length - exceeded.length;

    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          if (exceeded.isEmpty)
            Wrap(
              spacing: AppSpacing.small,
              runSpacing: AppSpacing.small,
              crossAxisAlignment: WrapCrossAlignment.center,
              children: [
                const AppStatusChip(
                  label: 'Limit aşımı yok',
                  icon: Icons.check_circle_outline,
                  tone: AppStatusTone.income,
                ),
                Text(
                  '${items.length} bütçe izleniyor',
                  style: theme.textTheme.bodySmall,
                ),
              ],
            )
          else ...[
            AppStatusChip(
              label: '${exceeded.length} bütçe limiti aştı',
              icon: Icons.warning_amber_rounded,
              tone: AppStatusTone.expense,
            ),
            for (final item in exceeded)
              Padding(
                padding: const EdgeInsets.only(top: AppSpacing.small),
                child: _BudgetExceededRow(item: item, currency: currency),
              ),
            // Aşmayanlar sayı olarak kalıyor: bölüm bütün bütçeleri hesaba
            // katmalı, yoksa "iki bütçem mi var" izlenimi doğuyor. Adları
            // yazılmıyor — bölümün söylediği şey aşım.
            if (withinCount > 0) ...[
              const SizedBox(height: AppSpacing.small),
              Text(
                '$withinCount bütçe limit içinde',
                style: theme.textTheme.bodySmall,
              ),
            ],
          ],
        ],
      ),
    );
  }
}

/// Limiti aşmış tek bir bütçe satırı; ekranın satır ölçeğinde
/// (ad `bodyMedium`, tutar `titleSmall`) ve kategori ikonuyla.
class _BudgetExceededRow extends StatelessWidget {
  const _BudgetExceededRow({required this.item, required this.currency});

  final BudgetVarianceItem item;
  final String currency;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Semantics(
      container: true,
      // Ekranda eksi işaretli bir tutar var; ekran okuyucuya "eksi 250 lira"
      // demek yanıltırdı — o para kaybı değil, limitin aşılan kısmı.
      label:
          '${item.categoryName}: limiti '
          '${MoneyText.format(item.remaining.replaceFirst('-', ''), currency)} '
          'aştı',
      child: ExcludeSemantics(
        child: Row(
          children: [
            Icon(
              AppFinanceIcons.forCategory(
                item.canonicalName,
                displayName: item.categoryName,
              ),
              size: 18,
              color: theme.colorScheme.onSurfaceVariant,
            ),
            const SizedBox(width: AppSpacing.small),
            Expanded(
              child: Text(
                item.categoryName,
                style: theme.textTheme.bodyMedium,
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ),
            const SizedBox(width: AppSpacing.small),
            AppMoneyText(
              amount: item.remaining,
              currency: currency,
              effect: AppMoneyEffect.expense,
              style: theme.textTheme.titleSmall,
            ),
          ],
        ),
      ),
    );
  }
}

class _AccountBalances extends StatelessWidget {
  const _AccountBalances({required this.report});

  final DashboardReport report;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final items = report.accountBalances;

    if (items.isEmpty) {
      return AppCard(
        child: Text('Henüz hesap yok.', style: theme.textTheme.bodySmall),
      );
    }

    return AppCard(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.small),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          for (var i = 0; i < items.length; i++) ...[
            if (i > 0)
              const Padding(
                padding: EdgeInsets.symmetric(horizontal: AppSpacing.medium),
                child: Divider(height: 1),
              ),
            AppListRow(
              // Nakit ile banka hesabı artık aynı ikonu paylaşmıyor; tür
              // sunucudan geliyor, addan tahmin edilmiyor.
              icon: AppFinanceIcons.forAccountType(items[i].type),
              title: items[i].accountName,
              subtitle: _typeLabel(items[i].type),
              trailing: AppMoneyText(
                amount: items[i].balance,
                currency: report.currency,
                style: theme.textTheme.titleSmall,
              ),
            ),
          ],
        ],
      ),
    );
  }

  static String? _typeLabel(String? type) => switch (type) {
    'cash' => 'Nakit',
    'bank' => 'Banka hesabı',
    _ => null,
  };
}

/// Bölümün gösterdiği zaman penceresi.
///
/// Sayaç değil sınır bildirir, o yüzden düz yazıdan ayrı duruyor: kendi
/// zemini, tam mürekkep rengi ve `labelMedium`'un ağırlığı. Rol renklerinden
/// birini almıyor — pencere finansal bir değer değil.
class _HorizonBadge extends StatelessWidget {
  const _HorizonBadge({required this.label});

  final String label;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Container(
      padding: const EdgeInsets.symmetric(
        horizontal: AppSpacing.small,
        vertical: AppSpacing.xSmall,
      ),
      decoration: BoxDecoration(
        color: AppSurfaces.of(context).cardMuted,
        borderRadius: BorderRadius.circular(AppRadius.field),
      ),
      child: Text(
        label,
        style: theme.textTheme.labelMedium?.copyWith(
          color: theme.colorScheme.onSurface,
        ),
      ),
    );
  }
}

/// Vadesi henüz gelmemiş yükümlülükler: en yakın birkaçı adıyla, vadesiyle ve
/// tutarıyla.
///
/// **Tutar toplamı yok, gecikmiş bandıyla aynı gerekçeyle.** İstemci finansal
/// toplamı ikinci kez hesaplamaz. Sunucunun 30 günlük `futureLoad` toplamı var
/// ama bu liste 7 günlük; ikisini aynı kartta yan yana koymak, kullanıcının
/// satırları toplayıp tutturamayacağı bir ekran üretirdi.
///
/// **Boş durumda gizlenmiyor.** Gecikmiş bandı bir istisnadır, yokken
/// çizilmemesi doğru. Bu ise sabit bir bölüm: gizlenirse sayfanın düzeni her
/// hafta değişir ve kullanıcı böyle bir bölümün var olduğunu hiç öğrenemez.
/// Ayrıca "ödeme yok" kendi başına iyi haberdir; boşluk aynı şeyi söylemez,
/// yalnız belirsizlik bırakır.
class _Upcoming extends StatelessWidget {
  const _Upcoming({required this.items, required this.hiddenCount});

  final List<PlannedActivity> items;
  final int hiddenCount;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    if (items.isEmpty) {
      return AppCard(
        child: Text('Bu hafta ödeme yok.', style: theme.textTheme.bodyMedium),
      );
    }

    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          for (var i = 0; i < items.length; i++) ...[
            if (i > 0) const SizedBox(height: AppSpacing.small),
            _UpcomingRow(item: items[i]),
          ],
          if (hiddenCount > 0) ...[
            const SizedBox(height: AppSpacing.small),
            const Divider(height: 1),
            const SizedBox(height: AppSpacing.small),
            // Yerinde açılmıyor, Planlananlar ekranına gidiyor: tam liste
            // zaten orada, ikinci bir liste davranışı bakım yükü olurdu.
            Semantics(
              button: true,
              label: '$hiddenCount kalem daha var. Planlananları açar.',
              child: ExcludeSemantics(
                child: InkWell(
                  onTap: () => context.push('/transactions/planned'),
                  borderRadius: BorderRadius.circular(AppRadius.field),
                  child: Padding(
                    padding: const EdgeInsets.symmetric(
                      vertical: AppSpacing.small,
                    ),
                    child: Row(
                      children: [
                        Expanded(
                          child: Text(
                            '$hiddenCount kalem daha',
                            style: theme.textTheme.bodySmall,
                          ),
                        ),
                        Icon(
                          Icons.chevron_right,
                          size: 20,
                          color: theme.colorScheme.onSurfaceVariant,
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }
}

/// Tek bir yaklaşan kalem; ekranın satır ölçeğinde.
///
/// Alt satır **vade + tür**: "20 Ağu · Tekrarlanan". Tür etiketi Flutter'da
/// üretiliyor, API kararlı makine değeri gönderiyor.
class _UpcomingRow extends StatelessWidget {
  const _UpcomingRow({required this.item});

  final PlannedActivity item;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Semantics(
      container: true,
      label:
          '${item.title}: ${MoneyText.format(item.amount, item.currency)}, '
          'vadesi ${DateText.dayMonth(item.dueDate)}, '
          '${item.plannedKind.label}',
      child: ExcludeSemantics(
        child: Row(
          children: [
            Icon(
              _icon(item.plannedKind),
              size: 18,
              color: theme.colorScheme.onSurfaceVariant,
            ),
            const SizedBox(width: AppSpacing.small),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                mainAxisSize: MainAxisSize.min,
                children: [
                  Text(
                    item.title,
                    style: theme.textTheme.bodyMedium,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                  Text(
                    '${DateText.dayMonth(item.dueDate)} · '
                    '${item.plannedKind.label}',
                    style: theme.textTheme.bodySmall,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
              ),
            ),
            const SizedBox(width: AppSpacing.small),
            // Gider kırmızısı. Bir ara nötr bırakılmıştı — "henüz gerçekleşmedi,
            // para çıkmış gibi okunmasın" diye. Ama bölümün bütün söylediği
            // şey bunların **ödenecek** olması; rengi kısmak uyarıyı kısıyor.
            // Gecikmiş bandı da aynı ailede duruyor.
            AppMoneyText(
              amount: item.amount,
              currency: item.currency,
              effect: AppMoneyEffect.expense,
              style: theme.textTheme.titleSmall,
            ),
          ],
        ),
      ),
    );
  }

  static IconData _icon(PlannedKind kind) => switch (kind) {
    PlannedKind.recurringOccurrence => Icons.autorenew,
    PlannedKind.cardStatement => Icons.receipt_long_outlined,
    PlannedKind.cardInstallment => Icons.credit_card,
    PlannedKind.debtInstallment => Icons.account_balance_outlined,
    PlannedKind.receivableInstallment => Icons.handshake_outlined,
    PlannedKind.payableObligation ||
    PlannedKind.receivableObligation => Icons.receipt_long_outlined,
  };
}

/// Vadesi geçmiş yükümlülüklerin uyarı bandı.
///
/// Neden var: gerçekleşmemiş bir kart taksidi ya da tekrarlanan plan hiçbir
/// yerde borç, gider veya ekstre üretmiyor. Bu doğru — onaysız para hareketi
/// olmuyor. Ama kullanıcı `Gerçekleştir`'e basmayı unutursa gerçekten yapılmış
/// bir harcama sessizce kayıt dışı kalıyor ve hiçbir ekran bunu söylemiyordu.
///
/// **Tutar toplamı yok, bilerek.** İstemci finansal toplamı ikinci kez
/// hesaplamaz; API de aynı gerekçeyle planlanan listesinde toplam vermiyor
/// (gelir, gider ve nötr yükümlülükleri tek sayıda toplamak yanıltırdı).
/// Bandın taşıdığı bilgi sayı ve en eski vade: ikisi de sayma işidir, para
/// aritmetiği değil.
///
/// Yalnız ödeme **yükümlülükleri** sayılıyor; bunu sunucu işaretliyor, kural
/// burada ikinci kez yazılmıyor.
class _OverdueBand extends StatelessWidget {
  const _OverdueBand({required this.items});

  final List<PlannedActivity> items;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final colors = AppFinanceColors.of(context);
    final oldest = items
        .map((item) => item.dueDate)
        .reduce((a, b) => a.compareTo(b) <= 0 ? a : b);

    return Semantics(
      container: true,
      liveRegion: true,
      // Bandın tamamı tek bir düğme. Ekran okuyucuya da öyle bildiriliyor;
      // görünürde buton olmayan bir dokunma hedefi, rolü söylenmezse
      // TalkBack kullanıcısı için hiç yok demektir.
      button: true,
      label:
          '${items.length} gecikmiş ödeme var. '
          'En eskisinin vadesi ${DateText.dayMonth(oldest)}. '
          'Planlananları açar.',
      child: ExcludeSemantics(
        child: AppCard(
          background: colors.expenseContainer,
          onTap: () => context.push('/transactions/planned'),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Row(
                children: [
                  // Renk tek başına anlam taşımaz: ikon ve metin de var.
                  Icon(
                    Icons.warning_amber_outlined,
                    color: colors.onExpenseContainer,
                  ),
                  const SizedBox(width: AppSpacing.small),
                  Expanded(
                    child: Text(
                      '${items.length} gecikmiş ödeme',
                      style: theme.textTheme.titleSmall?.copyWith(
                        color: colors.onExpenseContainer,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                  // Eylemin göstergesi bu; altta ayrı bir buton yok.
                  //
                  // Önce `FilledButton.tonal` vardı ve kendi kapsayıcı rengini
                  // (gri) kullanıyordu: pembe `expenseContainer`'ın üstüne
                  // ikinci bir renkli yüzey biniyor, bant hem yükseliyor hem
                  // alacalı duruyordu. Bandın kendisi zaten tek bir hedefe
                  // gidiyor — o hâlde bant düğmenin kendisi olsun.
                  Icon(Icons.chevron_right, color: colors.onExpenseContainer),
                ],
              ),
              const SizedBox(height: AppSpacing.xSmall),
              // İki bilgi, iki satır — ve satırları **biz** ayırıyoruz.
              //
              // Tek `Text` olarak `En eskisi 1 Ağustos • Onaylanana kadar
              // kayda geçmez` yazıldığında sarma noktası kabın genişliğine
              // düşüyordu ve telefonda `geçmez` tek başına ikinci satırda
              // kalıyordu. Yetim kelime satırı dağınık gösteriyor, üstelik
              // nerede kırılacağı cihaza ve yazı ölçeğine göre değişiyor.
              //
              // Bölünme artık anlamın olduğu yerde: birinci satır **ne zaman**
              // (vade), ikincisi **neden önemli** (onaylanmazsa kaydedilmez).
              // İkisi de tek satıra sığacak kadar kısa; yazı ölçeği
              // büyüdüğünde sarsalar bile kendi cümleleri içinde sarıyorlar.
              Text(
                'En eskisi ${DateText.dayMonth(oldest)}',
                style: theme.textTheme.bodySmall?.copyWith(
                  color: colors.onExpenseContainer,
                ),
              ),
              // "Onay bekliyor"a kısaltılmadı: o, durumu söyler, bandın var
              // olma sebebini söylemez. Bant tam da onaylanmayan bir hareketin
              // **hiçbir yerde görünmemesi** yüzünden var; kullanıcının
              // bilmesi gereken şey bekleme değil, sonucu.
              Text(
                'Onaylanana kadar kayda geçmez',
                style: theme.textTheme.bodySmall?.copyWith(
                  color: colors.onExpenseContainer,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
