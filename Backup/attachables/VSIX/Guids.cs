// Guids.cs
// MUST match guids.h
using System;

namespace ninlabs.attachables
{
    static class GuidList
    {
        public const string guidAttachablesPkgString = "2f0b2fc6-4251-4a42-a756-e3e6051bff7b";
        public const string guidAttachablesCmdSetString = "f8be7478-aa94-432d-8397-9cc1f5f330bc";
        public const string guidToolWindowPersistanceString = "582d2a69-ca21-4f67-a399-1aba2dd9803e";

        public static readonly Guid guidAttachablesCmdSet = new Guid(guidAttachablesCmdSetString);
    };
}