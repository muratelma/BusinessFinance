import 'package:flutter/material.dart';

import 'app/app_dependencies.dart';
import 'app/business_finance_app.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  final dependencies = AppDependencies.create();
  dependencies.authController.initialize();
  runApp(BusinessFinanceApp(dependencies: dependencies));
}
