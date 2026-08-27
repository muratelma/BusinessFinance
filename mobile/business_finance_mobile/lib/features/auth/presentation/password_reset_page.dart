import 'package:flutter/material.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/theme/app_spacing.dart';
import 'auth_validation.dart';
import 'widgets/password_field.dart';

/// Parolasını unutan kullanıcının kendi başına geri alması.
///
/// Tek sayfa, iki adım: adres yazılır ve kod istenir; sonra kod ile yeni parola
/// yazılır. İki ayrı ekran yapmak, gelen kodu okuyup geri dönen kullanıcıyı
/// adresini yeniden yazmaya zorlardı.
///
/// Birinci adımın cevabı **her zaman aynıdır**: kayıtlı olmayan bir adres de
/// "kod gönderildi" der. Sunucu hangi adresin hesabı olduğunu söylemiyor;
/// ekran da söylemez.
class PasswordResetPage extends StatefulWidget {
  const PasswordResetPage({
    required this.onRequestCode,
    required this.onReset,
    required this.onCompleted,
    required this.onBackToLogin,
    super.key,
    this.initialEmail,
  });

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
  final String? initialEmail;

  @override
  State<PasswordResetPage> createState() => _PasswordResetPageState();
}

class _PasswordResetPageState extends State<PasswordResetPage> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _emailController;
  final _codeController = TextEditingController();
  final _passwordController = TextEditingController();

  bool _codeRequested = false;
  bool _isSubmitting = false;
  String? _errorMessage;
  String? _infoMessage;

  @override
  void initState() {
    super.initState();
    _emailController = TextEditingController(text: widget.initialEmail);
  }

  @override
  void dispose() {
    _emailController.dispose();
    _codeController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Scaffold(
      appBar: AppBar(title: const Text('Parolamı unuttum')),
      body: SafeArea(
        child: Center(
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(AppSpacing.large),
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 440),
              child: AutofillGroup(
                child: Form(
                  key: _formKey,
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.stretch,
                    children: [
                      Text(
                        'Adresinize altı haneli bir kod göndeririz. Kod 15 '
                        'dakika geçerlidir ve yalnız bir kez kullanılır.',
                        style: theme.textTheme.bodyMedium,
                      ),
                      const SizedBox(height: AppSpacing.large),
                      TextFormField(
                        controller: _emailController,
                        keyboardType: TextInputType.emailAddress,
                        autofillHints: const [AutofillHints.email],
                        textInputAction: TextInputAction.next,
                        validator: AuthValidation.email,
                        enabled: !_codeRequested,
                        decoration: const InputDecoration(
                          labelText: 'E-posta',
                          prefixIcon: Icon(Icons.email_outlined),
                        ),
                      ),
                      if (_codeRequested) ...[
                        const SizedBox(height: AppSpacing.medium),
                        TextFormField(
                          controller: _codeController,
                          keyboardType: TextInputType.number,
                          maxLength: 6,
                          decoration: const InputDecoration(
                            labelText: 'Doğrulama kodu',
                            prefixIcon: Icon(Icons.pin_outlined),
                            counterText: '',
                          ),
                        ),
                        const SizedBox(height: AppSpacing.small),
                        PasswordField(
                          controller: _passwordController,
                          label: 'Yeni parola',
                          validator: AuthValidation.registrationPassword,
                          textInputAction: TextInputAction.done,
                          onFieldSubmitted: (_) => _reset(),
                        ),
                      ],
                      if (_infoMessage case final message?) ...[
                        const SizedBox(height: AppSpacing.medium),
                        Semantics(
                          liveRegion: true,
                          child: Text(
                            message,
                            style: theme.textTheme.bodyMedium,
                          ),
                        ),
                      ],
                      if (_errorMessage case final message?) ...[
                        const SizedBox(height: AppSpacing.medium),
                        Semantics(
                          liveRegion: true,
                          child: Text(
                            message,
                            style: TextStyle(color: theme.colorScheme.error),
                          ),
                        ),
                      ],
                      const SizedBox(height: AppSpacing.large),
                      FilledButton(
                        onPressed: _isSubmitting
                            ? null
                            : (_codeRequested ? _reset : _requestCode),
                        child: _isSubmitting
                            ? const SizedBox.square(
                                dimension: 20,
                                child: CircularProgressIndicator(
                                  strokeWidth: 2,
                                ),
                              )
                            : Text(
                                _codeRequested
                                    ? 'Parolamı değiştir'
                                    : 'Kod gönder',
                              ),
                      ),
                      if (_codeRequested)
                        TextButton(
                          onPressed: _isSubmitting ? null : _requestCode,
                          child: const Text('Kodu yeniden gönder'),
                        ),
                      TextButton(
                        onPressed: _isSubmitting ? null : widget.onBackToLogin,
                        child: const Text('Girişe dön'),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Future<void> _requestCode() async {
    final emailError = AuthValidation.email(_emailController.text);
    if (emailError != null) {
      setState(() => _errorMessage = emailError);
      return;
    }

    setState(() {
      _isSubmitting = true;
      _errorMessage = null;
      _infoMessage = null;
    });
    try {
      await widget.onRequestCode(_emailController.text.trim());
      if (!mounted) return;
      setState(() {
        _codeRequested = true;
        // Adresin kayıtlı olup olmadığı söylenmez; cümle iki durumda da aynı.
        _infoMessage =
            'Adres kayıtlıysa kod gönderildi. Gelen kutunuzu kontrol edin.';
      });
    } on ApiException catch (error) {
      if (mounted) setState(() => _errorMessage = error.message);
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }

  Future<void> _reset() async {
    if (_isSubmitting) return;
    final code = _codeController.text.trim();
    if (code.length != 6) {
      setState(() => _errorMessage = 'Altı haneli kodu yazın.');
      return;
    }
    final passwordError = AuthValidation.registrationPassword(
      _passwordController.text,
    );
    if (passwordError != null) {
      setState(() => _errorMessage = passwordError);
      return;
    }

    setState(() {
      _isSubmitting = true;
      _errorMessage = null;
      _infoMessage = null;
    });
    try {
      await widget.onReset(
        email: _emailController.text.trim(),
        code: code,
        newPassword: _passwordController.text,
      );
      if (mounted) widget.onCompleted(_emailController.text.trim());
    } on ApiException catch (error) {
      if (mounted) setState(() => _errorMessage = error.message);
    } finally {
      if (mounted) setState(() => _isSubmitting = false);
    }
  }
}
