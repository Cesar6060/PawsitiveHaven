using SendGrid;
using SendGrid.Helpers.Mail;
using PawsitiveHaven.Api.Configuration;
using PawsitiveHaven.Api.Models.Entities;

namespace PawsitiveHaven.Api.Services;

public interface IEmailService
{
    Task<bool> SendEscalationEmailAsync(Escalation escalation, List<ConversationMessage> conversationHistory);
}

public class EmailService : IEmailService
{
    private readonly SendGridClient? _client;
    private readonly SendGridConfig _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(SendGridConfig config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;

        if (!string.IsNullOrEmpty(config.ApiKey))
        {
            _client = new SendGridClient(config.ApiKey);
        }
        else
        {
            _logger.LogWarning("SendGrid API key not configured. Email sending will be disabled.");
        }
    }

    public async Task<bool> SendEscalationEmailAsync(Escalation escalation, List<ConversationMessage> conversationHistory)
    {
        if (_client == null)
        {
            _logger.LogWarning("Cannot send escalation email - SendGrid not configured");
            return false;
        }

        try
        {
            var from = new EmailAddress(_config.FromEmail, _config.FromName);
            var to = new EmailAddress(_config.EscalationEmail, "Foster Support Team");
            var subject = $"Support Request: {escalation.UserQuestion[..Math.Min(50, escalation.UserQuestion.Length)]}...";

            var htmlContent = BuildEscalationEmailHtml(escalation, conversationHistory);
            var plainTextContent = BuildEscalationEmailPlainText(escalation, conversationHistory);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            // Set reply-to as the user's email
            msg.SetReplyTo(new EmailAddress(escalation.UserEmail, escalation.UserName));

            var response = await _client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Escalation email sent successfully for escalation {EscalationId}", escalation.Id);
                return true;
            }
            else
            {
                var responseBody = await response.Body.ReadAsStringAsync();
                _logger.LogError("Failed to send escalation email. Status: {StatusCode}, Body: {Body}",
                    response.StatusCode, responseBody);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending escalation email for escalation {EscalationId}", escalation.Id);
            return false;
        }
    }

    private string BuildEscalationEmailHtml(Escalation escalation, List<ConversationMessage> conversationHistory)
    {
        var conversationHtml = string.Join("", conversationHistory
            .OrderBy(m => m.CreatedAt)
            .Select(m => $@"
                <div style=""margin-bottom: 12px; padding: 10px; background-color: {(m.Role == "user" ? "#e3f2fd" : "#f5f5f5")}; border-radius: 8px;"">
                    <strong style=""color: {(m.Role == "user" ? "#1976d2" : "#666")};"">
                        {(m.Role == "user" ? escalation.UserName : "AI Assistant")}
                    </strong>
                    <span style=""color: #999; font-size: 12px; margin-left: 10px;"">
                        {m.CreatedAt:MMM dd, yyyy h:mm tt}
                    </span>
                    <p style=""margin: 8px 0 0 0; white-space: pre-wrap;"">{System.Net.WebUtility.HtmlEncode(m.Content)}</p>
                </div>"));

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
</head>
<body style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <div style=""background: linear-gradient(135deg, #D97706 0%, #F59E0B 100%); padding: 20px; border-radius: 8px 8px 0 0;"">
        <h1 style=""color: white; margin: 0; font-size: 24px;"">Support Request</h1>
        <p style=""color: rgba(255,255,255,0.9); margin: 5px 0 0 0;"">Pawsitive Haven AI Assistant Escalation</p>
    </div>

    <div style=""background: #fff; border: 1px solid #e5e5e5; border-top: none; padding: 20px; border-radius: 0 0 8px 8px;"">
        <h2 style=""color: #D97706; font-size: 18px; margin-top: 0;"">User Information</h2>
        <table style=""width: 100%; border-collapse: collapse;"">
            <tr>
                <td style=""padding: 8px 0; border-bottom: 1px solid #eee;""><strong>Name:</strong></td>
                <td style=""padding: 8px 0; border-bottom: 1px solid #eee;"">{System.Net.WebUtility.HtmlEncode(escalation.UserName)}</td>
            </tr>
            <tr>
                <td style=""padding: 8px 0; border-bottom: 1px solid #eee;""><strong>Email:</strong></td>
                <td style=""padding: 8px 0; border-bottom: 1px solid #eee;"">
                    <a href=""mailto:{System.Net.WebUtility.HtmlEncode(escalation.UserEmail)}"">{System.Net.WebUtility.HtmlEncode(escalation.UserEmail)}</a>
                </td>
            </tr>
            <tr>
                <td style=""padding: 8px 0; border-bottom: 1px solid #eee;""><strong>Submitted:</strong></td>
                <td style=""padding: 8px 0; border-bottom: 1px solid #eee;"">{escalation.CreatedAt:MMMM dd, yyyy 'at' h:mm tt} UTC</td>
            </tr>
        </table>

        <h2 style=""color: #D97706; font-size: 18px; margin-top: 24px;"">Question</h2>
        <div style=""background: #FFF7ED; padding: 15px; border-radius: 8px; border-left: 4px solid #D97706;"">
            <p style=""margin: 0; white-space: pre-wrap;"">{System.Net.WebUtility.HtmlEncode(escalation.UserQuestion)}</p>
        </div>

        {(string.IsNullOrEmpty(escalation.AdditionalContext) ? "" : $@"
        <h2 style=""color: #D97706; font-size: 18px; margin-top: 24px;"">Additional Context</h2>
        <div style=""background: #f5f5f5; padding: 15px; border-radius: 8px;"">
            <p style=""margin: 0; white-space: pre-wrap;"">{System.Net.WebUtility.HtmlEncode(escalation.AdditionalContext)}</p>
        </div>
        ")}

        {(conversationHistory.Count > 0 ? $@"
        <h2 style=""color: #D97706; font-size: 18px; margin-top: 24px;"">Conversation History</h2>
        <div style=""background: #fafafa; padding: 15px; border-radius: 8px; max-height: 400px; overflow-y: auto;"">
            {conversationHtml}
        </div>
        " : "")}

        <div style=""margin-top: 24px; padding-top: 20px; border-top: 1px solid #eee; text-align: center; color: #999; font-size: 12px;"">
            <p>This email was sent from the Pawsitive Haven AI Assistant.</p>
            <p>Reply directly to this email to respond to the user.</p>
        </div>
    </div>
</body>
</html>";
    }

    private string BuildEscalationEmailPlainText(Escalation escalation, List<ConversationMessage> conversationHistory)
    {
        var conversationText = string.Join("\n\n", conversationHistory
            .OrderBy(m => m.CreatedAt)
            .Select(m => $"[{(m.Role == "user" ? escalation.UserName : "AI Assistant")} - {m.CreatedAt:MMM dd, yyyy h:mm tt}]\n{m.Content}"));

        return $@"SUPPORT REQUEST - Pawsitive Haven AI Assistant Escalation
==========================================================

USER INFORMATION
----------------
Name: {escalation.UserName}
Email: {escalation.UserEmail}
Submitted: {escalation.CreatedAt:MMMM dd, yyyy 'at' h:mm tt} UTC

QUESTION
--------
{escalation.UserQuestion}

{(string.IsNullOrEmpty(escalation.AdditionalContext) ? "" : $@"ADDITIONAL CONTEXT
------------------
{escalation.AdditionalContext}

")}
{(conversationHistory.Count > 0 ? $@"CONVERSATION HISTORY
--------------------
{conversationText}
" : "")}
---
This email was sent from the Pawsitive Haven AI Assistant.
Reply directly to this email to respond to the user.";
    }
}
