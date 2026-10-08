import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:business_finance_mobile/features/planning/data/planning_models.dart';
import 'package:flutter_test/flutter_test.dart';

/// Tekrarlayan plan formu kategorinin tarafını bu modelden okur. Alan
/// okunmadığında form her kategoriyi iki tarafa açık sanıyor ve tek taraflı
/// kategoride de çip gösteriyordu (8 Ekim 2026'da cihazda görüldü).
void main() {
  test('kategori seçimi kategorinin tarafını taşır', () {
    final personal = PlanningChoice.categoryFromJson(const {
      'id': 'c1',
      'name': 'Ev faturaları',
      'type': 'expense',
      'defaultScope': 'personal',
    });
    final open = PlanningChoice.categoryFromJson(const {
      'id': 'c2',
      'name': 'Sigorta',
      'type': 'expense',
      'defaultScope': null,
    });

    expect(personal.defaultScope, TransactionScope.personal);
    expect(open.defaultScope, isNull);
  });

  test('hesap ve kart seçimi etiketini taşır', () {
    final account = PlanningChoice.fromJson(const {
      'id': 'a1',
      'name': 'Dükkân kasası',
      'defaultScope': 'business',
    });

    expect(account.defaultScope, TransactionScope.business);
  });
}
