using System;
using System.Collections.Generic;
using System.Linq;

namespace MpMessageControlStoreNS
{
    // Contains a dictionary of messages identified by a hash created from the text and severity of the message 

    internal class MessagesStore
    {
        // Dictionary of messages
        // Key: hash of message text and severity

        private Dictionary<int, GCSMessage> Messages = new Dictionary<int, GCSMessage>();

        //Adding a message to the dictionary
        //If the message is already in the dictionary, update the occurence list and the last seen time
        public void AddMessage(GCSMessage msg)
        {
            int hash = msg.getHash();

            if (Messages.ContainsKey(hash))
            {
                GCSMessage m = Messages[hash]; //Get the message from the dictionary

                //Update the occurence list and the last seen time
                m.AddOccurence(msg.LastSeen);
                m.LastSeen = msg.LastSeen;
                m.Text = msg.Text;

                //Update the dictionary
                Messages[hash] = m;

            }
            else
            {
                //Add occurence
                msg.AddOccurence(msg.LastSeen);
                //Add the message to the dictionary
                Messages.Add(hash, msg);
            }
        }
        public void deleteMessage(GCSMessage msg)
        {
            int hash = msg.getHash();

            if (Messages.ContainsKey(hash))
            {
                Messages.Remove(hash);
            }
        }
        public void deleteAllMessages(SeverityLevels severity)
        {
            //If it is SeverityLevels.All, clear the dictionary
            if (severity == SeverityLevels.All)
            {
                Messages.Clear();
                return;
            }
            //Otherwise delete all messages with the given severity
            List<int> keys = Messages.Keys.ToList();
            foreach (int k in keys)
            {
                if ( (Messages[k].SeverityLevel & severity) != 0  )
                    Messages.Remove(k);
            }
        }
        public void ignoreMessage(GCSMessage msg)
        {
            int hash = msg.getHash();

            if (Messages.ContainsKey(hash))
            {
                GCSMessage m = Messages[hash]; //Get the message from the dictionary
                m.Supressed = true;
                //Update the dictionary
                Messages[hash] = m;
            }

        }

        public void clearIgnoreList()
        {
            List<int> keys = Messages.Keys.ToList();
            foreach (int k in keys)
            {
                if (Messages[k].Supressed == true)
                {
                    GCSMessage m = Messages[k];
                    m.Supressed = false;
                    Messages[k] = m;
                }
            }
        }

        public int getCount()
        {
            return Messages.Count;
        }

        public List<GCSMessage> getLastNMessages(int num)
        {
            List<GCSMessage> lastNMessages = new List<GCSMessage>();
            List<GCSMessage> sortedMessages = Messages.Values.ToList();
            sortedMessages.Sort((x, y) => y.LastSeen.CompareTo(x.LastSeen));
            for (int i = 0; i < num; i++)
            {
                if (i < sortedMessages.Count)
                    lastNMessages.Add(sortedMessages[i]);
            }
            return lastNMessages;
        }

        // Returns a sorted list of messages sorted by LastSeen 
        public List<GCSMessage> GetMessagesSortedByLastSeen()
        {
            List<GCSMessage> sortedMessages = Messages.Values.ToList();
            sortedMessages.Sort((x, y) => y.LastSeen.CompareTo(x.LastSeen));
            return sortedMessages;
        }

        //Returns a sorted list of messages sorted by severity and last seen
        public List<GCSMessage> GetMessagesSortedBySeverityAndLastSeen()
        {
            List<GCSMessage> sortedMessages = Messages.Values.ToList();
            sortedMessages = sortedMessages.GroupBy(m => m.Severity)
                              .OrderBy(g => g.Key)
                              .SelectMany(g => g.OrderByDescending(m => m.LastSeen))
                              .ToList();
            return sortedMessages;
        }

        // Get messages sorted by last seen, and not grouped by hash (Old fashined message list)
        public List<GCSMessage> GetUnfoldedMessages()
        {
            List<GCSMessage> unfoldedMessages = new List<GCSMessage>();
            foreach (GCSMessage m in Messages.Values.ToList())
            {
                foreach (DateTime t in m.occurences)
                {
                    GCSMessage unfoldedMessage = new GCSMessage();
                    unfoldedMessage.Text = m.Text;
                    unfoldedMessage.LastSeen = t;
                    unfoldedMessage.Severity = m.Severity;
                    unfoldedMessage.SystemId = m.SystemId;
                    unfoldedMessage.Supressed = m.Supressed;
                    unfoldedMessage.Acknowledged = m.Acknowledged;
                    unfoldedMessage.occurences = new List<DateTime> { t };
                    unfoldedMessages.Add(unfoldedMessage);
                }
            }
            unfoldedMessages.Sort((x, y) => y.LastSeen.CompareTo(x.LastSeen));
            return unfoldedMessages;
        }

        // delete message stacks older than maxAge basded on LastSeen
        public void cullingMessages(TimeSpan maxAge)
        {
            List<int> keys = Messages.Keys.ToList();
            foreach (int k in keys)
            {
                if (Messages[k].LastSeen < DateTime.Now - maxAge)
                    Messages.Remove(k);
            }
        }

        // Delete the oldest message stack based on LastSeen
        public void deleteOldestMessage()
        {
            //Find the oldest message
            DateTime oldest = DateTime.Now;
            int oldestKey = 0;
            foreach (int k in Messages.Keys.ToList())
            {
                if (Messages[k].LastSeen < oldest)
                {
                    oldest = Messages[k].LastSeen;
                    oldestKey = k;
                }
            }
            Messages.Remove(oldestKey);
        }
    }
}
