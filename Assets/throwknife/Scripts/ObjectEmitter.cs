using Unity.Entities;
using Unity.Tiny.Core2D;
using Unity.Tiny.Core;
using Unity.Collections;
using Unity.Mathematics;

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

        EntityQueryDesc GuideLineQueryDesc;
        EntityQuery GuideLineQuery;


        Unity.Mathematics.Random RandomData;

        protected override void OnCreate()
        {
            /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

            PrefabObjDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(Prefab), typeof(WallComp) },
            };

            KnifeObjDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(Translation) },
            };

            EmitCompDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(EmitterComp) },
            };


            GuideLineQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(Prefab), typeof(GuideLineTag) },
            };

            /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
            //�쐬�����N�G���̌��ʂ��擾���܂��B

            PrefabObjQuery = GetEntityQuery(PrefabObjDesc);
            KnifeQuery = GetEntityQuery(KnifeObjDesc);
            EmitCompQuery = GetEntityQuery(EmitCompDesc);
            GuideLineQuery = GetEntityQuery(GuideLineQueryDesc);

            RandomData = new Random(156461062);
        }

        protected override void OnUpdate()
        {

            NativeArray<Translation> KnifeTrans = KnifeQuery.ToComponentDataArray<Translation>(Allocator.TempJob);

            NativeArray<Entity> PrefabEntity = PrefabObjQuery.ToEntityArray(Allocator.TempJob);
            NativeArray<Entity> GuideLineEntity = GuideLineQuery.ToEntityArray(Allocator.TempJob);

            NativeArray<EmitterComp> EmitComponent = EmitCompQuery.ToComponentDataArray<EmitterComp>(Allocator.TempJob);

            if (PrefabEntity.Length <= 0 || EmitComponent.Length <= 0 || KnifeTrans.Length <= 0 || GuideLineEntity.Length<=0)
            {
                GuideLineEntity.Dispose();
                PrefabEntity.Dispose();
                EmitComponent.Dispose();
                KnifeTrans.Dispose();
                return;
            }


            if (KnifeTrans[0].Value.y >= EmitComponent[0].LastEmitHigher)
            {
                var NowEmitComp = EmitComponent[0];

                Entity EmitObj = EntityManager.Instantiate(PrefabEntity[0]);
                Translation ObjTrans = EntityManager.GetComponentData<Translation>(EmitObj);
                ObjTrans.Value.y = NowEmitComp.LastEmitHigher + 5f;
                ObjTrans.Value.x = RandomData.NextInt(-3,3);

                Entity GuideObj = EntityManager.Instantiate(GuideLineEntity[0]);
                Translation GuideTrans = EntityManager.GetComponentData<Translation>(GuideObj);

                GuideTrans.Value.y = ObjTrans.Value.y;
                GuideTrans.Value.x = 0;

                EntityManager.SetComponentData<Translation>(GuideObj, GuideTrans);

                WallComp NowWallComp = EntityManager.GetComponentData<WallComp>(EmitObj);

                NowWallComp.StartPos = 0;

                int SendSpeed = RandomData.NextInt(-4, 4);

                if(SendSpeed==0)
                {
                    SendSpeed = 1;
                }

                NowWallComp.MoveSpeed = SendSpeed;

                EntityManager.SetComponentData<Translation>(EmitObj, ObjTrans);
                EntityManager.SetComponentData<WallComp>(EmitObj, NowWallComp);

                NowEmitComp.LastEmitHigher += 5f;
                EmitComponent[0] = NowEmitComp;
            }

            EmitCompQuery.CopyFromComponentDataArray(EmitComponent);

            PrefabEntity.Dispose();
            EmitComponent.Dispose();
            KnifeTrans.Dispose();
            GuideLineEntity.Dispose();
        }
    }
}
