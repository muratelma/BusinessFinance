import 'package:business_finance_mobile/core/models/transaction_scope.dart';
import 'package:flutter_test/flutter_test.dart';

/// Taraf kuralının istemcideki önizlemesi (ADR 0020). Sunucudaki durum
/// tablosunun (`TransactionScopeResolutionTests`) aynı satırları: form,
/// sunucunun reddedeceği bir tarafı göstermemeli ve göndermemeli.
void main() {
  const business = TransactionScope.business;
  const personal = TransactionScope.personal;

  group('previewResolvedScope', () {
    test('tek taraflı kategori tarafı söyler; etiket ve seçim onu ezemez', () {
      expect(previewResolvedScope(category: personal), personal);
      expect(
        previewResolvedScope(category: personal, source: business),
        personal,
      );
      expect(
        previewResolvedScope(category: personal, explicit: business),
        personal,
      );
    });

    test('iki tarafa açık kategoride seçim, yoksa kaynağın etiketi', () {
      expect(
        previewResolvedScope(explicit: personal, source: business),
        personal,
      );
      expect(previewResolvedScope(source: business), business);
    });

    test('hiçbir işaret yoksa taraf uydurulmaz', () {
      expect(previewResolvedScope(), isNull);
    });
  });

  group('scopePreviewHelperText', () {
    test('ön değerin kaynağın etiketinden geldiğini adıyla söyler', () {
      expect(
        scopePreviewHelperText(source: business, sourceName: 'Dükkân kasası'),
        'Dükkân kasası etiketinden geldi — değiştirebilirsiniz.',
      );
    });

    test('seçim ve boş durum', () {
      expect(
        scopePreviewHelperText(explicit: personal),
        'Bu kayıt için siz seçtiniz.',
      );
      expect(scopePreviewHelperText(), 'Bu kayıt için seçin.');
    });
  });

  group('categoryAllowsSide', () {
    test('kategori kendi tarafında ve iki tarafa açıkken kullanılabilir', () {
      expect(categoryAllowsSide(business, business), isTrue);
      expect(categoryAllowsSide(null, business), isTrue);
      expect(categoryAllowsSide(personal, business), isFalse);
      expect(categoryAllowsSide(null, personal), isTrue);
    });
  });
}
