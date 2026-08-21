abstract final class AuthValidation {
  static String? email(String? value) {
    final email = value?.trim() ?? '';
    if (email.isEmpty) {
      return 'E-posta adresinizi girin.';
    }
    final at = email.indexOf('@');
    if (at <= 0 ||
        at == email.length - 1 ||
        !email.substring(at + 1).contains('.')) {
      return 'Geçerli bir e-posta adresi girin.';
    }
    return null;
  }

  static String? loginPassword(String? value) {
    if (value == null || value.isEmpty) {
      return 'Parolanızı girin.';
    }
    return null;
  }

  static String? registrationPassword(String? value) {
    final password = value ?? '';
    if (password.length < 12) {
      return 'Parola en az 12 karakter olmalıdır.';
    }
    if (!password.contains(RegExp('[a-z]')) ||
        !password.contains(RegExp('[A-Z]')) ||
        !password.contains(RegExp('[0-9]')) ||
        !password.contains(RegExp(r'[^a-zA-Z0-9]'))) {
      return 'Büyük/küçük harf, rakam ve özel karakter kullanın.';
    }
    return null;
  }
}
