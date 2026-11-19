namespace MiniLipila.Models;

public class DisbursementRequest
{
    public string ReferenceId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Narration { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string? ReferenceData { get; set; }
}

public class DisbursementResponse
{
    public string ReferenceId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ExternalId { get; set; }
    public string Narration { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string? Type { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string? PaymentType { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public string? Identifier { get; set; }
    public string? Message { get; set; }
}