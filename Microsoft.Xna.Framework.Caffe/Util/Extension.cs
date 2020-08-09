﻿// Danilo Borges Santos, 2020.

namespace Microsoft.Xna.Framework
{
    public static class Extension
    {
        /// <summary>
        /// Obtém a metade da largura de um retângulo.
        /// </summary>
        public static int GetHalfW(this Rectangle rectangle)
        {
            if (rectangle.Width != 0)
                return rectangle.Width / 2;
            else
                return 0;
        }

        /// <summary>
        /// Obtém a metade da altura de um retângulo.
        /// </summary>
        public static int GetHalfH(this Rectangle rectangle)
        {
            if (rectangle.Height != 0)
                return rectangle.Height / 2;
            else
                return 0;
        }

        /// <summary>
        /// Obtém a metade do tamanho de um retângulo.
        /// </summary>
        public static Point GetHalf(this Rectangle rectangle)
        {
            return new Point(rectangle.GetHalfW(), rectangle.GetHalfH());
        }
    }
}
