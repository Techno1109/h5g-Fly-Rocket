using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.Scenes;
using Unity.Tiny.UIControls;
using Unity.Tiny.UILayout;
using Unity.Collections;

namespace throwknife
{
    [UpdateAfter(typeof(FollowCameraSystem))]
    public class SceneChanger : ComponentSystem
    {
        EntityQueryDesc SceneChangeButtonDesc;
        EntityQuery SceneChangeButtonQuery;

        EntityQueryDesc KnifeDesc;
        EntityQuery KnifeQuery;

        EntityQueryDesc CameraQueryDesc;
        EntityQuery CameraQuery;

        EntityQueryDesc TargetQueryDesc;
        EntityQuery TargetQuery;

        EntityQueryDesc GuideLineQueryDesc;
        EntityQuery GuideLineQuery;

        EntityQueryDesc EmitCompDesc;
        EntityQuery EmitCompQuery;

        protected override void OnCreate()
        {
        /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/
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

        TargetQueryDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(TargetTag) ,typeof(ActiveDelete)},
        };

        GuideLineQueryDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(GuideLineTag) },
        };

        EmitCompDesc = new EntityQueryDesc()
        {
            All = new ComponentType[] { typeof(EmitterComp) },
        };

            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。
            SceneChangeButtonQuery = GetEntityQuery(SceneChangeButtonDesc);

            KnifeQuery = GetEntityQuery(KnifeDesc);

            CameraQuery = GetEntityQuery(CameraQueryDesc);

            TargetQuery = GetEntityQuery(TargetQueryDesc);

            GuideLineQuery = GetEntityQuery(GuideLineQueryDesc);

            EmitCompQuery = GetEntityQuery(EmitCompDesc);
        }

        protected override void OnUpdate()
        {
            NativeArray<ChangeTitleSceneTag> ChangeButtonsEnt = SceneChangeButtonQuery.ToComponentDataArray<ChangeTitleSceneTag>(Allocator.TempJob);
            NativeArray<PointerInteraction> ChangeButtons = SceneChangeButtonQuery.ToComponentDataArray<PointerInteraction>(Allocator.TempJob);
            NativeArray<RectTransform> MoveButtonTrans =SceneChangeButtonQuery.ToComponentDataArray<RectTransform>(Allocator.TempJob);
            NativeArray<EmitterComp> EmitComp = EmitCompQuery.ToComponentDataArray<EmitterComp>(Allocator.TempJob);

            NativeArray<Entity> TargetEntity = TargetQuery.ToEntityArray(Allocator.TempJob);
            NativeArray<Entity> GuideLineEntity = GuideLineQuery.ToEntityArray(Allocator.TempJob);

            if ( MoveButtonTrans.Length   <=0 || ChangeButtonsEnt.Length<=0 
                || ChangeButtons.Length   <=0 || TargetEntity.Length <= 0 
                || GuideLineEntity.Length <=0 || EmitComp.Length     <= 0)
            {
                TargetEntity.Dispose();
                GuideLineEntity.Dispose();
                EmitComp.Dispose();
                MoveButtonTrans.Dispose();
                ChangeButtonsEnt.Dispose();
                ChangeButtons.Dispose();

                return;
            }

            for (int i = 0; i < ChangeButtons.Length; i++)
            {
                if(ChangeButtons[i].clicked==true)
                {

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
                        Ttrans.Value.y = 0;
                    });

                    for(int EntityNum=0;EntityNum<TargetEntity.Length;EntityNum++)
                    {
                        EntityManager.DestroyEntity(TargetEntity[EntityNum]);
                    }

                    for (int EntityNum = 0; EntityNum < GuideLineEntity.Length; EntityNum++)
                    {
                        EntityManager.DestroyEntity(GuideLineEntity[EntityNum]);
                    }

                    var Config = World.TinyEnvironment().GetConfigData<GameStateConfig>();

                    Config.IsActive = true;

                    World.TinyEnvironment().SetConfigData(Config);

                    var TmpEmitComp = EmitComp[0];

                    TmpEmitComp.LastEmitHigher = 0;

                    EmitComp[0] = TmpEmitComp;

                    var Tmp = MoveButtonTrans[i];
                    Tmp.anchoredPosition.y = 800;
                    MoveButtonTrans[i] = Tmp;
                    break;
                }
            }

            SceneChangeButtonQuery.CopyFromComponentDataArray(MoveButtonTrans);
            EmitCompQuery.CopyFromComponentDataArray(EmitComp);

            TargetEntity.Dispose();
            GuideLineEntity.Dispose();
            EmitComp.Dispose();
            MoveButtonTrans.Dispose();
            ChangeButtonsEnt.Dispose();
            ChangeButtons.Dispose();
        }
    }
}
