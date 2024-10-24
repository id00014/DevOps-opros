using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PollsProject
{
    public class Question
    {
        public string text;
        public List<string> vars;
        public bool multiply;
        public bool optional;
        public Question(string t, List<string> v, bool mult, bool opt)
        {
            text = t;
            vars = v;
            multiply = mult;
            optional = opt;
        }
        public override string ToString()
        {
            string info = "";
            info += text + ";";
            info += string.Join(";", vars);
            info += ";" + multiply + ";";
            info += optional + ";";
            return info;
        }

        internal object GroupBy(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}
