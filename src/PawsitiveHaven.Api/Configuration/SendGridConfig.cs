namespace PawsitiveHaven.Api.Configuration;

public class SendGridConfig
{
    public string ApiKey { get; set; } = string.Empty;
    public string FromEmail { get; set; } = "noreply@pawsitivehaven.org";
    public string FromName { get; set; } = "Pawsitive Haven AI Assistant";
    public string EscalationEmail { get; set; } = "fostersupport@pawsitivehaven.org";
}
