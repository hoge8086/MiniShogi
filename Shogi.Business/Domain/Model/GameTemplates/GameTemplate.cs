using Shogi.Business.Domain.Model.Games;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    [DataContract]
    public class GameTemplate
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public Game Game { get; set; }
    }



}

