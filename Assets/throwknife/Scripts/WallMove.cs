using Unity.Entities;
using Unity.Tiny.Core2D;
using Unity.Collections;
using Unity.Tiny.Core;

namespace throwknife
{
    [UpdateBefore(typeof(KnifeHitCheck))]
    public class WallMove : ComponentSystem
    {
        const float MaxDist = 6f;

        EntityQueryDesc WallQueryDesc;
        EntityQuery WallQuery;

        EntityQueryDesc KnifeQueryDesc;
        EntityQuery KnifeQuery;

        protected override void OnCreate()
        {
            /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

            WallQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(WallComp), typeof(Translation)},
            };

            KnifeQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(Translation), typeof(RigidBody) },
            };

            /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
            //�쐬�����N�G���̌��ʂ��擾���܂��B

            WallQuery = GetEntityQuery(WallQueryDesc);
            KnifeQuery = GetEntityQuery(KnifeQueryDesc);

        }

        protected override void OnUpdate()
        {

            if(!World.TinyEnvironment().GetConfigData<GameStateConfig>().IsActive)
            {
                return;
            }

            NativeArray<Translation> KnifeTrans = KnifeQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            NativeArray<Entity> WallEntitys = WallQuery.ToEntityArray(Allocator.TempJob);
            NativeArray<Translation> WallTrans = WallQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            if (KnifeTrans.Length <= 0 || WallTrans.Length <= 0 || WallEntitys.Length <= 0)
            {
                KnifeTrans.Dispose();
                WallEntitys.Dispose();
                WallTrans.Dispose();
                return;
            }


            //Wall�𓮂���
            Entities.With(WallQuery).ForEach((ref WallComp Wcomp, ref Translation Wtrans) =>
            {
                Wtrans.Value.x += Wcomp.MoveSpeed * World.TinyEnvironment().fixedFrameDeltaTime;

                if (Wcomp.MoveSpeed > 0)
                {
                    if (Wtrans.Value.x >= Wcomp.StartPos + MaxDist)
                    {
                        Wtrans.Value.x = Wcomp.StartPos + MaxDist;
                        Wcomp.MoveSpeed *= -1;
                    }
                }
                else
                {
                    if (Wtrans.Value.x <= Wcomp.StartPos - MaxDist)
                    {
                        Wtrans.Value.x = Wcomp.StartPos - MaxDist;
                        Wcomp.MoveSpeed *= -1;
                    }
                }
            });

            //��ɗ���߂���Wall���폜

            for (int EntityNum = 0; EntityNum < WallEntitys.Length; EntityNum++)
            {
                if (WallTrans[EntityNum].Value.y - KnifeTrans[0].Value.y <= -8)
                {
                    EntityManager.DestroyEntity(WallEntitys[EntityNum]);
                }
            }

            //�폜����

            KnifeTrans.Dispose();
            WallEntitys.Dispose();
            WallTrans.Dispose();

        }
    }
}
