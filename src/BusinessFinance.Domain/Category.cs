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

    /// <summary>
    /// Bu kategoriye yazılan giderlerin indirilebilirlik varsayılanı
    /// (ADR 0016). Yalnız gider kategorisinde anlamlıdır.
    /// </summary>
    /// <remarks>
    /// Boş olması meşrudur ve "bu kategori cevabı belirlemiyor" demektir.
    /// Ticari mal alımı indirilir, trafik cezası indirilmez — ikisi de aynı
    /// kullanıcının işletme gideridir, ayıran şey kategoridir. Kullanıcı her
    /// kayıtta varsayılanı düzeltebilir; varsayılanı sonradan değiştirmek
    /// <b>geçmiş kayıtları değiştirmez</b>, tıpkı kapsam etiketinde olduğu gibi.
    /// </remarks>
    public bool? DefaultIsTaxDeductible { get; private set; }

    public Category(
        Guid id,
        Guid userId,
        string name,
        CategoryType type,
        TransactionScope? defaultScope = null,
        bool? defaultIsTaxDeductible = null)
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
        DefaultIsTaxDeductible = ValidateDeductibilityDefault(
            defaultIsTaxDeductible,
            type,
            nameof(defaultIsTaxDeductible));
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


    /// <summary>
    /// İndirilebilirlik varsayılanını belirler; <c>null</c> vermek kaldırır.
    /// </summary>
    public void SetDefaultTaxDeductibility(bool? defaultIsTaxDeductible)
    {
        DefaultIsTaxDeductible = ValidateDeductibilityDefault(
            defaultIsTaxDeductible,
            Type,
            nameof(defaultIsTaxDeductible));
    }

    private static bool? ValidateDeductibilityDefault(
        bool? defaultIsTaxDeductible,
        CategoryType type,
        string parameterName)
    {
        if (defaultIsTaxDeductible is not null && type != CategoryType.Expense)
        {
            throw new ArgumentException(
                "Only an expense category can carry a tax deductibility default.",
                parameterName);
        }

        return defaultIsTaxDeductible;
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
