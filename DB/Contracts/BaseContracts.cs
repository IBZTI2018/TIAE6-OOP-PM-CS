﻿using System.Runtime.Serialization;

namespace DB.Contracts
{
    [DataContract]
    public class IDRequest
    {
        [DataMember(Order = 1)]
        public int id { get; set; }
    }
}
