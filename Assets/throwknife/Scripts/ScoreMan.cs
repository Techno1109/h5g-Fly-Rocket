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

        EntityQueryDesc CamDesc;
        EntityQuery FollowCamQuery;

        protected override void OnCreate()
        {
            /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

            KnifeDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(Translation) },
            };

            ScoreDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(ScoreComp), typeof(TextString) },
            };

            CamDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(FollowCam), typeof(Translation) },
            };

            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。
            KnifeQuery = GetEntityQuery(KnifeDesc);
            ScoreQuery = GetEntityQuery(ScoreDesc);
            FollowCamQuery = GetEntityQuery(CamDesc);
        }

        protected override void OnUpdate()
        {

            NativeArray<FollowCam> FollowCamArray = FollowCamQuery.ToComponentDataArray<FollowCam>(Allocator.TempJob);
            NativeArray<Entity> ScoreEntity = ScoreQuery.ToEntityArray(Allocator.TempJob);

            if (FollowCamArray.Length <= 0 || ScoreEntity.Length <= 0)
            {
                FollowCamArray.Dispose();
                ScoreEntity.Dispose();

                return;
            }

            for (int EntityNum =0;EntityNum<FollowCamArray.Length;EntityNum++)
            {
                int SendScore = (int)(FollowCamArray[EntityNum].LastHigher * 10);
                EntityManager.SetBufferFromString<TextString>(ScoreEntity[EntityNum], SendScore.ToString());
            }
            FollowCamArray.Dispose();
            ScoreEntity.Dispose();
        }
    }
}
