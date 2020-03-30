﻿// Danilo Borges Santos, 2020. 
// Email: danilo.bsto@gmail.com
// Versão: Conillon [1.0]

namespace Microsoft.Xna.Framework.Graphics
{
    /// <summary>Classe que agrupa propriedades para disponibilidade da entidade.</summary>
    public class EnableGroup
    {
        /// <summary>Obtém ou define se a entidade está ativa.</summary>
        public bool IsEnabled { get; set; } = true;
        /// <summary>Obtém ou define se a entidade é visível.</summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>Inicia uma nova instância da classe EnableGroup</summary>
        public EnableGroup() { }

        /// <summary>Inicia uma nova instância da classe EnableGroup com suas preferências</summary>
        /// <param name="enabled">True se a entidade está ativa.</param>
        /// <param name="visible">True se a entidade é visível.</param>
        public EnableGroup(bool enabled, bool visible) 
        {
            IsEnabled = enabled;
            IsVisible = visible;
        }        

        /// <summary>Define as propriedades Enabled e Visible como true.</summary>
        public void Show()
        {
            IsEnabled = true;
            IsVisible = true;
        }

        /// <summary>Define as propriedades Enabled e Visible como false.</summary>
        public void Hide()
        {
            IsEnabled = false;
            IsVisible = false;
        }
    }
}