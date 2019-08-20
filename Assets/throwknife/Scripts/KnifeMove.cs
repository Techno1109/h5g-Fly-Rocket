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
            /*ECSにおいて、クエリの作成はOnCreateで行うのが定石となっています*/

            KnifeDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] { typeof(KnifeTag), typeof(RigidBody) },
            };

            ButtonDesc = new EntityQueryDesc()
            {
                All = new ComponentType[] {typeof(MoveButtonTag) ,typeof(PointerInteraction)},
            };



            /*GetEntityQueryで取得した結果は自動的に開放されるため、Freeを行う処理を書かなくていいです。*/
            //作成したクエリの結果を取得します。
            KnifeQuery = GetEntityQuery(KnifeDesc);

            ButtonQuery = GetEntityQuery(ButtonDesc);
        }

        protected override void OnUpdate()
        {
            /*チャンクイテレーションは行数が間延びしやすく、書くのが面倒なため、ForeachAPIを使用します*/

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