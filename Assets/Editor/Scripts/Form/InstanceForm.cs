
using System;
using System.Collections.Generic;
using Vector3 = System.Numerics.Vector3;
using Quaternion = System.Numerics.Quaternion;
namespace Form
{
    public class InstanceForm : BaseForm
    {
        public new const string TYPE = "Instance";
        public new const string CATEGORY = BaseForm.NONE_CATEGORY;
        public override string GetTYPE => TYPE;
        public override string GetCATEGORY => CATEGORY;
        public string protoID;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public List<Tuple<string, object>> overrides;
    }
}