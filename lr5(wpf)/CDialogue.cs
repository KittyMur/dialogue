using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace lr5_wpf_
{
    public class CDialogue
    {
        List <CMessage> messages = new List <CMessage>();
        long UID = 0;
        CMessage selectedMessage = null;
        CAnswer selectedAnswer = null;

        long getUID()
        {
            UID++;
            return UID;
        }

        CMessage findMsg(long msgID)
        {
            CMessage msg = null;
            foreach (CMessage i in messages)
            {
                if (i.msgID == msgID)
                    msg = i;
            }
            return msg;
        }

        CAnswer findAnsw(long answID)
        {
            CAnswer answ = null;
            foreach (CAnswer i in selectedMessage.answers)
            {
                if (i.answID == answID)
                    answ = i;
            }
            return answ;
        }

        public long addMessage(CMessage msg)
        {
            msg.msgID = getUID();
            messages.Add(msg);
            selectedMessage = msg;
            return msg.msgID;
        }
        public long addAnswer(CAnswer answ, string action)
        {
            answ.answID = getUID();
            answ.action = action;
            selectedMessage.answers.Add(answ);
            return answ.answID;
        }
        public long addAnswer1(CAnswer answ, string action, string stat)
        {
            answ.answID = getUID();
            answ.action = action;
            answ.stat = stat;
            selectedMessage.answers.Add(answ);
            return answ.answID;
        }
        public void connectAnswer(long msgID)
        {
            selectedAnswer.msgID = msgID;
        }
        public string selectMessage(long msgID)
        {
            selectedMessage = findMsg(msgID);
            return selectedMessage.text;
        }
        public string selectAnswer(long msgID, long answID)
        {
            selectMessage(msgID);
            selectedAnswer = findAnsw(answID);

            return selectedAnswer.text + "[action: " + selectedAnswer.action + " ]" + "[stat: " + selectedAnswer.stat + " ]";
        }
        public void removeMessage(long msgID)
        {
            selectMessage(msgID);
            messages.Remove(selectedMessage);
            selectedMessage = null;
        }

        public void removeAnswer(long msgID, long answID)
        {
            selectMessage(msgID);
            selectAnswer(msgID, answID);
            selectedMessage.answers.Remove(selectedAnswer);
            selectedAnswer = null;
        }
        public void updateMessage(string text)
        {
            selectedMessage.text = text;
            //selectedMessage = null;

        }
        public void updateAnswer(string text, string action)
        {
            selectedAnswer.text = text;
            selectedAnswer.action = action;
            //selectedAnswer = null;
        }
        public List<CMessage> getMessages()
        {
            return messages;
        }
        public long linkedUID()
        {
            return selectedAnswer.msgID;
        }
        public void Clear()
        {
            messages.Clear();
            UID = 0;
            selectedMessage = null;
            selectedAnswer = null;
        }
        public void loadMessage(CMessage msg)
        {
            messages.Add(msg);
            selectedMessage = msg;
        }
        public void loadAnswer(CAnswer answ)
        {
            selectedMessage.answers.Add(answ);
        }
        public long getLastUID()
        {
            return UID;
        }
        public void setLastUID(long uid)
        {
            UID = uid;
        }
    }
}
