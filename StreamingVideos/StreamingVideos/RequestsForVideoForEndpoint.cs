using System;
using System.Collections.Generic;
using System.Text;

namespace StreamingVideos
{
    class RequestsForVideoForEndpoint
    {
        public Endpoint Endpoint;
        public Video Video;
        public int Count;

        public RequestsForVideoForEndpoint(Endpoint endpoint, Video video, int count)
        {
            Endpoint = endpoint;
            Video = video;
            Count = count;
        }
    }
}
