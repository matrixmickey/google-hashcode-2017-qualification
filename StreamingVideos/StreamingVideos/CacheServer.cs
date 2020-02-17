using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingVideos
{
    class CacheServer
    {
        public int Id;
        public int Latency;

        public CacheServer(int id, int latency)
        {
            Id = id;
            Latency = latency;
        }
    }
}
