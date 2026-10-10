import 'package:flutter/material.dart';

import '../theme/app_spacing.dart';
import '../theme/app_surfaces.dart';
import 'app_adaptive_sheet.dart';
import 'app_form_error.dart';
import 'app_submit_button.dart';

/// Veri giren panellerin ortak kabuğu: başlık, kaydırılan gövde, eylem satırı.
///
/// `AppConfirmDialog` bir soruyu sorar; bu bileşen bir **form** taşır. İkisi
/// ayrı kalıptır ve ayrı bileşenlerdir.
///
/// Kabuğun tek yerde toplanmasının üç somut kazancı var:
///
/// 1. **Controller'lar artık rota ile birlikte ölür.** Panel içeriği bir
///    `StatefulWidget` olmak zorunda; `TextEditingController` onun
///    `State.dispose()`'unda bırakılır. Eskiden paneller `await showDialog`
///    döner dönmez controller'ı `dispose()` ediyordu — oysa o `await`
///    `Navigator.pop` anında döner, kapanma animasyonu daha bitmemiştir ve
///    hâlâ kare çizen `TextField` ölü controller'a dokunup uygulamayı
///    kırmıştı.
/// 2. **Klavye alanı bir kez çözülür.** Gövde `viewInsets` kadar aşağıdan
///    pay bırakır; yoksa telefonda klavye son alanları örter.
/// 3. **Gönderim kilidi bedava gelir**: eylem `AppSubmitButton` olduğu için
///    ikinci dokunuş ikinci yazma olmaz.
///
/// Barrier'a dokunmak vazgeçmektir: panel hiçbir zaman kapanışı onay saymaz,
/// sonucu yalnız gönderim butonu üretir.
class AppFormSheet<T> extends StatelessWidget {
  const AppFormSheet({
    required this.title,
    required this.children,
    required this.submitLabel,
    required this.onSubmit,
    super.key,
    this.description,
    this.cancelLabel = 'Vazgeç',
    this.secondaryLabel,
    this.onSecondary,
    this.errorMessage,
    this.horizontalPadding = AppSpacing.large,
  });

  /// Panelin iki yanındaki boşluk. Varsayılan bütün formların ortak
  /// boşluğudur; teslimi 16 dp çerçeveyle çizilmiş panel onu verir.
  final double horizontalPadding;

  /// Kaydın reddi. Eylem satırının hemen üstünde, kaydırılan gövdenin
  /// dışında durur: gövde nerede olursa olsun `Kaydet`e basan onu görür.
  final String? errorMessage;

  /// Panelin ne olduğunu söyleyen başlık. Zorunludur: başlıksız bir panel
  /// kullanıcıya neyin içinde olduğunu söylemez.
  final String title;

  /// Başlığın altındaki tek cümlelik açıklama. Kural veya sonuç anlatır.
  final String? description;

  final List<Widget> children;

  final String submitLabel;
  final String cancelLabel;

  /// Sonucu üretir. `null` dönerse panel açık kalır (doğrulama düştü).
  /// Null'ın kendisi verilirse buton görünür ama devre dışıdır.
  final Future<T?> Function()? onSubmit;

  /// Üçüncü eylem: gönderim de vazgeçme de olmayan, kendi sonucunu üreten bir
  /// seçenek. Filtre panelindeki `Temizle` bunun tek örneğidir — boş filtreyi
  /// döndürür, yani vazgeçmekten farklıdır (vazgeçmek hiçbir şey değiştirmez).
  final String? secondaryLabel;
  final Future<T?> Function()? onSecondary;

  /// Paneli pencere sınıfına göre bottom sheet veya dialog olarak açar.
  static Future<T?> show<T>({
    required BuildContext context,
    required WidgetBuilder builder,
  }) => AppAdaptiveSheet.show<T>(context: context, builder: builder);

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final surfaces = AppSurfaces.of(context);
    // Klavye açıkken gövde onun üstünde kalır. Panelin kendisi kaymaz;
    // kayan şey içeriktir, böylece başlık ve eylem satırı yerinde durur.
    final keyboardInset = MediaQuery.viewInsetsOf(context).bottom;

    return Column(
      mainAxisSize: MainAxisSize.min,
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        Padding(
          padding: EdgeInsets.fromLTRB(
            horizontalPadding,
            AppSpacing.medium,
            horizontalPadding,
            AppSpacing.small,
          ),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(title, style: theme.textTheme.titleLarge),
              if (description != null)
                Padding(
                  padding: const EdgeInsets.only(top: AppSpacing.xSmall),
                  child: Text(
                    description!,
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: theme.colorScheme.onSurfaceVariant,
                    ),
                  ),
                ),
            ],
          ),
        ),
        Flexible(
          child: SingleChildScrollView(
            padding: EdgeInsets.fromLTRB(
              horizontalPadding,
              AppSpacing.small,
              horizontalPadding,
              AppSpacing.medium + keyboardInset,
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.stretch,
              mainAxisSize: MainAxisSize.min,
              children: children,
            ),
          ),
        ),
        if (errorMessage != null)
          Padding(
            padding: EdgeInsets.fromLTRB(
              horizontalPadding,
              0,
              horizontalPadding,
              AppSpacing.small,
            ),
            child: AppFormError(message: errorMessage!),
          ),
        Container(
          decoration: BoxDecoration(border: Border(top: surfaces.cardBorder)),
          padding: EdgeInsets.fromLTRB(
            horizontalPadding,
            AppSpacing.small,
            horizontalPadding,
            AppSpacing.medium,
          ),
          // `Row` değil `OverflowBar`: 2.0× yazı ölçeğinde üç buton tek satıra
          // sığmıyor ve sığdırılmaya çalışılınca eylem satırı sağdan taşıyordu
          // (ölçülen 334 px). `OverflowBar` sığdığında yatay dizer, sığmadığında
          // alt alta geçer; eylemler hiçbir ölçekte ekran dışında kalmaz.
          child: OverflowBar(
            alignment: MainAxisAlignment.end,
            overflowAlignment: OverflowBarAlignment.end,
            spacing: AppSpacing.small,
            overflowSpacing: AppSpacing.small,
            children: [
              if (secondaryLabel != null && onSecondary != null)
                TextButton(
                  onPressed: () async {
                    final result = await onSecondary!();
                    if (result == null || !context.mounted) return;
                    Navigator.of(context).pop(result);
                  },
                  child: Text(secondaryLabel!),
                ),
              TextButton(
                onPressed: () => Navigator.of(context).pop(),
                child: Text(cancelLabel),
              ),
              AppSubmitButton(
                label: submitLabel,
                onSubmit: onSubmit == null
                    ? null
                    : () async {
                        final result = await onSubmit!();
                        // Doğrulama düştüyse sonuç yoktur ve panel açık
                        // kalır; kullanıcının düzeltmesi gereken alan hâlâ
                        // ekrandadır.
                        if (result == null || !context.mounted) return;
                        Navigator.of(context).pop(result);
                      },
              ),
            ],
          ),
        ),
      ],
    );
  }
}

/// Panellerin ortak alan aralığı. Her form kendi boşluğunu uydurmasın diye
/// alanlar bu sarmalayıcıyla dizilir.
class AppFormField extends StatelessWidget {
  const AppFormField({required this.child, super.key});

  final Widget child;

  @override
  Widget build(BuildContext context) => Padding(
    padding: const EdgeInsets.only(bottom: AppSpacing.medium),
    child: child,
  );
}
