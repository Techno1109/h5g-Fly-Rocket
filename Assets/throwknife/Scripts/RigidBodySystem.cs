using Unity.Entities;
using Unity.Tiny.Core2D;
using Unity.Tiny.Core;
using Unity.Collections;

namespace throwknife
{
    [UpdateBefore(typeof(KnifeMove))]
    public class RigidBodySystem : ComponentSystem
    {
        const float Gravity = 0.5f;
        EntityQueryDesc RigidBodyQueryDesc;
        EntityQuery RigidBodyQuery;

        protected override void OnCreate()
        {
            /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

            //RigidBodyComponent���A�^�b�`����Ă���A�Ȃ�����Translation���A�^�b�`����Ă���Entity���擾���܂�
            RigidBodyQueryDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(RigidBody),typeof(Translation) },
            };

            /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
            //�쐬�����N�G���̌��ʂ��擾���܂��B

            RigidBodyQuery = GetEntityQuery(RigidBodyQueryDesc);
        }

        protected override void OnUpdate()
        {
            NativeArray<RigidBody> RigidBodyArray = RigidBodyQuery.ToComponentDataArray<RigidBody>(Allocator.TempJob);
            NativeArray<Translation> TranslationArray = RigidBodyQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            float DeltaTime = World.TinyEnvironment().fixedFrameDeltaTime;

            for (int EntityNum = 0; EntityNum<RigidBodyArray.Length; EntityNum++)
            {
                RigidBody NowRigidBody = RigidBodyArray[EntityNum];
                Translation NowTranslation = TranslationArray[EntityNum];

                if (!NowRigidBody.IsActive)
                {
                    continue;
                }

                if(NowRigidBody.UseGravity)
                {
                    NowRigidBody.Velocity.y -= Gravity * DeltaTime;
                    if(NowRigidBody.Velocity.y<-2f)
                    {
                        NowRigidBody.Velocity.y = -2f;
                    }
                }

                if(NowRigidBody.ActiveVec.x)
                {
                    NowTranslation.Value.x += NowRigidBody.Velocity.x;
                }

                if (NowRigidBody.ActiveVec.y)
                {
                    NowTranslation.Value.y += NowRigidBody.Velocity.y;
                }

                if (NowRigidBody.ActiveVec.z)
                {
                    NowTranslation.Value.z += NowRigidBody.Velocity.z;
                }

                RigidBodyArray[EntityNum] = NowRigidBody;
                TranslationArray[EntityNum] = NowTranslation;
            }

            //�����܂ł�ToDataArray�Ŏ擾�ł���Array�̓L���b�V���̗l�Ȃ��̂Ȃ̂ŁA�{Entity�ɏ������ޕK�v������܂��B
            RigidBodyQuery.CopyFromComponentDataArray(RigidBodyArray);
            RigidBodyQuery.CopyFromComponentDataArray(TranslationArray);

            //NativeArray��Unsafe�ȓ��I�m�ۂ̂��߁A�����ŉ������܂���B
            //�����ƊJ�����܂��傤�B
            TranslationArray.Dispose();
            RigidBodyArray.Dispose();
        }
    }
}