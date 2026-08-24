import 'package:flutter/material.dart';

import '../../../core/formatters/date_text.dart';
import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_money_text.dart';
import '../data/receipt_models.dart';

/// Bir faturada son ödeme tarihi yazması, ödendiğini **söylemez.**
///
/// Yalnız ne zaman ödenmesi gerektiğini söyler. Ödenmemiş fatura belge tarihinde
/// gider tanıyan bir yükümlülüktür; kasa ancak ödeme kaydedildiğinde değişir.
///
/// Soru karar sayfasına (`BankDocumentDecisionPage`) **girmez** ve bilerek:
/// orada sorulan "bu tutar ne?", burada sorulan "ödendi mi?" — farklı sorular.
/// Üstelik fatura banka belgesi olmak zorunda değil; çoğu kâğıt faturadır.
class InvoiceDecisionPage extends StatelessWidget {
  const InvoiceDecisionPage({
    required this.draft,
    required this.onDecided,
    super.key,
  });

  final ReceiptDraft draft;

  /// `true` ödendi (gider), `false` ödenmedi (yükümlülük). Yönlendirme
  /// çağıranda: bu sayfa hangi rotanın neyi yazdığını bilmek zorunda değil.
  final void Function(bool isPaid) onDecided;

  @override
  Widget build(BuildContext context) {
    final currency = draft.currencyCode ?? 'TRY';
    return Scaffold(
      appBar: AppBar(title: const Text('Fatura okundu')),
      body: ListView(
        padding: const EdgeInsets.all(AppSpacing.medium),
        children: [
          AppCard(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                if (draft.counterpartyName case final name?)
                  Text(name, style: Theme.of(context).textTheme.titleMedium),
                if (draft.totalAmount case final amount?)
                  Padding(
                    padding: const EdgeInsets.only(top: AppSpacing.small),
                    child: AppMoneyText(amount: amount, currency: currency),
                  ),
                Padding(
                  padding: const EdgeInsets.only(top: AppSpacing.small),
                  child: Text(
                    'Son ödeme tarihi: '
                    '${DateText.dayMonthYear(draft.dueDate!)}',
                  ),
                ),
              ],
            ),
          ),
          const SizedBox(height: AppSpacing.large),
          Text(
            'Bu faturayı ödediniz mi?',
            style: Theme.of(context).textTheme.titleMedium,
          ),
          const SizedBox(height: AppSpacing.small),
          // Hiçbiri önceden seçili değil ve ikisi zıt sonuç üretiyor: biri
          // parayı bugün çıkarır, diğeri bir borç doğurur. Varsayılan koymak,
          // dalgın bir dokunuşla yanlışını yazdırırdı.
          _Choice(
            icon: Icons.check_circle_outline,
            title: 'Ödedim',
            subtitle:
                'Gider olarak yazılır; hesabınızdan düşer ve aylık gidere '
                'girer.',
            onTap: () => onDecided(true),
          ),
          const SizedBox(height: AppSpacing.small),
          _Choice(
            icon: Icons.event_outlined,
            title: 'Henüz ödemedim',
            subtitle:
                'Gider belge tarihinde yazılır; hesabınızdan şimdi para '
                'çıkmaz ve son ödeme tarihinde takip edilir.',
            onTap: () => onDecided(false),
          ),
        ],
      ),
    );
  }
}

class _Choice extends StatelessWidget {
  const _Choice({
    required this.icon,
    required this.title,
    required this.subtitle,
    required this.onTap,
  });

  final IconData icon;
  final String title;
  final String subtitle;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) => AppCard(
    onTap: onTap,
    child: Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Icon(icon),
        const SizedBox(width: AppSpacing.medium),
        Expanded(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(title, style: Theme.of(context).textTheme.titleSmall),
              const SizedBox(height: AppSpacing.xSmall),
              Text(subtitle, style: Theme.of(context).textTheme.bodySmall),
            ],
          ),
        ),
      ],
    ),
  );
}
