import 'package:flutter/material.dart';

import '../../../core/formatters/money_text.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_money_text.dart';
import '../data/receipt_models.dart';

/// İade fişi yeni bir kayıt üretmez — **eski** bir harcamayı geri alır.
///
/// Gider olarak yazılsaydı geri gelen para harcanmış görünürdü; gelir olarak
/// yazılsaydı kazanılmış görünürdü. Doğrusu, o harcamanın olmamış sayılması:
/// silme yerine iptal, çünkü finansal geçmiş silinmez.
///
/// Sayfa **hiçbir şeyi kendiliğinden yapmaz.** Sunucunun bulduğu aday burada
/// gösteriliyor ve kullanıcı tarih, tutar ve adı görüp onaylıyor. Yanlış kaydı
/// sessizce iptal etmek, hiç bulamamaktan kötüdür — kullanıcının bakmak için
/// bir sebebi olmazdı.
class RefundDecisionPage extends StatelessWidget {
  const RefundDecisionPage({
    required this.draft,
    required this.onCancelExpense,
    super.key,
  });

  final ReceiptDraft draft;

  /// Kullanıcı onayladığında çağrılır. İptalin kendisi burada değil: bu sayfa
  /// hangi deponun yazdığını bilmek zorunda değil.
  final void Function(ReceiptRefundMatch match) onCancelExpense;

  @override
  Widget build(BuildContext context) {
    final currency = draft.currencyCode ?? 'TRY';
    final match = draft.refundMatch;
    return Scaffold(
      appBar: AppBar(title: const Text('İade fişi okundu')),
      body: ListView(
        padding: const EdgeInsets.all(AppSpacing.medium),
        children: [
          AppCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                if (draft.counterpartyName case final name?)
                  Text(name, style: Theme.of(context).textTheme.titleMedium),
                if (draft.purchasedAt case final date?)
                  Padding(
                    padding: const EdgeInsets.only(top: AppSpacing.xSmall),
                    child: Text(date),
                  ),
                if (draft.totalAmount case final amount?)
                  Padding(
                    padding: const EdgeInsets.only(top: AppSpacing.small),
                    child: AppMoneyText(amount: amount, currency: currency),
                  ),
              ],
            ),
          ),
          const SizedBox(height: AppSpacing.large),
          if (match == null)
            ..._noMatch(context)
          else
            ..._match(context, match),
        ],
      ),
    );
  }

  /// Eşleşme yoksa uydurulmuş bir iptal yapılmaz; kullanıcıya olduğu gibi
  /// söylenir. Bulunamamış olması iadenin gerçek olmadığı anlamına gelmez —
  /// harcama başka bir adla yazılmış ya da hiç yazılmamış olabilir.
  List<Widget> _noMatch(BuildContext context) => [
    const AppInlineNotice(
      icon: Icons.search_off_outlined,
      message:
          'Bu iadeyle eşleşen bir harcama bulunamadı. Harcama başka bir adla '
          'yazılmış ya da hiç yazılmamış olabilir; ilgili kaydı listeden '
          'kendiniz iptal edebilirsiniz.',
    ),
    const SizedBox(height: AppSpacing.medium),
    FilledButton.tonal(
      onPressed: () => Navigator.of(context).maybePop(),
      child: const Text('Kapat'),
    ),
  ];

  List<Widget> _match(BuildContext context, ReceiptRefundMatch match) {
    final currency = draft.currencyCode ?? 'TRY';
    return [
      Text(
        'Bu harcamanızı iptal edeyim mi?',
        style: Theme.of(context).textTheme.titleMedium,
      ),
      const SizedBox(height: AppSpacing.small),
      AppCard(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(match.description ?? 'Açıklamasız harcama'),
            Padding(
              padding: const EdgeInsets.only(top: AppSpacing.xSmall),
              child: Text(match.transactionDate),
            ),
            Padding(
              padding: const EdgeInsets.only(top: AppSpacing.small),
              child: AppMoneyText(amount: match.amount, currency: currency),
            ),
          ],
        ),
      ),
      const SizedBox(height: AppSpacing.medium),
      // Kısmi iadede iptal tek başına yetmez: harcamanın bir kısmı gerçekten
      // yapıldı. Kalan tutar **sunucuda** hesaplandı; burada yeniden
      // hesaplanmıyor.
      AppInlineNotice(
        icon: Icons.info_outline,
        message: match.isPartial
            ? 'İade tutarı harcamadan küçük. Harcama iptal edilecek ve kalan '
                  '${MoneyText.format(match.remainingAmount!, currency)} için '
                  'yeni bir gider formu açılacak.'
            : 'Harcama silinmez, iptal edilir: finansal geçmiş korunur ve '
                  'raporlardan düşer.',
      ),
      const SizedBox(height: AppSpacing.medium),
      FilledButton(
        onPressed: () => onCancelExpense(match),
        child: Text(
          match.isPartial ? 'İptal et ve kalanı yaz' : 'Harcamayı iptal et',
        ),
      ),
      const SizedBox(height: AppSpacing.small),
      TextButton(
        onPressed: () => Navigator.of(context).maybePop(),
        child: const Text('Vazgeç'),
      ),
    ];
  }
}
