using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Amido.Azure.Storage.TableStorage.Tests.TestConfigProcess
{
    public class TestConfigurations 
    {
        public static readonly string DefaultTestConfigFilePath = @"TestConfigurations.xml";
        public string TargetTenantName { get; internal set; }
        public List<TenantConfiguration> TenantConfigurations { get; internal set; }

        public static TestConfigurations ReadFromXml(XDocument testConfigurationsDoc) 
        {
            var testConfigurationsElement = testConfigurationsDoc.Element("TestConfigurations");
            return ReadFromXml(testConfigurationsElement);
        }

        public static TestConfigurations ReadFromXml(XElement testConfigurationsElement) 
        {
            var result = new TestConfigurations
                {
                    TargetTenantName = (string) testConfigurationsElement.Element("TargetTestTenant"),
                    TenantConfigurations = new List<TenantConfiguration>()
                };

            foreach (var config in testConfigurationsElement.Element("TenantConfigurations").Elements("TenantConfiguration").Select(tenantConfigurationElement => new TenantConfiguration
                {
                    TenantName = (string) tenantConfigurationElement.Element("TenantName"),
                    AccountName = (string) tenantConfigurationElement.Element("AccountName"),
                    AccountKey = (string) tenantConfigurationElement.Element("AccountKey"),
                    BlobServiceEndpoint = (string) tenantConfigurationElement.Element("BlobServiceEndpoint"),
                    QueueServiceEndpoint = (string) tenantConfigurationElement.Element("QueueServiceEndpoint"),
                    TableServiceEndpoint = (string) tenantConfigurationElement.Element("TableServiceEndpoint"),
                    TenantType =
                        (TenantType)
                        Enum.Parse(typeof (TenantType), (string) tenantConfigurationElement.Element("TenantType"), true)
                }))
            {
                result.TenantConfigurations.Add(config);
            }

            return result;
        }
    }
}