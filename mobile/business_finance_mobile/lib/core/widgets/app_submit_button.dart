import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';

/// Gönderim butonu: istek uçarken ikinci dokunuşu yutar.
///
/// Sunucuda idempotency anahtarı yok; ikinci dokunuş ikinci yazma olurdu. Bu
/// kilit daha önce her formda elle tekrarlanıyordu ve her yeni formda yeniden
/// unutulabilir bir adımdı. `isBusy` verilirse durum çağıranın (controller'ın)
/// bildiği hâldir; verilmezse buton kendi gönderimini izler.
class AppSubmitButton extends StatefulWidget {
  const AppSubmitButton({
    required this.label,
    required this.onSubmit,
    super.key,
    this.icon,
    this.isBusy,
    this.style = AppSubmitButtonStyle.filled,
  });

  final String label;

  /// Null ise buton görünür ama devre dışıdır (koşul sağlanmadı).
  final Future<void> Function()? onSubmit;

  final IconData? icon;

  /// Dışarıdan yönetilen meşguliyet. Null ise buton kendi durumunu tutar.
  final bool? isBusy;

  final AppSubmitButtonStyle style;

  @override
  State<AppSubmitButton> createState() => _AppSubmitButtonState();
}

enum AppSubmitButtonStyle { filled, tonal }

class _AppSubmitButtonState extends State<AppSubmitButton> {
  bool _submitting = false;

  bool get _busy => widget.isBusy ?? _submitting;

  Future<void> _handlePress() async {
    if (_busy || widget.onSubmit == null) return;
    if (widget.isBusy == null) setState(() => _submitting = true);
    try {
      await widget.onSubmit!();
    } catch (error, stack) {
      // `onSubmit` hatasını kendi yönetmelidir (controller error mesajını
      // ekrana yazar). Yönetmediyse hata buradan kaçıp beklenmeyen bir async
      // hata olur: `onPressed` senkrondur ve dönen future'ı kimse beklemez.
      // Sessizce yutmak hatayı görünmez yapardı, bu yüzden bildiriliyor.
      FlutterError.reportError(
        FlutterErrorDetails(
          exception: error,
          stack: stack,
          library: 'app_submit_button',
          context: ErrorDescription('AppSubmitButton "${widget.label}" için'),
        ),
      );
    } finally {
      // Kilit her koşulda açılır; bir ağ hatası butonu kalıcı olarak ölü
      // bırakmamalı.
      if (widget.isBusy == null && mounted) {
        setState(() => _submitting = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final enabled = widget.onSubmit != null && !_busy;
    final icon = _busy
        ? const SizedBox.square(
            dimension: 16,
            child: CircularProgressIndicator(strokeWidth: 2),
          )
        : (widget.icon == null ? null : Icon(widget.icon));

    final child = Row(
      mainAxisSize: MainAxisSize.min,
      children: [
        if (icon != null) ...[icon, const SizedBox(width: AppSpacing.small)],
        Flexible(child: Text(widget.label)),
      ],
    );

    final onPressed = enabled ? _handlePress : null;
    return switch (widget.style) {
      AppSubmitButtonStyle.filled => FilledButton(
        onPressed: onPressed,
        child: child,
      ),
      AppSubmitButtonStyle.tonal => FilledButton.tonal(
        onPressed: onPressed,
        child: child,
      ),
    };
  }
}
