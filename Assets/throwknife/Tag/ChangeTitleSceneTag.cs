using Unity.Entities;
using Unity.Tiny.Scenes;
public struct ChangeTitleSceneTag : IComponentData
{
    public SceneReference NextScene;
}
