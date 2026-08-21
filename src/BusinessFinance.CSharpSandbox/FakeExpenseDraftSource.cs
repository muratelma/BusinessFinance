namespace BusinessFinance.CSharpSandbox;

public class FakeExpenseDraftSource : IExpenseDraftSource
{
    public async Task<List<ExpenseDraft>> GetExpensesAsync()
    {
        await Task.Delay(300);

        return new List<ExpenseDraft>
        {
            new(
                150m,
                new DateOnly(2026, 8, 10),
                "İnternet faturası"),
            new(
                75m,
                new DateOnly(2026, 8, 11),
                "Kahve")
        };
    }
}
