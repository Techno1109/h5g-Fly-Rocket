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
                All = new ComponentType[] { typeof(TargetTag), typeof(Translation), typeof(Sprite2DRendererHitBox2D)},
            };

            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。
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