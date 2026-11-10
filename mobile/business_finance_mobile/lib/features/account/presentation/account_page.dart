import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_avatar.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../auth/data/auth_models.dart';
import '../../auth/data/auth_repository.dart';
import '../../auth/presentation/auth_controller.dart';
import '../../auth/presentation/auth_validation.dart';
import '../../profile/data/profile_repository.dart';
import '../data/account_models.dart';
import '../data/account_repository.dart';
import 'account_controller.dart';
import 'account_status_controller.dart';

/// `Hesabım`.
///
/// Hesaba dair dağınık parçalar — e-posta, işletme cevabı, parola, açık
/// oturumlar, hesabı kapatma ve çıkış — tek sayfada toplanır.
///
/// Tasarım teslimi (27 Eylül 2026, HesabimV5) 9 Kasım 2026'da kullanıcıyla
/// gözden geçirildi: kimlik kart değil ortalanmış bir başlıktır (ad, e-posta,
/// doğrulama işareti); satırlar kendini anlatan kısa adlar taşır, açıklama
/// yalnız gerekli yerdedir; `Çıkış yap` en alttadır (kullanıcı onu orada
/// arıyor), `Hesabı kapat` onun üstündedir.
class AccountPage extends StatefulWidget {
  const AccountPage({
    required this.repository,
    required this.authRepository,
    super.key,
    this.now,
  });

  final AccountRepositoryContract repository;
  final AuthSessionRepository authRepository;

  /// Göreli zamanın (`7 gün önce açıldı`) saati; testte sabitlenir.
  final DateTime Function()? now;

  @override
  State<AccountPage> createState() => _AccountPageState();
}

class _AccountPageState extends State<AccountPage> {
  late final AccountController controller;

  @override
  void initState() {
    super.initState();
    controller = AccountController(widget.repository, widget.authRepository)
      ..addListener(_changed);
    controller.load();
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

  DateTime get _now => (widget.now ?? DateTime.now)();

  @override
  Widget build(BuildContext context) {
    final canPop = Navigator.of(context).canPop();
    return Scaffold(
      body: SafeArea(
        bottom: false,
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppPageHeader(
              title: 'Hesabım',
              onBack: canPop ? () => Navigator.of(context).maybePop() : null,
            ),
            if (controller.isSubmitting) const LinearProgressIndicator(),
            Expanded(child: _body()),
          ],
        ),
      ),
    );
  }

  Widget _body() {
    if (controller.unauthorized) return const AppUnauthorizedView();
    if (controller.isLoading && controller.account == null) {
      return const AppLoadingView(message: 'Hesap bilgileri yükleniyor');
    }
    final account = controller.account;
    if (account == null) {
      return AppErrorView(
        message: controller.errorMessage ?? 'Hesap bilgileri alınamadı.',
        onRetry: controller.load,
      );
    }
    final sessions = controller.sessions;
    final others = sessions
        .where((session) => session.sessionId != controller.currentSessionId)
        .length;

    return ListView(
      padding: AppPageInsets.list,
      children: [
        _ProfileHeader(account: account, onVerify: _openVerificationSheet),
        const _BusinessAnswerSection(),
        const SizedBox(height: AppGaps.section),
        const AppSectionHeader(title: 'Güvenlik'),
        AppCard(
          padding: EdgeInsets.zero,
          child: account.hasPassword
              ? AppRow(
                  padding: _rowPadding,
                  leading: const AppIconCapsule(icon: Icons.password_outlined),
                  title: 'Parolamı değiştir',
                  trailing: const _Chevron(),
                  onTap: _openPasswordSheet,
                )
              // Google ile açılan hesabın parolası yoktur (9 Kasım 2026).
              : AppRow(
                  padding: _rowPadding,
                  leading: const AppIconCapsule(icon: Icons.password_outlined),
                  title: 'Parola belirle',
                  subtitle: 'Şu an Google ile giriş yapıyorsunuz.',
                  trailing: const _Chevron(),
                  onTap: _openSetPasswordSheet,
                ),
        ),
        const SizedBox(height: AppGaps.section),
        AppSectionHeader(
          title: 'Açık oturumlar · ${sessions.length}',
          // Düğme varken satır 48 dp'dir ve boşluğu o verir.
          padding: others == 0 ? null : EdgeInsets.zero,
          trailing: others == 0
              ? null
              // Toplu eylem gri hap: satırdaki tekil `×`'ten ayrı okunur.
              : TextButton.icon(
                  style: TextButton.styleFrom(
                    backgroundColor: AppSurfaces.of(context).cardMuted,
                    foregroundColor: AppSurfaces.of(context).ink,
                    shape: const StadiumBorder(),
                    padding: const EdgeInsets.symmetric(
                      horizontal: AppSpacing.compact,
                    ),
                  ),
                  onPressed: controller.isSubmitting
                      ? null
                      : () => _confirmRevokeOthers(others),
                  icon: const Icon(Icons.logout, size: 18),
                  label: const Text('Diğerlerini kapat'),
                ),
        ),
        _SessionsCard(
          sessions: sessions,
          currentSessionId: controller.currentSessionId,
          now: _now,
          onRevoke: _confirmRevoke,
        ),
        const SizedBox(height: AppGaps.section),
        const AppSectionHeader(title: 'Hesabı kapat'),
        _DangerCard(
          onBackup: () => GoRouter.maybeOf(context)?.push('/more/data-tools'),
          onDelete: _openDeleteSheet,
        ),
        const SizedBox(height: AppSpacing.xLarge),
        // En altta: kullanıcı çıkışı orada arıyor (9 Kasım 2026).
        OutlinedButton.icon(
          onPressed: _confirmLogout,
          icon: const Icon(Icons.logout),
          label: const Text('Çıkış yap'),
        ),
      ],
    );
  }

  Future<void> _confirmRevoke(UserSessionSummary session) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.no_accounts_outlined,
      title: 'Oturum kapatılsın mı?',
      message:
          'O cihaz bir sonraki denemesinde yeniden giriş yapmak zorunda kalır.',
      highlight: _openedText(session.createdAtUtc, _now),
      confirmLabel: 'Oturumu kapat',
      destructive: true,
    );
    if (!confirmed || !mounted) return;
    final messenger = ScaffoldMessenger.of(context);
    final done = await controller.revokeSession(session.sessionId);
    messenger.showSnackBar(
      SnackBar(
        content: Text(
          done
              ? 'Oturum kapatıldı.'
              : controller.errorMessage ?? 'Oturum kapatılamadı.',
        ),
      ),
    );
  }

  Future<void> _confirmRevokeOthers(int count) async {
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.no_accounts_outlined,
      title: 'Diğer oturumlar kapatılsın mı?',
      message:
          '$count cihazdaki oturum kapanır; o cihazlarda yeniden giriş '
          'gerekir. Bu cihazdaki oturumunuz açık kalır.',
      confirmLabel: 'Hepsini kapat',
      destructive: true,
    );
    if (!confirmed || !mounted) return;
    final messenger = ScaffoldMessenger.of(context);
    final done = await controller.revokeOtherSessions();
    messenger.showSnackBar(
      SnackBar(
        content: Text(
          done
              ? 'Diğer oturumlar kapatıldı.'
              : controller.errorMessage ?? 'Bazı oturumlar kapatılamadı.',
        ),
      ),
    );
  }

  Future<void> _confirmLogout() async {
    final shouldLogout = await AppConfirmDialog.show(
      context: context,
      icon: Icons.logout,
      title: 'Çıkış yapılsın mı?',
      message:
          'Bu cihazdaki oturum bilgileri güvenli biçimde silinir; kayıtlarınız '
          'sunucuda kalır.',
      confirmLabel: 'Çıkış yap',
    );
    if (shouldLogout && mounted) {
      await context.read<AuthController?>()?.logout();
    }
  }

  Future<void> _openVerificationSheet() async {
    final confirmed = await AppFormSheet.show<bool>(
      context: context,
      builder: (_) => _VerifyEmailSheet(controller: controller),
    );
    if (confirmed == true && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('E-posta adresiniz doğrulandı.')),
      );
      // Özet ekranındaki nokta da düşmeli: uyarı tek bir gerçeği anlatıyor.
      await context.read<AccountStatusController?>()?.refresh();
    }
  }

  Future<void> _openPasswordSheet() async {
    final changed = await AppFormSheet.show<bool>(
      context: context,
      builder: (_) => _ChangePasswordSheet(controller: controller),
    );
    if (changed == true && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Parolanız değişti. Diğer cihazlardaki oturumlar kapatıldı.',
          ),
        ),
      );
      await controller.load();
    }
  }

  Future<void> _openSetPasswordSheet() async {
    final set = await AppFormSheet.show<bool>(
      context: context,
      builder: (_) => _SetPasswordSheet(controller: controller),
    );
    if (set == true && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text(
            'Parolanız belirlendi. Diğer cihazlardaki oturumlar kapatıldı.',
          ),
        ),
      );
      await controller.load();
    }
  }

  Future<void> _openDeleteSheet() async {
    final hasPassword = controller.account?.hasPassword ?? true;
    // İlk kapı: ne olacağını söyleyen açık onay. Yedek adımı artık aynı
    // kartın bir üst satırında duruyor.
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.delete_forever_outlined,
      title: 'Hesabınız silinsin mi?',
      message: hasPassword
          ? 'Hesabınız ve bütün kayıtlarınız kalıcı olarak silinir; geri '
                'getirilemez. Sonraki adımda parolanız sorulur.'
          : 'Hesabınız ve bütün kayıtlarınız kalıcı olarak silinir; geri '
                'getirilemez. Sonraki adımda Google hesabınızı seçerek '
                'onaylarsınız.',
      highlight: controller.account?.email,
      confirmLabel: 'Devam et',
      destructive: true,
    );
    if (!confirmed || !mounted) return;

    if (!hasPassword) {
      await _deleteWithGoogle();
      return;
    }

    // İkinci kapı: parolanın yeniden yazılması.
    final deleted = await AppFormSheet.show<bool>(
      context: context,
      builder: (_) => _DeleteAccountSheet(controller: controller),
    );
    if (deleted == true && mounted) {
      // Hesap sunucuda yok artık; elimizdeki token da ölü. Çıkış, kullanıcıyı
      // giriş ekranına götüren ve cihazdaki oturum bilgisini silen adımdır.
      await context.read<AuthController?>()?.logout();
    }
  }
}

extension on _AccountPageState {
  /// Parolası olmayan hesapta ikinci kapı Google'a yeniden giriştir; sunucu
  /// belirtecin bu hesaba bağlı Google hesabına ait olduğunu denetler.
  /// Kişi Google penceresini kapatırsa hiçbir şey olmaz.
  Future<void> _deleteWithGoogle() async {
    final auth = context.read<AuthController?>();
    final messenger = ScaffoldMessenger.of(context);
    final String? token;
    try {
      token = await auth?.obtainGoogleIdToken();
    } on GoogleSignInUnavailable catch (error) {
      messenger.showSnackBar(SnackBar(content: Text(error.message)));
      return;
    }
    if (token == null || !mounted) return;
    final deleted = await controller.deleteAccount(googleIdToken: token);
    if (!mounted) return;
    if (!deleted) {
      messenger.showSnackBar(
        SnackBar(content: Text(controller.errorMessage ?? 'Hesap silinemedi.')),
      );
      return;
    }
    await auth?.logout();
  }
}

const _rowPadding = EdgeInsets.fromLTRB(
  AppSpacing.medium,
  AppSpacing.compact,
  AppSpacing.compact,
  AppSpacing.compact,
);

class _Chevron extends StatelessWidget {
  const _Chevron();

  @override
  Widget build(BuildContext context) => Icon(
    Icons.chevron_right,
    size: 22,
    color: AppSurfaces.of(context).inkMuted,
  );
}

/// Kim olduğunuz: büyük avatar, ad ve e-posta, ortalanmış ve kartsız
/// (büyük uygulamaların hesap sayfası başı). Ad kullanıcı adıdır; yoksa
/// e-posta tek satır olarak başlığa çıkar. Doğrulanmış adresin yanında küçük
/// bir işaret durur; doğrulanmamış adresin uyarısı başlığın altındadır.
///
/// Hesap **kilitli değildir**: uyarı bir engel değil, bir hatırlatmadır. Metin
/// bunu açıkça söyler, yoksa kullanıcı uygulamanın yarısının kapalı olduğunu
/// sanır.
class _ProfileHeader extends StatelessWidget {
  const _ProfileHeader({required this.account, required this.onVerify});

  final UserAccount account;
  final Future<void> Function() onVerify;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final username = account.username;
    final email = Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        Flexible(
          child: Text(
            account.email,
            maxLines: 1,
            overflow: TextOverflow.ellipsis,
            style: username == null
                ? theme.textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.w600,
                  )
                : theme.textTheme.bodyMedium?.copyWith(
                    color: surfaces.inkMuted,
                  ),
          ),
        ),
        if (account.emailConfirmed) ...[
          const SizedBox(width: AppSpacing.xSmall),
          Tooltip(
            message: 'E-posta doğrulandı',
            child: Icon(
              Icons.verified,
              size: 16,
              semanticLabel: 'E-posta doğrulandı',
              color: AppFinanceColors.of(context).income,
            ),
          ),
        ],
      ],
    );
    return Padding(
      padding: const EdgeInsets.only(top: AppSpacing.small),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Center(
            child: AppAvatar(
              initials: accountInitials(account.email),
              size: 72,
            ),
          ),
          const SizedBox(height: AppSpacing.medium),
          if (username != null) ...[
            Text(
              username,
              textAlign: TextAlign.center,
              maxLines: 1,
              overflow: TextOverflow.ellipsis,
              style: theme.textTheme.titleLarge?.copyWith(
                fontWeight: FontWeight.w600,
              ),
            ),
            const SizedBox(height: AppSpacing.xxSmall),
          ],
          Center(child: email),
          if (!account.emailConfirmed) ...[
            const SizedBox(height: AppSpacing.medium),
            AppInlineNotice(
              icon: Icons.mark_email_unread_outlined,
              message:
                  'E-posta adresiniz doğrulanmadı. Uygulamayı kullanmaya '
                  'devam edebilirsiniz; doğrulama, parolanızı unutursanız '
                  'hesabınıza geri dönmenizi sağlar.',
              actionLabel: 'Adresimi doğrula',
              onAction: onVerify,
              margin: EdgeInsets.zero,
            ),
          ],
        ],
      ),
    );
  }
}

/// Kaydolurken sorulan sorunun sonradan değiştirilebildiği yer.
///
/// Cevap **hiçbir özelliği kapatmaz**: yalnız işletme/şahsi ayrımının
/// arayüzde görünüp görünmeyeceğini belirler. Kategorilere dokunmaz — o
/// noktada liste artık kullanıcınındır ve sildiği bir kategoriyi geri
/// getirmek silme eylemini anlamsız kılardı.
class _BusinessAnswerSection extends StatefulWidget {
  const _BusinessAnswerSection();

  @override
  State<_BusinessAnswerSection> createState() => _BusinessAnswerSectionState();
}

class _BusinessAnswerSectionState extends State<_BusinessAnswerSection> {
  bool _isSaving = false;

  @override
  Widget build(BuildContext context) {
    final scopeController = context.watch<ScopeController?>();
    final repository = context.read<ProfileRepositoryContract?>();
    // Bağlanmamış kabukta (test ya da bağımlılıksız kurulum) bölüm hiç
    // çizilmez: değiştirilemeyen bir anahtar göstermek, kullanıcıya
    // çalışmayan bir düğme vermek olurdu.
    if (scopeController == null || repository == null) {
      return const SizedBox.shrink();
    }
    final on = scopeController.isVisible;
    final onChanged = _isSaving
        ? null
        : (bool value) => _save(repository, scopeController, value);
    return Padding(
      padding: const EdgeInsets.only(top: AppSpacing.large),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          const AppSectionHeader(title: 'Tercihler'),
          AppCard(
            padding: EdgeInsets.zero,
            child: MergeSemantics(
              child: InkWell(
                onTap: onChanged == null ? null : () => onChanged(!on),
                child: Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.medium,
                    vertical: AppSpacing.compact,
                  ),
                  child: Row(
                    children: [
                      const AppIconCapsule(icon: Icons.storefront_outlined),
                      const SizedBox(width: AppSpacing.medium),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              'İşletmem var',
                              style: Theme.of(context).textTheme.titleSmall,
                            ),
                            const SizedBox(height: AppGaps.rowText),
                            // Anahtarın ne yaptığını söyler, durumunu değil:
                            // durumu anahtarın kendisi gösteriyor.
                            Text(
                              'İşletme ve şahsi ayrımını gösterir.',
                              style: Theme.of(context).textTheme.bodySmall,
                            ),
                          ],
                        ),
                      ),
                      const SizedBox(width: AppSpacing.small),
                      Switch(value: on, onChanged: onChanged),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  Future<void> _save(
    ProfileRepositoryContract repository,
    ScopeController scopeController,
    bool value,
  ) async {
    setState(() => _isSaving = true);
    final messenger = ScaffoldMessenger.of(context);
    try {
      final profile = await repository.update(hasBusiness: value);
      // Sunucunun döndürdüğü hâl uygulanır; istemcinin gönderdiği değil.
      await scopeController.applyHasBusiness(
        profile.hasBusiness,
        hasCounterpartyLedger: profile.hasCounterpartyLedger,
      );
    } on ApiException catch (error) {
      messenger.showSnackBar(SnackBar(content: Text(error.message)));
    } on FormatException {
      messenger.showSnackBar(
        const SnackBar(
          content: Text('Sunucudan beklenmeyen bir yanıt alındı.'),
        ),
      );
    } finally {
      if (mounted) setState(() => _isSaving = false);
    }
  }
}

class _VerifyEmailSheet extends StatefulWidget {
  const _VerifyEmailSheet({required this.controller});

  final AccountController controller;

  @override
  State<_VerifyEmailSheet> createState() => _VerifyEmailSheetState();
}

class _VerifyEmailSheetState extends State<_VerifyEmailSheet> {
  final _code = TextEditingController();
  String? _message;
  bool _codeRequested = false;

  @override
  void initState() {
    super.initState();
    // Panel açılır açılmaz kod istenir: kullanıcı zaten bunun için geldi.
    WidgetsBinding.instance.addPostFrameCallback((_) => _requestCode());
  }

  @override
  void dispose() {
    _code.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AppFormSheet<bool>(
      title: 'Adresimi doğrula',
      description: _codeRequested
          ? 'E-postanıza gönderilen altı haneli kodu yazın. Kod 15 dakika '
                'geçerlidir.'
          : 'Kod gönderiliyor…',
      submitLabel: 'Doğrula',
      onSubmit: _submit,
      secondaryLabel: 'Kodu yeniden gönder',
      onSecondary: _resend,
      children: [
        TextField(
          controller: _code,
          keyboardType: TextInputType.number,
          maxLength: 6,
          decoration: const InputDecoration(
            labelText: 'Doğrulama kodu',
            counterText: '',
          ),
        ),
        if (_message != null) ...[
          const SizedBox(height: AppSpacing.small),
          Text(
            _message!,
            style: TextStyle(color: Theme.of(context).colorScheme.error),
          ),
        ],
      ],
    );
  }

  Future<void> _requestCode() async {
    final result = await widget.controller.sendVerificationCode();
    if (!mounted) return;
    setState(() {
      _codeRequested = result != null;
      // Sunucuda gönderici yapılandırılmamışsa bunu söylemek gerekir; yoksa
      // kullanıcı hiç gelmeyecek bir postayı bekler.
      _message = result == null
          ? widget.controller.errorMessage
          : result.codeSent
          ? null
          : 'Kod gönderilemedi: e-posta servisi yapılandırılmamış.';
    });
  }

  Future<bool?> _resend() async {
    await _requestCode();
    return null;
  }

  Future<bool?> _submit() async {
    final code = _code.text.trim();
    if (code.length != 6) {
      setState(() => _message = 'Altı haneli kodu yazın.');
      return null;
    }
    final confirmed = await widget.controller.confirmEmail(code);
    if (!confirmed) {
      setState(
        () => _message = widget.controller.errorMessage ?? 'Kod doğrulanamadı.',
      );
      return null;
    }
    return true;
  }
}

class _SessionsCard extends StatelessWidget {
  const _SessionsCard({
    required this.sessions,
    required this.currentSessionId,
    required this.now,
    required this.onRevoke,
  });

  final List<UserSessionSummary> sessions;
  final String? currentSessionId;
  final DateTime now;
  final Future<void> Function(UserSessionSummary session) onRevoke;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    if (sessions.isEmpty) {
      return AppCard(
        child: Text(
          'Açık oturum görünmüyor.',
          style: theme.textTheme.bodyMedium,
        ),
      );
    }
    // Bu cihaz en üstte: kullanıcı önce kendini bulur.
    final ordered = [
      ...sessions.where((session) => session.sessionId == currentSessionId),
      ...sessions.where((session) => session.sessionId != currentSessionId),
    ];
    return AppCard(
      padding: EdgeInsets.zero,
      child: AppDividedColumn(
        inset: AppIconCapsule.rowInset,
        children: [
          for (final session in ordered)
            _sessionRow(session, session.sessionId == currentSessionId),
        ],
      ),
    );
  }

  Widget _sessionRow(UserSessionSummary session, bool current) {
    final subtitle =
        '${current ? 'Şu an açık' : _openedText(session.createdAtUtc, now)}'
        ' · ${_validUntil(session.expiresAtUtc, now)}';
    return AppRow(
      padding: current
          ? _rowPadding
          : const EdgeInsets.fromLTRB(
              AppSpacing.medium,
              AppSpacing.compact,
              AppSpacing.xSmall,
              AppSpacing.compact,
            ),
      leading: AppIconCapsule(
        icon: current ? Icons.smartphone_outlined : Icons.devices_outlined,
        tone: current ? AppStatusTone.neutral : null,
      ),
      title: current ? 'Bu cihaz' : 'Başka bir cihaz',
      subtitle: subtitle,
      // Kendi oturumunu buradan kapatmak "çıkış yap"tır ve onun kendi
      // düğmesi var; iki yerde iki farklı isimle aynı şeyi sunmak
      // kullanıcıyı yanıltırdı.
      trailing: current
          ? null
          : IconButton(
              icon: const Icon(Icons.close),
              tooltip: 'Oturumu kapat',
              onPressed: () => onRevoke(session),
            ),
    );
  }
}

/// Geri dönüşü olmayan eylemin kartı: önce yedek adımı, sonra silme.
class _DangerCard extends StatelessWidget {
  const _DangerCard({required this.onBackup, required this.onDelete});

  final VoidCallback onBackup;
  final Future<void> Function() onDelete;

  @override
  Widget build(BuildContext context) {
    final colors = AppFinanceColors.of(context);
    return AppCard(
      padding: EdgeInsets.zero,
      child: AppDividedColumn(
        inset: AppIconCapsule.rowInset,
        children: [
          AppRow(
            padding: _rowPadding,
            leading: const AppIconCapsule(icon: Icons.folder_outlined),
            title: 'Önce yedeğinizi alın',
            trailing: const _Chevron(),
            onTap: onBackup,
          ),
          AppRow(
            padding: _rowPadding,
            leading: const AppIconCapsule(
              icon: Icons.delete_forever_outlined,
              tone: AppStatusTone.expense,
            ),
            title: 'Hesabımı sil',
            titleStyle: Theme.of(
              context,
            ).textTheme.titleSmall?.copyWith(color: colors.expense),
            onTap: onDelete,
          ),
        ],
      ),
    );
  }
}

/// Parola alanı: göz düğmesiyle yazılanı gösterir ya da gizler.
class _PasswordField extends StatefulWidget {
  const _PasswordField({
    required this.controller,
    required this.label,
    required this.autofillHints,
    this.onChanged,
  });

  final TextEditingController controller;
  final String label;
  final List<String> autofillHints;
  final ValueChanged<String>? onChanged;

  @override
  State<_PasswordField> createState() => _PasswordFieldState();
}

class _PasswordFieldState extends State<_PasswordField> {
  bool _obscured = true;

  @override
  Widget build(BuildContext context) {
    return TextField(
      controller: widget.controller,
      obscureText: _obscured,
      autofillHints: widget.autofillHints,
      onChanged: widget.onChanged,
      decoration: InputDecoration(
        labelText: widget.label,
        suffixIcon: IconButton(
          tooltip: _obscured ? 'Parolayı göster' : 'Parolayı gizle',
          onPressed: () => setState(() => _obscured = !_obscured),
          icon: Icon(
            _obscured
                ? Icons.visibility_outlined
                : Icons.visibility_off_outlined,
          ),
        ),
      ),
    );
  }
}

/// Yeni parolanın kuralları, yazdıkça işaretlenir. Kural sunucununkiyle
/// aynıdır ([AuthValidation.registrationPassword]); tek satırlık "en az 12
/// karakter; büyük, küçük…" yardımı yerine hangi kuralın eksik kaldığını
/// gösterir.
class _PasswordRules extends StatelessWidget {
  const _PasswordRules({required this.password});

  final String password;

  @override
  Widget build(BuildContext context) {
    final rules = [
      ('En az 12 karakter', password.length >= 12),
      (
        'Büyük ve küçük harf',
        password.contains(RegExp('[a-z]')) &&
            password.contains(RegExp('[A-Z]')),
      ),
      ('Rakam', password.contains(RegExp('[0-9]'))),
      ('Simge (!, ?, # gibi)', password.contains(RegExp(r'[^a-zA-Z0-9]'))),
    ];
    final theme = Theme.of(context);
    final met = AppFinanceColors.of(context).income;
    final muted = AppSurfaces.of(context).inkMuted;
    return Padding(
      padding: const EdgeInsets.only(
        top: AppSpacing.small,
        left: AppSpacing.xSmall,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        spacing: AppSpacing.xSmall,
        children: [
          for (final (label, ok) in rules)
            Row(
              children: [
                Icon(
                  ok ? Icons.check_circle : Icons.radio_button_unchecked,
                  size: 16,
                  color: ok ? met : muted,
                ),
                const SizedBox(width: AppSpacing.small),
                Text(
                  label,
                  style: theme.textTheme.bodySmall?.copyWith(
                    color: ok ? theme.colorScheme.onSurface : muted,
                  ),
                ),
              ],
            ),
        ],
      ),
    );
  }
}

/// Parola değişince öbür cihazlarda ne olacağını söyleyen tek satır.
class _SessionsNote extends StatelessWidget {
  const _SessionsNote();

  @override
  Widget build(BuildContext context) {
    final muted = AppSurfaces.of(context).inkMuted;
    return Padding(
      padding: const EdgeInsets.only(top: AppSpacing.medium),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(Icons.info_outline, size: 16, color: muted),
          const SizedBox(width: AppSpacing.small),
          Expanded(
            child: Text(
              'Diğer cihazlardaki oturumlarınız kapanır.',
              style: Theme.of(
                context,
              ).textTheme.bodySmall?.copyWith(color: muted),
            ),
          ),
        ],
      ),
    );
  }
}

class _ErrorText extends StatelessWidget {
  const _ErrorText(this.message);

  final String message;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.only(top: AppSpacing.small),
    child: Text(
      message,
      style: TextStyle(color: Theme.of(context).colorScheme.error),
    ),
  );
}

class _ChangePasswordSheet extends StatefulWidget {
  const _ChangePasswordSheet({required this.controller});

  final AccountController controller;

  @override
  State<_ChangePasswordSheet> createState() => _ChangePasswordSheetState();
}

class _ChangePasswordSheetState extends State<_ChangePasswordSheet> {
  final _currentPassword = TextEditingController();
  final _newPassword = TextEditingController();
  String? _validationMessage;

  @override
  void dispose() {
    _currentPassword.dispose();
    _newPassword.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AppFormSheet<bool>(
      title: 'Parolamı değiştir',
      submitLabel: 'Değiştir',
      onSubmit: _submit,
      children: [
        _PasswordField(
          controller: _currentPassword,
          label: 'Mevcut parola',
          autofillHints: const [AutofillHints.password],
        ),
        const SizedBox(height: AppGaps.field),
        _PasswordField(
          controller: _newPassword,
          label: 'Yeni parola',
          autofillHints: const [AutofillHints.newPassword],
          onChanged: (_) => setState(() {}),
        ),
        _PasswordRules(password: _newPassword.text),
        const _SessionsNote(),
        if (_validationMessage != null) _ErrorText(_validationMessage!),
      ],
    );
  }

  Future<bool?> _submit() async {
    if (_currentPassword.text.isEmpty) {
      setState(() => _validationMessage = 'Mevcut parolanızı yazın.');
      return null;
    }
    final policyMessage = AuthValidation.registrationPassword(
      _newPassword.text,
    );
    if (policyMessage != null) {
      setState(
        () => _validationMessage = 'Yeni parola kuralların hepsini sağlamalı.',
      );
      return null;
    }
    final done = await widget.controller.changePassword(
      currentPassword: _currentPassword.text,
      newPassword: _newPassword.text,
    );
    if (!done) {
      setState(
        () => _validationMessage =
            widget.controller.errorMessage ?? 'Parola değiştirilemedi.',
      );
      return null;
    }
    return true;
  }
}

/// Google ile açılmış hesaba parola koyar. Kod e-postaya, pencere açılınca
/// gider: oturum açık diye kod sormadan parola koydurmak, telefonu ele geçiren
/// birinin hesabı da ele geçirmesine izin verirdi.
class _SetPasswordSheet extends StatefulWidget {
  const _SetPasswordSheet({required this.controller});

  final AccountController controller;

  @override
  State<_SetPasswordSheet> createState() => _SetPasswordSheetState();
}

class _SetPasswordSheetState extends State<_SetPasswordSheet> {
  final _code = TextEditingController();
  final _newPassword = TextEditingController();
  String? _validationMessage;
  bool _codeSent = false;
  bool _sending = true;

  @override
  void initState() {
    super.initState();
    // İstek sayfayı yeniden çizdirir; pencere çizilirken başlatılamaz.
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) _sendCode();
    });
  }

  @override
  void dispose() {
    _code.dispose();
    _newPassword.dispose();
    super.dispose();
  }

  Future<void> _sendCode() async {
    setState(() => _sending = true);
    final sent = await widget.controller.requestPasswordCode();
    if (!mounted) return;
    setState(() {
      _sending = false;
      _codeSent = sent;
      _validationMessage = sent
          ? null
          : widget.controller.errorMessage ?? 'Kod gönderilemedi.';
    });
  }

  @override
  Widget build(BuildContext context) {
    final email = widget.controller.account?.email ?? '';
    return AppFormSheet<bool>(
      title: 'Parola belirle',
      description: _sending
          ? 'Kod gönderiliyor…'
          : _codeSent
          ? '$email adresine gönderdiğimiz 6 haneli kodu yazın.'
          : 'Parola, $email adresine gönderilen 6 haneli kodla belirlenir.',
      submitLabel: 'Parolayı belirle',
      onSubmit: _submit,
      children: [
        TextField(
          controller: _code,
          keyboardType: TextInputType.number,
          autofillHints: const [AutofillHints.oneTimeCode],
          maxLength: 6,
          decoration: const InputDecoration(labelText: 'Kod', counterText: ''),
        ),
        Align(
          alignment: Alignment.centerRight,
          child: TextButton(
            onPressed: _sending ? null : _sendCode,
            child: const Text('Kodu yeniden gönder'),
          ),
        ),
        _PasswordField(
          controller: _newPassword,
          label: 'Yeni parola',
          autofillHints: const [AutofillHints.newPassword],
          onChanged: (_) => setState(() {}),
        ),
        _PasswordRules(password: _newPassword.text),
        const _SessionsNote(),
        if (_validationMessage != null) _ErrorText(_validationMessage!),
      ],
    );
  }

  Future<bool?> _submit() async {
    if (_code.text.trim().length != 6) {
      setState(
        () => _validationMessage = 'E-postanıza gelen 6 haneli kodu yazın.',
      );
      return null;
    }
    final policyMessage = AuthValidation.registrationPassword(
      _newPassword.text,
    );
    if (policyMessage != null) {
      setState(
        () => _validationMessage = 'Yeni parola kuralların hepsini sağlamalı.',
      );
      return null;
    }
    final done = await widget.controller.changePassword(
      newPassword: _newPassword.text,
      resetCode: _code.text.trim(),
    );
    if (!done) {
      setState(
        () => _validationMessage =
            widget.controller.errorMessage ?? 'Parola belirlenemedi.',
      );
      return null;
    }
    return true;
  }
}

class _DeleteAccountSheet extends StatefulWidget {
  const _DeleteAccountSheet({required this.controller});

  final AccountController controller;

  @override
  State<_DeleteAccountSheet> createState() => _DeleteAccountSheetState();
}

class _DeleteAccountSheetState extends State<_DeleteAccountSheet> {
  final _password = TextEditingController();
  String? _validationMessage;

  @override
  void dispose() {
    _password.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AppFormSheet<bool>(
      title: 'Hesabımı sil',
      description:
          'Silmeyi tamamlamak için parolanızı yazın. Bu adımdan sonra '
          'kayıtlarınız geri getirilemez.',
      submitLabel: 'Hesabımı sil',
      onSubmit: _submit,
      children: [
        TextField(
          controller: _password,
          obscureText: true,
          autofillHints: const [AutofillHints.password],
          decoration: const InputDecoration(labelText: 'Parolanız'),
        ),
        if (_validationMessage != null) ...[
          const SizedBox(height: AppSpacing.small),
          Text(
            _validationMessage!,
            style: TextStyle(color: Theme.of(context).colorScheme.error),
          ),
        ],
      ],
    );
  }

  Future<bool?> _submit() async {
    if (_password.text.isEmpty) {
      setState(() => _validationMessage = 'Parolanızı yazın.');
      return null;
    }
    final deleted = await widget.controller.deleteAccount(
      password: _password.text,
    );
    if (!deleted) {
      setState(
        () => _validationMessage =
            widget.controller.errorMessage ?? 'Hesap silinemedi.',
      );
      return null;
    }
    return true;
  }
}

/// Oturumun ne zaman açıldığı, göreli: `Bugün açıldı`, `7 gün önce açıldı`.
String _openedText(DateTime createdUtc, DateTime now) {
  final created = createdUtc.toLocal();
  final days = DateTime(
    now.year,
    now.month,
    now.day,
  ).difference(DateTime(created.year, created.month, created.day)).inDays;
  return switch (days) {
    <= 0 => 'Bugün açıldı',
    1 => 'Dün açıldı',
    _ => '$days gün önce açıldı',
  };
}

/// `18 Ekim'e kadar geçerli`. Türkçe yönelme eki ay adının son ünlüsüne
/// uyar; yıl değişiyorsa ek yerine yıl yazılır (`Geçerlilik: 3 Ocak 2027`).
String _validUntil(DateTime expiresUtc, DateTime now) {
  final local = expiresUtc.toLocal();
  final month = DateText.months[local.month - 1];
  if (local.year != now.year) {
    return 'Geçerlilik: ${local.day} $month ${local.year}';
  }
  const front = {'e', 'i', 'ö', 'ü'};
  final vowels = month
      .toLowerCase()
      .split('')
      .where((c) => 'aeıioöuü'.contains(c));
  final suffix = front.contains(vowels.last) ? 'e' : 'a';
  return "${local.day} $month'$suffix kadar geçerli";
}
