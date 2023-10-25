using System;
using System.Collections.Generic;
using Form;
using Schema;

namespace Mod
{
    public class Mod
    {
        public class Metadata
        {
            public string name = "";
            public string version = "";
            public string author = "";
            public string description = "";
            public string coverUrl = "";
            public DateTime createTime = DateTime.Now;
            public DateTime modifyTime = DateTime.Now;
        }

        public Metadata metadata = new Metadata();
        public List<string> bundles = new List<string>();
        public List<BaseForm> forms = new List<BaseForm>();
        
        
    }
}