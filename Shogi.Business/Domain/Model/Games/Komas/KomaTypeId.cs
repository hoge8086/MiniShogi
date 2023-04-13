using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Komas
{

    public enum KomaTypeKind
    {
        [Description("指定なし")]
        None,
        [Description("王として扱う")]
        AsKing,
        [Description("歩として扱う")]
        AsHu,
    }

    [DataContract]
    public class KomaTypeId
    {

        [DataMember]
        private string UniqueKey { get; set; }
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string PromotedName { get; private set; }
        [DataMember]
        public KomaTypeKind Kind { get; private set; }

        public bool IsKing { get => Kind == KomaTypeKind.AsKing; }
        public bool IsHu { get => Kind == KomaTypeKind.AsHu; }
        public KomaTypeId()
        {
            UniqueKey = Guid.Empty.ToString();
            Name = string.Empty;
            PromotedName = string.Empty;
            Kind = KomaTypeKind.None;
        }

        public KomaTypeId NewId()
        {
            return new KomaTypeId(Name, PromotedName, Kind);
        }

        public override string ToString()
        {
            return $"{Name}-{PromotedName}";
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
