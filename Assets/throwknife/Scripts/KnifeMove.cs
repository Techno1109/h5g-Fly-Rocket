using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Core2D;
using Unity.Tiny.UIControls;
using Unity.Mathematics;
using Unity.Collections;

namespace throwknife
{
    public class KnifeMove : ComponentSystem
    {

        EntityQueryDesc KnifeDesc;

        EntityQuery KnifeQuery;

        EntityQueryDesc ButtonDesc;

        EntityQuery ButtonQuery;

        protected override void OnCreate()
        {
            /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

            KnifeDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(RigidBody) },
            };

            ButtonDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] {typeof(MoveButtonTag) ,typeof(PointerInteraction)},
            };



            /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
            //�쐬�����N�G���̌��ʂ��擾���܂��B
            KnifeQuery = GetEntityQuery(KnifeDesc);

            ButtonQuery = GetEntityQuery(ButtonDesc);
        }

        protected override void OnUpdate()
        {
            /*�`�����N�C�e���[�V�����͍s�����ԉ��т��₷���A�����̂��ʓ|�Ȃ��߁AForeachAPI���g�p���܂�*/

            NativeArray<PointerInteraction> MoveButtons = ButtonQuery.ToComponentDataArray<PointerInteraction>(Allocator.TempJob);


            if (MoveButtons.Length <= 0)
            {
                MoveButtons.Dispose();
                return;
            }

            bool InputButton = false;

            for (int i=0;i<MoveButtons.Length; i++)
            {
                InputButton = InputButton || MoveButtons[i].down;
            }

            Entities.With(KnifeQuery).ForEach((ref RigidBody Rigid,ref KnifeTag Tag) =>
           {
               if (InputButton==true&& Rigid.UseGravity&&Tag.ActiveTag==true)
               {
                   if (!Rigid.IsActive)
                   {
                       Rigid.IsActive = true;
                   }
                   Rigid.Velocity.y += 0.8f*World.TinyEnvironment().fixedFrameDeltaTime;
                   if (Rigid.Velocity.y > 2f)
                   {
                       Rigid.Velocity.y = 2f;
                   }
               }

           });

            

            MoveButtons.Dispose();
        }
    }
}