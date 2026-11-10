import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import '../theme/app_radius.dart';
import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import '../theme/app_typography.dart';

/// E-postayla gelen kısa kodun alanı: her rakam kendi kutusunda.
///
/// Kutular yalnız çizimdir; yazılan şey tek bir görünmez `TextField`'dadır.
/// Böylece yapıştırma, silme, klavyenin "tek kullanımlık kod" önerisi ve ekran
/// okuyucu tek bir alanla çalışır; altı ayrı alan arasında odak taşımak
/// gerekmez. Rakam dışındaki karakterler (kodu `123 456` diye yapıştıran
/// kullanıcı) atılır.
class AppCodeInput extends StatefulWidget {
  const AppCodeInput({
    required this.controller,
    super.key,
    this.length = 6,
    this.hasError = false,
    this.enabled = true,
    this.autofocus = true,
    this.onChanged,
    this.onCompleted,
    this.semanticLabel = 'Doğrulama kodu',
  });

  final TextEditingController controller;
  final int length;

  /// Kod reddedildi: kutuların çerçevesi hata rengine döner.
  final bool hasError;
  final bool enabled;
  final bool autofocus;
  final ValueChanged<String>? onChanged;

  /// Son rakam yazıldığında bir kez çağrılır; kullanıcı ayrıca düğmeye
  /// basmak zorunda kalmaz.
  final ValueChanged<String>? onCompleted;
  final String semanticLabel;

  @override
  State<AppCodeInput> createState() => _AppCodeInputState();
}

class _AppCodeInputState extends State<AppCodeInput> {
  final _focus = FocusNode();

  @override
  void initState() {
    super.initState();
    _focus.addListener(_redraw);
    widget.controller.addListener(_redraw);
  }

  @override
  void didUpdateWidget(AppCodeInput oldWidget) {
    super.didUpdateWidget(oldWidget);
    if (oldWidget.controller != widget.controller) {
      oldWidget.controller.removeListener(_redraw);
      widget.controller.addListener(_redraw);
    }
  }

  @override
  void dispose() {
    widget.controller.removeListener(_redraw);
    _focus.dispose();
    super.dispose();
  }

  void _redraw() {
    if (mounted) setState(() {});
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final scheme = theme.colorScheme;
    final surfaces = AppSurfaces.of(context);
    final text = widget.controller.text;
    final active = text.length.clamp(0, widget.length - 1);
    final digitStyle = theme.textTheme.titleLarge?.copyWith(
      fontSize: AppTextSizes.metric,
      fontWeight: FontWeight.w600,
      fontFeatures: AppTypography.tabularFigures,
      color: surfaces.ink,
    );

    Widget box(int index) {
      final focused = _focus.hasFocus && widget.enabled && index == active;
      final color = widget.hasError
          ? scheme.error
          : focused
          ? scheme.primary
          : surfaces.borderStrong;
      return Expanded(
        child: Container(
          height: _boxHeight,
          alignment: Alignment.center,
          decoration: BoxDecoration(
            color: surfaces.card,
            borderRadius: BorderRadius.circular(AppRadius.inner),
            border: Border.all(color: color, width: focused ? 2 : 1),
          ),
          child: Text(
            index < text.length ? text[index] : '',
            style: digitStyle,
          ),
        ),
      );
    }

    return Stack(
      children: [
        ExcludeSemantics(
          child: Row(
            children: [
              for (var i = 0; i < widget.length; i++) ...[
                if (i > 0) const SizedBox(width: AppSpacing.small),
                box(i),
              ],
            ],
          ),
        ),
        Positioned.fill(
          child: Semantics(
            label: widget.semanticLabel,
            child: TextField(
              controller: widget.controller,
              focusNode: _focus,
              enabled: widget.enabled,
              autofocus: widget.autofocus,
              keyboardType: TextInputType.number,
              autofillHints: const [AutofillHints.oneTimeCode],
              inputFormatters: [
                FilteringTextInputFormatter.digitsOnly,
                LengthLimitingTextInputFormatter(widget.length),
              ],
              showCursor: false,
              // Görünmez alan: kutular çizer, alan yalnız dokunuşu ve
              // klavyeyi alır. Yazı rengi saydam, çerçeve yok.
              style: const TextStyle(color: Colors.transparent),
              decoration: const InputDecoration(
                filled: false,
                border: InputBorder.none,
                enabledBorder: InputBorder.none,
                focusedBorder: InputBorder.none,
                disabledBorder: InputBorder.none,
                counterText: '',
                contentPadding: EdgeInsets.zero,
              ),
              onChanged: (value) {
                widget.onChanged?.call(value);
                if (value.length == widget.length) {
                  widget.onCompleted?.call(value);
                }
              },
            ),
          ),
        ),
      ],
    );
  }

  /// Kutu yüksekliği düğme boyudur; yeni bir ölçü adı açılmadı.
  static const double _boxHeight = AppSizes.button;
}
