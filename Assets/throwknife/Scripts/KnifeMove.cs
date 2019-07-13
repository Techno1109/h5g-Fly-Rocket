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
            /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

            //KnifeTagがあり、なおかつTranslationを所持しているEntityのみを取り出すクエリを作成します。
            KnifeDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(Translation) },
            };

            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。
            KnifeQuery = GetEntityQuery(KnifeDesc);
        }

        protected override void OnUpdate()
        {
            /*チャンクイテレーションは行数が間延びしやすく、書くのが面倒なため、ForeachAPIを使用します*/

            Entities.With(KnifeQuery).ForEach((ref Translation Transform) =>
           {
               //TranslationのYに対して1をプラスします。
               Transform.Value.y += 1;
           });
        }
    }
}