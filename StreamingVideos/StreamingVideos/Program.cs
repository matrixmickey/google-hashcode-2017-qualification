using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace StreamingVideos
{
    static class Program
    {
        static void Main(string[] args)
        {
            var inputFilePath = args[0];
            var inputFileLines = File.ReadLines(inputFilePath);
            var i = 0;
            var j = 2;
            var endpoint = new Endpoint(0, 0);
            var numEndpoints = 0;
            var numCacheServers = 0;
            var cacheServerCapacity = 0;
            var numTotalCacheServerConnections = 0;
            var videos = new List<Video>();
            var endpoints = new List<Endpoint>();
            var requestsForVideosForEndpoints = new List<RequestsForVideoForEndpoint>();
            foreach (var line in inputFileLines)
            {
                var data = line.GetNumbersFromLine();
                if (i == 0)
                {
                    numEndpoints = data.ElementAt(1);
                    numCacheServers = data.ElementAt(3);
                    cacheServerCapacity = data.ElementAt(4);
                }
                else if (i == 1)
                {
                    foreach (var videoSize in data)
                    {
                        videos.Add(new Video(videos.Count, videoSize));
                    }
                }
                else if (i < numEndpoints + numTotalCacheServerConnections + 2)
                {
                    if (i == j)
                    {
                        endpoint = new Endpoint(endpoints.Count, data.ElementAt(0));
                        endpoints.Add(endpoint);
                        var numCacheServersConnections = data.ElementAt(1);
                        numTotalCacheServerConnections += numCacheServersConnections;
                        j += numCacheServersConnections + 1;
                    }
                    else
                    {
                        endpoint.CacheServers.Add(new CacheServer(data.ElementAt(0), data.ElementAt(1)));
                    }
                }
                else
                {
                    requestsForVideosForEndpoints.Add(new RequestsForVideoForEndpoint(endpoints.ElementAt(data.ElementAt(1)), videos.ElementAt(data.ElementAt(0)), data.ElementAt(2)));
                }
                i++;
            }
            var possibleCombinationsOfVideos = new List<List<Video>>();
            for (var bitsCombinationOfVideos = 0; bitsCombinationOfVideos < Math.Pow(2, videos.Count()); bitsCombinationOfVideos++)
            {
                var combinationOfVideos = new List<Video>();
                foreach (var video in videos)
                {
                    if ((bitsCombinationOfVideos & (1 << video.Id)) != 0)
                    {
                        combinationOfVideos.Add(video);
                    }
                }
                if (combinationOfVideos.Sum(video => video.Size) <= cacheServerCapacity)
                {
                    possibleCombinationsOfVideos.Add(combinationOfVideos);
                }
            }
            var numPossibleCombinationsOfVideos = possibleCombinationsOfVideos.Count;
            var maxScore = 0;
            var bestSolution = new List<CacheServerWithVideos>();
            for (var bitsPossibleSolutions = 0; bitsPossibleSolutions < Math.Pow(numPossibleCombinationsOfVideos, numCacheServers); bitsPossibleSolutions++)
            {
                var possibleSolution = new List<CacheServerWithVideos>();
                for (var cacheServerId = 0; cacheServerId < numCacheServers; cacheServerId++)
                {
                    var possibleCombinationsOfVideosIndex = (int)(bitsPossibleSolutions / Math.Pow(numPossibleCombinationsOfVideos, cacheServerId) % numPossibleCombinationsOfVideos);
                    possibleSolution.Add(new CacheServerWithVideos(cacheServerId, possibleCombinationsOfVideos.ElementAt(possibleCombinationsOfVideosIndex)));
                }
                var score = 0;
                foreach (var requestDescription in requestsForVideosForEndpoints)
                {
                    var minLatency = requestDescription.Endpoint.DataCenterLatency;
                    var cacheServersForEndpointWithVideo = requestDescription.Endpoint.CacheServers.Where(cacheServerFromEndpoint => possibleSolution.First(cacheServer => cacheServer.CacheServerId == cacheServerFromEndpoint.Id).Videos.Contains(requestDescription.Video));
                    var maxTimeSaved = 0;
                    foreach (var cacheServerForEndpointWithVideo in cacheServersForEndpointWithVideo)
                    {
                        maxTimeSaved = Math.Max(maxTimeSaved, requestDescription.Endpoint.DataCenterLatency - cacheServerForEndpointWithVideo.Latency);
                    }
                    score += requestDescription.Count * maxTimeSaved;
                }
                if (maxScore <= score)
                {
                    maxScore = score;
                    bestSolution = possibleSolution;
                }
            }
            var strOutput = "";
            strOutput += numCacheServers + "\n";
            foreach (var cacheServer in bestSolution)
            {
                strOutput += cacheServer.CacheServerId;
                foreach (var video in cacheServer.Videos)
                {
                    strOutput += " " + video.Id;
                }
                strOutput += "\n";
            }
            var outputFilePath = args[1];
            File.WriteAllText(outputFilePath, strOutput);
        }

        static IEnumerable<int> GetNumbersFromLine(this string line)
        {
            return line.Split(' ').Select(strNumber => int.Parse(strNumber));
        }
    }
}
