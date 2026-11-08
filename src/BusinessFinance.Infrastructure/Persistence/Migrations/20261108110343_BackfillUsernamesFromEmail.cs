using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessFinance.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// Kullanıcı adı alınmadan açılmış hesaplara (Identity'nin <c>UserName</c>'i
    /// e-postanın kopyası) e-postanın <c>@</c> öncesinden bir kullanıcı adı
    /// yazar (kullanıcı kararı, 8 Kasım 2026: o güne kadarki hesapların hepsi
    /// test hesabıdır, kullanıcı adı boş kalmasın).
    /// </summary>
    /// <remarks>
    /// Şema değişmez: kolon ve benzersiz <c>UserNameIndex</c> InitialCreate'ten
    /// beri vardır. Kural uygulamadaki <c>Usernames</c> ile aynıdır: küçük harf;
    /// yalnız a–z, rakam, nokta, alt çizgi ve tire kalır (Türkçe harf dahil
    /// gerisi atılır); 3 karakterden kısa kalırsa başına "kullanici" eklenir;
    /// en çok 30 karakter; başka bir hesapla çakışırsa sonuna sayı eklenir.
    /// Hesaplar açılış sırasıyla işlenir: ilk açılan adı sayısız alır. Kullanıcı
    /// adı zaten seçilmiş (içinde <c>@</c> olmayan) hesaba dokunulmaz; migration
    /// ikinci kez çalışsa da bir şey değiştirmez.
    /// </remarks>
    public partial class BackfillUsernamesFromEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DECLARE @Id uniqueidentifier, @Email nvarchar(256), @Local nvarchar(256),
                        @Base nvarchar(256), @Candidate nvarchar(256), @Ch nchar(1),
                        @N int, @I int;
                DECLARE @Allowed nvarchar(64) =
                    N'abcdefghijklmnopqrstuvwxyz0123456789._-' COLLATE Latin1_General_BIN;

                DECLARE users CURSOR LOCAL FAST_FORWARD FOR
                    SELECT [Id], [Email]
                    FROM [AspNetUsers]
                    WHERE [UserName] LIKE N'%@%' AND [Email] LIKE N'_%@%'
                    ORDER BY [CreatedAtUtc], [Id];

                OPEN users;
                FETCH NEXT FROM users INTO @Id, @Email;
                WHILE @@FETCH_STATUS = 0
                BEGIN
                    SET @Local = LOWER(LEFT(@Email, CHARINDEX(N'@', @Email) - 1));

                    -- Her karakter izin listesinde aranır (ikili harmanlama: Türkçe
                    -- harf İngilizce karşılığı sayılmaz). Karakter sınıfı
                    -- ([^a-z0-9._-]) kullanılmadı: tireyi düz karakter saymadı.
                    SET @Base = N'';
                    SET @I = 1;
                    WHILE @I <= DATALENGTH(@Local) / 2
                    BEGIN
                        SET @Ch = SUBSTRING(@Local, @I, 1);
                        IF CHARINDEX(@Ch COLLATE Latin1_General_BIN, @Allowed) > 0
                            SET @Base = @Base + @Ch;
                        SET @I += 1;
                    END;

                    IF LEN(@Base) < 3 SET @Base = CONCAT(N'kullanici', @Base);
                    SET @Base = LEFT(@Base, 30);

                    SET @Candidate = @Base;
                    SET @N = 1;
                    WHILE EXISTS (
                        SELECT 1 FROM [AspNetUsers]
                        WHERE [NormalizedUserName] = UPPER(@Candidate) AND [Id] <> @Id)
                    BEGIN
                        SET @N += 1;
                        SET @Candidate = CONCAT(
                            LEFT(@Base, 30 - LEN(CAST(@N AS nvarchar(10)))),
                            @N);
                    END;

                    UPDATE [AspNetUsers]
                    SET [UserName] = @Candidate,
                        [NormalizedUserName] = UPPER(@Candidate)
                    WHERE [Id] = @Id;

                    FETCH NEXT FROM users INTO @Id, @Email;
                END;

                CLOSE users;
                DEALLOCATE users;
                """);
        }

        /// <summary>
        /// Geri dönüş her hesabın kullanıcı adını yeniden e-postası yapar;
        /// sonradan seçilmiş kullanıcı adları da kaybolur (kullanıcı adından
        /// önceki şemaya dönüşün anlamı budur).
        /// </summary>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE [AspNetUsers]
                SET [UserName] = [Email],
                    [NormalizedUserName] = [NormalizedEmail]
                WHERE [Email] IS NOT NULL AND [UserName] NOT LIKE N'%@%';
                """);
        }
    }
}
