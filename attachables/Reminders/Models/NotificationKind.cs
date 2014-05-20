using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ninlabs.attachables.Models
{
    [DataContract]
    public enum NotificationType : int
    {
        [EnumMember]
        Viewport = 0x0000,
        [EnumMember]
        BuildError = 0x0001,
        [EnumMember]
        MessageBox = 0x0010, // Gray Out Background...
        [EnumMember]
        None = 0x1000 // Inert reminder.
    }
}
