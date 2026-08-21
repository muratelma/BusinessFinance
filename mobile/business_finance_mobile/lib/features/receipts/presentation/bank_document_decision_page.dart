import 'package:flutter/material.dart';

import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/formatters/money_text.dart';
import '../../../core/widgets/app_money_text.dart';
import '../data/receipt_models.dart';

/// Bir dekonttaki ana tutarın ne olduğu **belgeden okunamaz.**
///
/// 5.000 TL bir ödeme de olabilir, borç verme de, kendi hesabına aktarma da;
/// kâğıt üçünü ayırt etmez ve model de ayırt edemez — cevabı bilmek, belgedeki
/// hesapların kime ait olduğunu bilmeyi gerektirir. Soru bu yüzden kullanıcıya
/// sorulur, ama fotoğraf çekilmeden **önce** değil: o an kullanıcı belgede ne
/// yazdığını bilmiyor. Burada tutar, tarih ve karşı taraf ekranda dururken
/// soruluyor.
///
/// Sayfa **ayrıştırıcıdır**: hiçbir formu yeniden yazmaz, seçime göre mevcut
/// forma önerilerle gider. Dört formu burada barındırmak quick-add, transfer ve
/// kart ödeme formlarını ikiye çatallamak olurdu.
class BankDocumentDecisionPage extends StatefulWidget {
  const BankDocumentDecisionPage({
    required this.draft,
    required this.onDecided,
    super.key,
  });

  final ReceiptDraft draft;

  /// Seçim yapıldığında çağrılır. Yönlendirme burada değil çağıranda: bu sayfa
  /// hangi rotanın neyi yazdığını bilmek zorunda değil.
  final void Function(BankDocumentDecision decision, bool recordFee) onDecided;

  @override
  State<BankDocumentDecisionPage> createState() =>
      _BankDocumentDecisionPageState();
}

/// Dekonttaki ana tutarın ne olduğu.
///
/// Dördü ekonomik olarak birbirinin zıddı: harcama parayı harcar, aktarma
/// taşır, kart ödemesi borcu kapatır (ve gider **üretmez**, aynı harcama iki kez
/// sayılırdı), borç verme ise geri beklenen bir alacaktır.
enum BankDocumentDecision { expense, ownTransfer, cardPayment, lending }

class _BankDocumentDecisionPageState extends State<BankDocumentDecisionPage> {
  /// Hiçbir seçenek önceden seçili değil.
  ///
  /// Varsayılan "Harcama" olsaydı, dalgın bir dokunuş kart ödemesini gider
  /// yazdırır ve aynı harcama iki kez sayılırdı. Formdaki "ödeme kaynağı boş
  /// başlar ve zorunludur" kuralının aynısı.
  BankDocumentDecision? _decision;

  /// Ücret satırı yalnız belgede ücret **yazıyorsa** çıkar; çıktığında da
  /// açık başlar, çünkü o para gerçekten harcandı.
  late bool _recordFee = widget.draft.hasFee;

  @override
  Widget build(BuildContext context) {
    final draft = widget.draft;
    return Scaffold(
      appBar: AppBar(title: const Text('Dekont okundu')),
      body: ListView(
        padding: const EdgeInsets.all(AppSpacing.medium),
        children: [
          _Summary(draft: draft),
          const SizedBox(height: AppSpacing.large),
          Text(
            draft.totalAmount == null
                ? 'Bu tutar ne?'
                : 'Bu ${MoneyText.format(draft.totalAmount!, draft.currencyCode ?? 'TRY')} ne?',
            style: Theme.of(context).textTheme.titleMedium,
          ),
          const SizedBox(height: AppSpacing.small),
          // Belge kart borcu ödemesi gibi görünüyorsa söylenir — ama seçim yine
          // yapılmaz. Kart ödemesini sessizce varsaymak, kullanıcının görmediği
          // bir karar vermek olurdu.
          if (draft.documentKind == ReceiptDocumentKind.bankCardPayment)
            Padding(
              padding: const EdgeInsets.only(bottom: AppSpacing.small),
              child: Text(
                'Bu belge kart borcu ödemesi gibi görünüyor.',
                style: Theme.of(context).textTheme.bodySmall,
              ),
            ),
          _options(),
          if (draft.hasFee) ...[
            const Divider(height: AppSpacing.large * 2),
            // Ücret ana tutardan bağımsızdır: ana tutar ne olursa olsun banka o
            // parayı aldı ve geri gelmeyecek. Yani her durumda giderdir.
            //
            // Onay burada veriliyor ve ücret kendi formunu açmıyor: 4,50 gibi
            // bir tutar için ikinci bir form doldurmak — üstelik ödeme
            // kaynağını yeniden seçmek — kaydın kendisinden pahalı bir iş
            // yüküydü.
            SwitchListTile(
              value: _recordFee,
              onChanged: (value) => setState(() => _recordFee = value),
              title: const Text('İşlem ücretini ayrı gider olarak kaydet'),
              subtitle: Text(
                '${MoneyText.format(draft.feeAmount!, draft.currencyCode ?? 'TRY')} · '
                'Diğer gider · ana kayıtla aynı kaynaktan, form açılmadan',
              ),
            ),
          ],
          const SizedBox(height: AppSpacing.large),
          FilledButton(
            // Seçim yapılmadan devam edilemez.
            onPressed: _decision == null
                ? null
                : () => widget.onDecided(_decision!, _recordFee),
            child: const Text('Devam'),
          ),
        ],
      ),
    );
  }

  /// Seçeneklerin sırası **ipucuna** göre değişir, seçim yapılmaz.
  ///
  /// Fatura numarası taşıyan bir belge büyük olasılıkla bir ödemedir, ATM
  /// makbuzu ise aktarma; ama olasılık seçim değildir. `paymentHint` kuralının
  /// aynısı, aynı gerekçeyle.
  Widget _options() {
    final ordered = switch (widget.draft.documentKind) {
      ReceiptDocumentKind.bankCardPayment => [
        BankDocumentDecision.cardPayment,
        BankDocumentDecision.expense,
        BankDocumentDecision.ownTransfer,
        BankDocumentDecision.lending,
      ],
      ReceiptDocumentKind.bankDocument => [
        BankDocumentDecision.ownTransfer,
        BankDocumentDecision.expense,
        BankDocumentDecision.cardPayment,
        BankDocumentDecision.lending,
      ],
      _ => [
        BankDocumentDecision.expense,
        BankDocumentDecision.ownTransfer,
        BankDocumentDecision.cardPayment,
        BankDocumentDecision.lending,
      ],
    };

    return RadioGroup<BankDocumentDecision>(
      groupValue: _decision,
      onChanged: (value) => setState(() => _decision = value),
      child: Column(
        children: [
          for (final option in ordered)
            RadioListTile<BankDocumentDecision>(
              value: option,
              title: Text(_label(option)),
              subtitle: Text(_explanation(option)),
            ),
        ],
      ),
    );
  }

  static String _label(BankDocumentDecision option) => switch (option) {
    BankDocumentDecision.expense => 'Harcama',
    BankDocumentDecision.ownTransfer => 'Kendi hesabıma aktarma',
    BankDocumentDecision.cardPayment => 'Kart ödemesi',
    // "Borç verme" Türkçede iki yöne de bakıyor — kullanıcı "borçlandım mı,
    // borç mu verdim" diye duraksıyordu. Fiilin öznesi yönü tek başına
    // söylüyor.
    BankDocumentDecision.lending => 'Geri bekliyorum',
  };

  /// Her seçeneğin deftere ne yaptığı yazılı: dördü zıt sonuçlar üretiyor ve
  /// aradaki farkı kullanıcının tahmin etmesi beklenemez.
  static String _explanation(BankDocumentDecision option) => switch (option) {
    BankDocumentDecision.expense => 'Para harcandı; gider olarak yazılır.',
    BankDocumentDecision.ownTransfer =>
      'Para kendi hesaplarınız arasında taşındı; gelir/gider toplamı değişmez.',
    BankDocumentDecision.cardPayment =>
      'Kart borcu kapatıldı; gider yazılmaz, harcamalar zaten sayıldı.',
    BankDocumentDecision.lending =>
      'Alacak olarak kaydedilir, gider raporuna girmez.',
  };
}

class _Summary extends StatelessWidget {
  const _Summary({required this.draft});

  final ReceiptDraft draft;

  @override
  Widget build(BuildContext context) => AppCard(
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
            child: AppMoneyText(
              amount: amount,
              currency: draft.currencyCode ?? 'TRY',
            ),
          ),
      ],
    ),
  );
}
