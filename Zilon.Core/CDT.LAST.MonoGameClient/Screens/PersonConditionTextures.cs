using System;

using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.Screens
{
    /// <summary>
    /// Structure to store icon info to drawing.
    /// </summary>
    public record PersonConditionTextures
    {
        public PersonConditionTextures(Texture2D icon, Texture2D background)
        {
            Icon = icon ?? throw new ArgumentNullException(nameof(icon));
            Background = background ?? throw new ArgumentNullException(nameof(background));
        }

        public Texture2D Background { get; }

        public Texture2D Icon { get; }
    }
}