using Unity.Entities;
using Unity.Tiny.Core;
using Unity.Tiny.Input;

namespace throwknife
{
    public class KnifeMove : ComponentSystem
    {

        EntityQueryDesc KnifeDesc;

        EntityQuery KnifeQuery;


        protected override void OnCreate()
        {
            /*ECS�ɂ����āA�N�G���̍쐬��OnCreate�ōs���̂���΂ƂȂ��Ă��܂�*/

            KnifeDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(RigidBody) },
            };


            /*GetEntityQuery�Ŏ擾�������ʂ͎����I�ɊJ������邽�߁AFree���s�������������Ȃ��Ă����ł��B*/
            //�쐬�����N�G���̌��ʂ��擾���܂��B
            KnifeQuery = GetEntityQuery(KnifeDesc);
        }

        protected override void OnUpdate()
        {

            var Input = EntityManager.World.GetExistingSystem<InputSystem>();

            bool InputButton = false;

            InputButton = Input.GetMouseButton(0);

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

        }
    }
}