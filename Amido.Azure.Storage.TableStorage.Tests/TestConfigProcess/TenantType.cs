using System;

namespace Amido.Azure.Storage.TableStorage.Tests.TestConfigProcess
{
    [Flags]
    public enum TenantType 
    {
        None = 0x0,
        DevStore = 0x1,
        DevFabric = 0x2,
        Cloud = 0x4,
        All = 0x7
    }
}