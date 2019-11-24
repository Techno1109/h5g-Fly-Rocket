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
            /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

            KnifeDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(RigidBody) },
            };


            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。
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