using Unity.Entities;
using Unity.Tiny.Core2D;
using Unity.Tiny.Core;
using Unity.Collections;

namespace throwknife
{
    [UpdateAfter(typeof(ObjectEmitter))]
    public class FollowCameraSystem : ComponentSystem
    {
        EntityQueryDesc CameraQueryDesc;
        EntityQuery CameraQuery;
        protected override void OnCreate()
        {
            /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

            CameraQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(FollowCam), typeof(Translation) },
            };

            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。

            CameraQuery = GetEntityQuery(CameraQueryDesc);
        }

        protected override void OnUpdate()
        {
            NativeArray<FollowCam> CamArray = CameraQuery.ToComponentDataArray<FollowCam>(Allocator.TempJob);
            NativeArray<Translation> CamTransArray = CameraQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            if (CamArray.Length <= 0 || CamTransArray.Length <= 0)
            {

                CamArray.Dispose();
                CamTransArray.Dispose();
                return;
            }


            for (int EntityNum =0; EntityNum<CamArray.Length; EntityNum++)
            {
                Translation TargetTrans =EntityManager.GetComponentData<Translation>(CamArray[EntityNum].TargetEntity);

                if(CamArray[EntityNum].LastHigher<=TargetTrans.Value.y)
                {
                    var Cam = CamArray[EntityNum];
                    var Trans = CamTransArray[EntityNum];

                    Cam.LastHigher = TargetTrans.Value.y;
                    Trans.Value.y = TargetTrans.Value.y + Cam.Offset;

                    CamTransArray[EntityNum] = Trans;
                    CamArray[EntityNum] = Cam;
                }

            }

            //あくまでもToDataArrayで取得できるArrayはキャッシュの様なものなので、本Entityに書き込む必要があります。
            CameraQuery.CopyFromComponentDataArray(CamArray);
            CameraQuery.CopyFromComponentDataArray(CamTransArray);

            CamArray.Dispose();
            CamTransArray.Dispose();
        }
    }
}
