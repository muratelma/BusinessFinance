namespace BusinessFinance.Domain;

/// <summary>
/// Bir giderin vergi matrahından düşülebilir sayılıp sayılmadığı.
/// </summary>
/// <remarks>
/// ADR 0016'nın üçüncü kararı. Bu alan <b>kapsamdan ayrıdır</b>: kapsam "bu
/// para kimin?" sorusunu, indirilebilirlik "bu gider matrahtan düşülebilir mi?"
/// sorusunu yanıtlar. Her işletme gideri indirilebilir değildir — trafik cezası
/// işletmenin giderdir ama indirilemez. İkisini tek alanda birleştirmek,
/// ADR 0013'ün reddettiği "kapsamı başka bir şeyle temsil etme" hatasının
/// tekrarı olurdu.
///
/// Alan <b>iki durumludur</b>: kısmi indirilebilirlik oranı modellenmez, çünkü
/// oran girmek hesaplamaya giden ilk adımdır ve kısmi durumların kuralı
/// muhasebecinindir. Boş olması üçüncü bir durum değil, <b>sorunun
/// sorulmamış</b> olmasıdır: şahsi kayıtta soru anlamsızdır, gelir kaydında
/// yoktur ve alandan önce yazılmış kayıtlarda cevap bilinmez.
///
/// İndirilebilirlik <b>işletme netini değiştirmez</b>. Nakit esaslı işletme
/// neti paranın hareketini ölçer; indirilebilirlik matrahı ilgilendirir ve
/// matrah bu üründe hesaplanmaz. Etkilediği tek çıktı muhasebeci paketidir.
/// </remarks>
public static class TaxDeductibility
{
    /// <summary>
    /// Bir kaydın indirilebilirlik cevabını doğrular.
    /// </summary>
    /// <param name="isTaxDeductible">Cevap; <c>null</c> "sorulmadı" demektir.</param>
    /// <param name="scope">Kaydın kapsamı.</param>
    /// <param name="recognizesExpense">Kayıt gider mi tanıyor.</param>
    public static bool? Validate(
        bool? isTaxDeductible,
        TransactionScope scope,
        bool recognizesExpense,
        string parameterName)
    {
        if (isTaxDeductible is null)
        {
            return null;
        }

        if (scope != TransactionScope.Business)
        {
            throw new ArgumentException(
                "Tax deductibility is only meaningful for business records; " +
                "a personal record is not asked the question.",
                parameterName);
        }

        if (!recognizesExpense)
        {
            throw new ArgumentException(
                "Tax deductibility is only meaningful for records that recognize an expense.",
                parameterName);
        }

        return isTaxDeductible;
    }
}
