using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Mathematics;

namespace throwknife
{
    public class KnifeMove : ComponentSystem
    {

        EntityQueryDesc KnifeDesc;

        EntityQuery KnifeQuery;

        protected override void OnCreate()
        {
            /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

            //KnifeTag������A�Ȃ�����Translation���������Ă���Entity�݂̂����o���N�G�����쐬���܂��B
            KnifeDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(Translation) },
            };

            /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
            //�쐬�����N�G���̌��ʂ��擾���܂��B
            KnifeQuery = GetEntityQuery(KnifeDesc);
        }

        protected override void OnUpdate()
        {
            /*�`�����N�C�e���[�V�����͍s�����ԉ��т��₷���A�����̂��ʓ|�Ȃ��߁AForeachAPI���g�p���܂�*/

            Entities.With(KnifeQuery).ForEach((ref Translation Transform) =>
           {
               //Translation��Y�ɑ΂���1���v���X���܂��B
               Transform.Value.y += 1;
           });
        }
    }
}