using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Scenes;
using Unity.Tiny.UIControls;
using Unity.Tiny.UILayout;
using Unity.Collections;

namespace throwknife
{
    [UpdateAfter(typeof(KnifeHitCheck))]
    public class SceneChanger : ComponentSystem
    {
        EntityQueryDesc SceneChangeButtonDesc;
        EntityQuery SceneChangeButtonQuery;

        EntityQueryDesc KnifeDesc;
        EntityQuery KnifeQuery;

        EntityQueryDesc CameraQueryDesc;
        EntityQuery CameraQuery;

        protected override void OnCreate()
        {
        /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/
        SceneChangeButtonDesc= new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(ChangeTitleSceneTag), typeof(PointerInteraction),typeof(RectTransform)},
        };

        KnifeDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(KnifeTag), typeof(Translation), typeof(RigidBody) },
        };


        CameraQueryDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(FollowCam), typeof(Translation) },
        };

        /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
        //�쐬�����N�G���̌��ʂ��擾���܂��B
            SceneChangeButtonQuery = GetEntityQuery(SceneChangeButtonDesc);

            KnifeQuery = GetEntityQuery(KnifeDesc);

            CameraQuery = GetEntityQuery(CameraQueryDesc);

        }

        protected override void OnUpdate()
        {
            NativeArray<ChangeTitleSceneTag> ChangeButtonsEnt = SceneChangeButtonQuery.ToComponentDataArray<ChangeTitleSceneTag>(Allocator.TempJob);
            NativeArray<PointerInteraction> ChangeButtons = SceneChangeButtonQuery.ToComponentDataArray<PointerInteraction>(Allocator.TempJob);
            NativeArray<RectTransform> MoveButtonTrans =SceneChangeButtonQuery.ToComponentDataArray<RectTransform>(Allocator.TempJob);

            for (int i = 0; i < ChangeButtons.Length; i++)
            {
                if(ChangeButtons[i].clicked==true)
                {
                    //var NowConfig = World.TinyEnvironment().GetConfigData<GameConfig>();
                    //SceneService.UnloadAllSceneInstances(World.TinyEnvironment().GetConfigData<GameConfig>().NowScene);

                    //SceneService.LoadSceneAsync(ChangeButtonsEnt[i].NextScene);

                    //NowConfig.NowScene = ChangeButtonsEnt[i].NextScene;
                    //World.TinyEnvironment().SetConfigData(NowConfig);

                    Entities.With(KnifeQuery).ForEach((ref Translation Ttrans,ref RigidBody Rigid,ref KnifeTag Tag) =>
                    {
                        Rigid.IsActive = false;
                        Tag.ActiveTag = true;
                        Tag.ScoreUp = false;
                        Rigid.Velocity.y = 0;
                        Ttrans.Value.y = 0;
                    });

                    Entities.With(CameraQuery).ForEach((ref FollowCam CamData,ref Translation Ttrans) =>
                    {
                        CamData.LastHigher = 0;
                        Ttrans.Value.y = 2;
                    });
                    var Tmp = MoveButtonTrans[i];
                    Tmp.anchoredPosition.y = 800;
                    MoveButtonTrans[i] = Tmp;
                    break;
                }
            }

            SceneChangeButtonQuery.CopyFromComponentDataArray(MoveButtonTrans);

            MoveButtonTrans.Dispose();
            ChangeButtonsEnt.Dispose();
            ChangeButtons.Dispose();
        }
    }
}
