using System.Security.Cryptography;
using System.Text;

namespace BusinessFinance.Application.Abstractions.Identifiers;

/// <summary>
/// İstemcinin istek kimliğinden, o kullanıcıya özgü ve her seferinde aynı
/// çıkan bir kayıt kimliği türetir.
/// </summary>
/// <remarks>
/// <para>
/// Tekrar gönderilen istek (zayıf bağlantıda ikinci dokunuş, istemcinin
/// yeniden denemesi) aynı kimliği üretir ve ikinci bir kayıt yazılmaz: kayıt
/// zaten varsa o döner. Bu, gider tablosuna bir "istek kimliği" kolonu
/// eklemeden idempotentlik sağlar.
/// </para>
/// <para>
/// Kimlik kullanıcı kimliğiyle karıştırılır: iki kullanıcı aynı istek
/// kimliğini gönderse bile farklı kayıt kimlikleri çıkar, biri diğerinin
/// kaydına çarpamaz ve bir kaydın varlığını yoklayamaz. Amaç (<c>purpose</c>)
/// da karışır, böylece aynı istek kimliği iki ayrı akışta aynı kaydı üretmez.
/// </para>
/// </remarks>
public static class RequestScopedId
{
    public static Guid Create(string purpose, Guid userId, Guid clientRequestId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(purpose);
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));
        if (clientRequestId == Guid.Empty)
        {
            throw new ArgumentException("Client request id cannot be empty.", nameof(clientRequestId));
        }

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes($"{purpose}:{userId:N}:{clientRequestId:N}"));
        var bytes = hash.AsSpan(0, 16).ToArray();

        // RFC 9562 biçimi: sürüm 8 (özel), varyant 10xx. Rastgele kimliklerle
        // aynı alanı paylaşır ama onlarla karışmaz.
        bytes[7] = (byte)((bytes[7] & 0x0F) | 0x80);
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);
        return new Guid(bytes);
    }
}
