import 'package:flutter/material.dart';

class PasswordField extends StatefulWidget {
  const PasswordField({
    required this.controller,
    required this.validator,
    required this.textInputAction,
    super.key,
    this.onFieldSubmitted,
    this.label = 'Parola',
  });

  final TextEditingController controller;
  final FormFieldValidator<String> validator;
  final TextInputAction textInputAction;
  final ValueChanged<String>? onFieldSubmitted;

  /// Sıfırlama akışında alan yeni parolayı ister; etiketi "Parola" bırakmak
  /// kullanıcıya mevcut parolasını soruyormuş gibi görünürdü.
  final String label;

  @override
  State<PasswordField> createState() => _PasswordFieldState();
}

class _PasswordFieldState extends State<PasswordField> {
  bool _obscureText = true;

  @override
  Widget build(BuildContext context) {
    return TextFormField(
      controller: widget.controller,
      obscureText: _obscureText,
      autofillHints: const [AutofillHints.password],
      textInputAction: widget.textInputAction,
      validator: widget.validator,
      onFieldSubmitted: widget.onFieldSubmitted,
      decoration: InputDecoration(
        labelText: widget.label,
        prefixIcon: const Icon(Icons.lock_outline),
        suffixIcon: IconButton(
          tooltip: _obscureText ? 'Parolayı göster' : 'Parolayı gizle',
          onPressed: () => setState(() => _obscureText = !_obscureText),
          icon: Icon(_obscureText ? Icons.visibility : Icons.visibility_off),
        ),
      ),
    );
  }
}
