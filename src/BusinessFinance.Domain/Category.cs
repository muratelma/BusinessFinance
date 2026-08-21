namespace BusinessFinance.Domain;

public sealed class Category
{
    public const int MaximumNameLength = 100;

    public Guid Id { get; }
    public Guid UserId { get; }
    public string Name { get; private set; }
    public CategoryType Type { get; }
    public bool IsActive { get; private set; }

    public Category(
        Guid id,
        Guid userId,
        string name,
        CategoryType type)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Category id cannot be empty.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User id cannot be empty.", nameof(userId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name is required.", nameof(name));
        }

        var normalizedName = name.Trim();
        if (normalizedName.Length > MaximumNameLength)
        {
            throw new ArgumentException(
                $"Category name cannot exceed {MaximumNameLength} characters.",
                nameof(name));
        }

        if (type is not CategoryType.Income and not CategoryType.Expense)
        {
            throw new ArgumentOutOfRangeException(
                nameof(type),
                type,
                "Category type is not supported.");
        }

        Id = id;
        UserId = userId;
        Name = normalizedName;
        Type = type;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name is required.", nameof(name));
        }

        var normalizedName = name.Trim();
        if (normalizedName.Length > MaximumNameLength)
        {
            throw new ArgumentException(
                $"Category name cannot exceed {MaximumNameLength} characters.",
                nameof(name));
        }

        Name = normalizedName;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
