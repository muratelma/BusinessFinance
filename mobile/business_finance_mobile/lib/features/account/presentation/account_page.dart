import 'dart:async';
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:provider/provider.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_finance_colors.dart';
import '../../../core/theme/app_radius.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_avatar.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_code_input.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_divided_column.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_icon_capsule.dart';
import '../../../core/widgets/app_page_header.dart';
import '../../../core/widgets/app_row.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_state_views.dart';
import '../../../core/widgets/app_status_chip.dart';
import '../../../core/widgets/app_text_action.dart';
import '../../auth/data/auth_models.dart';
import '../../auth/data/auth_repository.dart';
import '../../auth/presentation/auth_controller.dart';
import '../../auth/presentation/auth_validation.dart';
import '../../auth/presentation/widgets/auth_form_parts.dart';
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
/// Tasarım teslimi `design/design_handoff_hesabim` (10 Kasım 2026): kimlik
/// kartı, Tercihler, Açık oturumlar, Güvenlik ve en altta `Çıkış yap`.
/// Görsel düzen teslimden, ölçüler ortak kuraldan gelir. `Hesabımı sil`
/// Güvenlik'in satırıdır ve kendi sayfasını açar.
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

    return ListView(
      padding: AppPageInsets.list,
      children: [
        _ProfileCard(account: account, onVerify: _openVerificationSheet),
        const _BusinessAnswerSection(),
        // Tek oturumda da bölüm durur ve yalnız `Bu cihaz · Şu an açık` yazar
        // (kullanıcı kararı, 10 Kasım 2026): kişi başka yerde açık oturumu
        // olmadığını görür; bölüm cihaz sayısına göre gelip gitmez.
        if (sessions.isNotEmpty) ...[
          const SizedBox(height: AppGaps.section),
          const AppSectionHeader(title: 'Açık oturumlar'),
          _SessionsCard(
            sessions: sessions,
            currentSessionId: controller.currentSessionId,
            now: _now,
            onRevoke: _revoke,
            onOpenAll: _openSessionsPage,
          ),
        ],
        const SizedBox(height: AppGaps.section),
        const AppSectionHeader(title: 'Güvenlik'),
        AppCard(
          padding: EdgeInsets.zero,
          child: AppDividedColumn(
            inset: AppIconCapsule.rowInset,
            children: [
              if (account.hasPassword)
                AppRow(
                  padding: _rowPadding,
                  leading: const AppIconCapsule(icon: Icons.password_outlined),
                  title: 'Parolamı değiştir',
                  trailing: const _Chevron(),
                  onTap: _openPasswordSheet,
                )
              else
                // Google ile açılan hesabın parolası yoktur (9 Kasım 2026).
                AppRow(
                  padding: _rowPadding,
                  leading: const AppIconCapsule(icon: Icons.password_outlined),
                  title: 'Parola belirle',
                  subtitle: 'Şu an Google ile giriş yapıyorsunuz.',
                  trailing: const _Chevron(),
                  onTap: _openSetPasswordSheet,
                ),
              AppRow(
                padding: _rowPadding,
                leading: const AppIconCapsule(
                  icon: Icons.delete_forever_outlined,
                  tone: AppStatusTone.expense,
                ),
                title: 'Hesabımı sil',
                titleStyle: Theme.of(context).textTheme.titleSmall?.copyWith(
                  color: AppFinanceColors.of(context).expense,
                ),
                trailing: const _Chevron(),
                onTap: _openDeletePage,
              ),
            ],
          ),
        ),
        const SizedBox(height: AppSpacing.xLarge),
        // En altta ve kırmızı değil: çıkış yapmak bir şey silmez.
        OutlinedButton.icon(
          onPressed: _confirmLogout,
          icon: const Icon(Icons.logout),
          label: const Text('Çıkış yap'),
        ),
      ],
    );
  }

  /// Oturum `×` ile doğrudan kapanır, onay sorulmaz: geri alınabilir bir
  /// iştir, o cihazda yeniden giriş yapılır.
  Future<void> _revoke(UserSessionSummary session) async {
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

  Future<void> _openSessionsPage() => Navigator.of(context).push<void>(
    MaterialPageRoute(
      builder: (_) => _SessionsPage(
        controller: controller,
        now: () => _now,
        onRevoke: _revoke,
        onRevokeOthers: _confirmRevokeOthers,
      ),
    ),
  );

  Future<void> _openDeletePage() => Navigator.of(context).push<void>(
    MaterialPageRoute(
      builder: (_) => _DeleteAccountPage(controller: controller),
    ),
  );

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
      // Kodla değiştirildiyse adres de doğrulandı: Özet ve Diğer'deki işaret
      // aynı anda düşsün.
      final status = context.read<AccountStatusController?>();
      await controller.load();
      await status?.refresh();
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

/// Kim olduğunuz: avatar, ad ve e-posta tek kartta (teslim
/// `design/design_handoff_hesabim`, 10 Kasım 2026). Ad kullanıcı adıdır;
/// yoksa e-posta başlığa çıkar.
///
/// Doğrulanmış adresin işareti avatarın köşesindeki yeşil tiktir. Doğrulanmamış
/// adres kartın ikinci satırıdır ve doğrulama panelini açar. Hesap **kilitli
/// değildir**: satır bir engel değil, bir hatırlatmadır.
class _ProfileCard extends StatelessWidget {
  const _ProfileCard({required this.account, required this.onVerify});

  final UserAccount account;
  final Future<void> Function() onVerify;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    final username = account.username;
    return AppCard(
      padding: EdgeInsets.zero,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Padding(
            padding: const EdgeInsets.all(AppSpacing.medium),
            child: Row(
              children: [
                Semantics(
                  label: account.emailConfirmed ? 'E-posta doğrulandı' : null,
                  child: AppAvatar(
                    initials: accountInitials(account.email),
                    size: 56,
                    status: account.emailConfirmed
                        ? AppAvatarStatus.verified
                        : null,
                  ),
                ),
                const SizedBox(width: AppSpacing.medium),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        username ?? account.email,
                        maxLines: 1,
                        overflow: TextOverflow.ellipsis,
                        style: theme.textTheme.titleMedium?.copyWith(
                          fontWeight: FontWeight.w700,
                        ),
                      ),
                      if (username != null) ...[
                        const SizedBox(height: AppGaps.rowText),
                        Text(
                          account.email,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: surfaces.inkMuted,
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
              ],
            ),
          ),
          if (!account.emailConfirmed) ...[
            Divider(
              height: 1,
              thickness: 1,
              indent: AppSpacing.medium,
              color: surfaces.border,
            ),
            AppRow(
              padding: _rowPadding,
              leading: const AppIconCapsule(
                icon: Icons.mark_email_unread_outlined,
                tone: AppStatusTone.neutral,
              ),
              title: 'E-postanızı doğrulayın',
              trailing: const _Chevron(),
              onTap: onVerify,
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
                              'İşletme ve şahsi ayrılır',
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

/// E-postaya giden kodun istenme sonucu. "Az önce gönderildi" bir hata
/// değildir: kod posta kutusunda duruyor, kullanıcı yalnız onu yazacak.
sealed class _CodeRequest {
  const _CodeRequest();
}

class _CodeSent extends _CodeRequest {
  const _CodeSent();
}

class _CodeSentEarlier extends _CodeRequest {
  const _CodeSentEarlier();
}

class _CodeFailed extends _CodeRequest {
  const _CodeFailed(this.message);

  final String message;
}

const _tooSoonCode = 'account.verification_code_too_soon';

/// E-postayla gelen altı haneli kodun girişi: kutular ve altında tek bir
/// durum satırı (gönderiliyor, sayaç, `Kodu yeniden gönder` ya da
/// gönderilemedi). E-posta doğrulama, `Parola belirle` ve `Parolamı unuttum`
/// aynı parçayı kullanır; kodun nasıl isteneceğini çağıran söyler.
///
/// Kod parça kurulunca bir kez istenir: kullanıcı zaten bunun için geldi.
class _EmailCodeEntry extends StatefulWidget {
  const _EmailCodeEntry({
    required this.controller,
    required this.request,
    this.error,
    this.onChanged,
    this.onCompleted,
  });

  final TextEditingController controller;
  final Future<_CodeRequest> Function() request;

  /// Kodun reddi; kutular kırmızıya döner, cümle altlarında yazar.
  final String? error;
  final ValueChanged<String>? onChanged;
  final ValueChanged<String>? onCompleted;

  /// Sunucunun yeniden gönderim aralığı (`VerificationPolicy.ResendInterval`).
  /// Sunucu kalan süreyi söylemez; sayaç kendi isteğimizden başlar.
  static const resendSeconds = 60;
  static const length = 6;

  @override
  State<_EmailCodeEntry> createState() => _EmailCodeEntryState();
}

enum _SendState { sending, sent, sentEarlier, failed }

class _EmailCodeEntryState extends State<_EmailCodeEntry> {
  _SendState _send = _SendState.sending;
  String? _sendError;
  int _secondsLeft = 0;
  Timer? _timer;

  @override
  void initState() {
    super.initState();
    // İstek sayfayı yeniden çizdirir; parça çizilirken başlatılamaz.
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted) _request();
    });
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  Future<void> _request() async {
    setState(() {
      _send = _SendState.sending;
      _sendError = null;
    });
    final result = await widget.request();
    if (!mounted) return;
    switch (result) {
      case _CodeSent():
        _startCountdown(_SendState.sent);
      case _CodeSentEarlier():
        _startCountdown(_SendState.sentEarlier);
      case _CodeFailed(:final message):
        setState(() {
          _send = _SendState.failed;
          _sendError = message;
        });
    }
  }

  void _startCountdown(_SendState state) {
    _timer?.cancel();
    setState(() {
      _send = state;
      _secondsLeft = _EmailCodeEntry.resendSeconds;
    });
    _timer = Timer.periodic(const Duration(seconds: 1), (timer) {
      if (!mounted) return;
      setState(() => _secondsLeft--);
      if (_secondsLeft <= 0) timer.cancel();
    });
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final error = widget.error;
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      mainAxisSize: MainAxisSize.min,
      children: [
        AppCodeInput(
          controller: widget.controller,
          length: _EmailCodeEntry.length,
          hasError: error != null,
          onChanged: widget.onChanged,
          onCompleted: widget.onCompleted,
        ),
        if (error != null) ...[
          const SizedBox(height: AppSpacing.small),
          Semantics(
            liveRegion: true,
            child: Text(
              error,
              style: theme.textTheme.bodySmall?.copyWith(
                color: theme.colorScheme.error,
              ),
            ),
          ),
        ],
        const SizedBox(height: AppSpacing.small),
        ConstrainedBox(
          constraints: const BoxConstraints(minHeight: AppSizes.button),
          child: Align(
            alignment: AlignmentDirectional.centerStart,
            child: _status(theme),
          ),
        ),
      ],
    );
  }

  Widget _status(ThemeData theme) {
    final muted = theme.textTheme.bodySmall?.copyWith(
      color: theme.colorScheme.onSurfaceVariant,
    );
    switch (_send) {
      case _SendState.sending:
        return Text('Kod gönderiliyor…', style: muted);
      case _SendState.failed:
        return Row(
          children: [
            Flexible(
              child: Text(
                _sendError ?? 'Kod gönderilemedi.',
                style: theme.textTheme.bodySmall?.copyWith(
                  color: theme.colorScheme.error,
                ),
              ),
            ),
            const SizedBox(width: AppSpacing.small),
            AppTextAction(
              label: 'Tekrar dene',
              onPressed: _request,
              underlined: true,
            ),
          ],
        );
      case _SendState.sent:
      case _SendState.sentEarlier:
        if (_secondsLeft > 0) {
          return Text(
            _send == _SendState.sentEarlier
                ? 'Kod az önce gönderildi.'
                : 'Yeni kodu ${_clock(_secondsLeft)} sonra isteyebilirsiniz.',
            style: muted,
          );
        }
        return AppTextAction(
          label: 'Kodu yeniden gönder',
          onPressed: _request,
          underlined: true,
        );
    }
  }

  static String _clock(int seconds) =>
      '${seconds ~/ 60}:${(seconds % 60).toString().padLeft(2, '0')}';
}

/// Parola kodunu ister ve sonucu ortak biçime çevirir.
Future<_CodeRequest> _requestPasswordCode(AccountController controller) async {
  final sent = await controller.requestPasswordCode();
  if (sent) return const _CodeSent();
  if (controller.errorCode == _tooSoonCode) return const _CodeSentEarlier();
  return _CodeFailed(controller.errorMessage ?? 'Kod gönderilemedi.');
}

/// Kodun reddini kutuların altındaki kısa cümleye çevirir.
String _codeErrorText(AccountController controller, String fallback) =>
    switch (controller.errorCode) {
      'authentication.invalid_reset_code' ||
      'account.invalid_verification_code' => 'Kod yanlış ya da süresi dolmuş.',
      _ => controller.errorMessage ?? fallback,
    };

class _VerifyEmailSheet extends StatefulWidget {
  const _VerifyEmailSheet({required this.controller});

  final AccountController controller;

  @override
  State<_VerifyEmailSheet> createState() => _VerifyEmailSheetState();
}

class _VerifyEmailSheetState extends State<_VerifyEmailSheet> {
  final _code = TextEditingController();
  String? _codeError;
  bool _verifying = false;

  @override
  void dispose() {
    _code.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final email = widget.controller.account?.email;
    return AppFormSheet<bool>(
      title: 'E-postanızı doğrulayın',
      description: email == null
          ? 'E-postanıza gelen 6 haneli kodu yazın.'
          : '$email adresine gelen 6 haneli kodu yazın.',
      submitLabel: 'Doğrula',
      showCancel: false,
      onSubmit: _code.text.length == _EmailCodeEntry.length && !_verifying
          ? _submit
          : null,
      children: [
        _EmailCodeEntry(
          controller: _code,
          request: _request,
          error: _codeError,
          onChanged: (_) => setState(() => _codeError = null),
          onCompleted: (_) => _autoVerify(),
        ),
      ],
    );
  }

  Future<_CodeRequest> _request() async {
    final result = await widget.controller.sendVerificationCode();
    if (!mounted) return const _CodeSent();
    if (result != null && result.alreadyConfirmed) {
      Navigator.of(context).pop(true);
      return const _CodeSent();
    }
    if (result != null && result.codeSent) return const _CodeSent();
    if (result == null && widget.controller.errorCode == _tooSoonCode) {
      return const _CodeSentEarlier();
    }
    // Sunucuda gönderici yapılandırılmamışsa bunu söylemek gerekir; yoksa
    // kullanıcı hiç gelmeyecek bir postayı bekler.
    return _CodeFailed(
      result == null
          ? widget.controller.errorMessage ?? 'Kod gönderilemedi.'
          : 'Kod gönderilemedi: e-posta servisi yapılandırılmamış.',
    );
  }

  /// Son rakam yazılınca kod kendiliğinden denenir.
  Future<void> _autoVerify() async {
    final confirmed = await _verify();
    if (confirmed && mounted) Navigator.of(context).pop(true);
  }

  Future<bool?> _submit() async => await _verify() ? true : null;

  Future<bool> _verify() async {
    if (_verifying || _code.text.length != _EmailCodeEntry.length) {
      return false;
    }
    setState(() => _verifying = true);
    final confirmed = await widget.controller.confirmEmail(_code.text);
    if (!mounted) return confirmed;
    setState(() {
      _verifying = false;
      if (!confirmed) {
        _codeError = _codeErrorText(widget.controller, 'Kod doğrulanamadı.');
      }
    });
    return confirmed;
  }
}

/// Hesabım'daki oturum kartı: bu cihaz ve diğerleri.
///
/// Diğer cihaz iki taneye kadar satır satır yazılır ve `×` ile kapatılır;
/// üç ve fazlası tek satıra toplanır (`Diğer cihazlar · n`) ve kendi
/// sayfasında açılır. Toplu kapatma yalnız o sayfadadır.
class _SessionsCard extends StatelessWidget {
  const _SessionsCard({
    required this.sessions,
    required this.currentSessionId,
    required this.now,
    required this.onRevoke,
    required this.onOpenAll,
  });

  final List<UserSessionSummary> sessions;
  final String? currentSessionId;
  final DateTime now;
  final Future<void> Function(UserSessionSummary session) onRevoke;
  final VoidCallback onOpenAll;

  /// Bu sayıya kadar diğer cihazlar Hesabım'da tek tek yazılır.
  static const inlineLimit = 2;

  @override
  Widget build(BuildContext context) {
    final others = _others(sessions, currentSessionId);
    return AppCard(
      padding: EdgeInsets.zero,
      child: AppDividedColumn(
        inset: AppIconCapsule.rowInset,
        children: [
          const _ThisDeviceRow(),
          if (others.length <= inlineLimit)
            for (final session in others)
              _OtherDeviceRow(session: session, now: now, onRevoke: onRevoke)
          else
            AppRow(
              padding: _rowPadding,
              leading: const AppIconCapsule(icon: Icons.devices_outlined),
              title: 'Diğer cihazlar · ${others.length}',
              subtitle:
                  'En son ${_lowerFirst(_openedText(others.first.createdAtUtc, now))}',
              trailing: const _Chevron(),
              onTap: onOpenAll,
            ),
        ],
      ),
    );
  }
}

/// Bu cihazın dışındaki oturumlar, en yeni açılan üstte.
List<UserSessionSummary> _others(
  List<UserSessionSummary> sessions,
  String? currentSessionId,
) =>
    sessions.where((session) => session.sessionId != currentSessionId).toList()
      ..sort((a, b) => b.createdAtUtc.compareTo(a.createdAtUtc));

String _lowerFirst(String text) =>
    text.isEmpty ? text : text[0].toLowerCase() + text.substring(1);

class _ThisDeviceRow extends StatelessWidget {
  const _ThisDeviceRow();

  // Kendi oturumunu buradan kapatmak "çıkış yap"tır ve onun kendi düğmesi
  // var; iki yerde iki farklı isimle aynı şeyi sunmak kullanıcıyı yanıltırdı.
  @override
  Widget build(BuildContext context) => const AppRow(
    padding: _rowPadding,
    leading: AppIconCapsule(
      icon: Icons.smartphone_outlined,
      tone: AppStatusTone.neutral,
    ),
    title: 'Bu cihaz',
    subtitle: 'Şu an açık',
  );
}

class _OtherDeviceRow extends StatelessWidget {
  const _OtherDeviceRow({
    required this.session,
    required this.now,
    required this.onRevoke,
  });

  final UserSessionSummary session;
  final DateTime now;
  final Future<void> Function(UserSessionSummary session) onRevoke;

  @override
  Widget build(BuildContext context) => AppRow(
    padding: const EdgeInsets.fromLTRB(
      AppSpacing.medium,
      AppSpacing.compact,
      AppSpacing.xSmall,
      AppSpacing.compact,
    ),
    leading: const AppIconCapsule(icon: Icons.devices_outlined),
    title: 'Başka bir cihaz',
    subtitle: _openedText(session.createdAtUtc, now),
    trailing: IconButton(
      icon: Icon(
        Icons.close,
        size: 22,
        color: AppSurfaces.of(context).inkMuted,
      ),
      tooltip: 'Oturumu kapat',
      onPressed: () => onRevoke(session),
    ),
  );
}

/// `Açık oturumlar`: üç ve fazla başka cihaz olduğunda açılan sayfa.
class _SessionsPage extends StatelessWidget {
  const _SessionsPage({
    required this.controller,
    required this.now,
    required this.onRevoke,
    required this.onRevokeOthers,
  });

  final AccountController controller;
  final DateTime Function() now;
  final Future<void> Function(UserSessionSummary session) onRevoke;
  final Future<void> Function(int count) onRevokeOthers;

  @override
  Widget build(BuildContext context) => Scaffold(
    body: SafeArea(
      bottom: false,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          AppPageHeader(
            title: 'Açık oturumlar',
            onBack: () => Navigator.of(context).maybePop(),
          ),
          Expanded(
            child: AnimatedBuilder(
              animation: controller,
              builder: (context, _) {
                final others = _others(
                  controller.sessions,
                  controller.currentSessionId,
                );
                final today = now();
                return ListView(
                  padding: AppPageInsets.list,
                  children: [
                    if (controller.isSubmitting)
                      const LinearProgressIndicator(),
                    const AppCard(
                      padding: EdgeInsets.zero,
                      child: _ThisDeviceRow(),
                    ),
                    if (others.isNotEmpty) ...[
                      const SizedBox(height: AppGaps.section),
                      AppSectionHeader(
                        title: 'Diğer cihazlar · ${others.length}',
                      ),
                      AppCard(
                        padding: EdgeInsets.zero,
                        child: AppDividedColumn(
                          inset: AppIconCapsule.rowInset,
                          children: [
                            for (final session in others)
                              _OtherDeviceRow(
                                session: session,
                                now: today,
                                onRevoke: onRevoke,
                              ),
                          ],
                        ),
                      ),
                      const SizedBox(height: AppGaps.block),
                      AppCard(
                        padding: EdgeInsets.zero,
                        child: AppRow(
                          padding: _rowPadding,
                          leading: const AppIconCapsule(
                            icon: Icons.logout,
                            tone: AppStatusTone.expense,
                          ),
                          title: 'Diğer oturumları kapat',
                          titleStyle: Theme.of(context).textTheme.titleSmall
                              ?.copyWith(
                                color: AppFinanceColors.of(context).expense,
                              ),
                          onTap: controller.isSubmitting
                              ? null
                              : () => onRevokeOthers(others.length),
                        ),
                      ),
                      const SizedBox(height: AppGaps.notice),
                      Padding(
                        padding: const EdgeInsets.symmetric(
                          horizontal: AppSpacing.xSmall,
                        ),
                        child: Text(
                          'Kapatılan cihazda yeniden giriş yapmak gerekir.',
                          style: Theme.of(context).textTheme.bodySmall,
                        ),
                      ),
                    ],
                  ],
                );
              },
            ),
          ),
        ],
      ),
    ),
  );
}

/// `Hesabımı sil`: ne olacağını söyleyen sayfa, yedek kapısı ve kimlik
/// doğrulama.
///
/// Silme bir panel değil sayfadır: geri dönüşü olmayan iş acele ettirilmez,
/// yedek adımı da aynı yerde durur. Kimlik doğrulama silmeden ayrıdır
/// (kullanıcı kararı, 10 Kasım 2026; Google'ın "Kimliğinizi doğrulayın"
/// adımı ve GitHub'ın "erişimi onaylayın"ı gibi): parola yazılır ya da Google
/// hesabı seçilir, kırmızı düğme ancak ondan sonra açılır ve **yalnız o
/// siler**. Google penceresi silmez, kimliği doğrular.
///
/// - Parola var, Google yok: yalnız parola.
/// - Parola yok (Google ile açılmış): yalnız `Google ile doğrula`.
/// - İkisi de var: parola, `ya da`, `Google ile doğrula`.
///
/// Sunucu Google belirtecinin bu hesaba bağlı Google hesabından geldiğini
/// silme anında denetler; başka bir hesap seçildiyse doğrulama düşer ve
/// sebebi bölümün altında yazar.
class _DeleteAccountPage extends StatefulWidget {
  const _DeleteAccountPage({required this.controller});

  final AccountController controller;

  @override
  State<_DeleteAccountPage> createState() => _DeleteAccountPageState();
}

class _DeleteAccountPageState extends State<_DeleteAccountPage> {
  final _password = TextEditingController();
  String? _error;
  bool _busy = false;
  bool _deleted = false;

  /// Google'ın verdiği belirteç ve seçilen hesabın adresi (yalnız
  /// göstermek için; doğrulamayı sunucu yapar).
  String? _googleToken;
  String? _googleEmail;
  bool _googleOpening = false;

  @override
  void dispose() {
    _password.dispose();
    super.dispose();
  }

  UserAccount? get _account => widget.controller.account;
  bool get _hasPassword => _account?.hasPassword ?? true;
  bool get _hasGoogle => !_hasPassword || (_account?.hasGoogle ?? false);
  bool get _verifiedWithGoogle => _googleToken != null;

  @override
  Widget build(BuildContext context) {
    final email = _account?.email ?? '';
    if (_deleted) return _DeletedView(email: email);
    final theme = Theme.of(context);
    final colors = theme.colorScheme;
    final canSubmit =
        !_busy &&
        (_verifiedWithGoogle || (_hasPassword && _password.text.isNotEmpty));
    return Scaffold(
      body: SafeArea(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            AppPageHeader(
              title: 'Hesabımı sil',
              onBack: () => Navigator.of(context).maybePop(),
            ),
            if (_busy) const LinearProgressIndicator(),
            Expanded(
              child: ListView(
                padding: AppPageInsets.form,
                children: [
                  Text.rich(
                    TextSpan(
                      children: [
                        TextSpan(
                          text: email,
                          style: const TextStyle(fontWeight: FontWeight.w600),
                        ),
                        const TextSpan(
                          text:
                              ' hesabı ve bütün kayıtları kalıcı olarak '
                              'silinir. Silinen hesap geri getirilemez.',
                        ),
                      ],
                    ),
                    style: theme.textTheme.bodyLarge,
                  ),
                  const SizedBox(height: AppGaps.block),
                  AppCard(
                    padding: EdgeInsets.zero,
                    child: AppRow(
                      padding: _rowPadding,
                      leading: const AppIconCapsule(
                        icon: Icons.folder_open_outlined,
                      ),
                      title: 'Verilerinizi yedekleyin',
                      trailing: const _Chevron(),
                      onTap: () =>
                          GoRouter.maybeOf(context)?.push('/more/data-tools'),
                    ),
                  ),
                  const SizedBox(height: AppGaps.section),
                  const AppSectionHeader(title: 'Kimliğinizi doğrulayın'),
                  if (_verifiedWithGoogle)
                    _GoogleVerifiedCard(
                      email: _googleEmail,
                      onUndo: _busy
                          ? null
                          : () => setState(() {
                              _googleToken = null;
                              _googleEmail = null;
                              _error = null;
                            }),
                    )
                  else ...[
                    if (_hasPassword)
                      _PasswordField(
                        controller: _password,
                        label: 'Parolanız',
                        autofillHints: const [AutofillHints.password],
                        onChanged: (_) => setState(() => _error = null),
                      ),
                    if (_hasPassword && _hasGoogle) const _OrDivider(),
                    if (_hasGoogle)
                      _GoogleVerifyButton(
                        busy: _googleOpening,
                        onPressed: _busy || _googleOpening
                            ? null
                            : _verifyWithGoogle,
                      ),
                  ],
                  if (_error != null)
                    Padding(
                      padding: const EdgeInsets.only(top: AppSpacing.small),
                      child: Semantics(
                        liveRegion: true,
                        child: Text(
                          _error!,
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: colors.error,
                          ),
                        ),
                      ),
                    ),
                ],
              ),
            ),
            Padding(
              padding: AppSheetInsets.actions,
              child: FilledButton(
                style: FilledButton.styleFrom(
                  backgroundColor: colors.error,
                  foregroundColor: colors.onError,
                  minimumSize: const Size.fromHeight(AppSizes.button),
                ),
                onPressed: canSubmit ? _delete : null,
                child: const Text('Hesabımı sil'),
              ),
            ),
          ],
        ),
      ),
    );
  }

  /// Google'ın hesap penceresi. Seçilen hesap kimliği doğrular; silmez.
  /// Pencere kapatılırsa hiçbir şey olmaz.
  Future<void> _verifyWithGoogle() async {
    setState(() {
      _googleOpening = true;
      _error = null;
    });
    String? token;
    try {
      token = await context.read<AuthController?>()?.obtainGoogleIdToken();
    } on GoogleSignInUnavailable catch (error) {
      if (mounted) setState(() => _error = error.message);
    } finally {
      if (mounted) setState(() => _googleOpening = false);
    }
    if (token == null || !mounted) return;
    setState(() {
      _googleToken = token;
      _googleEmail = _emailInIdToken(token!);
    });
  }

  Future<void> _delete() async {
    setState(() {
      _busy = true;
      _error = null;
    });
    final viaGoogle = _verifiedWithGoogle;
    final deleted = await widget.controller.deleteAccount(
      password: viaGoogle ? null : _password.text,
      googleIdToken: _googleToken,
    );
    if (!mounted) return;
    setState(() {
      _busy = false;
      _deleted = deleted;
      if (deleted) return;
      _error = widget.controller.errorMessage ?? 'Hesap silinemedi.';
      // Başka bir Google hesabı seçildiyse doğrulama düşer; kişi doğru
      // hesabı yeniden seçer.
      if (viaGoogle) {
        _googleToken = null;
        _googleEmail = null;
      }
    });
  }
}

/// Google belirtecindeki adres; yalnız "hangi hesabı seçtiniz" diye
/// göstermek için okunur. İmzayı sunucu denetler, burada güven yoktur.
String? _emailInIdToken(String token) {
  final parts = token.split('.');
  if (parts.length != 3) return null;
  try {
    final payload = utf8.decode(
      base64Url.decode(base64Url.normalize(parts[1])),
    );
    final json = jsonDecode(payload);
    return json is Map<String, dynamic> ? json['email'] as String? : null;
  } on FormatException {
    return null;
  }
}

/// Parola ile Google arasındaki ayraç: ince çizgi, ortada `ya da`.
class _OrDivider extends StatelessWidget {
  const _OrDivider();

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final line = Expanded(
      child: Divider(color: AppSurfaces.of(context).border, height: 1),
    );
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: AppSpacing.medium),
      child: Row(
        children: [
          line,
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: AppSpacing.compact),
            child: Text(
              'ya da',
              style: theme.textTheme.bodySmall?.copyWith(
                color: theme.colorScheme.onSurfaceVariant,
              ),
            ),
          ),
          line,
        ],
      ),
    );
  }
}

/// `Google ile doğrula`: çerçeveli, tam genişlik, Google'ın işaretiyle.
class _GoogleVerifyButton extends StatelessWidget {
  const _GoogleVerifyButton({required this.busy, required this.onPressed});

  final bool busy;
  final VoidCallback? onPressed;

  @override
  Widget build(BuildContext context) {
    final ink = AppSurfaces.of(context).ink;
    return OutlinedButton(
      style: OutlinedButton.styleFrom(
        minimumSize: const Size.fromHeight(AppSizes.button),
        foregroundColor: ink,
      ),
      onPressed: onPressed,
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          if (busy)
            SizedBox.square(
              dimension: 18,
              child: CircularProgressIndicator(strokeWidth: 2, color: ink),
            )
          else
            const AuthGoogleMark(size: 18),
          const SizedBox(width: AppSpacing.compact),
          const Flexible(child: Text('Google ile doğrula')),
        ],
      ),
    );
  }
}

/// Google ile doğrulandı: hangi hesapla olduğu ve geri almak için `Vazgeç`.
class _GoogleVerifiedCard extends StatelessWidget {
  const _GoogleVerifiedCard({required this.email, required this.onUndo});

  final String? email;
  final VoidCallback? onUndo;

  @override
  Widget build(BuildContext context) => AppCard(
    padding: EdgeInsets.zero,
    child: AppRow(
      padding: _rowPadding,
      leading: const AppIconCapsule(
        icon: Icons.check_rounded,
        tone: AppStatusTone.income,
      ),
      title: 'Google ile doğrulandı',
      subtitle: email,
      trailing: AppTextAction(label: 'Vazgeç', onPressed: onUndo),
    ),
  );
}

/// Silmeden sonraki son ekran. Hesap sunucuda yoktur ve eldeki oturum ölüdür;
/// buradan geri dönülmez, tek yol giriş ekranıdır.
class _DeletedView extends StatelessWidget {
  const _DeletedView({required this.email});

  final String email;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    return PopScope(
      canPop: false,
      child: Scaffold(
        body: SafeArea(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Expanded(
                child: Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.xLarge,
                  ),
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const AppIconCapsule(icon: Icons.check, size: 72),
                      const SizedBox(height: AppSpacing.large),
                      Text(
                        'Hesabınız silindi',
                        textAlign: TextAlign.center,
                        style: theme.textTheme.titleLarge,
                      ),
                      const SizedBox(height: AppSpacing.small),
                      Text(
                        '$email ile ilişkili bütün kayıtlar kaldırıldı.',
                        textAlign: TextAlign.center,
                        style: theme.textTheme.bodyMedium?.copyWith(
                          color: surfaces.inkMuted,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
              Padding(
                padding: AppSheetInsets.actions,
                child: OutlinedButton(
                  style: OutlinedButton.styleFrom(
                    minimumSize: const Size.fromHeight(AppSizes.button),
                  ),
                  // Çıkış, cihazdaki oturum bilgisini silen ve giriş ekranına
                  // götüren adımdır.
                  onPressed: () => context.read<AuthController?>()?.logout(),
                  child: const Text('Giriş ekranına dön'),
                ),
              ),
            ],
          ),
        ),
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
    this.focusNode,
  });

  final TextEditingController controller;
  final FocusNode? focusNode;
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
      focusNode: widget.focusNode,
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

/// Parolası olan hesapta parola değiştirme. İki hâli vardır: mevcut parolayla
/// ya da `Parolamı unuttum` ile e-postaya giden kodla (10 Kasım 2026).
/// Kodla değiştirmek adresin sahibini de kanıtlar; e-posta doğrulanmış
/// sayılır.
class _ChangePasswordSheet extends StatefulWidget {
  const _ChangePasswordSheet({required this.controller});

  final AccountController controller;

  @override
  State<_ChangePasswordSheet> createState() => _ChangePasswordSheetState();
}

class _ChangePasswordSheetState extends State<_ChangePasswordSheet> {
  final _currentPassword = TextEditingController();
  final _code = TextEditingController();
  final _newPassword = TextEditingController();
  final _newPasswordFocus = FocusNode();
  bool _byCode = false;
  String? _codeError;
  String? _validationMessage;

  @override
  void dispose() {
    _currentPassword.dispose();
    _code.dispose();
    _newPassword.dispose();
    _newPasswordFocus.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final email = widget.controller.account?.email ?? '';
    return AppFormSheet<bool>(
      title: 'Parolamı değiştir',
      description: _byCode
          ? '$email adresine gelen 6 haneli kodu ve yeni parolanızı yazın.'
          : null,
      submitLabel: 'Değiştir',
      onSubmit: _submit,
      children: [
        if (_byCode) ...[
          _EmailCodeEntry(
            controller: _code,
            request: () => _requestPasswordCode(widget.controller),
            error: _codeError,
            onChanged: (_) => setState(() => _codeError = null),
            // Kod tamamlanınca sıradaki alan yeni parola.
            onCompleted: (_) => _newPasswordFocus.requestFocus(),
          ),
        ] else ...[
          _PasswordField(
            controller: _currentPassword,
            label: 'Mevcut parola',
            autofillHints: const [AutofillHints.password],
          ),
          Align(
            alignment: AlignmentDirectional.centerEnd,
            child: AppTextAction(
              label: 'Parolamı unuttum',
              underlined: true,
              onPressed: () => setState(() {
                _byCode = true;
                _validationMessage = null;
              }),
            ),
          ),
        ],
        if (!_byCode) const SizedBox(height: AppSpacing.xSmall),
        _PasswordField(
          controller: _newPassword,
          focusNode: _newPasswordFocus,
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
    if (_byCode && _code.text.length != _EmailCodeEntry.length) {
      setState(() => _codeError = 'E-postanıza gelen 6 haneli kodu yazın.');
      return null;
    }
    if (!_byCode && _currentPassword.text.isEmpty) {
      setState(() => _validationMessage = 'Mevcut parolanızı yazın.');
      return null;
    }
    if (AuthValidation.registrationPassword(_newPassword.text) != null) {
      setState(
        () => _validationMessage = 'Yeni parola kuralların hepsini sağlamalı.',
      );
      return null;
    }
    final done = await widget.controller.changePassword(
      currentPassword: _byCode ? null : _currentPassword.text,
      newPassword: _newPassword.text,
      resetCode: _byCode ? _code.text : null,
    );
    if (!done) {
      setState(() {
        if (_byCode &&
            widget.controller.errorCode ==
                'authentication.invalid_reset_code') {
          _codeError = _codeErrorText(widget.controller, 'Kod doğrulanamadı.');
        } else {
          _validationMessage =
              widget.controller.errorMessage ?? 'Parola değiştirilemedi.';
        }
      });
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
  final _newPasswordFocus = FocusNode();
  String? _codeError;
  String? _validationMessage;

  @override
  void dispose() {
    _code.dispose();
    _newPassword.dispose();
    _newPasswordFocus.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final email = widget.controller.account?.email ?? '';
    return AppFormSheet<bool>(
      title: 'Parola belirle',
      description:
          '$email adresine gelen 6 haneli kodu ve yeni parolanızı yazın.',
      submitLabel: 'Parolayı belirle',
      onSubmit: _submit,
      children: [
        _EmailCodeEntry(
          controller: _code,
          request: () => _requestPasswordCode(widget.controller),
          error: _codeError,
          onChanged: (_) => setState(() => _codeError = null),
          onCompleted: (_) => _newPasswordFocus.requestFocus(),
        ),
        _PasswordField(
          controller: _newPassword,
          focusNode: _newPasswordFocus,
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
    if (_code.text.length != _EmailCodeEntry.length) {
      setState(() => _codeError = 'E-postanıza gelen 6 haneli kodu yazın.');
      return null;
    }
    if (AuthValidation.registrationPassword(_newPassword.text) != null) {
      setState(
        () => _validationMessage = 'Yeni parola kuralların hepsini sağlamalı.',
      );
      return null;
    }
    final done = await widget.controller.changePassword(
      newPassword: _newPassword.text,
      resetCode: _code.text,
    );
    if (!done) {
      setState(() {
        if (widget.controller.errorCode ==
            'authentication.invalid_reset_code') {
          _codeError = _codeErrorText(widget.controller, 'Kod doğrulanamadı.');
        } else {
          _validationMessage =
              widget.controller.errorMessage ?? 'Parola belirlenemedi.';
        }
      });
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
