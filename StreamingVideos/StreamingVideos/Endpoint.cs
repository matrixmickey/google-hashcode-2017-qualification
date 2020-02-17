using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingVideos
{
    class Endpoint
    {
        public int Id;
        public int DataCenterLatency;
        public List<CacheServer> CacheServers;

        public Endpoint(int id, int dataCenterLatency)
        {
            Id = id;
            DataCenterLatency = dataCenterLatency;
            CacheServers = new List<CacheServer>();
        }
    }
}
