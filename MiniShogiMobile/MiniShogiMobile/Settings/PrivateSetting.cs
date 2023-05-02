using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MiniShogiMobile.Settings
{
    [DataContract]
    public class PrivateSetting
    {
        [DataMember]
        public string AdUnitIdForBanner { get; set; }
        [DataMember]
        public string AdUnitIdForInterstitial{ get; set; }
    }
}
