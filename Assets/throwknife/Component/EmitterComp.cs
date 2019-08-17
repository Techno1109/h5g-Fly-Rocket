using Unity.Entities;

namespace throwknife
{
    public struct EmitterComp : IComponentData
    {
        public float LastEmitHigher; 
    }
}