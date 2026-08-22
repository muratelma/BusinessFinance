namespace BusinessFinance.Domain;

/// <summary>
/// Bir kaydın işletmeye mi sahibinin cebine mi ait olduğunu söyleyen boyut.
/// </summary>
/// <remarks>
/// Şahıs şirketinde kasa ile cep hukuken ayrılmadığı için para tek havuzda
/// yaşar; kapsam o havuzu bölmez, yalnız gelir/gider raporlarını böler
/// (ADR 0013). Bakiye, kart borcu ve net varlık kapsam filtresinden
/// etkilenmez.
///
/// Üçüncü bir "bilinmiyor" durumu bilerek yoktur. Kapsam boyutu boş bir
/// veritabanına eklendi; yorumlanacak bir geçmiş olmadığı için belirsizliği
/// temsil edecek bir değere de ihtiyaç yok. Kapsamı bilinmeyen bir kayıt
/// yaratılamaz: sunucu kapsam uydurmaz, isteği reddeder.
/// </remarks>
public enum TransactionScope
{
    Business = 1,
    Personal = 2
}

internal static class TransactionScopeGuard
{
    internal static TransactionScope Validate(TransactionScope scope, string parameterName)
    {
        if (!Enum.IsDefined(scope))
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                scope,
                "Transaction scope is not supported.");
        }

        return scope;
    }

    internal static TransactionScope? ValidateOptional(
        TransactionScope? scope,
        string parameterName)
    {
        return scope is null ? null : Validate(scope.Value, parameterName);
    }
}
