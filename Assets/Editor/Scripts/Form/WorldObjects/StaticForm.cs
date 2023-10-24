using System;

namespace Form.WorldObjects
{
    public class StaticForm : BaseForm
    {
        public new const string TYPE = "Static";
        public new const string CATEGORY = "WorldObjects";
        public override string GetTYPE => TYPE;
        public override string GetCATEGORY => CATEGORY;
        public string model;    
    }    
}
