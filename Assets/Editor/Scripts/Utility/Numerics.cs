namespace Editor.Scripts.Utility
{
    public static class Numerics
    {
        public static System.Numerics.Vector3 ConvertToSystemNumerics(UnityEngine.Vector3 unityVector)
        {
            return new System.Numerics.Vector3(unityVector.x, unityVector.y, unityVector.z);
        }

        public static UnityEngine.Vector3 ConvertToUnityNumerics(System.Numerics.Vector3 systemVector)
        {
            return new UnityEngine.Vector3(systemVector.X, systemVector.Y, systemVector.Z);
        }

        public static System.Numerics.Quaternion ConvertToSystemNumerics(UnityEngine.Quaternion unityQuat)
        {
            return new System.Numerics.Quaternion(unityQuat.x, unityQuat.y, unityQuat.z, unityQuat.w);
        }

        public static UnityEngine.Quaternion ConvertToUnityNumerics(System.Numerics.Quaternion systemQuat)
        {
            return new UnityEngine.Quaternion(systemQuat.X, systemQuat.Y, systemQuat.Z, systemQuat.W);
        }

    }
}