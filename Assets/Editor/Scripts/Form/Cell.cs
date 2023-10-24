
using System.Collections.Generic;
using Vector3 = System.Numerics.Vector3;
namespace Form
{
    public class Cell : BaseForm
    {
        public new const string TYPE = "Cell";
        public new const string CATEGORY = BaseForm.NONE_CATEGORY;
        public override string GetTYPE => TYPE;
        public override string GetCATEGORY => CATEGORY;
        public string name;
        public string worldSpace;
        public Vector3 origin;
        public List<string> instanceFormIDs;
    }
}