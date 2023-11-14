using System;
using System.Collections.Generic;
using MpMessageControlStoreNS;

namespace MPMessageControlBoxNS
{

    //Every line contains a message, which has severity, timestamp and text
    public struct Line
    {
        public GCSMessage Msg {  get; set; }
        internal int startY;

        public class LineYComparer : IComparer<Line>
        {
            private readonly int Y;

            public LineYComparer(int Y)
            {
                this.Y = Y;
            }

            public int Compare(Line x, Line y)
            {
                if (x.startY == -10)
                    return -y.startY.CompareTo(Y);
                else
                    return x.startY.CompareTo(Y);
            }
        }
    }
}


