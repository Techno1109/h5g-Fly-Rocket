using Unity.Entities;

namespace throwknife
{
    public struct KnifeTag : IComponentData
    {
        //判別用のタグなのでなかには何も持たせません。
       public bool ActiveTag;
       public bool ScoreUp;
       public bool MoveFlag;
    }
}