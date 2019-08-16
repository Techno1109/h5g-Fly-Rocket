using Unity.Entities;
using Unity.Mathematics;
namespace throwknife
{
    public struct RigidBody : IComponentData
    {
        public bool IsActive;
        public bool UseGravity;
        public float3 Velocity;
        public bool3 ActiveVec;
    }
}