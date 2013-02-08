namespace Amido.Azure.Storage.TableStorage.Tests.TestConfigProcess
{
    public class TenantConfiguration {
        public string TenantName { get; internal set; }
        public string AccountName { get; internal set; }
        public string AccountKey { get; internal set; }
        public string BlobServiceEndpoint { get; internal set; }
        public string QueueServiceEndpoint { get; internal set; }
        public string TableServiceEndpoint { get; internal set; }
        public TenantType TenantType { get; internal set; }
    }
}