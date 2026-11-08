import 'dart:async';
import 'dart:math' as math;

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/flutter_svg.dart';

import '../nomiva_brand.dart';

/// Giriş akışının ortak parçaları: alan, ana düğme, metin bağlantısı, uyarı,
/// "ya da" ayırıcısı ve Google / Apple düğmeleri.
///
/// Bütün ölçüler tasarım birimindedir ve [NomivaScale] ile ekrana çevrilir.
/// Tek alt sınır dokunma hedefidir: ölçek küçük bir ekranda 48 pt'nin altına
/// inerse alan ve düğme 48'de kalır.

enum AuthFieldIcon { person, mail, lock, code }

const _iconPaths = {
  AuthFieldIcon.person:
      '<circle cx="12" cy="8" r="4.2"/>'
      '<path d="M3.8 20.5c0-4.3 3.7-7 8.2-7s8.2 2.7 8.2 7z"/>',
  AuthFieldIcon.mail:
      '<path d="M4.2 5h15.6c.7 0 1.2.5 1.2 1.2v.3l-9 5.7-9-5.7v-.3C3 5.5 3.5 '
      '5 4.2 5zM3 8.9l9 5.7 9-5.7v8.9c0 .7-.5 1.2-1.2 1.2H4.2c-.7 0-1.2-.5-'
      '1.2-1.2z"/>',
  AuthFieldIcon.lock:
      '<path d="M7 10V7.5a5 5 0 0 1 10 0V10h.8A2.2 2.2 0 0 1 20 12.2v7.6A2.2 '
      '2.2 0 0 1 17.8 22H6.2A2.2 2.2 0 0 1 4 19.8v-7.6A2.2 2.2 0 0 1 6.2 10z'
      'm2.4 0h5.2V7.5a2.6 2.6 0 0 0-5.2 0z"/>',
  AuthFieldIcon.code:
      '<path d="M5 4h14a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V6a2 2 '
      '0 0 1 2-2zm2 6.5a1.5 1.5 0 1 0 0 3 1.5 1.5 0 0 0 0-3zm5 0a1.5 1.5 0 1 '
      '0 0 3 1.5 1.5 0 0 0 0-3zm5 0a1.5 1.5 0 1 0 0 3 1.5 1.5 0 0 0 0-3z"/>',
};

/// Dokunma hedefinin alt sınırı.
const double _minTarget = kMinInteractiveDimension;

/// Figtree'nin "normal" satır yüksekliği (yazı boyunun katı); prototipteki
/// CSS de bunu kullanır.
const double _lineHeight = nomivaLineHeight;

TextStyle _label(NomivaColors c, NomivaScale n) => TextStyle(
  fontFamily: nomivaFont,
  height: nomivaLineHeight,
  letterSpacing: nomivaLetterSpacing,
  fontSize: n(12),
  fontWeight: FontWeight.w500,
  color: c.muted,
);

/// Etiketi üstte, simgesi solda duran dolu alan. Hata gelince kenarı ve zemini
/// kırmızıya döner; kural yalnız o zaman alanın altında yazar.
///
/// Hata zamanlaması mobil formların yaygın kuralıdır ("reward early, punish
/// late"; kullanıcı kararı, 8 Kasım 2026): yazarken denetlenmez; kişi
/// alandan çıkınca ya da düğmeye basınca denetlenir. Hata görünürken
/// yazılan değer kurala uyduğu anda yazı kalkar; uymuyorsa aynı yazı durur,
/// yazarken başka bir hataya dönmez.
class AuthField extends StatefulWidget {
  const AuthField({
    required this.label,
    required this.controller,
    required this.icon,
    super.key,
    this.validator,
    this.keyboardType,
    this.autofillHints,
    this.textInputAction,
    this.onFieldSubmitted,
    this.enabled = true,
    this.obscurable = false,
    this.maxLength,
    this.remoteValidator,
    this.hint,
  });

  final String label;

  /// Boş alanda görünen örnek ("ornek@isletme.com").
  final String? hint;
  final TextEditingController controller;
  final AuthFieldIcon icon;
  final FormFieldValidator<String>? validator;
  final TextInputType? keyboardType;
  final Iterable<String>? autofillHints;
  final TextInputAction? textInputAction;
  final ValueChanged<String>? onFieldSubmitted;
  final bool enabled;

  /// Parola alanı: göz simgesiyle gösterilip gizlenir.
  final bool obscurable;
  final int? maxLength;

  /// Sunucuya sorulan denetim ("Bu kullanıcı adı alınmış"). Yerel kural
  /// tutunca, alandan çıkınca ve gönderirken ([AuthFieldState.checkRemote])
  /// sorulur; her harfte sorulmaz. Hata metni ya da null döner; sunucuya
  /// ulaşılamazsa null döndürmelidir (kişiyi bekletmez, kayıt kendisi reddeder).
  final Future<String?> Function(String value)? remoteValidator;

  @override
  State<AuthField> createState() => AuthFieldState();
}

class AuthFieldState extends State<AuthField> {
  var _field = GlobalKey<FormFieldState<String>>();
  final _focus = FocusNode();
  bool _obscured = true;

  /// Alanın altında şu an bir hata yazıyor mu.
  bool get hasError => _field.currentState?.hasError ?? false;

  /// Sunucunun söylediği hata ve hangi değer için söylediği. Değer değişince
  /// hata o değere ait olmadığı için düşer.
  String? _remoteError;
  String? _remoteErrorFor;

  String? _validate(String? value) {
    final local = widget.validator?.call(value);
    if (local != null) return local;
    if (_remoteError != null && value?.trim() == _remoteErrorFor) {
      return _remoteError;
    }
    return null;
  }

  /// Yerel kural tutuyorsa sunucuya sorar ve cevabı alanın altına yazar.
  /// Alan geçerliyse `true` döner. Sorulan değer cevap gelmeden değiştiyse
  /// cevap atılır.
  Future<bool> checkRemote() async {
    final value = widget.controller.text.trim();
    final remote = widget.remoteValidator;
    if (widget.validator?.call(value) != null) return false;
    if (remote == null) return true;
    final message = await remote(value);
    if (!mounted || widget.controller.text.trim() != value) {
      return message == null;
    }
    _remoteError = message;
    _remoteErrorFor = value;
    _field.currentState?.validate();
    return message == null;
  }

  @override
  void initState() {
    super.initState();
    _focus.addListener(_onFocusChange);
  }

  @override
  void dispose() {
    _focus
      ..removeListener(_onFocusChange)
      ..dispose();
    super.dispose();
  }

  /// Alandan çıkınca denetlenir; boş alan denetlenmez (yalnız geçip giden
  /// kişiye "girin" denmez, düğmeye basınca söylenir).
  void _onFocusChange() {
    if (_focus.hasFocus || widget.validator == null) return;
    if (widget.controller.text.trim().isEmpty) return;
    final valid = _field.currentState?.validate() ?? false;
    if (valid && widget.remoteValidator != null) {
      unawaited(checkRemote());
    }
  }

  /// Hata görünürken yazılan değer kurala uyunca yazı hemen kalkar.
  void _onChanged(String value) {
    if (!hasError || _validate(value) != null) return;
    _field.currentState?.validate();
  }

  /// Hata yazısını kaldırır, yazılanı korur. Alanın denetim durumu yenilenir;
  /// metin dışarıdaki denetleyicide durduğu için kaybolmaz.
  void clearError() {
    if (!hasError) return;
    setState(() => _field = GlobalKey<FormFieldState<String>>());
  }

  /// Gönderim reddedilince bu alana gidilir: ekrana kaydırılır ve imleç
  /// buraya gelir.
  void reveal() {
    _focus.requestFocus();
    Scrollable.ensureVisible(
      context,
      alignment: .2,
      duration: MediaQuery.disableAnimationsOf(context)
          ? Duration.zero
          : const Duration(milliseconds: 250),
      curve: Curves.easeOutCubic,
    );
  }

  @override
  Widget build(BuildContext context) {
    final c = NomivaColors.of(context);
    final n = NomivaScale.of(context);
    final iconSize = n(NomivaSpacing.fieldIcon);
    // Alanın yüksekliği iç boşluktan gelir: InputDecoration.constraints
    // kutuyu büyütür ama dolguyu büyütmez, fazlası alanın altında boş kalır.
    final textLine =
        MediaQuery.textScalerOf(context).scale(n(14)) * _lineHeight;
    final height = math.max(n(NomivaSpacing.fieldHeight), _minTarget);
    final padY = math.max(0.0, (height - textLine) / 2);
    OutlineInputBorder border(Color color) => OutlineInputBorder(
      borderRadius: BorderRadius.circular(n(NomivaSpacing.radius)),
      borderSide: BorderSide(color: color, width: 1.5),
    );
    // Etiket alanın adıdır: ekran okuyucu ikisini tek öğe olarak okur.
    return MergeSemantics(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        spacing: n(NomivaSpacing.labelGap),
        children: [
          Text(widget.label, style: _label(c, n)),
          // Hata yazısı çıkınca ya da kalkınca alan boyunu yumuşakça
          // değiştirir; altındaki alanlar ve düğme sıçramaz.
          AnimatedSize(
            duration: MediaQuery.disableAnimationsOf(context)
                ? Duration.zero
                : const Duration(milliseconds: 200),
            curve: Curves.easeOutCubic,
            alignment: Alignment.topCenter,
            child: TextFormField(
              key: _field,
              controller: widget.controller,
              focusNode: _focus,
              onChanged: _onChanged,
              // Klavye açılınca alan klavyenin üstüne kaydırılırken altındaki
              // hata yazısı da görünür kalır.
              scrollPadding: EdgeInsets.only(
                top: n(NomivaSpacing.side),
                bottom: n(NomivaSpacing.keyboardClearance),
              ),
              enabled: widget.enabled,
              validator: _validate,
              keyboardType: widget.keyboardType,
              autofillHints: widget.autofillHints,
              textInputAction: widget.textInputAction,
              onFieldSubmitted: widget.onFieldSubmitted,
              obscureText: widget.obscurable && _obscured,
              maxLength: widget.maxLength,
              maxLengthEnforcement: MaxLengthEnforcement.enforced,
              // Sayaç çizilmez: boş bir sayaç bile alanın altında bir satır yer
              // ayırır ve alanlar tasarımdakinden aralıklı dururdu.
              buildCounter:
                  (
                    _, {
                    required currentLength,
                    required isFocused,
                    maxLength,
                  }) => null,
              cursorColor: c.text,
              // Kenarlık çerçeveli olduğu için yazı alanın dikey ortasındadır.
              textAlignVertical: TextAlignVertical.center,
              style: TextStyle(
                fontFamily: nomivaFont,
                fontSize: n(14),
                height: _lineHeight,
                letterSpacing: nomivaLetterSpacing,
                color: c.text,
              ),
              decoration: InputDecoration(
                hintText: widget.hint,
                hintStyle: TextStyle(
                  fontFamily: nomivaFont,
                  height: nomivaLineHeight,
                  letterSpacing: nomivaLetterSpacing,
                  fontSize: n(14),
                  color: c.fieldIcon,
                ),
                isDense: true,
                filled: true,
                fillColor: WidgetStateColor.resolveWith(
                  (states) => states.contains(WidgetState.error)
                      ? c.errorFill
                      : c.field,
                ),
                // Baş taraftaki boşluğu simge kendisi taşır; içerik boşluğu
                // başta sıfırdır ki hata yazısı alanın kenarından başlasın.
                contentPadding: EdgeInsetsDirectional.only(
                  end: n(NomivaSpacing.fieldInset),
                  top: padY,
                  bottom: padY,
                ),
                prefixIcon: Padding(
                  padding: EdgeInsets.only(
                    left: n(NomivaSpacing.fieldInset),
                    right: n(NomivaSpacing.iconGap),
                  ),
                  child: _svgIcon(widget.icon, c.fieldIcon, iconSize),
                ),
                prefixIconConstraints: BoxConstraints(
                  minWidth:
                      n(NomivaSpacing.fieldInset) +
                      iconSize +
                      n(NomivaSpacing.iconGap),
                  minHeight: iconSize,
                ),
                suffixIcon: widget.obscurable
                    ? IconButton(
                        tooltip: _obscured
                            ? 'Parolayı göster'
                            : 'Parolayı gizle',
                        onPressed: () => setState(() => _obscured = !_obscured),
                        icon: Icon(
                          _obscured
                              ? Icons.visibility_outlined
                              : Icons.visibility_off_outlined,
                          size: n(20),
                          color: c.fieldIcon,
                        ),
                      )
                    : null,
                border: border(Colors.transparent),
                enabledBorder: border(Colors.transparent),
                disabledBorder: border(Colors.transparent),
                focusedBorder: border(c.fieldIcon),
                errorBorder: border(c.error),
                focusedErrorBorder: border(c.error),
                errorStyle: TextStyle(
                  fontFamily: nomivaFont,
                  height: nomivaLineHeight,
                  letterSpacing: nomivaLetterSpacing,
                  fontSize: n(11.5),
                  fontWeight: FontWeight.w500,
                  color: c.error,
                ),
                errorMaxLines: 3,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

/// Panelden ayrılırken alanların hata yazılarını kaldırır.
void clearErrors(Iterable<GlobalKey<AuthFieldState>> fields) {
  for (final key in fields) {
    key.currentState?.clearError();
  }
}

/// Gönderim reddedilince ilk hatalı alana gider (sırayla verilen alanlar).
void revealFirstError(Iterable<GlobalKey<AuthFieldState>> fields) {
  for (final key in fields) {
    final field = key.currentState;
    if (field != null && field.hasError) {
      field.reveal();
      return;
    }
  }
}

Widget _svgIcon(AuthFieldIcon icon, Color color, double size) =>
    SvgPicture.string(
      '<svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">'
      '${_iconPaths[icon]}</svg>',
      width: size,
      height: size,
      colorFilter: ColorFilter.mode(color, BlendMode.srcIn),
    );

/// Ana düğme. Beklerken daralıp dönen göstergeye dönüşür; cevap gelince geri
/// açılır.
class AuthPrimaryButton extends StatelessWidget {
  const AuthPrimaryButton({
    required this.label,
    required this.onPressed,
    super.key,
    this.busy = false,
  });

  final String label;
  final VoidCallback? onPressed;
  final bool busy;

  static const _morph = Cubic(.4, 0, .2, 1);

  @override
  Widget build(BuildContext context) {
    final c = NomivaColors.of(context);
    final n = NomivaScale.of(context);
    final height = math.max(n(NomivaSpacing.buttonHeight), _minTarget);
    final reduce = MediaQuery.disableAnimationsOf(context);
    return LayoutBuilder(
      builder: (context, box) => Center(
        child: AnimatedContainer(
          duration: reduce ? Duration.zero : const Duration(milliseconds: 300),
          curve: _morph,
          width: busy ? height : box.maxWidth,
          height: height,
          child: FilledButton(
            onPressed: busy ? null : onPressed,
            style: FilledButton.styleFrom(
              backgroundColor: c.button,
              foregroundColor: c.onButton,
              disabledBackgroundColor: c.button,
              disabledForegroundColor: c.onButton,
              shape: const StadiumBorder(),
              padding: EdgeInsets.zero,
              minimumSize: Size.square(height),
              textStyle: TextStyle(
                fontFamily: nomivaFont,
                height: nomivaLineHeight,
                letterSpacing: nomivaLetterSpacing,
                fontSize: n(15),
                fontWeight: FontWeight.w600,
              ),
            ),
            child: AnimatedSwitcher(
              duration: reduce
                  ? Duration.zero
                  : const Duration(milliseconds: 150),
              child: busy
                  ? SizedBox.square(
                      key: const ValueKey('busy'),
                      dimension: n(NomivaSpacing.spinner),
                      child: CircularProgressIndicator(
                        strokeWidth: n(2.5),
                        color: c.onButton,
                        backgroundColor: c.onButton.withValues(alpha: .25),
                        semanticsLabel: 'Bekleniyor',
                      ),
                    )
                  : Text(
                      label,
                      key: const ValueKey('label'),
                      maxLines: 1,
                      overflow: TextOverflow.fade,
                      softWrap: false,
                    ),
            ),
          ),
        ),
      ),
    );
  }
}

/// Altı çizili metin bağlantısı. Görünen yazı küçüktür ama dokunma alanı 48
/// pt'dir; yanındaki boşluk bağlantı değildir.
class AuthTextLink extends StatelessWidget {
  const AuthTextLink({
    required this.label,
    required this.onPressed,
    super.key,
    this.leading,
    this.fontSize = 13,
  });

  final String label;
  final VoidCallback? onPressed;
  final Widget? leading;

  /// Tasarım biriminde.
  final double fontSize;

  @override
  Widget build(BuildContext context) {
    final c = NomivaColors.of(context);
    final n = NomivaScale.of(context);
    final text = Text(
      label,
      style: TextStyle(
        fontFamily: nomivaFont,
        height: nomivaLineHeight,
        letterSpacing: nomivaLetterSpacing,
        fontSize: n(fontSize),
        fontWeight: FontWeight.w600,
        color: c.text,
        decoration: TextDecoration.underline,
        decorationColor: c.text,
      ),
    );
    return TextButton(
      onPressed: onPressed,
      style: TextButton.styleFrom(
        foregroundColor: c.text,
        minimumSize: const Size.square(_minTarget),
        padding: EdgeInsets.zero,
        tapTargetSize: MaterialTapTargetSize.padded,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(n(8)),
        ),
      ),
      child: leading == null
          ? text
          : Row(
              mainAxisSize: MainAxisSize.min,
              spacing: n(3),
              children: [
                leading!,
                Flexible(child: text),
              ],
            ),
    );
  }
}

/// Yerleşimde yalnız yazısı kadar ([lineHeight] + [after]) yer kaplayan
/// bağlantı; 48 pt'lik dokunma alanı yazının üstüne ve altına taşar.
/// Tasarımdaki boşluklar yazıya göre çizildi; dokunma alanı yerleşime girseydi
/// alttaki her şey aşağı kayardı.
class AuthOverhangingLink extends StatelessWidget {
  const AuthOverhangingLink({
    required this.child,
    super.key,
    this.alignment = Alignment.topRight,
    this.fontSize = 13,
    this.after = 0,
  });

  final Widget child;
  final Alignment alignment;

  /// Bağlantının yazı boyu ve altındaki boşluk, tasarım biriminde.
  final double fontSize;
  final double after;

  @override
  Widget build(BuildContext context) {
    final n = NomivaScale.of(context);
    final line = MediaQuery.textScalerOf(
      context,
    ).scale(n(fontSize) * _lineHeight);
    final overhang = math.max(0, _minTarget - line) / 2;
    return SizedBox(
      height: line + n(after),
      child: OverflowBox(
        alignment: alignment,
        maxHeight: double.infinity,
        child: Transform.translate(
          offset: Offset(0, -overhang),
          child: Align(alignment: alignment, child: child),
        ),
      ),
    );
  }
}

/// Uyarı kutusu: formun içinde, düğmenin hemen üstünde.
class AuthBanner extends StatelessWidget {
  const AuthBanner(this.message, {super.key});

  final String message;

  @override
  Widget build(BuildContext context) {
    final c = NomivaColors.of(context);
    final n = NomivaScale.of(context);
    return Semantics(
      liveRegion: true,
      child: Container(
        padding: EdgeInsets.symmetric(
          horizontal: n(NomivaSpacing.bannerX),
          vertical: n(NomivaSpacing.bannerY),
        ),
        decoration: BoxDecoration(
          color: c.errorFill,
          borderRadius: BorderRadius.circular(n(NomivaSpacing.radius)),
        ),
        child: Text(
          message,
          style: TextStyle(
            fontFamily: nomivaFont,
            height: nomivaLineHeight,
            letterSpacing: nomivaLetterSpacing,
            fontSize: n(13),
            fontWeight: FontWeight.w500,
            color: c.error,
          ),
        ),
      ),
    );
  }
}

/// Panel başlığı ("Giriş yap", "Hesap oluştur" …).
class AuthTitle extends StatelessWidget {
  const AuthTitle(this.text, {super.key});

  final String text;

  @override
  Widget build(BuildContext context) => Semantics(
    header: true,
    child: Text(
      text,
      style: TextStyle(
        fontFamily: nomivaFont,
        height: nomivaLineHeight,
        letterSpacing: nomivaLetterSpacing,
        fontSize: NomivaScale.of(context)(23),
        fontWeight: FontWeight.w700,
        color: NomivaColors.of(context).text,
      ),
    ),
  );
}

/// Başlığın altındaki gri cümle ("Son bir soru."). Başlığa 6 pt yaklaşır.
class AuthSubtitle extends StatelessWidget {
  const AuthSubtitle(this.text, {super.key});

  final String text;

  @override
  Widget build(BuildContext context) {
    final n = NomivaScale.of(context);
    return Padding(
      padding: EdgeInsets.only(bottom: n(NomivaSpacing.subtitleAfter)),
      child: Transform.translate(
        offset: Offset(0, -n(NomivaSpacing.subtitlePull)),
        child: Text(
          text,
          style: TextStyle(
            fontFamily: nomivaFont,
            fontSize: n(13),
            height: 1.4,
            letterSpacing: nomivaLetterSpacing,
            color: NomivaColors.of(context).muted,
          ),
        ),
      ),
    );
  }
}

/// Alt bağlantı: "Hesabın yok mu? Hesap oluştur". Büyük yazıda iki satıra
/// iner, taşmaz.
class AuthFooter extends StatelessWidget {
  const AuthFooter({
    required this.question,
    required this.action,
    required this.onPressed,
    super.key,
  });

  final String question;
  final String action;
  final VoidCallback? onPressed;

  @override
  Widget build(BuildContext context) {
    final c = NomivaColors.of(context);
    final n = NomivaScale.of(context);
    return Wrap(
      alignment: WrapAlignment.center,
      crossAxisAlignment: WrapCrossAlignment.center,
      children: [
        Text(
          '$question ',
          style: TextStyle(
            fontFamily: nomivaFont,
            height: nomivaLineHeight,
            letterSpacing: nomivaLetterSpacing,
            fontSize: n(13),
            color: c.muted,
          ),
        ),
        AuthTextLink(label: action, onPressed: onPressed),
      ],
    );
  }
}

/// Alt bağlantının yeri: sosyal düğmelerden (ya da ana düğmeden) 30 pt aşağı,
/// altında 28 pt pay. Bağlantının dokunma alanı yazıdan yüksektir; fazlası
/// bu boşluktan düşülür ki yazı tasarımdaki yerinde dursun.
class AuthFooterSlot extends StatelessWidget {
  const AuthFooterSlot({required this.child, super.key});

  final Widget child;

  @override
  Widget build(BuildContext context) {
    final n = NomivaScale.of(context);
    final line = MediaQuery.textScalerOf(context).scale(n(13) * _lineHeight);
    final overhang = math.max(0, _minTarget - line) / 2;
    return Padding(
      padding: EdgeInsets.only(
        top: math.max(0, n(NomivaSpacing.footerTop) - overhang),
        bottom: math.max(0, n(NomivaSpacing.footerBottom) - overhang),
      ),
      child: child,
    );
  }
}

/// "ya da" ayırıcısı.
class AuthOrDivider extends StatelessWidget {
  const AuthOrDivider({super.key});

  @override
  Widget build(BuildContext context) {
    final c = NomivaColors.of(context);
    final n = NomivaScale.of(context);
    Widget line() => Expanded(child: Container(height: 1, color: c.line));
    return ExcludeSemantics(
      child: Row(
        spacing: n(NomivaSpacing.orGap),
        children: [
          line(),
          Text(
            'ya da',
            style: TextStyle(
              fontFamily: nomivaFont,
              height: nomivaLineHeight,
              letterSpacing: nomivaLetterSpacing,
              fontSize: n(12),
              fontWeight: FontWeight.w500,
              color: c.fieldIcon,
            ),
          ),
          line(),
        ],
      ),
    );
  }
}

/// Sosyal giriş sağlayıcısı.
enum AuthSocialProvider {
  google('Google'),
  apple('Apple');

  const AuthSocialProvider(this.label);

  final String label;
}

const _googleSvg =
    '<svg viewBox="0 0 48 48" xmlns="http://www.w3.org/2000/svg">'
    '<path fill="#EA4335" d="M24 9.5c3.5 0 6.6 1.2 9 3.5l6.7-6.7C35.6 2.4 30.2 '
    '0 24 0 14.6 0 6.6 5.4 2.7 13.3l7.8 6C12.4 13.6 17.7 9.5 24 9.5z"/>'
    '<path fill="#4285F4" d="M46.1 24.5c0-1.6-.1-3.1-.4-4.5H24v8.5h12.4c-.5 '
    '2.9-2.2 5.3-4.6 6.9l7.5 5.8c4.4-4 6.8-10 6.8-16.7z"/>'
    '<path fill="#FBBC05" d="M10.5 28.7c-.5-1.4-.8-3-.8-4.7s.3-3.2.8-4.7l-7.8-6'
    'C1 16.6 0 20.2 0 24s1 7.4 2.7 10.7l7.8-6z"/>'
    '<path fill="#34A853" d="M24 48c6.5 0 11.9-2.1 15.9-5.8l-7.5-5.8c-2.1 1.4-'
    '4.8 2.3-8.4 2.3-6.3 0-11.6-4.1-13.5-9.8l-7.8 6C6.6 42.6 14.6 48 24 48z"/>'
    '</svg>';

const _appleSvg =
    '<svg viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg"><path d="M16.4 '
    '12.6c0-2.6 2.1-3.8 2.2-3.9-1.2-1.8-3.1-2-3.7-2-1.6-.2-3.1.9-3.9.9-.8 0-2-'
    '.9-3.4-.9-1.7 0-3.3 1-4.2 2.6-1.8 3.1-.5 7.7 1.3 10.2.8 1.2 1.8 2.6 3.1 '
    '2.5 1.3-.1 1.7-.8 3.2-.8s1.9.8 3.2.8c1.3 0 2.2-1.2 3-2.4.9-1.4 1.3-2.7 '
    '1.3-2.8-.1 0-2.6-1-2.6-4.2zM13.9 5c.7-.8 1.2-2 1-3.1-1 0-2.2.7-2.9 1.5-.6'
    '.7-1.2 1.9-1 3 1.1.1 2.2-.6 2.9-1.4z"/></svg>';

/// Google ve Apple düğmeleri, yuvarlak ve ortada. Apple yalnız iPhone'da
/// görünür.
class AuthSocialRow extends StatelessWidget {
  const AuthSocialRow({required this.onPressed, super.key});

  final ValueChanged<AuthSocialProvider>? onPressed;

  @override
  Widget build(BuildContext context) {
    final c = NomivaColors.of(context);
    final n = NomivaScale.of(context);
    final size = math.max(n(NomivaSpacing.socialSize), _minTarget);
    final iconSize = n(NomivaSpacing.socialIcon);
    final ios = Theme.of(context).platform == TargetPlatform.iOS;

    Widget button(AuthSocialProvider provider, Widget icon) => Semantics(
      button: true,
      label: '${provider.label} ile devam et',
      excludeSemantics: true,
      child: SizedBox.square(
        dimension: size,
        child: Material(
          color: c.chip,
          shape: CircleBorder(side: BorderSide(color: c.line, width: 1.5)),
          clipBehavior: Clip.antiAlias,
          child: InkWell(
            onTap: onPressed == null ? null : () => onPressed!(provider),
            child: Center(child: icon),
          ),
        ),
      ),
    );

    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      spacing: n(NomivaSpacing.socialGap),
      children: [
        button(
          AuthSocialProvider.google,
          SvgPicture.string(_googleSvg, width: iconSize, height: iconSize),
        ),
        if (ios)
          button(
            AuthSocialProvider.apple,
            SvgPicture.string(
              _appleSvg,
              width: iconSize,
              height: iconSize,
              colorFilter: ColorFilter.mode(c.text, BlendMode.srcIn),
            ),
          ),
      ],
    );
  }
}
