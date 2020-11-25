﻿// Danilo Borges Santos, 2020.

// PolygonCollision, IntervalDistance, ProjectPolygon by
//
// Copyright (c) 2006 Laurent Cozic
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// Code: https://www.codeproject.com/Articles/15573/2D-Polygon-Collision-Detection

using System;

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>Expõe métodos para verificação de colisão entre dois atores.</summary>
    public static class Collision
    {
        /// <summary>
        /// Verifica se os limites do ator 1 está intersectando os limites do ator 2,
        /// e retorna um objeto CollisionResult para verificação do resultado.        
        /// </summary>
        public static CollisionResult ActorCollision(Actor actor1, Actor actor2)
        {
            CollisionResult result = new CollisionResult
            {
                Type = CollisionType.None,
                HasCollided = false
            };

            //Fazemos o cálculo simples de intersecção entre retângulos se não há rotação nos atores.
            if (actor1.Transform.Rotation == 0 && actor2.Transform.Rotation == 0)
            {
                //Verifica se  há colisão.
                if (BoundsCollision(actor1.Bounds, actor2.Bounds))
                {
                    //Cria o resultado da colisão entre retângulos.
                    RectangleCollisionResult rcr = new RectangleCollisionResult
                    {
                        Intersection = Rectangle.Intersect(actor1.Bounds, actor2.Bounds)
                    };

                    var ab = actor1.Bounds;
                    var ob = actor2.Bounds;

                    //O vetor de subtração a ser completado e adicionado.
                    rcr.Subtract = Subtract(ab, ob);

                    result.HasCollided = true;
                    result.Type = CollisionType.Rectangle;
                    result.RectangleResult = rcr;
                }       
            }
            //se as entidades estão rotacionadas.
            else
            {
                PolygonCollisionResult pcr = PolygonCollision(actor1.BoundsR, actor2.BoundsR, actor1.Transform.Velocity);
                
                if(pcr.Intersect)
                {
                    result.HasCollided = true;
                    result.Type = CollisionType.Polygon;
                    result.PolygonResult = pcr;
                }
            }

            return result;
        }

        /// <summary>
        /// Verifica se os limites do retângulo 1 está intersectando os limites da retângulo 2.
        /// </summary>
        public static bool BoundsCollision(Rectangle bounds1, Rectangle bounds2)
        {
            if (bounds1.Intersects(bounds2))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Retorna o valor necessário para voltar a posição anterior da intersecção entre dois retângulos.
        /// </summary>
        public static Vector2 Subtract(Rectangle one, Rectangle two)
        {
            Rectangle rcr = Rectangle.Intersect(one, two);
            Vector2 sub = Vector2.Zero;

            //Lógica de colisão entre retângulos

            //Se na intersecção entre os retângulos
            //A altura é maior que a largura da intersecção,
            //Então significa que foi uma colisão lateral.
            if (rcr.Height > rcr.Width)
            {
                //Verificamos o limite.
                //Se a ponta direita é maior que a ponta esquerda do outro retângulo
                //e essa ponta está dentro do outro retângulo.
                //Então encontramos o valor de subtração.
                //A lógica serve para o restante.
                if (one.Right > two.Left && one.Right < two.Right)
                {
                    sub.X -= one.Right - two.Left;
                }
                else if (one.Left < two.Right && one.Left > two.Left)
                {
                    sub.X -= one.Left - two.Right;
                }
            }
            //O contrário é uma colisão vertical.
            if (rcr.Width > rcr.Height)
            {
                if (one.Bottom > two.Top && one.Bottom < two.Bottom)
                {
                    sub.Y -= one.Bottom - two.Top;
                }
                else if (one.Top < two.Bottom && one.Top > two.Top)
                {
                    sub.Y -= one.Top - two.Bottom;
                }
            }

            return sub;
        }        

        /// <summary>
        /// Verifica se os pixels do ator 1 intersecta outros pixels do ator 2.
        /// </summary>
        /// <param name="bounds1">Os limites atuais do objeto 1 (posição e tamanho).</param>
        /// <param name="data1">O array de cores recebido da textura 1.</param>
        /// <param name="bounds2">Os limites atuais do objeto 2 (posição e tamanho).</param>
        /// <param name="data2">O array de cores recebido da textura 2.</param>
        public static bool PerPixelCollision(Rectangle bounds1, Color[] data1, Rectangle bounds2, Color[] data2)
        {
            //"https://www.austincc.edu/cchrist1/GAME1343/PerPixelCollision/PerPixelCollision.htm"

            bool result = false;

            int top = Math.Max(bounds1.Top, bounds2.Top);
            int bottom = Math.Min(bounds1.Bottom, bounds2.Bottom);
            int left = Math.Max(bounds1.Left, bounds2.Left);
            int right = Math.Min(bounds1.Right, bounds2.Right);

            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    Color colorOne = data1[(x - bounds1.Left) + (y - bounds1.Top) * bounds1.Width];
                    Color colorTwo = data2[(x - bounds2.Left) + (y - bounds2.Top) * bounds2.Width];

                    if (colorOne.A != 0 && colorTwo.A != 0)
                        result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Retorna se os pixels do ator A está intersectando os pixels do ator B utilizando matrizes para cálculos.
        /// </summary>
        /// <param name="transformA">A matrix de transformação do objeto A.</param>
        /// <param name="widthA">A largura do objeto A.</param>
        /// <param name="heightA">A altura do objeto A.</param>
        /// <param name="dataA">O array de cores do objeto A.</param>
        /// <param name="transformB">A matrix de transformação do objeto B.</param>
        /// <param name="widthB">A largura do objeto B.</param>
        /// <param name="heightB">A altura do objeto B.</param>
        /// <param name="dataB">O array de cores do objeto B.</param>
        public static bool InserctPixels(Matrix transformA, int widthA, int heightA, Color[] dataA, Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            // Calculate a matrix which transforms from A's local space into
            // world space and then into B's local space
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            // When a point moves in A's local space, it moves in B's local space with a
            // fixed direction and distance proportional to the movement in A.
            // This algorithm steps through A one pixel at a time along A's X and Y axes
            // Calculate the analogous steps in B:
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            // Calculate the top left corner of A in B's local space
            // This variable will be reused to keep track of the start of each row
            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);

            // For each row of pixels in A
            for (int yA = 0; yA < heightA; yA++)
            {
                // Start at the beginning of the row
                Vector2 posInB = yPosInB;

                // For each pixel in this row
                for (int xA = 0; xA < widthA; xA++)
                {
                    // Round to the nearest pixel
                    int xB = (int)Math.Round(posInB.X);
                    int yB = (int)Math.Round(posInB.Y);

                    // If the pixel lies within the bounds of B
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        // Get the colors of the overlapping pixels
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        // If both pixels are not completely transparent,
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            // then an intersection has been found
                            return true;
                        }
                    }

                    // Move to the next pixel in the row
                    posInB += stepX;
                }

                // Move to the next row
                yPosInB += stepY;
            }

            // No intersection found
            return false;
        }

        /// <summary>
        /// Verifica se um polígono colidiu outro polígono.
        /// </summary>
        /// <param name="polygonA">O primeiro polígono.</param>
        /// <param name="polygonB">O segundo polígono.</param>
        /// <param name="velocity">A velocidade do primeiro polígono.</param>        
        public static PolygonCollisionResult PolygonCollision(Polygon polygonA, Polygon polygonB, Vector2 velocity)
        {
            PolygonCollisionResult result = new PolygonCollisionResult();
            result.Intersect = true;
            result.WillIntersect = true;

            int edgeCountA = polygonA.Edges.Count;
            int edgeCountB = polygonB.Edges.Count;
            float minIntervalDistance = float.PositiveInfinity;
            Vector2 translationAxis = new Vector2();
            Vector2 edge;

            // Loop through all the edges of both polygons
            for (int edgeIndex = 0; edgeIndex < edgeCountA + edgeCountB; edgeIndex++)
            {
                if (edgeIndex < edgeCountA)
                {
                    edge = polygonA.Edges[edgeIndex];
                }
                else
                {
                    edge = polygonB.Edges[edgeIndex - edgeCountA];
                }

                // ===== 1. Find if the polygons are currently intersecting =====

                // Find the axis perpendicular to the current edge
                Vector2 axis = new Vector2(-edge.Y, edge.X);
                axis.Normalize();

                // Find the projection of the polygon on the current axis
                float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
                ProjectPolygon(axis, polygonA, ref minA, ref maxA);
                ProjectPolygon(axis, polygonB, ref minB, ref maxB);

                // Check if the polygon projections are currentlty intersecting
                if (IntervalDistance(minA, maxA, minB, maxB) > 0) result.Intersect = false;

                // ===== 2. Now find if the polygons *will* intersect =====

                // Project the velocity on the current axis
                float velocityProjection = Vector2.Dot(axis, velocity);

                // Get the projection of polygon A during the movement
                if (velocityProjection < 0)
                {
                    minA += velocityProjection;
                }
                else
                {
                    maxA += velocityProjection;
                }

                // Do the same test as above for the new projection
                float intervalDistance = IntervalDistance(minA, maxA, minB, maxB);
                if (intervalDistance > 0) result.WillIntersect = false;

                // If the polygons are not intersecting and won't intersect, exit the loop
                if (!result.Intersect && !result.WillIntersect) break;

                // Check if the current interval distance is the minimum one. If so store
                // the interval distance and the current distance.
                // This will be used to calculate the minimum translation vector
                intervalDistance = Math.Abs(intervalDistance);
                if (intervalDistance < minIntervalDistance)
                {
                    minIntervalDistance = intervalDistance;
                    translationAxis = axis;

                    Vector2 d = polygonA.Center - polygonB.Center;
                    if (Vector2.Dot(d, translationAxis) < 0) translationAxis = -translationAxis;
                }
            }

            // The minimum translation vector can be used to push the polygons appart.
            // First moves the polygons by their velocity
            // then move polygonA by MinimumTranslationVector.
            if (result.WillIntersect) result.Subtract = translationAxis * minIntervalDistance;

            return result;
        }

        // Calculate the distance between [minA, maxA] and [minB, maxB]
        // The distance will be negative if the intervals overlap
        private static float IntervalDistance(float minA, float maxA, float minB, float maxB)
        {
            if (minA < minB)
            {
                return minB - maxA;
            }
            else
            {
                return minA - maxB;
            }
        }

        // Calculate the projection of a polygon on an axis and returns it as a [min, max] interval
        private static void ProjectPolygon(Vector2 axis, Polygon polygon, ref float min, ref float max)
        {
            // To project a point on an axis use the dot product            
            float d = Vector2.Dot(axis, polygon.Points[0]);
            min = d;
            max = d;
            for (int i = 0; i < polygon.Points.Count; i++)
            {
                d = Vector2.Dot(polygon.Points[i], axis);
                if (d < min)
                {
                    min = d;
                }
                else
                {
                    if (d > max)
                    {
                        max = d;
                    }
                }
            }
        }
    }
}