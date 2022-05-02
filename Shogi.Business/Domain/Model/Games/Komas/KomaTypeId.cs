using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Komas
{

    public enum KomaTypeKind
    {
        None,
        AsHu,
        AsKing,
    }

    [DataContract]
    public class KomaTypeId
    {

        [DataMember]
        public string UniqueKey { get; private set; }
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string PromotedName { get; private set; }
        [DataMember]
        public KomaTypeKind Kind { get; private set; }

        public KomaTypeId()
        {
            UniqueKey = Guid.Empty.ToString();
            Name = string.Empty;
            PromotedName = string.Empty;
            Kind = KomaTypeKind.None;
        }

        public override string ToString()
        {
            return Name;
        }
        public KomaTypeId(string name, KomaTypeKind kind)
        {
            UniqueKey = Guid.NewGuid().ToString();
            Name = name;
            PromotedName = string.Empty;
            Kind = kind;
        }
        public KomaTypeId(string name, string promotedName, KomaTypeKind kind)
        {
            UniqueKey = Guid.NewGuid().ToString();
            Name = name;
            PromotedName = promotedName;
            Kind = kind;
        }
        public override bool Equals(object obj)
        {
            return obj is KomaTypeId id &&
                   UniqueKey == id.UniqueKey &&
                   Name == id.Name &&
                   PromotedName == id.PromotedName &&
                   Kind == id.Kind;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(UniqueKey, Name, PromotedName, Kind);
        }

        public static bool operator ==(KomaTypeId left, KomaTypeId right)
        {
            return EqualityComparer<KomaTypeId>.Default.Equals(left, right);
        }

        public static bool operator !=(KomaTypeId left, KomaTypeId right)
        {
            return !(left == right);
        }
    }
}
