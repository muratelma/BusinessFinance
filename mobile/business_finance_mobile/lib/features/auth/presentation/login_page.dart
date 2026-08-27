import 'package:flutter/material.dart';

import '../../../core/network/api_exception.dart';
import '../../../core/theme/app_spacing.dart';
import 'auth_validation.dart';
import 'widgets/password_field.dart';

typedef LoginSubmit = Future<void> Function(String email, String password);

class LoginPage extends StatefulWidget {
  const LoginPage({
    required this.onSubmit,
    required this.onRegister,
    required this.onForgotPassword,
    super.key,
    this.initialEmail,
  });

  final LoginSubmit onSubmit;
  final VoidCallback onRegister;

  /// Parolasını unutan kullanıcının kapısı. Girişin hemen altında durur:
  /// aranacağı yer, kaybedildiği yerdir.
  final void Function(String email) onForgotPassword;
  final String? initialEmail;

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _formKey = GlobalKey<FormState>();
  late final TextEditingController _emailController;
  final _passwordController = TextEditingController();
  bool _isSubmitting = false;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    _emailController = TextEditingController(text: widget.initialEmail);
  }

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
    setState(() {
      _isSubmitting = true;
      _errorMessage = null;
    });
    try {
      await widget.onSubmit(
        _emailController.text.trim(),
        _passwordController.text,
      );
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
                      Icon(
                        Icons.account_balance_wallet,
                        size: 64,
                        color: Theme.of(context).colorScheme.primary,
                        semanticLabel: 'Kişisel Bütçe',
                      ),
                      const SizedBox(height: AppSpacing.medium),
                      Text(
                        'Tekrar hoş geldiniz',
                        textAlign: TextAlign.center,
                        style: Theme.of(context).textTheme.headlineSmall,
                      ),
                      const SizedBox(height: AppSpacing.large),
                      TextFormField(
                        controller: _emailController,
                        keyboardType: TextInputType.emailAddress,
                        autofillHints: const [AutofillHints.email],
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
                        validator: AuthValidation.loginPassword,
                        textInputAction: TextInputAction.done,
                        onFieldSubmitted: (_) => _submit(),
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
                                child: CircularProgressIndicator(
                                  strokeWidth: 2,
                                ),
                              )
                            : const Text('Giriş yap'),
                      ),
                      TextButton(
                        onPressed: _isSubmitting
                            ? null
                            : () => widget.onForgotPassword(
                                _emailController.text.trim(),
                              ),
                        child: const Text('Parolamı unuttum'),
                      ),
                      TextButton(
                        onPressed: _isSubmitting ? null : widget.onRegister,
                        child: const Text('Hesap oluştur'),
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
}
