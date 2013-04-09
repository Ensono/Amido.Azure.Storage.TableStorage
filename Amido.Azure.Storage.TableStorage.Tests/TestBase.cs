using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Amido.Azure.Storage.TableStorage.Account;
using Amido.Azure.Storage.TableStorage.Tests.Integration;
using Amido.Azure.Storage.TableStorage.Tests.TestConfigProcess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace Amido.Azure.Storage.TableStorage.Tests
{
    [TestClass]
    public abstract class TestBase
    {
        protected static CloudTable CurrentTable = null;
        protected ITableStorageRepository<TestEntity> Repository;
        
        static TestBase() 
        {
            var element = XElement.Load(TestConfigurations.DefaultTestConfigFilePath);
            TestConfigurations = TestConfigurations.ReadFromXml(element);

            foreach (var tenant in TestConfigurations.TenantConfigurations.Where(tenant => tenant.TenantName == TestConfigurations.TargetTenantName))
            {
                TargetTenantConfig = tenant;
                break;
            }

            StorageCredentials = new StorageCredentials(TargetTenantConfig.AccountName,
                TargetTenantConfig.AccountKey);

            CurrentTenantType = TargetTenantConfig.TenantType;
        }

        public TestContext TestContext { get; set; }

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext) 
        {
            var process = Process.Start(@"C:\Program Files\Microsoft SDKs\Windows Azure\Emulator\csrun", "/devstore");
            if(process != null) {
                process.WaitForExit();
            }
            else {
                throw new ApplicationException("Unable to start storage emulator.");
            }
        }

        [TestInitialize]
        public void TestInitialize() 
        {
            CC();
        }

        [TestCleanup]
        public void TestCleanup() 
        {
            DD();
        }

        public virtual void CC() 
        {
            var accountConnection = new AccountConnection<TestEntity>(Properties.Resources.AccountConnectionString);

            Repository = new TableStorageRepository<TestEntity>(accountConnection);
        }

        public virtual void DD() 
        {
        }

        public static CloudTableClient GenerateCloudTableClient() 
        {
            var baseAddressUri = new Uri(TargetTenantConfig.TableServiceEndpoint);
            return new CloudTableClient(baseAddressUri, StorageCredentials);
        }

        public static string GenerateRandomTableName() {
            return "tbl" + Guid.NewGuid().ToString("N");
        }

        public static string GenerateRandomStringFromCharset(int tableNameLength, string legalChars, Random rand) {
            var retString = new StringBuilder();
            for(int n = 0; n < tableNameLength; n++) {
                retString.Append(legalChars[rand.Next(legalChars.Length - 1)]);
            }

            return retString.ToString();
        }
        
        public static TestConfigurations TestConfigurations 
        {
            get;
            private set;
        }

        public static TenantConfiguration TargetTenantConfig 
        {
            get;
            private set;
        }

        public static TenantType CurrentTenantType 
        {
            get;
            private set;
        }

        /// <summary>
        /// The StorageCredentials created from account settings in the target tenant config.
        /// </summary>
        public static StorageCredentials StorageCredentials 
        {
            get;
            private set;
        }
    }
}