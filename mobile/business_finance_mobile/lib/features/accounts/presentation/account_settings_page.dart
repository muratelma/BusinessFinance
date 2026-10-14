import 'package:flutter/material.dart';
import '../../../core/network/api_exception.dart';
import '../../../core/widgets/app_state_views.dart';
import '../data/account_models.dart';
import 'account_form_page.dart';

/// Ayarlar dokunulduğu anda açılır; okuma hatası ve tekrar deneme görünür kalır.
class AccountSettingsPage extends StatefulWidget {
  const AccountSettingsPage({
    required this.load,
    required this.onSave,
    required this.onDelete,
    this.readError,
    super.key,
  });
  final Future<Account> Function() load;
  final SaveAccount onSave;
  final DeleteAccount onDelete;
  final String? Function()? readError;
  @override
  State<AccountSettingsPage> createState() => _AccountSettingsPageState();
}

class _AccountSettingsPageState extends State<AccountSettingsPage> {
  late Future<Account> request = widget.load();
  @override
  Widget build(BuildContext context) => FutureBuilder<Account>(
    future: request,
    builder: (context, snapshot) {
      if (snapshot.hasData) {
        return AccountFormPage(
          account: snapshot.data,
          onSave: widget.onSave,
          onDelete: widget.onDelete,
          readError: widget.readError,
        );
      }
      final error = snapshot.error;
      return Scaffold(
        appBar: AppBar(title: const Text('Hesap ayarları')),
        body: error == null
            ? const AppLoadingView(message: 'Hesap ayarları alınıyor')
            : error is ApiException && error.isUnauthorized
            ? const AppUnauthorizedView()
            : AppErrorView(
                message: error is ApiException
                    ? error.message
                    : 'Hesap ayarları alınamadı.',
                onRetry: () => setState(() {
                  request = widget.load();
                }),
              ),
      );
    },
  );
}
