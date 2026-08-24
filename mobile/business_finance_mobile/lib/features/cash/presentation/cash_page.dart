import 'package:flutter/material.dart';

import '../../../core/presentation/scope_controller.dart';
import '../../pos/presentation/pos_controller.dart';
import '../../pos/presentation/pos_settlements_view.dart';
import 'cash_controller.dart';
import 'cash_count_view.dart';

/// `Kasa`: tezgâh üstü esnafın günlük ekranı.
///
/// İki sekme iki ayrı soruya bakar ve tek ekranda durmalarının sebebi ikisinin
/// de **günün parası** olması: kasadaki nakit ve müşterinin kartla ödediği,
/// henüz yolda olan para. Kredi kartı borcu burada değildir — o başka bir
/// şeydir ve adı `Kredi kartlarım`dır (ADR 0015).
class CashPage extends StatefulWidget {
  const CashPage({
    required this.cashController,
    required this.posController,
    super.key,
    this.scopeController,
    this.ownsControllers = true,
  });

  final CashCountController cashController;
  final PosController posController;
  final ScopeController? scopeController;
  final bool ownsControllers;

  @override
  State<CashPage> createState() => _CashPageState();
}

class _CashPageState extends State<CashPage> {
  @override
  void dispose() {
    if (widget.ownsControllers) {
      widget.cashController.dispose();
      widget.posController.dispose();
    }
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return DefaultTabController(
      length: 2,
      child: Scaffold(
        appBar: AppBar(
          title: const Text('Kasa'),
          bottom: const TabBar(
            tabs: [
              Tab(text: 'Gün sonu'),
              Tab(text: 'POS tahsilatları'),
            ],
          ),
        ),
        body: TabBarView(
          children: [
            CashCountView(
              controller: widget.cashController,
              scopeController: widget.scopeController,
            ),
            PosSettlementsView(
              controller: widget.posController,
              scopeController: widget.scopeController,
            ),
          ],
        ),
      ),
    );
  }
}
