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
            /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

            CameraQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(FollowCam), typeof(Translation) },
            };

            /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
            //�쐬�����N�G���̌��ʂ��擾���܂��B

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

            //�����܂ł�ToDataArray�Ŏ擾�ł���Array�̓L���b�V���̗l�Ȃ��̂Ȃ̂ŁA�{Entity�ɏ������ޕK�v������܂��B
            CameraQuery.CopyFromComponentDataArray(CamArray);
            CameraQuery.CopyFromComponentDataArray(CamTransArray);

            CamArray.Dispose();
            CamTransArray.Dispose();
        }
    }
}
