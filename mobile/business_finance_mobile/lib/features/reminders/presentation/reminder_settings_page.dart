import 'package:flutter/material.dart';

import '../../../core/theme/app_spacing.dart';
import '../../../core/widgets/app_card.dart';
import '../../../core/widgets/app_inline_notice.dart';
import '../../../core/widgets/app_section_header.dart';
import '../data/reminder_models.dart';
import 'reminder_controller.dart';

/// `Hatırlatmalar`.
///
/// Ekranın tamamı cihaz ayarıdır: hiçbir şey sunucuya yazılmaz. Sıra karara
/// göre — önce hatırlatılsın mı, sonra ne, en sonda ne zaman.
class ReminderSettingsPage extends StatefulWidget {
  const ReminderSettingsPage({required this.controller, super.key});

  final ReminderController controller;

  @override
  State<ReminderSettingsPage> createState() => _ReminderSettingsPageState();
}

class _ReminderSettingsPageState extends State<ReminderSettingsPage> {
  @override
  void initState() {
    super.initState();
    widget.controller.addListener(_changed);
    widget.controller.ensureLoaded();
  }

  void _changed() {
    if (mounted) setState(() {});
  }

  @override
  void dispose() {
    widget.controller.removeListener(_changed);
    super.dispose();
  }

  @override
  Widget build(BuildContext context) => Scaffold(
    appBar: AppBar(title: const Text('Hatırlatmalar')),
    body: _body(),
  );

  Widget _body() {
    final controller = widget.controller;
    final settings = controller.settings;

    return Column(
      children: [
        if (controller.isBusy) const LinearProgressIndicator(),
        Expanded(
          child: ListView(
            padding: const EdgeInsets.fromLTRB(
              AppSpacing.medium,
              AppSpacing.medium,
              AppSpacing.medium,
              AppSpacing.fabClearance,
            ),
            children: [
              AppCard(
                padding: EdgeInsets.zero,
                child: SwitchListTile(
                  value: settings.isEnabled,
                  onChanged: controller.isBusy
                      ? null
                      : (value) => controller.setEnabled(value),
                  title: const Text('Hatırlatmaları aç'),
                  subtitle: const Text(
                    'Vadesi gelen kayıtlar için telefonunuz sizi uyarır. '
                    'Bildirim cihazda kurulur; internet gerektirmez.',
                  ),
                ),
              ),
              if (controller.permissionDenied) ...[
                const SizedBox(height: AppSpacing.small),
                const AppInlineNotice(
                  icon: Icons.notifications_off_outlined,
                  message:
                      'Bildirim izni verilmedi. Uygulama çalışmaya devam eder; '
                      'hatırlatma kurmak için izni telefon ayarlarından '
                      'açabilirsiniz.',
                ),
              ],
              if (controller.syncFailed) ...[
                const SizedBox(height: AppSpacing.small),
                AppInlineNotice(
                  icon: Icons.cloud_off_outlined,
                  message:
                      'Yaklaşan kayıtlar okunamadı; kurulu hatırlatmalar '
                      'olduğu gibi duruyor.',
                  actionLabel: 'Tekrar dene',
                  onAction: controller.sync,
                ),
              ],
              if (settings.isEnabled) ...[
                const SizedBox(height: AppSpacing.medium),
                const AppSectionHeader(title: 'Neler hatırlatılsın'),
                AppCard(
                  padding: EdgeInsets.zero,
                  child: Column(
                    children: [
                      for (final kind in ReminderKind.values)
                        SwitchListTile(
                          value: settings.kinds.contains(kind),
                          onChanged: controller.isBusy
                              ? null
                              : (value) =>
                                    controller.setKindEnabled(kind, value),
                          title: Text(kind.label),
                        ),
                    ],
                  ),
                ),
                const SizedBox(height: AppSpacing.medium),
                const AppSectionHeader(title: 'Ne zaman'),
                AppCard(
                  padding: EdgeInsets.zero,
                  child: ListTile(
                    leading: const Icon(Icons.schedule_outlined),
                    title: const Text('Hatırlatma saati'),
                    subtitle: const Text(
                      'Vade günü, seçtiğiniz saatte tek bildirim gelir.',
                    ),
                    trailing: Text(settings.timeLabel),
                    onTap: controller.isBusy ? null : _pickTime,
                  ),
                ),
                const SizedBox(height: AppSpacing.small),
                // Kullanıcının "kuruldu mu?" sorusunun cevabı. Kaç gün için
                // kurulduğunu söyler; hangi kayıt olduğunu değil.
                Padding(
                  padding: const EdgeInsets.symmetric(
                    horizontal: AppSpacing.small,
                  ),
                  child: Text(
                    settings.kinds.isEmpty
                        ? 'Hiçbir tür seçili değil; hatırlatma kurulmadı.'
                        : _scheduleSummary(controller.scheduledCount),
                    style: Theme.of(context).textTheme.bodySmall,
                  ),
                ),
              ],
              const SizedBox(height: AppSpacing.medium),
              const AppInlineNotice(
                icon: Icons.lock_outline,
                message:
                    'Bildirimde tutar ve kişi adı yazmaz: kilit ekranında '
                    'görünen bir metin finansal bilgi taşımamalı.',
              ),
            ],
          ),
        ),
      ],
    );
  }

  String _scheduleSummary(int count) => count == 0
      ? 'Önümüzdeki 30 günde hatırlatılacak bir kayıt yok.'
      : 'Önümüzdeki 30 gün için $count güne hatırlatma kuruldu.';

  Future<void> _pickTime() async {
    final settings = widget.controller.settings;
    final picked = await showTimePicker(
      context: context,
      initialTime: TimeOfDay(hour: settings.hour, minute: settings.minute),
    );
    if (picked == null) return;
    await widget.controller.setTime(hour: picked.hour, minute: picked.minute);
  }
}
