namespace BusinessFinance.CSharpSandbox;

public interface IExpenseDraftSource
{
    Task<List<ExpenseDraft>> GetExpensesAsync();
}
