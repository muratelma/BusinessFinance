import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

import 'app/app_dependencies.dart';
import 'app/business_finance_app.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  // Figtree uygulamanın içinde dağıtılıyor; SIL OFL lisansı metninin
  // uygulamayla birlikte gösterilmesini ister (Lisanslar sayfası).
  LicenseRegistry.addLicense(() async* {
    final text = await rootBundle.loadString('assets/fonts/figtree/OFL.txt');
    yield LicenseEntryWithLineBreaks(const ['Figtree'], text);
  });
  final dependencies = AppDependencies.create();
  dependencies.authController.initialize();
  runApp(BusinessFinanceApp(dependencies: dependencies));
}
