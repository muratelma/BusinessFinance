import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/presentation/scope_controller.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/theme/app_surfaces.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_confirm_dialog.dart';
import '../../../core/widgets/app_form_sheet.dart';
import '../../../core/widgets/app_list_row.dart';
import '../../../core/widgets/app_section_header.dart';
import '../../../core/widgets/app_state_views.dart';
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
/// Hesaba dair dağınık parçalar — e-posta, işletme cevabı, açık oturumlar,
/// parola, çıkış ve hesabı kapatma — tek sayfada toplanır. Sıra tehlikeye
/// göredir: önce kim olduğunuz, sonra nereden açık olduğunuz, en sonda geri
/// dönüşü olmayan eylem.
class AccountPage extends StatefulWidget {
  const AccountPage({
    required this.repository,
    required this.authRepository,
    super.key,
  });

  final AccountRepositoryContract repository;
  final AuthSessionRepository authRepository;

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

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(title: const Text('Hesabım')),
    body: _body(),
  );

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

    return Column(
      children: [
        if (controller.isSubmitting) const LinearProgressIndicator(),
        Expanded(
          child: ListView(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.medium,
              AppSpacing.medium,
              AppSpacing.medium,
              AppSpacing.fabClearance,
            ),
            children: [
              _IdentityCard(account: account),
              if (!account.emailConfirmed) ...[
                const SizedBox(height: AppSpacing.medium),
                _UnverifiedEmailCard(onVerify: _openVerificationSheet),
              ],
              const SizedBox(height: AppSpacing.medium),
              const _BusinessAnswerCard(),
              const SizedBox(height: AppSpacing.medium),
              const AppSectionHeader(title: 'Açık oturumlar'),
              _SessionsCard(
                sessions: controller.sessions,
                currentSessionId: controller.currentSessionId,
                onRevoke: _confirmRevoke,
              ),
              const SizedBox(height: AppSpacing.medium),
              const AppSectionHeader(title: 'Güvenlik'),
              AppCard(
                padding: EdgeInsets.zero,
                child: Column(
                  children: [
                    AppListRow(
                      icon: Icons.password_outlined,
                      title: 'Parolamı değiştir',
                      subtitle:
                          'Değiştirdiğinizde diğer cihazlardaki oturumlar kapanır.',
                      trailing: const Icon(Icons.chevron_right),
                      onTap: _openPasswordSheet,
                    ),
                    AppListRow(
                      icon: Icons.logout,
                      title: 'Çıkış yap',
                      subtitle: 'Bu cihazdaki güvenli oturum kapatılır.',
                      onTap: _confirmLogout,
                    ),
                  ],
                ),
              ),
              const SizedBox(height: AppSpacing.medium),
              _DangerCard(onDelete: _openDeleteSheet),
            ],
          ),
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
      highlight: 'Açılış: ${_formatDateTime(session.createdAtUtc)}',
      confirmLabel: 'Kapat',
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

  Future<void> _confirmLogout() async {
    final shouldLogout = await AppConfirmDialog.show(
      context: context,
      icon: Icons.logout,
      title: 'Çıkış yapılsın mı?',
      message: 'Bu cihazdaki oturum bilgileri güvenli biçimde silinecek.',
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

  Future<void> _openDeleteSheet() async {
    // İlk kapı: ne olacağını söyleyen açık onay.
    final confirmed = await AppConfirmDialog.show(
      context: context,
      icon: Icons.delete_forever_outlined,
      title: 'Hesabınız silinsin mi?',
      message:
          'Hesabınız ve bütün kayıtlarınız kalıcı olarak silinir; geri '
          'getirilemez. Silmeden önce Diğer > Veri ve yedek adımından '
          'yedeğinizi almanız önerilir.',
      highlight: controller.account?.email,
      confirmLabel: 'Devam et',
      destructive: true,
    );
    if (!confirmed || !mounted) return;

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

class _IdentityCard extends StatelessWidget {
  const _IdentityCard({required this.account});

  final UserAccount account;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text('E-posta', style: theme.textTheme.labelMedium),
          const SizedBox(height: AppSpacing.xSmall),
          Text(account.email, style: theme.textTheme.titleMedium),
          const SizedBox(height: AppSpacing.small),
          Text(
            'Hesap açılışı: ${_formatDate(account.createdAtUtc)}',
            style: theme.textTheme.bodySmall?.copyWith(
              color: theme.colorScheme.onSurfaceVariant,
            ),
          ),
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
class _BusinessAnswerCard extends StatefulWidget {
  const _BusinessAnswerCard();

  @override
  State<_BusinessAnswerCard> createState() => _BusinessAnswerCardState();
}

class _BusinessAnswerCardState extends State<_BusinessAnswerCard> {
  bool _isSaving = false;

  @override
  Widget build(BuildContext context) {
    final scopeController = context.watch<ScopeController?>();
    final repository = context.read<ProfileRepositoryContract?>();
    // Bağlanmamış kabukta (test ya da bağımlılıksız kurulum) kart hiç
    // çizilmez: değiştirilemeyen bir anahtar göstermek, kullanıcıya
    // çalışmayan bir düğme vermek olurdu.
    if (scopeController == null || repository == null) {
      return const SizedBox.shrink();
    }
    return AppCard(
      child: SwitchListTile(
        value: scopeController.isVisible,
        onChanged: _isSaving
            ? null
            : (value) => _save(repository, scopeController, value),
        contentPadding: EdgeInsets.zero,
        title: const Text('İşletmem var'),
        subtitle: const Text(
          'Açıkken kayıtlarınızı işletme ve şahsi olarak ayrı '
          'okuyabilirsiniz. Kapalıyken bu ayrım hiç görünmez. '
          'Kategorileriniz iki durumda da olduğu gibi kalır.',
        ),
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
      await scopeController.applyHasBusiness(profile.hasBusiness);
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

/// Doğrulanmamış adresin kalıcı uyarısı.
///
/// Hesap **kilitli değildir**: kart bir engel değil, bir hatırlatmadır. Metin
/// bunu açıkça söyler, yoksa kullanıcı uygulamanın yarısının kapalı olduğunu
/// sanır.
class _UnverifiedEmailCard extends StatelessWidget {
  const _UnverifiedEmailCard({required this.onVerify});

  final Future<void> Function() onVerify;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(
                Icons.mark_email_unread_outlined,
                color: theme.colorScheme.primary,
              ),
              const SizedBox(width: AppSpacing.small),
              Expanded(
                child: Text(
                  'E-posta adresiniz doğrulanmadı',
                  style: theme.textTheme.titleMedium,
                ),
              ),
            ],
          ),
          const SizedBox(height: AppSpacing.xSmall),
          Text(
            'Uygulamayı kullanmaya devam edebilirsiniz; doğrulama hesabınızı '
            'güvenceye alır ve parolanızı unuttuğunuzda geri almanızı sağlar.',
            style: theme.textTheme.bodySmall?.copyWith(
              color: theme.colorScheme.onSurfaceVariant,
            ),
          ),
          const SizedBox(height: AppSpacing.small),
          Align(
            alignment: Alignment.centerLeft,
            child: FilledButton.tonalIcon(
              onPressed: onVerify,
              icon: const Icon(Icons.mark_email_read_outlined),
              label: const Text('Adresimi doğrula'),
            ),
          ),
        ],
      ),
    );
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
    required this.onRevoke,
  });

  final List<UserSessionSummary> sessions;
  final String? currentSessionId;
  final Future<void> Function(UserSessionSummary session) onRevoke;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);

    if (sessions.isEmpty) {
      return AppCard(
        child: Text(
          'Açık oturum görünmüyor.',
          style: theme.textTheme.bodyMedium,
        ),
      );
    }

    return AppCard(
      padding: EdgeInsets.zero,
      child: Column(
        children: [
          for (final (index, session) in sessions.indexed) ...[
            if (index > 0)
              Divider(height: 1, thickness: 1, color: surfaces.border),
            AppListRow(
              icon: Icons.devices_outlined,
              title: session.sessionId == currentSessionId
                  ? 'Bu cihaz'
                  : 'Başka bir cihaz',
              subtitle:
                  'Açılış: ${_formatDateTime(session.createdAtUtc)}\n'
                  'Geçerlilik: ${_formatDate(session.expiresAtUtc)}',
              trailing: session.sessionId == currentSessionId
                  // Kendi oturumunu buradan kapatmak "çıkış yap"tır ve onun
                  // kendi satırı var; iki yerde iki farklı isimle aynı şeyi
                  // sunmak kullanıcıyı yanıltırdı.
                  ? null
                  : IconButton(
                      icon: const Icon(Icons.close),
                      tooltip: 'Oturumu kapat',
                      onPressed: () => onRevoke(session),
                    ),
            ),
          ],
        ],
      ),
    );
  }
}

class _DangerCard extends StatelessWidget {
  const _DangerCard({required this.onDelete});

  final Future<void> Function() onDelete;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Hesabı kapat',
            style: theme.textTheme.titleMedium?.copyWith(
              color: theme.colorScheme.error,
            ),
          ),
          const SizedBox(height: AppSpacing.xSmall),
          Text(
            'Hesabınız ve bütün kayıtlarınız kalıcı olarak silinir. '
            'Bu işlem geri alınamaz.',
            style: theme.textTheme.bodySmall?.copyWith(
              color: theme.colorScheme.onSurfaceVariant,
            ),
          ),
          const SizedBox(height: AppSpacing.small),
          Align(
            alignment: Alignment.centerLeft,
            child: TextButton.icon(
              onPressed: onDelete,
              icon: const Icon(Icons.delete_forever_outlined),
              label: const Text('Hesabımı sil'),
              style: TextButton.styleFrom(
                foregroundColor: theme.colorScheme.error,
              ),
            ),
          ),
        ],
      ),
    );
  }
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
      description:
          'Parolanız değişince bütün cihazlardaki oturumlar kapanır; bu '
          'cihazda açık kalırsınız.',
      submitLabel: 'Değiştir',
      onSubmit: _submit,
      children: [
        TextField(
          controller: _currentPassword,
          obscureText: true,
          autofillHints: const [AutofillHints.password],
          decoration: const InputDecoration(labelText: 'Mevcut parola'),
        ),
        const SizedBox(height: AppSpacing.medium),
        TextField(
          controller: _newPassword,
          obscureText: true,
          autofillHints: const [AutofillHints.newPassword],
          decoration: const InputDecoration(
            labelText: 'Yeni parola',
            helperText:
                'En az 12 karakter; büyük, küçük, rakam ve simge içermeli.',
            helperMaxLines: 2,
          ),
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
    final policyMessage = AuthValidation.registrationPassword(
      _newPassword.text,
    );
    if (policyMessage != null) {
      setState(() => _validationMessage = policyMessage);
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

String _formatDate(DateTime utc) {
  final local = utc.toLocal();
  return '${local.day.toString().padLeft(2, '0')}.'
      '${local.month.toString().padLeft(2, '0')}.${local.year}';
}

String _formatDateTime(DateTime utc) {
  final local = utc.toLocal();
  return '${_formatDate(utc)} '
      '${local.hour.toString().padLeft(2, '0')}:'
      '${local.minute.toString().padLeft(2, '0')}';
}
