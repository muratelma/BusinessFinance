import 'package:flutter/material.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/theme/app_spacing.dart';
import 'auth_validation.dart';
import 'widgets/password_field.dart';

typedef RegisterSubmit =
    Future<void> Function(
      String email,
      String password, {
      required bool hasBusiness,
    });
typedef RegistrationCompleted = void Function(String email);

class RegisterPage extends StatefulWidget {
  const RegisterPage({
    required this.onSubmit,
    required this.onCompleted,
    required this.onBackToLogin,
    super.key,
  });

  final RegisterSubmit onSubmit;
  final RegistrationCompleted onCompleted;
  final VoidCallback onBackToLogin;

  @override
  State<RegisterPage> createState() => _RegisterPageState();
}

class _RegisterPageState extends State<RegisterPage> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  bool _isSubmitting = false;
  String? _errorMessage;

  /// Onboarding'in tek sorusu. Varsayılan **hayır**: soru sorulmadan
  /// işletme sahibi varsaymak, esnaf olmayan kullanıcıyı hiç kullanmayacağı
  /// otuz işletme kategorisiyle karşılardı.
  bool _hasBusiness = false;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (_isSubmitting || !_formKey.currentState!.validate()) {
      return;
    }
    final email = _emailController.text.trim();
    setState(() {
      _isSubmitting = true;
      _errorMessage = null;
    });
    try {
      await widget.onSubmit(
        email,
        _passwordController.text,
        hasBusiness: _hasBusiness,
      );
      if (mounted) {
        widget.onCompleted(email);
      }
    } on ApiException catch (error) {
      if (mounted) {
        setState(() => _errorMessage = error.message);
      }
    } finally {
      if (mounted) {
        setState(() => _isSubmitting = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Hesap oluştur')),
      body: SafeArea(
        child: Center(
          child: SingleChildScrollView(
            padding: const EdgeInsets.all(AppSpacing.large),
            child: ConstrainedBox(
              constraints: const BoxConstraints(maxWidth: 440),
              child: Form(
                key: _formKey,
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    TextFormField(
                      controller: _emailController,
                      keyboardType: TextInputType.emailAddress,
                      autofillHints: const [AutofillHints.newUsername],
                      textInputAction: TextInputAction.next,
                      validator: AuthValidation.email,
                      decoration: const InputDecoration(
                        labelText: 'E-posta',
                        prefixIcon: Icon(Icons.email_outlined),
                      ),
                    ),
                    const SizedBox(height: AppSpacing.medium),
                    PasswordField(
                      controller: _passwordController,
                      validator: AuthValidation.registrationPassword,
                      textInputAction: TextInputAction.done,
                      onFieldSubmitted: (_) => _submit(),
                    ),
                    const SizedBox(height: AppSpacing.small),
                    const Text(
                      'En az 12 karakter; büyük ve küçük harf, rakam ve özel karakter kullanın.',
                    ),
                    const SizedBox(height: AppSpacing.medium),
                    // Cevap hiçbir özelliği kapatmaz: yalnız hangi kategori
                    // setiyle başlanacağını ve işletme/şahsi ayrımının
                    // arayüzde görünüp görünmeyeceğini belirler.
                    SwitchListTile(
                      value: _hasBusiness,
                      onChanged: _isSubmitting
                          ? null
                          : (value) => setState(() => _hasBusiness = value),
                      contentPadding: EdgeInsets.zero,
                      title: const Text('İşletmem var'),
                      subtitle: const Text(
                        'Esnaf ya da şahıs şirketiyseniz işletme '
                        'kategorileriyle başlarsınız ve kayıtlarınızı işletme '
                        'ile şahsi olarak ayrı okuyabilirsiniz. Gündelik '
                        'harcamalarınız yine aynı uygulamada durur.',
                      ),
                    ),
                    if (_errorMessage case final message?) ...[
                      const SizedBox(height: AppSpacing.medium),
                      Semantics(
                        liveRegion: true,
                        child: Text(
                          message,
                          style: TextStyle(
                            color: Theme.of(context).colorScheme.error,
                          ),
                        ),
                      ),
                    ],
                    const SizedBox(height: AppSpacing.large),
                    FilledButton(
                      onPressed: _isSubmitting ? null : _submit,
                      child: _isSubmitting
                          ? const SizedBox.square(
                              dimension: 20,
                              child: CircularProgressIndicator(strokeWidth: 2),
                            )
                          : const Text('Kayıt ol'),
                    ),
                    TextButton(
                      onPressed: _isSubmitting ? null : widget.onBackToLogin,
                      child: const Text('Giriş ekranına dön'),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
