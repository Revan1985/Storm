﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StormLib.Interfaces;
using StormLib.Streams;

namespace StormLib.Helpers
{
    public static class StreamFactory
    {
        private const string http = "http://";
        private const string https = "https://";

        private static readonly StringComparison sc = StringComparison.CurrentCultureIgnoreCase;

        public static bool TryCreate(string line, out IStream? stream)
        {
            if (!line.StartsWith(https, sc) && !line.StartsWith(http, sc))
            {
                line = $"{https}{line}";
            }

            if (line.StartsWith(http, sc))
            {
                line = line.Insert(4, "s");
            }

            if (!Uri.TryCreate(line, UriKind.Absolute, out Uri uri))
            {
                stream = null;
                return false;
            }

            string host = uri.DnsSafeHost;

            try
            {
                if (host.EndsWith("twitch.tv", sc))
                {
                    stream = new TwitchStream(uri);
                }
                else if (host.EndsWith("chaturbate.com", sc))
                {
                    stream = new ChaturbateStream(uri);
                }
                else if (host.EndsWith("mixer.com", sc))
                {
                    stream = new MixerStream(uri);
                }
                else if (host.EndsWith("mixlr.com", sc))
                {
                    stream = new MixlrStream(uri);
                }
                else
                {
                    stream = new UnsupportedStream(uri);
                }
            }
            catch (ArgumentException)
            {
                stream = null;
                return false;
            }

            return true;
        }

        public static IReadOnlyCollection<IStream> CreateMany(string[] lines, string commentCharacter)
        {
            ConcurrentBag<IStream> streams = new ConcurrentBag<IStream>();

            if (lines.Length > 0)
            {
                IEnumerable<string> nonCommentLines = lines.Where(l => !l.StartsWith(commentCharacter, StringComparison.OrdinalIgnoreCase));

                Parallel.ForEach(nonCommentLines, (line, loopState) =>
                {
                    if (TryCreate(line, out IStream? stream))
                    {
#nullable disable
                        if (!streams.Contains(stream))
                        {
                            streams.Add(stream);
                        }
#nullable enable
                    }
                });
            }

            return (IReadOnlyCollection<IStream>)streams.AsEnumerable();
        }
    }
}