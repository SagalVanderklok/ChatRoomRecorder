using System;
using System.Text.RegularExpressions;

namespace ChatRoomRecorder
{
    public record struct ChatRoomResolution : IComparable
    {
        public ChatRoomResolution(short width, short height)
        {
            if (width >= 0 && height >= 0)
            {
                _width = width;
                _height = height;
            }
            else
            {
                _width = 0;
                _height = 0;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", _width, _height);
        }

        public int ToInt32()
        {
            return (int)_width << 16 | _height;
        }

        public int CompareTo(object resolution)
        {
            return TotalPixels.CompareTo(((ChatRoomResolution)resolution).TotalPixels);
        }

        public static ChatRoomResolution Parse(string resolution)
        {
            try
            {
                MatchCollection matches = Regex.Matches(resolution, "^([0-9]*)x([0-9]*)$", RegexOptions.IgnoreCase);
                return new ChatRoomResolution(short.Parse(matches[0].Groups[1].Value), short.Parse(matches[0].Groups[2].Value));
            }
            catch (Exception)
            {
                return ChatRoomResolution.MinValue;
            }
        }

        public static ChatRoomResolution Parse(int resolution)
        {
            return new ChatRoomResolution(Convert.ToInt16(resolution >> 16), Convert.ToInt16(resolution << 16 >> 16));
        }

        public static int FindClosest(ChatRoomResolution desiredResolution, ChatRoomResolution[] allResolutions)
        {
            int index = -1;
            int difference = -1;

            if (allResolutions != null)
            {
                for (int i = 0; i < allResolutions.Length; i++)
                {
                    if (allResolutions[i] != ChatRoomResolution.MinValue)
                    {
                        int current_difference = Math.Abs(desiredResolution.TotalPixels - allResolutions[i].TotalPixels);
                        if (index == -1 || current_difference < difference)
                        {
                            index = i;
                            difference = current_difference;
                        }
                    }
                }
            }

            return index;
        }

        public static bool operator <(ChatRoomResolution left, ChatRoomResolution right)
        {
            return left.TotalPixels < right.TotalPixels;
        }

        public static bool operator >(ChatRoomResolution left, ChatRoomResolution right)
        {
            return left.TotalPixels > right.TotalPixels;
        }

        public static bool operator <=(ChatRoomResolution left, ChatRoomResolution right)
        {
            return left.TotalPixels <= right.TotalPixels;
        }

        public static bool operator >=(ChatRoomResolution left, ChatRoomResolution right)
        {
            return left.TotalPixels >= right.TotalPixels;
        }

        public int TotalPixels
        {
            get
            {
                return (int)_width * (int)_height;
            }
        }

        public short Width
        {
            get
            {
                return _width;
            }
        }

        public short Height
        {
            get
            {
                return _height;
            }
        }

        public static ChatRoomResolution MinValue
        {
            get
            {
                return new ChatRoomResolution(0, 0);
            }
        }

        public static ChatRoomResolution MaxValue
        {
            get
            {
                return new ChatRoomResolution(short.MaxValue, short.MaxValue);
            }
        }

        public static ChatRoomResolution[] CommonResolutions
        {
            get
            {
                return new ChatRoomResolution[]
                {
                    new ChatRoomResolution(640, 360),
                    new ChatRoomResolution(960, 540),
                    new ChatRoomResolution(1280, 720),
                    new ChatRoomResolution(1600, 900),
                    new ChatRoomResolution(1920, 1080),
                    new ChatRoomResolution(2560, 1440),
                    new ChatRoomResolution(3840, 2160)
                };
            }
        }

        private short _width;
        private short _height;
    }
}
