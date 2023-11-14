using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MpMessageControlStoreNS
{
    // Dual use, Info/Warning/Error marks the severity of the message
    // Info/Warning/Error/Error&Warning/All marks the displayed severities

    [Flags]
    public enum SeverityLevels
    {
        None = 0,
        Error = 1 << 0,
        Warning = 1 << 1,
        Info = 1 << 2,
        ErrorAndWarning = Error | Warning,
        All = Error | Warning | Info
    }
    public struct GCSMessage
    {
        public string Text { get; set; }
        public DateTime LastSeen { get; set; }
        public int Severity
        {
            get { return _severity; }
            set
            {
                _severity = value;
                //Set the SeverityLevel according to the mavlink severity
                if (value >= 0 && value <= 3)
                {
                    _severityLevel = SeverityLevels.Error;
                }
                else if (value >= 4 && value <= 5)
                {
                    _severityLevel = SeverityLevels.Warning;
                }
                else
                {
                    _severityLevel = SeverityLevels.Info;
                }
            }
        }
        public int SystemId { get; set; }
        public bool Supressed { get; set; }
        public bool Acknowledged { get; set; }
        public List<DateTime> occurences { get; set; }              // List of timestamps when this message was seen
        public SeverityLevels SeverityLevel
        {
            get { return _severityLevel; }
        }

        //private backing fields
        private bool _ignoreNumbers;
        private int _severity;
        private SeverityLevels _severityLevel;


        public bool IgnoreNumbers
        {
            get { return _ignoreNumbers; }
            set { _ignoreNumbers = value; }         //We should cleat the list when this is set
        }

        public int getHash()
        {
            string toHash;

            if (_ignoreNumbers)
            {
                // if we ignore numbers then we need to replace them with a # so that the hash is the same for messages with different numbers
                string cleanText = Regex.Replace(Text + " ", @"(?<=\s)[-+]?\d+(\.\d+)?(?=\s|,)", "#"); // Add a space to inclue the last number in the text
                toHash = SystemId.ToString() + cleanText + Severity.ToString();
            }
            else
            {
                toHash = SystemId.ToString() + Text + Severity.ToString();
            }
            return toHash.GetHashCode();
        }

        public void AddOccurence(DateTime occ)
        {
            if (occurences == null)
                occurences = new List<DateTime>();

            occurences.Add(occ);
        }

    }
}
