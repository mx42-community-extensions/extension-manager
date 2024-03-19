using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ExtensionManager.Contracts.DataContracts.Licensing
{
    public class LicenseTransport
    {
        public License License { get; set; }
        public string Signature { get; set; }
    }

    public enum LicenseMetric
    {
        User = 0,
        System = 1,
        Machine = 2
    }

    public class License
    {
        public Guid ExtensionId { get; set; }

        public Guid IssuerId { get; set; }

        public LicenseMetric Metric { get; set; }

        public int Count { get; set; }
        public DateTime? ValidUntil { get; set; }
        public DateTime? ValidFrom { get; set; }
        public ComponentLicense[] Components { get; set; }
    }

    public class ComponentLicense
    {
        public string Name { get; set; }
        public Guid ComponentId { get; set; }

        public DateTime? ValidUntil { get; set; }
        public DateTime? ValidFrom { get; set; }
    }
}
