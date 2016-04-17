﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Farmhand.API.Utilities
{
    public static class TextureUtility
    {
        public static void AddSpriteToSpritesheet(ref Texture2D spritesheet, Texture2D sprite, int spritesheetIndex, int spriteWidth, int spriteHeight)
        {
            var rect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, spritesheetIndex, spriteWidth,
                spriteHeight);
            spritesheet = PatchTexture(spritesheet, sprite, new Rectangle(0, 0, spriteWidth, spriteHeight), rect);
        }

        public static Texture2D PatchTexture(Texture2D @base, Texture2D input, Rectangle source, Rectangle destination, bool asNewTexture = false)
        {
            if ((source.Width*source.Height) != (destination.Width*destination.Height))
            {
                Logging.Log.Exception("Error patching texture", new Exception("Texture source and destination must match when trying to patch a texture"));
            }
            
            var newData = new Color[source.Width * source.Height];
            input.GetData<Color>(0, source, newData, 0, source.Width * source.Height);

            if (asNewTexture || destination.Bottom > @base.Height || destination.Right > @base.Width)
            {
                Rectangle originalRect = new Rectangle(0, 0, @base.Width, @base.Height);
                var originalSize = @base.Width * @base.Height;
                var originalData = new Color[originalSize];
                @base.GetData<Color>(originalData);
                @base = new Texture2D(Game1.graphics.GraphicsDevice, Math.Max(destination.Right, @base.Width), Math.Max(destination.Bottom, @base.Height));
                @base.SetData<Color>(0, originalRect, originalData, 0, originalSize);
            }

            @base.SetData<Color>(0, destination, newData, 0, destination.Width * destination.Height);

            return @base;
        }
    }
}
