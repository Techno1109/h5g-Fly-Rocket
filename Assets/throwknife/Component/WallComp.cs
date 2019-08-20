using Unity.Entities;


namespace throwknife
{
    public struct WallComp : IComponentData
    {
        public float StartPos;
        public float MoveSpeed;
    }
}