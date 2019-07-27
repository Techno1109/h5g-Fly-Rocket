using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Mathematics;
using Unity.Tiny.Text;
using Unity.Collections;
namespace throwknife
{

    public class ScoreMan : ComponentSystem
    {
        EntityQueryDesc KnifeDesc;
        EntityQuery KnifeQuery;

        EntityQueryDesc ScoreDesc;
        EntityQuery ScoreQuery;


        protected override void OnCreate()
        {
            /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

            //KnifeTag������A�Ȃ�����Translation���������Ă���Entity�݂̂����o���N�G�����쐬���܂��B
            KnifeDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(Translation) },
            };

            ScoreDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(ScoreComp), typeof(TextString) },
            };

            /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
            //�쐬�����N�G���̌��ʂ��擾���܂��B
            KnifeQuery = GetEntityQuery(KnifeDesc);
            ScoreQuery = GetEntityQuery(ScoreDesc);
        }

        protected override void OnUpdate()
        {
            /*�`�����N�C�e���[�V�����͍s�����ԉ��т��₷���A�����̂��ʓ|�Ȃ��߁AForeachAPI���g�p���܂�*/

            Entities.With(KnifeQuery).ForEach((ref Translation Transform, ref KnifeTag Tag) =>
            {

                if (Tag.ScoreUp == true && Tag.ActiveTag == false)
                {
                    Entities.With(ScoreQuery).ForEach((Entity entity,ref ScoreComp score) =>
                    {
                        score.Score += 100;
                        EntityManager.SetBufferFromString<TextString>(entity, score.Score.ToString());
                    });

                   Tag.ScoreUp = false;
                }
            });
        }
    }
}
