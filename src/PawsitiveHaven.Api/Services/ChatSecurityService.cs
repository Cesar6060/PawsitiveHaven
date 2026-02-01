using System.Text.RegularExpressions;

namespace PawsitiveHaven.Api.Services;

public interface IChatSecurityService
{
    ChatValidationResult ValidateMessage(string message);
    string SanitizeInput(string message);
    bool DetectPromptInjection(string message);
}

public class ChatSecurityService : IChatSecurityService
{
    private readonly ILogger<ChatSecurityService> _logger;

    // Maximum message length (in characters)
    private const int MaxMessageLength = 2000;

    // Minimum message length
    private const int MinMessageLength = 1;

    // Patterns that indicate prompt injection attempts
    private static readonly string[] InjectionPatterns = new[]
    {
        // Direct instruction override attempts
        @"ignore\s+(all\s+)?(previous|prior|above|earlier)\s+(instructions|prompts|rules|directions)",
        @"disregard\s+(all\s+)?(previous|prior|above|earlier)\s+(instructions|prompts|rules|directions)",
        @"forget\s+(all\s+)?(previous|prior|above|earlier)\s+(instructions|prompts|rules|directions)",
        @"override\s+(all\s+)?(previous|prior|above|earlier)\s+(instructions|prompts|rules)",
        @"do\s+not\s+follow\s+(the\s+)?(previous|prior|above|system)\s+(instructions|prompts|rules)",

        // Role manipulation attempts
        @"you\s+are\s+now\s+(a|an)\s+",
        @"pretend\s+(to\s+be|you\s+are|you're)\s+",
        @"act\s+as\s+(if\s+you\s+(were|are)\s+)?(a|an)?\s*",
        @"roleplay\s+as\s+",
        @"assume\s+the\s+role\s+of",
        @"switch\s+to\s+.+\s+mode",
        @"enter\s+.+\s+mode",
        @"new\s+persona",
        @"from\s+now\s+on\s+you\s+(are|will)",

        // System prompt extraction attempts
        @"(show|tell|reveal|display|print|output|repeat|give)\s+(me\s+)?(your|the)\s+(system\s+)?(prompt|instructions|rules|guidelines|configuration)",
        @"what\s+(are|is)\s+your\s+(system\s+)?(prompt|instructions|rules)",
        @"(initial|original|starting)\s+(prompt|instructions)",

        // Jailbreak markers
        @"\[system\]",
        @"\[admin\]",
        @"\[override\]",
        @"\[developer\]",
        @"\[sudo\]",
        @"\[root\]",
        @"<<<.+>>>",
        @"\{\{.+\}\}",
        @"<!--.*-->",
        @"<\|.*\|>",

        // DAN/jailbreak specific patterns
        @"dan\s*(mode)?",
        @"developer\s+mode",
        @"jailbreak",
        @"bypass\s+(restrictions|filters|safety)",
        @"no\s+restrictions",
        @"unrestricted\s+mode",
        @"without\s+(any\s+)?limitations",

        // Social engineering patterns
        @"for\s+(educational|research|testing)\s+purposes",
        @"this\s+is\s+(just\s+)?(a\s+)?test",
        @"hypothetically",
        @"my\s+(grandmother|grandma|mom|dad)\s+(used\s+to|would)\s+(tell|read|say)",
    };

    // Compiled regex patterns for performance
    private static readonly Regex[] CompiledPatterns = InjectionPatterns
        .Select(p => new Regex(p, RegexOptions.IgnoreCase | RegexOptions.Compiled))
        .ToArray();

    public ChatSecurityService(ILogger<ChatSecurityService> logger)
    {
        _logger = logger;
    }

    public ChatValidationResult ValidateMessage(string message)
    {
        // Null or empty check
        if (string.IsNullOrWhiteSpace(message))
        {
            return ChatValidationResult.Failure("Message cannot be empty.");
        }

        // Sanitize first
        var sanitized = SanitizeInput(message);

        // Length validation
        if (sanitized.Length < MinMessageLength)
        {
            return ChatValidationResult.Failure("Message is too short.");
        }

        if (sanitized.Length > MaxMessageLength)
        {
            return ChatValidationResult.Failure($"Message exceeds maximum length of {MaxMessageLength} characters.");
        }

        // Prompt injection detection
        if (DetectPromptInjection(sanitized))
        {
            _logger.LogWarning("Prompt injection attempt detected in message");
            return ChatValidationResult.Failure("Your message couldn't be processed. Please rephrase your question about pet care.");
        }

        return ChatValidationResult.Success(sanitized);
    }

    public string SanitizeInput(string message)
    {
        if (string.IsNullOrEmpty(message))
            return string.Empty;

        var sanitized = message;

        // Trim whitespace
        sanitized = sanitized.Trim();

        // Remove control characters (except newlines and tabs)
        sanitized = Regex.Replace(sanitized, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");

        // Normalize Unicode (prevent homoglyph attacks)
        sanitized = sanitized.Normalize(System.Text.NormalizationForm.FormKC);

        // Remove zero-width characters
        sanitized = Regex.Replace(sanitized, @"[\u200B-\u200D\uFEFF]", "");

        // Remove RTL/LTR override characters
        sanitized = Regex.Replace(sanitized, @"[\u202A-\u202E\u2066-\u2069]", "");

        // Collapse multiple newlines to max 2
        sanitized = Regex.Replace(sanitized, @"\n{3,}", "\n\n");

        // Collapse multiple spaces to single space
        sanitized = Regex.Replace(sanitized, @" {2,}", " ");

        return sanitized;
    }

    public bool DetectPromptInjection(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;

        // Check against all compiled patterns
        foreach (var pattern in CompiledPatterns)
        {
            if (pattern.IsMatch(message))
            {
                _logger.LogWarning(
                    "Prompt injection pattern matched: {PatternDescription}",
                    pattern.ToString().Length > 50
                        ? pattern.ToString()[..50] + "..."
                        : pattern.ToString()
                );
                return true;
            }
        }

        // Check for excessive special characters (potential encoding attack)
        var specialCharRatio = (double)message.Count(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)) / message.Length;
        if (specialCharRatio > 0.3 && message.Length > 50)
        {
            _logger.LogWarning("Suspicious special character ratio: {Ratio:P}", specialCharRatio);
            return true;
        }

        return false;
    }
}

public class ChatValidationResult
{
    public bool IsValid { get; }
    public string? ErrorMessage { get; }
    public string? SanitizedMessage { get; }

    private ChatValidationResult(bool isValid, string? errorMessage, string? sanitizedMessage)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
        SanitizedMessage = sanitizedMessage;
    }

    public static ChatValidationResult Success(string sanitizedMessage)
        => new(true, null, sanitizedMessage);

    public static ChatValidationResult Failure(string errorMessage)
        => new(false, errorMessage, null);
}
