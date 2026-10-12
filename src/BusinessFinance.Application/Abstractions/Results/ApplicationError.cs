namespace BusinessFinance.Application.Abstractions.Results;

/// <param name="Code">Kararlı makine değeri; istemci cümlesini bundan kurar.</param>
/// <param name="Message">Geliştiriciye yönelik açıklama.</param>
/// <param name="Type">HTTP durumuna çevrilen tür.</param>
/// <param name="Details">
/// İstemcinin cümlesini kurarken ihtiyaç duyduğu makine değerleri (ör. onay
/// isteyen cevapta bekleyen kalemin günü). Kullanıcı cümlesi taşımaz.
/// </param>
public sealed record ApplicationError(
    string Code,
    string Message,
    ApplicationErrorType Type,
    IReadOnlyDictionary<string, string>? Details = null);
