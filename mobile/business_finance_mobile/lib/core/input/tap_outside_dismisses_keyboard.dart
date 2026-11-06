import 'package:flutter/foundation.dart';
import 'package:flutter/gestures.dart';
import 'package:flutter/widgets.dart';

/// Metin alanının dışına dokunmak klavyeyi kapatır.
///
/// Flutter telefonda alanın dışına dokunulduğunda odağı bilerek bırakmaz
/// (`EditableTextTapOutsideIntent`'in varsayılanı). iPhone'da klavyeyi
/// kapatan geri tuşu olmadığı için kullanıcı boş bir yere dokunmayı bekler.
/// Bu sarmalayıcı aynı niyeti uygulama genelinde ezer:
///
/// * Dokunma alanın dışında başlar ve parmak kaydırmadan kalkarsa alan odağı
///   bırakır. Kaydırma klavyeyi kapatmaz; uzun formda aşağı inerken klavye
///   kaybolmasın.
/// * Başka bir metin alanına dokunmak "dışarı" sayılmaz; odak oraya geçer.
/// * Fare ve kalem gibi dokunma dışı girişte Flutter'ın varsayılanı korunur:
///   odak basıldığı an bırakılır.
///
/// Kendi `onTapOutside` geri çağrısını veren alan bu kuralın dışındadır.
class TapOutsideDismissesKeyboard extends StatefulWidget {
  const TapOutsideDismissesKeyboard({super.key, required this.child});

  final Widget child;

  @override
  State<TapOutsideDismissesKeyboard> createState() =>
      _TapOutsideDismissesKeyboardState();
}

class _TapOutsideDismissesKeyboardState
    extends State<TapOutsideDismissesKeyboard> {
  /// Dışarıda başlamış ve henüz kalkmamış dokunuşlar, işaretçi kimliğiyle.
  final Map<int, _PendingTouch> _pending = {};

  late final Map<Type, Action<Intent>> _actions = {
    EditableTextTapOutsideIntent: CallbackAction<EditableTextTapOutsideIntent>(
      onInvoke: _onTapDownOutside,
    ),
    EditableTextTapUpOutsideIntent:
        CallbackAction<EditableTextTapUpOutsideIntent>(
          onInvoke: _onTapUpOutside,
        ),
  };

  Object? _onTapDownOutside(EditableTextTapOutsideIntent intent) {
    final event = intent.pointerDownEvent;
    if (event.kind == PointerDeviceKind.touch && !kIsWeb) {
      // Parmak alanın içinde kalkarsa "kalktı" bildirimi gelmez; kalan kayıt
      // bir sonraki dokunuşta silinir.
      _pending.clear();
      _pending[event.pointer] = _PendingTouch(
        focusNode: intent.focusNode,
        position: event.position,
      );
    } else {
      intent.focusNode.unfocus();
    }
    return null;
  }

  Object? _onTapUpOutside(EditableTextTapUpOutsideIntent intent) {
    final event = intent.pointerUpEvent;
    final touch = _pending.remove(event.pointer);
    if (touch == null || !identical(touch.focusNode, intent.focusNode)) {
      return null;
    }
    final moved = (event.position - touch.position).distance;
    if (moved <= kTouchSlop && touch.focusNode.hasFocus) {
      touch.focusNode.unfocus();
    }
    return null;
  }

  @override
  Widget build(BuildContext context) =>
      Actions(actions: _actions, child: widget.child);
}

class _PendingTouch {
  const _PendingTouch({required this.focusNode, required this.position});

  final FocusNode focusNode;
  final Offset position;
}
