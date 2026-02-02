namespace PawsitiveHaven.Api.Configuration;

public class OpenAiAssistantConfig
{
    public string ApiKey { get; set; } = string.Empty;
    public string? AssistantId { get; set; }
    public string? VectorStoreId { get; set; }
    public string Model { get; set; } = "gpt-4o-mini";
    public string AssistantName { get; set; } = "Pawsitive Haven Assistant";
}
