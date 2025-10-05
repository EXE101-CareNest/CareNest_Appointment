# CareNest Appointment Service

## Account Service Integration

### Endpoints
- Get Account by Username: GET /api/accounts/username/{username}

### Usage Example
```csharp
// Inject IAccountService
private readonly IAccountService _accountService;

// Get current user account
var account = await _accountService.GetCurrentAccountAsync();

// The account object contains:
// - Id
// - Username
// - Email
// - Role
// - FirstName
// - LastName
// - Phone
// - Status
```

### Configuration
```json
{
  "ServiceUrls": {
    "Account": "http://localhost:8082"
  }
}
```

### Error Handling
The service will throw:
- `UnauthorizedException` when user is not authenticated
- `Exception` for other errors (API failures, network issues, etc.)

## Authorization Implementation Guide