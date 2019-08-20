using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.UILayout;
using Unity.Tiny.Core2D;
using Unity.Tiny.HitBox2D;
using Unity.Mathematics;
using Unity.Collections;

namespace throwknife
{
    [UpdateAfter(typeof(RigidBodySystem))]
    public class KnifeHitCheck : ComponentSystem
    {
        EntityQueryDesc KnifeDesc;
        EntityQuery KnifeQuery;

        EntityQueryDesc TargetDesc;
        EntityQuery TargetQuery;

        EntityQueryDesc SceneChangeButtonDesc;
        EntityQuery SceneChangeButtonQuery;

        protected override void OnCreate()
        {
            /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

            KnifeDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag),typeof (Translation), typeof(Sprite2DRendererHitBox2D),typeof(RigidBody) },
            };


            TargetDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(TargetTag), typeof(Translation), typeof(Sprite2DRendererHitBox2D), typeof(HitBoxOverlap) },
            };

            SceneChangeButtonDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(ChangeTitleSceneTag), typeof(RectTransform) },

            };
            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。
            KnifeQuery  = GetEntityQuery(KnifeDesc);
            TargetQuery = GetEntityQuery(TargetDesc);
            SceneChangeButtonQuery = GetEntityQuery(SceneChangeButtonDesc);
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> KnifeEntitys = KnifeQuery.ToEntityArray(Allocator.TempJob);
            NativeArray<KnifeTag> KnifeTags = KnifeQuery.ToComponentDataArray<KnifeTag>(Allocator.TempJob);
            NativeArray<RigidBody> KnifeRigidBodys = KnifeQuery.ToComponentDataArray<RigidBody>(Allocator.TempJob);
            NativeArray<Entity> SceneChangeEntity = SceneChangeButtonQuery.ToEntityArray(Allocator.TempJob);

            if (SceneChangeEntity.Length <= 0 || KnifeRigidBodys.Length <= 0 || KnifeEntitys.Length <= 0 || KnifeTags.Length <= 0)
            {
                SceneChangeEntity.Dispose();
                KnifeRigidBodys.Dispose();
                KnifeEntitys.Dispose();
                KnifeTags.Dispose();
                return;
            }

            Entities.With(TargetQuery).ForEach((Entity ThisEntity, ref Translation Ttrans) =>
            {
                var HitEntity = EntityManager.GetBuffer<HitBoxOverlap>(ThisEntity);
                for(int i=0;i<KnifeEntitys.Length; i++)
                {
                    if(KnifeTags[i].ActiveTag==false)
                    {
                        continue;
                    }
                    bool HitResultFlag = false;

                    for(int k=0;k<HitEntity.Length;k++)
                    {
                        if(KnifeEntitys[i]==HitEntity[k].otherEntity)
                        {
                            HitResultFlag = true;
                            break;
                        }
                    }

                    if(HitResultFlag==true)
                    {
                        var Tmp = KnifeTags[i];
                        var Rigid = KnifeRigidBodys[i];
                        Rigid.IsActive = false;
                        Tmp.ActiveTag = false;
                        Tmp.ScoreUp = true;
                        Entities.With(SceneChangeButtonQuery).ForEach((ref RectTransform CamData) =>
                        {
                            CamData.anchoredPosition.y = 0;
                        });
                        KnifeTags[i] = Tmp;
                        KnifeRigidBodys[i] = Rigid;
                        var Config = World.TinyEnvironment().GetConfigData<GameStateConfig>();

                        Config.IsActive = false;

                        World.TinyEnvironment().SetConfigData(Config);
                    }
                }
            });

            KnifeQuery.CopyFromComponentDataArray(KnifeTags);
            KnifeQuery.CopyFromComponentDataArray(KnifeRigidBodys);

            SceneChangeEntity.Dispose();
            KnifeRigidBodys.Dispose();
            KnifeEntitys.Dispose();
            KnifeTags.Dispose();
        }
    }
}