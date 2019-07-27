using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.HitBox2D;
using Unity.Mathematics;
using Unity.Collections;

namespace throwknife
{
    public class KnifeHitCheck : ComponentSystem
    {
        EntityQueryDesc KnifeDesc;
        EntityQuery KnifeQuery;

        EntityQueryDesc TargetDesc;
        EntityQuery TargetQuery;

        protected override void OnCreate()
        {
            /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

            //KnifeTag������A�Ȃ�����Translation���������Ă���Entity�݂̂����o���N�G�����쐬���܂��B
            KnifeDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag),typeof (Translation), typeof(Sprite2DRendererHitBox2D)},
            };


            TargetDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(TargetTag), typeof(Translation), typeof(Sprite2DRendererHitBox2D)},
            };

            /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
            //�쐬�����N�G���̌��ʂ��擾���܂��B
            KnifeQuery  = GetEntityQuery(KnifeDesc);
            TargetQuery = GetEntityQuery(TargetDesc);
        }

        protected override void OnUpdate()
        {
            Entities.With(KnifeQuery).ForEach((ref KnifeTag tag,ref Translation Ktrans) =>
            {
                float Hight = 0;
                bool Result = tag.ActiveTag;

                if (Result==true)
                {
                    Hight = Ktrans.Value.y;

                    Entities.With(TargetQuery).ForEach((ref Translation Ttrans) =>
                    {
                        if(Ttrans.Value.y>Hight)
                        {
                            Result = false;
                        }
                    });
                }

                tag.ActiveTag = Result;
            });

        }
    }
}