namespace BusinessFinance.Domain;

public sealed class Category
{
    public const int MaximumNameLength = 100;

    public Guid Id { get; }
    public Guid UserId { get; }
    public string Name { get; private set; }
    public CategoryType Type { get; }
    public bool IsActive { get; private set; }

    /// <summary>
    /// Bu kategori üzerinden girilen kayıtların varsayılan kapsamı.
    /// </summary>
    /// <remarks>
    /// Boş olması meşrudur ve eksik veri değildir: tek kategori setiyle her şeyi
    /// yöneten esnaf için kapsam kategoriden türer. Boş bırakmak "kapsamı
    /// bilmiyorum" değil, "bu kategori kapsamı belirlemiyor" demektir.
    /// </remarks>
    public TransactionScope? DefaultScope { get; private set; }

    public Category(
        Guid id,
        Guid userId,
        string name,
        CategoryType type,
        TransactionScope? defaultScope = null)
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
        DefaultScope = TransactionScopeGuard.ValidateOptional(
            defaultScope,
            nameof(defaultScope));
    }

    /// <summary>
    /// Varsayılan kapsamı belirler; <c>null</c> vermek etiketi kaldırır.
    /// </summary>
    public void SetDefaultScope(TransactionScope? defaultScope)
    {
        DefaultScope = TransactionScopeGuard.ValidateOptional(
            defaultScope,
            nameof(defaultScope));
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
