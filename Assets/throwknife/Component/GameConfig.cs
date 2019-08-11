using Unity.Entities;
using Unity.Tiny.Scenes;

public struct GameConfig : IComponentData
{
    public SceneReference NowScene;
}
