import 'package:business_finance_mobile/core/presentation/scope_controller.dart';
import 'package:business_finance_mobile/features/account/presentation/account_status_controller.dart';
import 'package:business_finance_mobile/features/more/presentation/more_page.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:provider/provider.dart';

import 'design_sample_data.dart';
import 'screenshot_harness.dart';

void main() {
  testWidgets('16 diğer', (tester) async {
    final scope = ScopeController(readHasBusiness: () async => true);
    final status = AccountStatusController(DesignAccount());
    await tester.runAsync(() async {
      await scope.ensureLoaded();
      await status.ensureLoaded();
    });
    await captureScreen(
      tester,
      '16-diger',
      MultiProvider(
        providers: [
          ChangeNotifierProvider<ScopeController?>.value(value: scope),
          ChangeNotifierProvider<AccountStatusController?>.value(value: status),
        ],
        child: const MorePage(),
      ),
      selectedTab: 3,
    );
  }, skip: !screenshotsEnabled);
}
