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
            /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

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


            /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
            //�쐬�����N�G���̌��ʂ��擾���܂��B

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
