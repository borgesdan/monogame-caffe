﻿// Danilo Borges Santos, 2020.

using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// Componente que verifica uma colisão de boxes entre AnimatedEntities. 
    /// </summary>
    public class BoxCollisionComponent : ActorComponent
    {
        /// <summary>
        /// Obtém o resultado da colisão entre boxes.
        /// </summary>
        /// <typeparam name="T1">É uma estrutura que armazena valores da colisão.</typeparam>
        /// <typeparam name="T2">É uma estrutura que armazena valores da colisão.</typeparam>
        public class Result<T1, T2> where T1 : struct where T2 : struct
        {
            /// <summary>
            /// A entidade que implementa este delegate.
            /// </summary>
            public AnimatedActor Source { get; set; }
            /// <summary>
            /// A entidade que participou da colisão.
            /// </summary>
            public AnimatedActor CollidedEntity { get; set; }
            /// <summary>
            /// Fornece acesso aos valores de tempo do jogo.
            /// </summary>
            public GameTime GameTime { get; set; }
            /// <summary>
            /// As caixas recorrentes da colisão.
            /// </summary>
            public Tuple<T1, T2> Boxes { get; set; }
            /// <summary>
            /// O resultado da colisão entre os boxes.
            /// </summary>
            public RectangleCollisionResult CollisionResult { get; set; }

            public Result(AnimatedActor source, AnimatedActor collided, GameTime gameTime, Tuple<T1, T2> boxes, RectangleCollisionResult result)
            {
                Source = source;
                CollidedEntity = collided;
                GameTime = gameTime;
                Boxes = boxes;
                CollisionResult = result;
            }
        }

        //---------------------------------------//
        //-----         VARIÁVEIS           -----//
        //---------------------------------------//

        AnimatedActor entity = null;
        Result<CollisionBox, CollisionBox> ccResult = new Result<CollisionBox, CollisionBox>(null, null, null, null, new RectangleCollisionResult());
        Result<CollisionBox, AttackBox> caResult = new Result<CollisionBox, AttackBox>(null, null, null, null, new RectangleCollisionResult());
        Result<AttackBox, AttackBox> aaResult = new Result<AttackBox, AttackBox>(null, null, null, null, new RectangleCollisionResult());
        Result<CollisionBox, Rectangle> cbResult = new Result<CollisionBox, Rectangle>(null, null, null, null, new RectangleCollisionResult());
        Result<AttackBox, Rectangle> abResult = new Result<AttackBox, Rectangle>(null, null, null, null, new RectangleCollisionResult());

        //---------------------------------------//
        //-----         PROPRIEDADES        -----//
        //---------------------------------------//

        /// <summary>Obtém ou define se o componente deve utilizar uma tela do tipo LayeredScreen para busca de atores para colisão.</summary>
        LayeredScreen Screen { get; set; } = null;
        /// <summary>Obtém ou define os atores para verificação caso a propriedade Screen seja nulo.</summary>
        List<AnimatedActor> Entities { get; set; } = new List<AnimatedActor>();

        //---------------------------------------//
        //-----         EVENTOS             -----//
        //---------------------------------------//

        /// <summary>
        /// Encapsula um método a ser chamado como resultado de uma colisão entre CollisionBoxes.        
        public event Action<Result<CollisionBox, CollisionBox>> CxCCollision;
        /// <summary>
        /// Encapsula um método a ser chamado como resultado de uma colisão entre um CollisionBox e um AttackBox.
        /// </summary>
        public event Action<Result<CollisionBox, AttackBox>> CxACollision;
        /// <summary>
        /// Encapsula um método a ser chamado como resultado de uma colisão entre AttackBoxes.
        /// </summary>
        public event Action<Result<AttackBox, AttackBox>> AxACollision;
        /// <summary>
        /// Encapsula um método a ser chamado como resultado de uma colisão entre um CollisionBox e um retângulo.
        /// </summary>
        public event Action<Result<CollisionBox, Rectangle>> CxBCollision;
        /// <summary>
        /// Encapsula um método a ser chamado como resultado de uma colisão entre um AttackBox e um retângulo.
        /// </summary>
        public event Action<Result<AttackBox, Rectangle>> AxBCollision;

        //---------------------------------------//
        //-----         CONSTRUTOR          -----//
        //---------------------------------------//        

        /// <summary>
        /// Inicializa uma nova instância de BoxCollisionComponent.
        /// </summary>
        /// <param name="animatedEntity">Define o AnimatedEntity o qual esse componente será associado.</param>
        public BoxCollisionComponent(AnimatedActor animatedEntity) : base(animatedEntity)
        {
            Name = nameof(BoxCollisionComponent);
            entity = animatedEntity;
        }

        /// <summary>
        /// Inicializa uma nova instância da classe BoxCollisionComponent como uma cópia de outro BoxCollisionComponent.
        /// </summary>
        /// <param name="destination">O ator a ser associado esse componente.</param>
        /// <param name="source">A origem para cópia.</param>
        public BoxCollisionComponent(AnimatedActor destination, BoxCollisionComponent source) : base(destination, source)
        {
            AxACollision = source.AxACollision;
            AxBCollision = source.AxBCollision;
            CxACollision = source.CxACollision;
            CxBCollision = source.CxBCollision;
            CxCCollision = source.CxCCollision;
            Screen = source.Screen;
            Entities = source.Entities;
        }

        /// <summary>
        /// Define a tela a ser utilizada para buscar os atores para verificação de colisão.
        /// </summary>        
        public void SetScreen(LayeredScreen screen)
        {
            Screen = screen;
            Entities.Clear();
        }

        /// <summary>
        /// Define as entidades a serem utilizadas para a verificação de colisão ao invés de utilizar uma tela.
        /// </summary>
        public void SetEntites(params AnimatedActor[] entities)
        {
            Entities.Clear();
            Entities.AddRange(entities);

            Screen = null;
        }

        /// <summary>Atualiza o componente.</summary>
        /// <param name="gameTime">Fornece acesso aos valores de tempo do jogo.</param>
        protected override void _Update(GameTime gameTime)
        {
            if (Screen != null)
            {
                //Busca todas as entidades vísiveis da tela.
                foreach (var other in Screen.VisibleActors)
                {
                    // Prossegue se a entidade atual é diferente da entidade da lista.
                    if (!entity.Equals(other))
                    {   
                        if(other is AnimatedActor o)
                            Check(entity, o, gameTime);
                    }
                }
            }
            else
            {
                foreach (var other in Entities)
                {
                    // Prossegue se a entidade atual é diferente da entidade da lista.
                    if (!entity.Equals(other))
                    {
                        Check(entity, other, gameTime);
                    }
                }
            }
        }

        private void Check(AnimatedActor e, AnimatedActor o, GameTime gameTime)
        {
            RectangleCollisionResult result = new RectangleCollisionResult();

            //Procura todos os CollisionBox da entidade
            for (int i = 0; i < e.CollisionBoxes.Count; i++)
            {
                CollisionBox ecb = e.CollisionBoxes[i];

                for (int j = 0; j < o.CollisionBoxes.Count; j++)
                {
                    CollisionBox ocb = o.CollisionBoxes[j];

                    if (Collision.BoundsCollision(ecb.Bounds, ocb.Bounds))
                    {
                        result.Intersection = Rectangle.Intersect(ecb.Bounds, ocb.Bounds);
                        result.Subtract = Collision.Subtract(ecb.Bounds, ocb.Bounds);

                        ccResult.Source = entity;
                        ccResult.CollidedEntity = o;
                        ccResult.GameTime = gameTime;
                        ccResult.Boxes = new Tuple<CollisionBox, CollisionBox>(ecb, ocb);
                        ccResult.CollisionResult = result;

                        CxCCollision?.Invoke(ccResult);
                    }
                }

                for (int j = 0; j < o.AttackBoxes.Count; j++)
                {
                    AttackBox oab = o.AttackBoxes[j];

                    if (Collision.BoundsCollision(ecb.Bounds, oab.Bounds))
                    {
                        result.Intersection = Rectangle.Intersect(ecb.Bounds, oab.Bounds);
                        result.Subtract = Collision.Subtract(ecb.Bounds, oab.Bounds);

                        caResult.Source = entity;
                        caResult.CollidedEntity = o;
                        caResult.GameTime = gameTime;
                        caResult.Boxes = new Tuple<CollisionBox, AttackBox>(ecb, oab);
                        caResult.CollisionResult = result;

                        CxACollision?.Invoke(caResult);
                    }
                }

                if (Collision.BoundsCollision(e.CollisionBoxes[i].Bounds, o.Bounds))
                {
                    result.Intersection = Rectangle.Intersect(ecb.Bounds, o.Bounds);
                    result.Subtract = Collision.Subtract(ecb.Bounds, o.Bounds);

                    cbResult.Source = entity;
                    cbResult.CollidedEntity = o;
                    cbResult.GameTime = gameTime;
                    cbResult.Boxes = new Tuple<CollisionBox, Rectangle>(ecb, o.Bounds);
                    cbResult.CollisionResult = result;

                    CxBCollision?.Invoke(cbResult);
                }
            }

            //Procura todos os AttackBox da entidade
            for (int i = 0; i < e.AttackBoxes.Count; i++)
            {
                AttackBox eab = e.AttackBoxes[i];

                for (int j = 0; j < o.CollisionBoxes.Count; j++)
                {
                    CollisionBox ocb = o.CollisionBoxes[j];

                    if (Collision.BoundsCollision(eab.Bounds, ocb.Bounds))
                    {
                        result.Intersection = Rectangle.Intersect(eab.Bounds, ocb.Bounds);
                        result.Subtract = Collision.Subtract(eab.Bounds, ocb.Bounds);

                        caResult.Source = entity;
                        caResult.CollidedEntity = o;
                        caResult.GameTime = gameTime;
                        caResult.Boxes = new Tuple<CollisionBox, AttackBox>(ocb, eab);
                        caResult.CollisionResult = result;

                        CxACollision?.Invoke(caResult);
                    }
                }

                for (int j = 0; j < o.AttackBoxes.Count; j++)
                {
                    AttackBox oab = o.AttackBoxes[j];

                    if (Collision.BoundsCollision(eab.Bounds, oab.Bounds))
                    {
                        result.Intersection = Rectangle.Intersect(eab.Bounds, oab.Bounds);
                        result.Subtract = Collision.Subtract(eab.Bounds, oab.Bounds);

                        aaResult.Source = entity;
                        aaResult.CollidedEntity = o;
                        aaResult.GameTime = gameTime;
                        aaResult.Boxes = new Tuple<AttackBox, AttackBox>(eab, oab);
                        aaResult.CollisionResult = result;

                        AxACollision?.Invoke(aaResult);
                    }
                }

                if (Collision.BoundsCollision(e.AttackBoxes[i].Bounds, o.Bounds))
                {
                    result.Intersection = Rectangle.Intersect(eab.Bounds, o.Bounds);
                    result.Subtract = Collision.Subtract(eab.Bounds, o.Bounds);

                    abResult.Source = entity;
                    abResult.CollidedEntity = o;
                    abResult.GameTime = gameTime;
                    abResult.Boxes = new Tuple<AttackBox, Rectangle>(eab, o.Bounds);
                    abResult.CollisionResult = result;

                    AxBCollision?.Invoke(abResult);
                }
            }
        }

        protected override void _Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            
        }

        //---------------------------------------//
        //-----         DISPOSE             -----//
        //---------------------------------------//

        private bool disposed;

        protected override void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                this.aaResult = null;
                this.abResult = null;
                this.AxACollision = null;
                this.AxBCollision = null;
                this.caResult = null;
                this.cbResult = null;
                this.ccResult = null;
                this.CxACollision = null;
                this.CxBCollision = null;
                this.CxCCollision = null;
                this.entity = null;
                this.Screen = null;
            }

            disposed = true;

            base.Dispose(disposing);
        }
    }
}