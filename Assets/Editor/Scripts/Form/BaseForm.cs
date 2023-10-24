using System;
using System.Collections.Generic;

namespace Form
{
    public class BaseForm : IEquatable<BaseForm>
    {
        public const string TYPE = "BaseForm";
        public const string NONE_CATEGORY = "None";
        public const string CATEGORY = NONE_CATEGORY;
        public virtual string GetTYPE => TYPE;
        public virtual string GetCATEGORY => CATEGORY;
        
        public string editorID = "";
        public string formID = GenID(); // ulid
        public DateTime createTime = DateTime.Now;
        public DateTime modifyTime = DateTime.Now;

        public static string GenID() => Ulid.NewUlid().ToString();
        
        public sealed override bool Equals(object obj)
        {
            if (obj is BaseForm other)
            {
                return formID == other.formID;
            }
            return false;
        }
        
        public bool Equals(BaseForm other)
        {
            return other != null && formID == other.formID;
        }
        
        public static bool operator ==(BaseForm left, BaseForm right)
        {
            return left?.Equals(right) ?? right is null;
        }

        public static bool operator !=(BaseForm left, BaseForm right)
        {
            return !(left == right);
        }


        public sealed override int GetHashCode()
        {
            return formID.GetHashCode();
        }
        
    }    
}
