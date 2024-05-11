namespace api;

public class AwsSettings
{
    public required string Region { get; set; } 
    public required string AwsAccessKeyId { get; set; }
    public required string AwsSecretAccessKey { get; set; }
    public required string QueueDepositName { get; set; }
    public required string QueueTransferName { get; set; }


}
