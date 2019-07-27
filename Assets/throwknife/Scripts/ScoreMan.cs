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
            /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

            //KnifeTagがあり、なおかつTranslationを所持しているEntityのみを取り出すクエリを作成します。
            KnifeDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(Translation) },
            };

            ScoreDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(ScoreComp), typeof(TextString) },
            };

            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。
            KnifeQuery = GetEntityQuery(KnifeDesc);
            ScoreQuery = GetEntityQuery(ScoreDesc);
        }

        protected override void OnUpdate()
        {
            /*チャンクイテレーションは行数が間延びしやすく、書くのが面倒なため、ForeachAPIを使用します*/

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
