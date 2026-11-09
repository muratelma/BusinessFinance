import 'package:flutter/material.dart';

import '../../../core/network/api_exception.dart';
import 'auth_validation.dart';
import 'nomiva_brand.dart';
import 'widgets/auth_form_parts.dart';

/// Basılan düğmenin ekrandaki ortası; başarılı girişte Özet'in rengi oradan
/// yayılır.
Offset? centerOf(GlobalKey key) {
  final box = key.currentContext?.findRenderObject() as RenderBox?;
  if (box == null || !box.hasSize) return null;
  return box.localToGlobal(box.size.center(Offset.zero));
}

//// Panelin içeriği: yan boşluk 24, öğeler arası [gap] (tasarım biriminde).
/// Geniş ekranda form ortada, tasarımın genişliğinde durur.
class AuthPanelBody extends StatelessWidget {
  const AuthPanelBody({
    required this.children,
    super.key,
    this.gap = NomivaSpacing.gap,
  });

  final List<Widget> children;
  final double gap;

  @override
  Widget build(BuildContext context) {
    final n = NomivaScale.of(context);
    return SingleChildScrollView(
      padding: EdgeInsets.fromLTRB(
        n(NomivaSpacing.side),
        n(NomivaSpacing.panelTop),
        n(NomivaSpacing.side),
        MediaQuery.paddingOf(context).bottom,
      ),
      child: Center(
        child: ConstrainedBox(
          constraints: BoxConstraints(
            maxWidth: NomivaScale.maxFormWidth - 2 * n(NomivaSpacing.side),
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            spacing: n(gap),
            children: children,
          ),
        ),
      ),
    );
  }
}

/// Apple ile giriş görünür ama henüz bağlı değil (ücretli Apple geliştirici
/// hesabından sonra). Düğme sessiz kalmaz; formun içinde söyler.
String socialNotReady(AuthSocialProvider provider) =>
    '${provider.label} ile giriş henüz hazır değil. '
    'Şimdilik e-posta ve parolanızla devam edebilirsiniz.';

/// Formun içinde gösterilecek, sunucudan gelmeyen bir uyarı (Google penceresi
/// açılamadı, Apple henüz hazır değil).
class AuthNotice implements Exception {
  const AuthNotice(this.message);

  final String message;

  @override
  String toString() => message;
}

/// Google ya da Apple düğmesi. Panel bekleme hâlini ve uyarıyı kendisi
/// gösterir; [origin] başarılı girişte rengin yayılacağı noktadır.
typedef AuthSocialSubmit =
    Future<void> Function(AuthSocialProvider provider, Offset? origin);

/// Parola sıfırlamanın cümleleri. Kodun gittiği cümle adresin kayıtlı olup
/// olmadığını söylemez; sunucu da söylemiyor.
const resetCodeSent =
    'Adres kayıtlıysa kod gönderildi. Gelen kutunuzu kontrol edin.';
const resetCodeMissing = 'E-postanıza gelen 6 haneli kodu yazın.';

typedef LoginPanelSubmit =
    Future<void> Function(String email, String password, Offset? origin);

class LoginPanel extends StatefulWidget {
  const LoginPanel({
    required this.emailController,
    required this.onSubmit,
    required this.onForgotPassword,
    required this.onRegister,
    this.active = true,
    this.onSocial,
    super.key,
  });

  final TextEditingController emailController;
  final LoginPanelSubmit onSubmit;
  final VoidCallback onForgotPassword;
  final VoidCallback onRegister;

  /// Google/Apple düğmesi; null ise düğme "henüz hazır değil" der.
  final AuthSocialSubmit? onSocial;

  /// Panel ekranda mı. Kişi panelden ayrılınca hata yazıları ve uyarı
  /// temizlenir, yazılanlar kalır: hata bir denemeye aittir, geri dönen kişi
  /// temiz bir ekran görür (kullanıcı kararı, 8 Kasım 2026).
  final bool active;

  @override
  State<LoginPanel> createState() => _LoginPanelState();
}

class _LoginPanelState extends State<LoginPanel> {
  final _formKey = GlobalKey<FormState>();
  final _identifierField = GlobalKey<AuthFieldState>();
  final _passwordField = GlobalKey<AuthFieldState>();
  final _password = TextEditingController();
  final _button = GlobalKey();
  bool _busy = false;

  /// Google penceresi açıkken ya da cevap beklenirken: form kilitlenir ama
  /// `Giriş yap` beklemeye geçmez (basılan o değil).
  bool _socialBusy = false;
  String? _error;

  Future<void> _social(AuthSocialProvider provider) async {
    final submit = widget.onSocial;
    if (submit == null) {
      setState(() => _error = socialNotReady(provider));
      return;
    }
    FocusScope.of(context).unfocus();
    setState(() {
      _socialBusy = true;
      _error = null;
    });
    try {
      await submit(provider, null);
    } on ApiException catch (error) {
      if (mounted) setState(() => _error = error.message);
    } on AuthNotice catch (notice) {
      if (mounted) setState(() => _error = notice.message);
    } finally {
      if (mounted) setState(() => _socialBusy = false);
    }
  }

  @override
  void didUpdateWidget(LoginPanel oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.active && !widget.active) {
      clearErrors([_identifierField, _passwordField]);
      _error = null;
    }
  }

  @override
  void dispose() {
    _password.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (_busy || _socialBusy) return;
    if (!_formKey.currentState!.validate()) {
      revealFirstError([_identifierField, _passwordField]);
      return;
    }
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      await widget.onSubmit(
        widget.emailController.text.trim(),
        _password.text,
        centerOf(_button),
      );
    } on ApiException catch (error) {
      if (mounted) setState(() => _error = error.message);
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  @override
  Widget build(BuildContext context) => AutofillGroup(
    child: Form(
      key: _formKey,
      child: AuthPanelBody(
        children: [
          const AuthTitle('Giriş yap'),
          AuthField(
            key: _identifierField,
            label: 'E-posta ya da kullanıcı adı',
            controller: widget.emailController,
            icon: AuthFieldIcon.person,
            keyboardType: TextInputType.emailAddress,
            autofillHints: const [AutofillHints.email, AutofillHints.username],
            textInputAction: TextInputAction.next,
            validator: AuthValidation.loginIdentifier,
          ),
          AuthField(
            key: _passwordField,
            label: 'Parola',
            controller: _password,
            icon: AuthFieldIcon.lock,
            obscurable: true,
            autofillHints: const [AutofillHints.password],
            textInputAction: TextInputAction.done,
            validator: AuthValidation.loginPassword,
            onFieldSubmitted: (_) => _submit(),
          ),
          AuthOverhangingLink(
            after: NomivaSpacing.beforeButton,
            child: AuthTextLink(
              label: 'Parolamı unuttum',
              onPressed: _busy || _socialBusy ? null : widget.onForgotPassword,
            ),
          ),
          if (_error case final message?) AuthBanner(message),
          AuthPrimaryButton(
            key: _button,
            label: 'Giriş yap',
            busy: _busy,
            onPressed: _submit,
          ),
          const AuthOrDivider(),
          AuthSocialRow(onPressed: _busy || _socialBusy ? null : _social),
          AuthFooterSlot(
            child: AuthFooter(
              question: 'Hesabın yok mu?',
              action: 'Hesap oluştur',
              onPressed: _busy || _socialBusy ? null : widget.onRegister,
            ),
          ),
        ],
      ),
    ),
  );
}

/// Hesap oluşturmanın ilk adımı: kullanıcı adı, e-posta ve parola. Hesap
/// burada açılmaz; "Bir adım kaldı"daki `Devam et`'te açılır, vazgeçen kişi
/// yarım hesap bırakmaz.
///
/// Kullanıcı adı sunucuya henüz gitmez: sunucu tarafı Aşama 06.4'ün sıradaki
/// adımıdır. Alan, ekran tasarımla aynı olsun diye şimdiden sorulur ve
/// doğrulanır (kullanıcı kararı, 8 Kasım 2026).
class RegisterPanel extends StatefulWidget {
  const RegisterPanel({
    required this.onNext,
    required this.onLogin,
    this.active = true,
    super.key,
    this.error,
    this.onCheckUsername,
    this.onSocial,
  });

  final void Function(String username, String email, String password) onNext;

  /// Google/Apple düğmesi; null ise düğme "henüz hazır değil" der.
  final AuthSocialSubmit? onSocial;

  /// Kullanıcı adı boş mu; alandan çıkınca ve `Hesap oluştur`'a basınca
  /// sorulur. Null ise sorulmaz (kayıt kendisi reddeder).
  final Future<bool> Function(String username)? onCheckUsername;
  final VoidCallback onLogin;

  /// Panel ekranda mı. Kişi panelden ayrılınca hata yazıları ve uyarı
  /// temizlenir, yazılanlar kalır: hata bir denemeye aittir, geri dönen kişi
  /// temiz bir ekran görür (kullanıcı kararı, 8 Kasım 2026).
  final bool active;

  /// Bir sonraki adımda sunucunun reddi; kişi buraya geri döner.
  final String? error;

  @override
  State<RegisterPanel> createState() => _RegisterPanelState();
}

class _RegisterPanelState extends State<RegisterPanel> {
  final _formKey = GlobalKey<FormState>();
  final _usernameField = GlobalKey<AuthFieldState>();
  final _emailField = GlobalKey<AuthFieldState>();
  final _passwordField = GlobalKey<AuthFieldState>();
  final _username = TextEditingController();
  final _email = TextEditingController();
  final _password = TextEditingController();
  String? _notice;

  @override
  void didUpdateWidget(RegisterPanel oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.active && !widget.active) {
      clearErrors([_usernameField, _emailField, _passwordField]);
      _notice = null;
    }
  }

  @override
  void dispose() {
    _username.dispose();
    _email.dispose();
    _password.dispose();
    super.dispose();
  }

  bool _checking = false;
  bool _socialBusy = false;

  Future<void> _social(AuthSocialProvider provider) async {
    final submit = widget.onSocial;
    if (submit == null) {
      setState(() => _notice = socialNotReady(provider));
      return;
    }
    FocusScope.of(context).unfocus();
    setState(() {
      _socialBusy = true;
      _notice = null;
    });
    try {
      await submit(provider, null);
    } on ApiException catch (error) {
      if (mounted) setState(() => _notice = error.message);
    } on AuthNotice catch (notice) {
      if (mounted) setState(() => _notice = notice.message);
    } finally {
      if (mounted) setState(() => _socialBusy = false);
    }
  }

  /// Sunucuya ulaşılamazsa ad "boş" sayılır: kişi bekletilmez, ad alınmışsa
  /// kayıt `authentication.username_taken` ile reddeder ve kişi buraya döner.
  Future<String?> _checkUsername(String username) async {
    final check = widget.onCheckUsername;
    if (check == null) return null;
    try {
      return await check(username) ? null : AuthValidation.usernameTaken;
    } on Object {
      return null;
    }
  }

  Future<void> _next() async {
    if (_checking || _socialBusy) return;
    setState(() => _notice = null);
    if (!_formKey.currentState!.validate()) {
      revealFirstError([_usernameField, _emailField, _passwordField]);
      return;
    }
    // Bu arada başkası aynı adı almış olabilir: basınca bir kez daha sorulur.
    setState(() => _checking = true);
    final free = await _usernameField.currentState?.checkRemote() ?? true;
    if (!mounted) return;
    setState(() => _checking = false);
    if (!free) {
      revealFirstError([_usernameField]);
      return;
    }
    FocusScope.of(context).unfocus();
    widget.onNext(_username.text.trim(), _email.text.trim(), _password.text);
  }

  @override
  Widget build(BuildContext context) {
    final n = NomivaScale.of(context);
    final message = _notice ?? widget.error;
    return AutofillGroup(
      child: Form(
        key: _formKey,
        child: AuthPanelBody(
          gap: NomivaSpacing.gapTight,
          children: [
            const AuthTitle('Hesap oluştur'),
            AuthField(
              key: _usernameField,
              label: 'Kullanıcı adı',
              controller: _username,
              icon: AuthFieldIcon.person,
              autofillHints: const [AutofillHints.newUsername],
              textInputAction: TextInputAction.next,
              maxLength: 30,
              validator: AuthValidation.username,
              remoteValidator: _checkUsername,
            ),
            AuthField(
              key: _emailField,
              label: 'E-posta',
              controller: _email,
              icon: AuthFieldIcon.mail,
              keyboardType: TextInputType.emailAddress,
              autofillHints: const [AutofillHints.email],
              textInputAction: TextInputAction.next,
              validator: AuthValidation.email,
            ),
            AuthField(
              key: _passwordField,
              label: 'Parola',
              controller: _password,
              icon: AuthFieldIcon.lock,
              obscurable: true,
              autofillHints: const [AutofillHints.newPassword],
              textInputAction: TextInputAction.done,
              validator: AuthValidation.registrationPassword,
              onFieldSubmitted: (_) => _next(),
            ),
            if (message != null)
              Padding(
                padding: EdgeInsets.only(top: n(NomivaSpacing.beforeButton)),
                child: AuthBanner(message),
              ),
            Padding(
              padding: EdgeInsets.only(top: n(NomivaSpacing.beforeButton)),
              child: AuthPrimaryButton(
                label: 'Hesap oluştur',
                busy: _checking,
                onPressed: _next,
              ),
            ),
            const AuthOrDivider(),
            AuthSocialRow(onPressed: _checking || _socialBusy ? null : _social),
            AuthFooterSlot(
              child: AuthFooter(
                question: 'Hesabın var mı?',
                action: 'Giriş yap',
                onPressed: widget.onLogin,
              ),
            ),
          ],
        ),
      ),
    );
  }
}

typedef BusinessStepSubmit =
    Future<void> Function({
      required bool hasBusiness,
      Offset? origin,
      String? username,
    });

/// "Bir adım kaldı": her kayıt yolunun sonu. Onboarding'in tek sorusu burada
/// sorulur.
///
/// E-postayla kayıtta yalnız soru vardır ("Son bir soru."). Google ile ilk
/// girişte ([provider]) kullanıcı adı da burada seçilir; alan e-postadan
/// türetilmiş boş bir adla dolu gelir ([suggestedUsername]) ve altta
/// `Vazgeç` çıkar. Hesap `Devam et`'te açılır; vazgeçen kişi iz bırakmaz.
class BusinessStepPanel extends StatefulWidget {
  const BusinessStepPanel({
    required this.onSubmit,
    super.key,
    this.provider,
    this.suggestedUsername,
    this.onCheckUsername,
    this.onCancel,
  });

  final BusinessStepSubmit onSubmit;
  final AuthSocialProvider? provider;
  final String? suggestedUsername;

  /// Kullanıcı adı boş mu; alandan çıkınca sorulur.
  final Future<bool> Function(String username)? onCheckUsername;
  final VoidCallback? onCancel;

  @override
  State<BusinessStepPanel> createState() => _BusinessStepPanelState();
}

class _BusinessStepPanelState extends State<BusinessStepPanel> {
  final _formKey = GlobalKey<FormState>();
  final _button = GlobalKey();
  final _usernameField = GlobalKey<AuthFieldState>();
  late final _username = TextEditingController(text: widget.suggestedUsername);

  /// Varsayılan **hayır**: soru sorulmadan işletme sahibi varsaymak, esnaf
  /// olmayan kullanıcıyı hiç kullanmayacağı işletme kategorileriyle
  /// karşılardı. Cevap hiçbir özelliği kapatmaz.
  bool _hasBusiness = false;
  bool _busy = false;
  String? _error;

  @override
  void didUpdateWidget(BusinessStepPanel oldWidget) {
    super.didUpdateWidget(oldWidget);
    // Google'la her yeni girişte öneri yeniden yazılır, eski deneme silinir.
    if (oldWidget.suggestedUsername != widget.suggestedUsername ||
        oldWidget.provider != widget.provider) {
      _username.text = widget.suggestedUsername ?? '';
      _usernameField.currentState?.clearError();
      _error = null;
    }
  }

  @override
  void dispose() {
    _username.dispose();
    super.dispose();
  }

  Future<String?> _checkUsername(String username) async {
    final check = widget.onCheckUsername;
    if (check == null) return null;
    try {
      return await check(username) ? null : AuthValidation.usernameTaken;
    } on Object {
      return null;
    }
  }

  Future<void> _submit() async {
    if (_busy) return;
    final withUsername = widget.provider != null;
    if (withUsername && !_formKey.currentState!.validate()) {
      revealFirstError([_usernameField]);
      return;
    }
    FocusScope.of(context).unfocus();
    setState(() {
      _busy = true;
      _error = null;
    });
    try {
      await widget.onSubmit(
        hasBusiness: _hasBusiness,
        origin: centerOf(_button),
        username: withUsername ? _username.text.trim() : null,
      );
    } on ApiException catch (error) {
      if (!mounted) return;
      if (error.code == 'authentication.username_taken' ||
          error.code == 'authentication.invalid_username') {
        _usernameField.currentState?.showError(error.message);
        revealFirstError([_usernameField]);
      } else {
        setState(() => _error = error.message);
      }
    } on AuthNotice catch (notice) {
      if (mounted) setState(() => _error = notice.message);
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final c = NomivaColors.of(context);
    final n = NomivaScale.of(context);
    final provider = widget.provider;
    return Form(
      key: _formKey,
      child: AuthPanelBody(
        children: [
          const AuthTitle('Bir adım kaldı'),
          AuthSubtitle(
            provider == null
                ? 'Son bir soru.'
                : '${provider.label} hesabınızla kayıt oluyorsunuz.',
          ),
          if (provider != null)
            AuthField(
              key: _usernameField,
              label: 'Kullanıcı adı',
              controller: _username,
              icon: AuthFieldIcon.person,
              autofillHints: const [AutofillHints.newUsername],
              textInputAction: TextInputAction.done,
              maxLength: 30,
              validator: AuthValidation.username,
              remoteValidator: _checkUsername,
            ),
          Material(
            color: c.field,
            borderRadius: BorderRadius.circular(n(NomivaSpacing.radius)),
            clipBehavior: Clip.antiAlias,
            child: SwitchListTile(
              value: _hasBusiness,
              onChanged: _busy
                  ? null
                  : (value) => setState(() => _hasBusiness = value),
              contentPadding: EdgeInsets.symmetric(
                horizontal: n(NomivaSpacing.switchX),
                vertical: n(NomivaSpacing.switchY) / 2,
              ),
              activeTrackColor: NomivaColors.accent,
              activeThumbColor: NomivaColors.thumb,
              title: Text(
                'İşletmem var',
                style: TextStyle(
                  fontFamily: nomivaFont,
                  height: nomivaLineHeight,
                  letterSpacing: nomivaLetterSpacing,
                  fontSize: n(14),
                  fontWeight: FontWeight.w600,
                  color: c.text,
                ),
              ),
              subtitle: Text(
                'İşletme kategorileriyle başlarsın',
                style: TextStyle(
                  fontFamily: nomivaFont,
                  height: nomivaLineHeight,
                  letterSpacing: nomivaLetterSpacing,
                  fontSize: n(12),
                  color: c.muted,
                ),
              ),
            ),
          ),
          if (_error case final message?) AuthBanner(message),
          AuthPrimaryButton(
            key: _button,
            label: 'Devam et',
            busy: _busy,
            onPressed: _submit,
          ),
          if (provider != null)
            // Düğmenin hemen altında, ekranın öbür bağlantılarıyla aynı dilde
            // (9 Kasım 2026): teslimdeki yerinde düğmeden kopuk ve küçük
            // kalıyordu. Çerçeveli ikinci bir düğme Vazgeç'e Devam et kadar
            // ağırlık verirdi.
            Center(
              child: AuthTextLink(
                label: 'Vazgeç',
                fontSize: 14,
                onPressed: _busy ? null : widget.onCancel,
              ),
            ),
        ],
      ),
    );
  }
}

// Parolasını unutan kullanıcının kendi başına geri alması.
///
/// Tek panel, iki adım: adres yazılır ve kod istenir; sonra kod ile yeni parola
/// yazılır. İki ayrı ekran, gelen kodu okuyup geri dönen kullanıcıyı adresini
/// yeniden yazmaya zorlardı.
///
/// Birinci adımın cevabı **her zaman aynıdır**: kayıtlı olmayan bir adres de
/// "kod gönderildi" der. Sunucu hangi adresin hesabı olduğunu söylemiyor;
/// ekran da söylemez.
class PasswordResetPanel extends StatefulWidget {
  const PasswordResetPanel({
    required this.emailController,
    required this.onRequestCode,
    required this.onReset,
    required this.onCompleted,
    required this.onBackToLogin,
    this.active = true,
    super.key,
  });

  final TextEditingController emailController;
  final Future<void> Function(String email) onRequestCode;
  final Future<void> Function({
    required String email,
    required String code,
    required String newPassword,
  })
  onReset;

  /// Sıfırlama bittiğinde çağrılır; kullanıcı yeni parolasıyla giriş yapar.
  final void Function(String email) onCompleted;
  final VoidCallback onBackToLogin;

  /// Panel ekranda mı. Kişi panelden ayrılınca hata yazıları ve uyarı
  /// temizlenir, yazılanlar kalır: hata bir denemeye aittir, geri dönen kişi
  /// temiz bir ekran görür (kullanıcı kararı, 8 Kasım 2026).
  final bool active;

  @override
  State<PasswordResetPanel> createState() => _PasswordResetPanelState();
}

class _PasswordResetPanelState extends State<PasswordResetPanel> {
  final _code = TextEditingController();
  final _password = TextEditingController();
  final _formKey = GlobalKey<FormState>();

  bool _codeRequested = false;
  bool _busy = false;
  String? _error;
  String? _info;

  @override
  void didUpdateWidget(PasswordResetPanel oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.active && !widget.active) _error = null;
  }

  @override
  void dispose() {
    _code.dispose();
    _password.dispose();
    super.dispose();
  }

  Future<void> _requestCode() async {
    final emailError = AuthValidation.email(widget.emailController.text);
    if (emailError != null) {
      setState(() => _error = emailError);
      return;
    }
    setState(() {
      _busy = true;
      _error = null;
      _info = null;
    });
    try {
      await widget.onRequestCode(widget.emailController.text.trim());
      if (!mounted) return;
      setState(() {
        _codeRequested = true;
        // Adresin kayıtlı olup olmadığı söylenmez; cümle iki durumda da aynı.
        _info = resetCodeSent;
      });
    } on ApiException catch (error) {
      if (mounted) setState(() => _error = error.message);
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  Future<void> _reset() async {
    if (_busy) return;
    final code = _code.text.trim();
    if (code.length != 6) {
      setState(() => _error = resetCodeMissing);
      return;
    }
    final passwordError = AuthValidation.registrationPassword(_password.text);
    if (passwordError != null) {
      setState(() => _error = passwordError);
      return;
    }
    setState(() {
      _busy = true;
      _error = null;
      _info = null;
    });
    try {
      await widget.onReset(
        email: widget.emailController.text.trim(),
        code: code,
        newPassword: _password.text,
      );
      if (!mounted) return;
      final email = widget.emailController.text.trim();
      // Panel açık kalır; bir sonraki sıfırlama baştan başlar.
      setState(() {
        _codeRequested = false;
        _code.clear();
        _password.clear();
      });
      widget.onCompleted(email);
    } on ApiException catch (error) {
      if (mounted) setState(() => _error = error.message);
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final c = NomivaColors.of(context);
    final n = NomivaScale.of(context);
    return AutofillGroup(
      child: Form(
        key: _formKey,
        child: AuthPanelBody(
          gap: NomivaSpacing.gapTight,
          children: [
            // Tasarımda bağlantı 32 pt'lik bir kutuda, üstten 6 ve alttan 2 pt
            // içeri çekilmiş: yerleşimde 24 pt kaplar.
            AuthOverhangingLink(
              alignment: Alignment.topLeft,
              fontSize: 14,
              after: 7,
              child: AuthTextLink(
                label: 'Girişe dön',
                fontSize: 14,
                leading: Icon(
                  Icons.chevron_left_rounded,
                  size: n(20),
                  color: c.text,
                ),
                onPressed: _busy ? null : widget.onBackToLogin,
              ),
            ),
            const AuthTitle('Parolanı sıfırla'),
            AuthField(
              label: 'E-posta',
              controller: widget.emailController,
              icon: AuthFieldIcon.mail,
              enabled: !_codeRequested,
              hint: 'ornek@isletme.com',
              keyboardType: TextInputType.emailAddress,
              autofillHints: const [AutofillHints.email],
              textInputAction: TextInputAction.next,
            ),
            if (_codeRequested) ...[
              AuthField(
                label: 'Doğrulama kodu',
                controller: _code,
                icon: AuthFieldIcon.code,
                keyboardType: TextInputType.number,
                autofillHints: const [AutofillHints.oneTimeCode],
                maxLength: 6,
                textInputAction: TextInputAction.next,
              ),
              AuthField(
                label: 'Yeni parola',
                controller: _password,
                icon: AuthFieldIcon.lock,
                obscurable: true,
                autofillHints: const [AutofillHints.newPassword],
                textInputAction: TextInputAction.done,
                onFieldSubmitted: (_) => _reset(),
              ),
            ],
            if (_info case final message?)
              Semantics(
                liveRegion: true,
                child: Text(
                  message,
                  style: TextStyle(
                    fontFamily: nomivaFont,
                    height: nomivaLineHeight,
                    letterSpacing: nomivaLetterSpacing,
                    fontSize: n(13),
                    color: c.muted,
                  ),
                ),
              ),
            if (_error case final message?) AuthBanner(message),
            Padding(
              padding: EdgeInsets.only(top: n(NomivaSpacing.beforeButton)),
              child: AuthPrimaryButton(
                label: _codeRequested ? 'Parolamı değiştir' : 'Kod gönder',
                busy: _busy,
                onPressed: _codeRequested ? _reset : _requestCode,
              ),
            ),
            if (_codeRequested)
              Center(
                child: AuthTextLink(
                  label: 'Kodu yeniden gönder',
                  onPressed: _busy ? null : _requestCode,
                ),
              ),
          ],
        ),
      ),
    );
  }
}
