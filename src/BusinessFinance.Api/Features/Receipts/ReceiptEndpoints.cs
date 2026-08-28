using Microsoft.AspNetCore.Mvc;
using BusinessFinance.Api.Contracts;
using BusinessFinance.Api.Errors;
using BusinessFinance.Api.Extensions;
using BusinessFinance.Application.Abstractions.Results;
using BusinessFinance.Application.Receipts;

namespace BusinessFinance.Api.Features.Receipts;

public static class ReceiptEndpoints
{
    private const long MaximumMultipartBodyBytes =
        AnalyzeReceiptUseCase.MaximumFileSizeBytes + 65_536;

    public static IEndpointRouteBuilder MapReceiptEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/v1/receipts/analyze", AnalyzeAsync)
            .WithName("AnalyzeReceipt")
            .WithTags("Receipts")
            .RequireAuthorization()
            .RequireRateLimiting(RateLimitingExtensions.ReceiptAnalysisPolicy)
            .Accepts<IFormFile>("multipart/form-data")
            .Produces<ReceiptAnalysisResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status413PayloadTooLarge)
            .ProducesProblem(StatusCodes.Status429TooManyRequests)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable)
            .WithMetadata(new RequestSizeLimitAttribute(MaximumMultipartBodyBytes))
            .DisableAntiforgery();

        return endpoints;
    }

    private static async Task<IResult> AnalyzeAsync(
        HttpRequest request,
        AnalyzeReceiptUseCase useCase,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        if (request.ContentLength > MaximumMultipartBodyBytes)
        {
            return ApiProblemResults.Create(
                httpContext,
                StatusCodes.Status413PayloadTooLarge,
                "Payload is too large.",
                "Fiş fotoğrafı 5 MiB boyut sınırını aşıyor.",
                "receipt.file_too_large");
        }

        if (!request.HasFormContentType)
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Content-Type multipart/form-data olmalıdır.",
                "receipt.unsupported_file");
        }

        var form = await request.ReadFormAsync(cancellationToken);
        var file = form.Files.GetFile("file");
        if (file is null || file.Length == 0)
        {
            return ApiProblemResults.Validation(
                httpContext,
                "Boş olmayan bir fiş fotoğrafı seçin.",
                "receipt.unsupported_file");
        }

        if (file.Length > AnalyzeReceiptUseCase.MaximumFileSizeBytes)
        {
            return ApiProblemResults.Create(
                httpContext,
                StatusCodes.Status413PayloadTooLarge,
                "Payload is too large.",
                "Fiş fotoğrafı 5 MiB boyut sınırını aşıyor.",
                "receipt.file_too_large");
        }

        if (!TryReadIntent(form["intent"], out var intent))
        {
            return ApiProblemResults.Validation(
                httpContext,
                "intent yalnız \"expense\", \"income\", \"transfer\" veya "
                    + "\"bank_slip\" olabilir.",
                "receipt.unsupported_file");
        }

        await using var stream = file.OpenReadStream();
        var result = await useCase.ExecuteAsync(
            new AnalyzeReceiptCommand(
                file.FileName,
                file.ContentType,
                stream,
                file.Length,
                intent),
            cancellationToken);

        return result.IsSuccess
            ? Results.Ok(ToResponse(result.Value.Draft))
            : ToProblemResult(result.Error, httpContext);
    }

    internal static ReceiptAnalysisResponse ToResponse(ReceiptDraft draft) => new(
        DocumentKind(draft.DocumentKind),
        draft.CounterpartyName,
        FieldState(draft.CounterpartyState),
        draft.CounterpartyId,
        draft.PurchasedAt is DateOnly purchasedAt ? FinanceContract.Date(purchasedAt) : null,
        FieldState(draft.PurchasedAtState),
        draft.DueDate is DateOnly dueDate ? FinanceContract.Date(dueDate) : null,
        FieldState(draft.DueDateState),
        draft.TotalAmount is decimal amount ? FinanceContract.Money(amount) : null,
        FieldState(draft.TotalAmountState),
        draft.Vat is null
            ? null
            : new VatContract(
                FinanceContract.OptionalMoney(draft.Vat.Rate),
                FinanceContract.OptionalMoney(draft.Vat.Amount)),
        FieldState(draft.VatState),
        draft.FeeAmount is decimal fee ? FinanceContract.Money(fee) : null,
        FieldState(draft.FeeAmountState),
        draft.InstallmentCount,
        draft.CurrencyCode,
        PaymentHint(draft.PaymentHint),
        draft.CategoryId,
        draft.CategoryName,
        FieldState(draft.CategoryState),
        draft.Warnings.Select(warning =>
            new ReceiptWarningResponse(warning.Code, warning.Message)).ToArray(),
        draft.RefundMatch is null
            ? null
            : new ReceiptRefundMatchResponse(
                draft.RefundMatch.TransactionId,
                FinanceContract.Date(draft.RefundMatch.TransactionDate),
                FinanceContract.Money(draft.RefundMatch.Amount),
                draft.RefundMatch.Description,
                draft.RefundMatch.RemainingAmount is decimal remaining
                    ? FinanceContract.Money(remaining)
                    : null));

    private static string FieldState(ReceiptFieldState state) => state switch
    {
        ReceiptFieldState.Read => "read",
        ReceiptFieldState.Suspect => "suspect",
        ReceiptFieldState.Missing => "missing",
        _ => throw new ArgumentOutOfRangeException(nameof(state))
    };

    /// <summary>
    /// Missing means expense: that is the direction the product assumed before
    /// 12.11 and the one a client that has not been updated still means. An
    /// unrecognised value is a different thing — it is a client saying something
    /// the server does not understand, and guessing the direction there is
    /// exactly the guess this feature exists to avoid.
    /// </summary>
    private static bool TryReadIntent(string? raw, out ReceiptCaptureIntent intent)
    {
        intent = ReceiptCaptureIntent.Expense;
        if (string.IsNullOrWhiteSpace(raw))
            return true;

        switch (raw.Trim().ToLowerInvariant())
        {
            case "expense":
                return true;
            case "income":
                intent = ReceiptCaptureIntent.Income;
                return true;
            case "transfer":
                intent = ReceiptCaptureIntent.Transfer;
                return true;
            // Yön değil, belge sınıfı: yön okuma bittikten sonra karar
            // sayfasında sorulur.
            case "bank_slip":
                intent = ReceiptCaptureIntent.BankSlip;
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Yalnız kabul edilen türler buraya gelir; gerisi kapıda reddedilir. Yine de
    /// tanınmayan bir değer sessizce "fiş" sayılmaz, açıkça patlar.
    /// </summary>
    private static string DocumentKind(ReceiptDocumentKind kind) => kind switch
    {
        ReceiptDocumentKind.PurchaseReceipt => "purchase_receipt",
        ReceiptDocumentKind.InvoiceOrVoucher => "invoice_or_voucher",
        ReceiptDocumentKind.BankDocument => "bank_document",
        ReceiptDocumentKind.BankPayment => "bank_payment",
        ReceiptDocumentKind.CardPaymentSlip => "bank_card_payment",
        ReceiptDocumentKind.RefundReceipt => "refund_receipt",
        _ => throw new ArgumentOutOfRangeException(nameof(kind))
    };

    private static string PaymentHint(ReceiptPaymentHint hint) => hint switch
    {
        ReceiptPaymentHint.Unknown => "unknown",
        ReceiptPaymentHint.Cash => "cash",
        ReceiptPaymentHint.Card => "card",
        ReceiptPaymentHint.CreditCard => "credit_card",
        ReceiptPaymentHint.DebitCard => "debit_card",
        _ => throw new ArgumentOutOfRangeException(nameof(hint))
    };

    private static IResult ToProblemResult(
        ApplicationError error,
        HttpContext httpContext) => error.Code switch
        {
            "receipt.provider_rate_limited" => ApiProblemResults.Create(
                httpContext,
                StatusCodes.Status429TooManyRequests,
                "Receipt provider rate limit reached.",
                error.Message,
                error.Code),
            "receipt.provider_unavailable" or "receipt.disabled" => ApiProblemResults.Create(
                httpContext,
                StatusCodes.Status503ServiceUnavailable,
                "Receipt analysis is unavailable.",
                error.Message,
                error.Code),
            _ => error.ToProblemResult(httpContext)
        };
}
