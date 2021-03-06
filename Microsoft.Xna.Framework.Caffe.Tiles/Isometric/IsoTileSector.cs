﻿// Danilo Borges Santos, 2020.

using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>
    /// Representa um setor de um mapa de tiles.
    /// </summary>
    public class IsoTileSector<T> : IDisposable where T : struct
    {
        T[,] array = null;        

        /// <summary>
        /// Obtém ou define a tabela de índices com seus respectivos Tiles.
        /// </summary>
        public Dictionary<T, IsoTile> Table { get; set; } = new Dictionary<T, IsoTile>();        

        /// <summary>
        /// Inicializa uma nova instância de Setor.
        /// </summary>
        /// <param name="_array">Um array com a mesma quantidade de linhas e colunas da propriedade Length desta classe.</param>
        /// <param name="table">Define a tabela de índices com seus respectivos Tiles.</param>
        public IsoTileSector(T[,] _array, Dictionary<T, IsoTile> table)
        {
            array = _array;
            Table = table;
        }

        /// <summary>
        /// Obtém o mapa com a numeração dos tiles.
        /// </summary>
        public T[,] GetMap()
        {
            return (T[,])array.Clone();
        }

        //---------------------------------------//
        //-----         DISPOSE             -----//
        //---------------------------------------//

        private bool disposed = false;

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                this.array = null;
                this.Table.Clear();
                this.Table = null;
            }

            disposed = true;
        }
    }
}