﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using DMT.Partition.Interfaces;

namespace DMT.Module.Common.Service
{
    public class MatcherInfo
    {
        public Guid Id { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }

        [XmlIgnore]
        public bool Ready { get; set;}

        [XmlIgnore]
        public bool Done { get; set; }

        [XmlIgnore]
        public IPartition Partition { get; set; }

        [XmlIgnore]
        public string Url
        {
            get
            {
                return string.Format("http://{0}:{1}", Host, Port);
            }
        }
    }
}
