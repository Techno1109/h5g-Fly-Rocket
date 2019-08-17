using Unity.Entities;
using Unity.Tiny.Core2D;
using Unity.Tiny.Core;
using Unity.Collections;

namespace throwknife
{
    [UpdateAfter(typeof(KnifeMove))]
    public class ObjectEmitter : ComponentSystem
    {
        EntityQueryDesc PrefabObjDesc;
        EntityQuery PrefabObjQuery;

        EntityQueryDesc KnifeObjDesc;
        EntityQuery KnifeQuery;

        EntityQueryDesc EmitCompDesc;
        EntityQuery EmitCompQuery;

        protected override void OnCreate()
        {
            /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

            PrefabObjDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(Prefab), typeof(TargetTag) },
            };

            KnifeObjDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(Translation) },
            };

            EmitCompDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(EmitterComp) },
            };


            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。

            PrefabObjQuery = GetEntityQuery(PrefabObjDesc);
            KnifeQuery = GetEntityQuery(KnifeObjDesc);
            EmitCompQuery = GetEntityQuery(EmitCompDesc);
        }

        protected override void OnUpdate()
        {
            NativeArray<Translation> KnifeTrans = KnifeQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<Entity> PrefabEntity = PrefabObjQuery.ToEntityArray(Allocator.TempJob);
            NativeArray<EmitterComp> EmitComponent = EmitCompQuery.ToComponentDataArray<EmitterComp>(Allocator.TempJob);

            if (KnifeTrans[0].Value.y>= EmitComponent[0].LastEmitHigher)
            {
                var NowEmitComp = EmitComponent[0];

                Entity EmitObj = EntityManager.Instantiate(PrefabEntity[0]);
                Translation ObjTrans = EntityManager.GetComponentData<Translation>(EmitObj);
                ObjTrans.Value.y = NowEmitComp.LastEmitHigher + 5f;
                EntityManager.SetComponentData<Translation>(EmitObj, ObjTrans);

                NowEmitComp.LastEmitHigher += 5f;
                EmitComponent[0] = NowEmitComp;
            }

            EmitCompQuery.CopyFromComponentDataArray(EmitComponent);

            PrefabEntity.Dispose();
            EmitComponent.Dispose();
            KnifeTrans.Dispose();
        }
    }
}
