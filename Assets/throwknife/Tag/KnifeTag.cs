using Unity.Entities;

namespace throwknife
{
    public struct KnifeTag : IComponentData
    {
        //���ʗp�̃^�O�Ȃ̂łȂ��ɂ͉����������܂���B
       public bool ActiveTag;
       public bool ScoreUp;
       public bool MoveFlag;
    }
}