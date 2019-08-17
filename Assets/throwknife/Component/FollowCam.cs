using Unity.Entities;
using Unity.Mathematics;

namespace throwknife
{
    public struct FollowCam : IComponentData
    {
        public Entity TargetEntity;
        public float LastHigher;
        public float Offset;
    }
}
