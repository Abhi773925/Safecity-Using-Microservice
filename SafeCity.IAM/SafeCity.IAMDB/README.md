# SafeCity.IAMDB

Database-per-service persistence library for `SafeCity.IAM`.

## What this contains

- `Data/SafeCityDbContext.cs`: IAM service database context
- `Entities/User.cs`: IAM user aggregate model
- `Enums/UserStatus.cs`: user status enum
- `Enums/UserRoleOption.cs`: role enum used by IAM

## Usage from IAM API

```csharp
var connectionString = builder.Configuration.GetConnectionString("IAMDatabase");

if (string.IsNullOrWhiteSpace(connectionString))
{
  connectionString = "Server=LTIN718874\\SQLEXPRESS;Database=SafeCity_IAMDB;Trusted_Connection=True;TrustServerCertificate=True;";
}

builder.Services.AddDbContext<SafeCityDbContext>(options =>
{
  options.UseSqlServer(connectionString);
});
```

## Connection string

Set in API `appsettings.json`:

```json
"ConnectionStrings": {
  "IAMDatabase": "Server=(localdb)\\\\MSSQLLocalDB;Database=SafeCity_IAMDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
