using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LGED.Domain.Base
{
    //mark as json object so all attributes inside will be serialized
    [JsonObject]
    //to enable json attribute name change using DataMember
    //need to add DataContract annotation
    [DataContract]
    public class ViewModelBase
    {
        //default name
        [DataMember(Name = "id", Order = 1)]
        public Guid Id { get; set; }
    }
}