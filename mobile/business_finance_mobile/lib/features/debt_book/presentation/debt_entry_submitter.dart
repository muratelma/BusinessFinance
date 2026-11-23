import '../../../core/network/api_exception.dart';
import '../../../core/presentation/financial_data_changes.dart';
import '../data/debt_book_models.dart';
import '../data/debt_book_repository.dart';
import 'debt_entry_form_view.dart';
import 'debt_entry_sheet.dart';

/// Yeni kaydın nereye yazılacağını uygulama bilir (ADR 0022 İ1, K1):
///
/// - Taksitliyse ya da kişisi yoksa ayrı kayıt (`POST /api/v1/debts`).
/// - Kişili, taksitsiz ve uygulamadan önce oluşmuşsa kişinin açılış bakiyesi.
/// - Avans tahsilat ya da ödemedir.
/// - Satış ve alım kişinin hesabına borçlandırma (peşin tutarıyla tek yazma).
/// - Borç verme ve alma kişinin hesabına borç para.
///
/// Kullanıcıya hangi yolun seçildiği sorulmaz.
class DebtEntrySubmitter {
  const DebtEntrySubmitter(this._repository, {this.changes});

  final DebtBookRepositoryContract _repository;
  final FinancialDataChanges? changes;

  /// Yazıldıysa `null`, reddedildiyse formda gösterilecek cümle.
  Future<String?> submit(
    DebtEntryDraft draft, {
    required String asOfDate,
  }) async {
    try {
      var personId = draft.person?.id;
      final person = draft.person;
      if (person != null && person.isNew) {
        personId = await _repository.createCounterparty(
          person.name,
          scope: person.scope,
        );
      }
      if (draft.isInstallments || personId == null) {
        await _repository.createDebt(_debtBody(draft, personId, asOfDate));
        changes?.debtChanged();
        return null;
      }
      if (draft.carriedOver) {
        final page = await _repository.loadPerson(personId, asOfDate: asOfDate);
        final receivable = draft.side == DebtSide.receivable;
        await _repository.setOpeningBalance(
          personId,
          receivable: receivable ? draft.amount : page.openingReceivable,
          payable: receivable ? page.openingPayable : draft.amount,
        );
      } else if (draft.kind == DebtEntryKind.advance) {
        // `Aldım` (kişiye borçlanılır) tahsilattır, `Verdim` ödemedir.
        await _repository.recordPayment(personId, {
          'direction': draft.side == DebtSide.payable
              ? 'receivable'
              : 'payable',
          'amount': draft.amount,
          'currency': 'TRY',
          'accountId': draft.accountId,
          'paymentDate': draft.date,
          'description': draft.description,
          'clientRequestId': draft.clientRequestId,
        });
      } else if (draft.kind == DebtEntryKind.sale ||
          draft.kind == DebtEntryKind.purchase) {
        await _repository.createCharge(personId, {
          'direction': draft.side.apiValue,
          'amount': draft.amount,
          'currency': 'TRY',
          'categoryId': draft.categoryId,
          'chargeDate': draft.date,
          'scope': ?draft.scope?.apiValue,
          'description': draft.description,
          'dueDate': draft.dueDate,
          'clientRequestId': draft.clientRequestId,
          if (draft.upfrontAmount != null)
            'paidNow': {
              'amount': draft.upfrontAmount,
              'accountId': draft.upfrontAccountId,
            },
        });
      } else {
        await _repository.createLoan(personId, {
          'direction': draft.side.apiValue,
          'amount': draft.amount,
          'currency': 'TRY',
          'accountId': draft.accountId,
          'loanDate': draft.date,
          'dueDate': draft.dueDate,
          'description': draft.description,
          'clientRequestId': draft.clientRequestId,
        });
      }
      changes?.counterpartyLedgerChanged();
      return null;
    } on ApiException catch (error) {
      return error.message;
    }
  }

  Map<String, Object?> _debtBody(
    DebtEntryDraft draft,
    String? personId,
    String asOfDate,
  ) {
    final cash =
        draft.kind == DebtEntryKind.lend || draft.kind == DebtEntryKind.borrow;
    final planKind = draft.isInstallments
        ? 'installments'
        : draft.dueDate != null
        ? 'single'
        : 'none';
    return {
      'counterpartyId': personId,
      'direction': draft.side.apiValue,
      'scope': draft.scope?.apiValue,
      'principal': draft.amount,
      'totalRepayment': null,
      'annualInterestRate': null,
      'installmentAmount': draft.installmentAmount,
      'currency': 'TRY',
      'sourceType': cash
          ? 'cash'
          : draft.side == DebtSide.receivable
          ? 'income'
          : 'expense',
      'openingAccountId': cash && !draft.carriedOver ? draft.accountId : null,
      'categoryId': cash || draft.carriedOver ? null : draft.categoryId,
      'startDate': draft.date,
      'firstDueDate': draft.firstDueDate ?? draft.dueDate,
      'installmentCount': draft.installmentCount ?? 1,
      'description': draft.description,
      'asOfDate': asOfDate,
      'planKind': planKind,
      'isCarriedOver': draft.carriedOver,
      if (draft.upfrontAmount != null)
        'upfrontPayment': {
          'amount': draft.upfrontAmount,
          'accountId': draft.upfrontAccountId,
        },
      'clientRequestId': draft.clientRequestId,
    };
  }
}
