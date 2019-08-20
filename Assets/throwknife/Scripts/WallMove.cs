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
            /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

            WallQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(WallComp), typeof(Translation)},
            };

            KnifeQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(Translation), typeof(RigidBody) },
            };

            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。

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


            //Wallを動かす
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

            //先に離れ過ぎたWallを削除

            for (int EntityNum = 0; EntityNum < WallEntitys.Length; EntityNum++)
            {
                if (WallTrans[EntityNum].Value.y - KnifeTrans[0].Value.y <= -8)
                {
                    EntityManager.DestroyEntity(WallEntitys[EntityNum]);
                }
            }

            //削除完了

            KnifeTrans.Dispose();
            WallEntitys.Dispose();
            WallTrans.Dispose();

        }
    }
}
