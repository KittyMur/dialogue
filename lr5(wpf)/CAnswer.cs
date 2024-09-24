using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lr5_wpf_
{
    public class CAnswer
    {
        public long answID = -1;
        public string text = "";
        public long msgID = -1;
        public string action = "";
        public string stat = "";
    }
    public class CMessage
    {
        public long msgID = -1;
        public string text = "";
        public List<CAnswer> answers = new List<CAnswer>();
    }
}
