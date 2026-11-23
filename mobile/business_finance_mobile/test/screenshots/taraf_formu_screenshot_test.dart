import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/features/categories/data/category_models.dart';
import 'package:business_finance_mobile/features/categories/presentation/category_form_page.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

import 'screenshot_harness.dart';

/// Kategori formundaki `İkisi de` seçeneği (ADR 0020, Aşama 06.3 Grup 8).
/// Tasarım teslimi yok; mevcut dille kuruldu. Bilgi satırı ve çip karelerini
/// çizen eski yükümlülük formu Faz 8'de kalktı (ADR 0022).
void main() {
  testWidgets('kategori formu: İkisi de', (tester) async {
    await captureScreen(
      tester,
      'taraf-03-kategori-formu',
      ChangeNotifierProvider<ScopeController?>.value(
        value: _VisibleScope(),
        child: CategoryFormPage(
          category: const BudgetCategory(
            id: 'sigorta',
            name: 'Sigorta',
            type: 'expense',
            isActive: true,
          ),
          onSave:
              ({
                category,
                required name,
                required type,
                required isActive,
                defaultScope,
                isTax = false,
              }) async => true,
        ),
      ),
      withNavBar: false,
      pushed: true,
    );
  }, skip: !screenshotsEnabled);
}

class _VisibleScope extends ScopeController {
  @override
  bool get isVisible => true;
}
