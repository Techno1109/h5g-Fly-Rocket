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
            /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

            //KnifeTagがあり、なおかつTranslationを所持しているEntityのみを取り出すクエリを作成します。
            KnifeDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag),typeof (Translation), typeof(Sprite2DRendererHitBox2D)},
            };


            TargetDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(TargetTag), typeof(Translation), typeof(Sprite2DRendererHitBox2D), typeof(HitBoxOverlap) },
            };

            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。
            KnifeQuery  = GetEntityQuery(KnifeDesc);
            TargetQuery = GetEntityQuery(TargetDesc);
        }

        protected override void OnUpdate()
        {
            NativeArray<Entity> KnifeEntitys = KnifeQuery.ToEntityArray(Allocator.TempJob);
            NativeArray<KnifeTag> KnifeTags = KnifeQuery.ToComponentDataArray<KnifeTag>(Allocator.TempJob);

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
                        Tmp.ActiveTag = false;
                        Tmp.ScoreUp = true;
                        KnifeTags[i] = Tmp;
                    }
                }
            });

            KnifeQuery.CopyFromComponentDataArray(KnifeTags);

            KnifeEntitys.Dispose();
            KnifeTags.Dispose();
        }
    }
}