using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingVideos
{
    class CacheServerWithVideos
    {
        public int CacheServerId;
        public List<Video> Videos;

        public CacheServerWithVideos(int cacheServerId, List<Video> videos)
        {
            CacheServerId = cacheServerId;
            Videos = videos;
        }
    }
}
