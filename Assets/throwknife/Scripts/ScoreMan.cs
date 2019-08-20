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
            /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

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

            /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
            //�쐬�����N�G���̌��ʂ��擾���܂��B
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
