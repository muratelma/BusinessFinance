namespace BusinessFinance.CSharpSandbox;


public class FailingExpenseDraftSource : IExpenseDraftSource
{
    public async Task<List<ExpenseDraft>> GetExpensesAsync()
    {
        await Task.Delay(100);

        throw new InvalidOperationException(
            "Sentetik veri kaynağı hatası.");
    }
}

