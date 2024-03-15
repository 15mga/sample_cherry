using UnityEngine;

namespace Script.Model
{
    public static class Vector
    {
        public static Pb.Vector2 Vec2ToPb(this Vector2 v)
        {
            return new Pb.Vector2 { X = v.x, Y = v.y };
        }
        
        public static Vector2 PbToVec2(this Pb.Vector2 v)
        {
            return new Vector2 { x = v.X, y = v.Y };
        }
        
        public static Vector3 PbToVec3(this Pb.Vector2 v)
        {
            return new Vector3 { x = v.X, z = v.Y };
        }
    }
}