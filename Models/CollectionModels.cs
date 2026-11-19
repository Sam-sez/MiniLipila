namespace MiniLipila.Models;

public class CollectionRequest
{
    public string ReferenceId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Narration { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? ReferenceData { get; set; }
}

public class CollectionResponse
{
    public string ReferenceId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? PaymentType { get; set; }
    public string? Type { get; set; }
    public string? IpAddress { get; set; }
    public string? CardRedirectionUrl { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public string? Identifier { get; set; }
    public string? Message { get; set; }
}