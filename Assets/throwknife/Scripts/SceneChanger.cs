using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Scenes;
using Unity.Tiny.UIControls;
using Unity.Collections;

namespace throwknife
{
    public class SceneChanger : ComponentSystem
    {
        EntityQueryDesc SceneChangeButtonDesc;
        EntityQuery SceneChangeButtonQuery;

        protected override void OnCreate()
        {
        /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/
         SceneChangeButtonDesc= new EntityQueryDesc()
         {
             All = new ComponentType[] { typeof(ChangeTitleSceneTag), typeof(PointerInteraction)},
         };

        /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
        //作成したクエリの結果を取得します。
        SceneChangeButtonQuery= GetEntityQuery(SceneChangeButtonDesc);
        }

        protected override void OnUpdate()
        {
            NativeArray<ChangeTitleSceneTag> ChangeButtonsEnt = SceneChangeButtonQuery.ToComponentDataArray<ChangeTitleSceneTag>(Allocator.TempJob);
            NativeArray<PointerInteraction> ChangeButtons = SceneChangeButtonQuery.ToComponentDataArray<PointerInteraction>(Allocator.TempJob);

            for (int i = 0; i < ChangeButtons.Length; i++)
            {
                if(ChangeButtons[i].clicked==true)
                {
                    var NowConfig = World.TinyEnvironment().GetConfigData<GameConfig>();
                    SceneService.UnloadAllSceneInstances(World.TinyEnvironment().GetConfigData<GameConfig>().NowScene);

                    SceneService.LoadSceneAsync(ChangeButtonsEnt[i].NextScene);

                    NowConfig.NowScene = ChangeButtonsEnt[i].NextScene;
                    World.TinyEnvironment().SetConfigData(NowConfig);
                    break;
                }
            }

            ChangeButtonsEnt.Dispose();
            ChangeButtons.Dispose();
        }
    }
}
