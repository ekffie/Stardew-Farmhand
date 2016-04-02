﻿using Revolution.Attributes;
using Revolution.Logging;
using System;
using System.Collections.Generic;
using Revolution.Registries;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Revolution.Registries.Containers;

namespace Revolution.Content
{
    public class ContentManager : Microsoft.Xna.Framework.Content.ContentManager
    {
        private Dictionary<string, Texture2D> _cachedAlteredTextures = new Dictionary<string, Texture2D>();

        [Hook(HookType.Entry, "StardewValley.Game1", "LoadContent")]
        internal static void ConstructionHook()
        {
            Log.Verbose("Using Revolution's ContentManager");
            Game1.game1.Content = new ContentManager(Game1.game1.Content.ServiceProvider, Game1.game1.Content.RootDirectory);
        }

        public ContentManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }
        public ContentManager(IServiceProvider serviceProvider, string rootDirectory)
            : base(serviceProvider, rootDirectory)
        {
        }

        public override T Load<T>(string assetName)
        {
            var item = XnbRegistry.GetItem(assetName);
            if (item?.OwningMod?.ModState == null || item.OwningMod.ModState != ModState.Loaded) return base.Load<T>(assetName);
            
            try
            {
                if (item.IsXnb)
                {
                    var currentDirectory = Path.GetDirectoryName(item.AbsoluteFilePath);
                    var relPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" +
                                  RootDirectory + "\\";
                    if (currentDirectory != null)
                    {
                        var relRootUri = new Uri(relPath, UriKind.Absolute);
                        var fullPath = new Uri(currentDirectory, UriKind.Absolute);
                        var relUri = relRootUri.MakeRelativeUri(fullPath) + "/" + item.File;

                        Log.Verbose($"Using own asset replacement: {assetName} = {relPath}");
                        return base.Load<T>(relUri);
                    }
                }
                else if (item.IsTexture && typeof(T) == typeof(Texture2D))
                {
                    return (T)(LoadTexture(assetName, item));
                }
            }
            catch (Exception ex)
            {
                Log.Exception("Error reading own file", ex);
                return base.Load<T>(assetName);
            }
            return base.Load<T>(assetName);
        }

        private object LoadTexture(string assetName, ModXnb item)
        {
            var obj = TextureRegistry.GetItem(item.OwningMod, item.Texture).Texture;

            if (obj == null) return null;

            Log.Success(item.Destination.ToString());

            if (item.Destination != null)
            {
                //TODO, Error checking on this.
                //TODO, Multiple mods should be able to edit this
                var originalTexture = base.Load<Texture2D>(assetName);

                Log.Verbose("Is A Constructed Texture");
                string assetKey = $"{assetName}-\u2764-modified";
                if (_cachedAlteredTextures.ContainsKey(assetKey))
                {
                    Log.Verbose("Which we already had cached");
                    return _cachedAlteredTextures[assetKey];
                }
                else
                {
                    Log.Verbose("Trying to construct texture from scratch");
                    var originalData = new Color[originalTexture.Width * originalTexture.Height];
                    var modData = new Color[obj.Width * obj.Height];
                    originalTexture.GetData<Color>(originalData);
                    obj.GetData<Color>(modData);

                    var newObject = new Texture2D(Game1.graphics.GraphicsDevice, originalTexture.Width, originalTexture.Height);
                    newObject.SetData<Color>(originalData);
                    newObject.SetData<Color>(0, item.Destination, modData, 0, obj.Width * obj.Height);

                    _cachedAlteredTextures[assetKey] = newObject;
                    obj = newObject;
                }
            }

            Log.Verbose($"Using own asset replacement: {assetName} = {item.OwningMod.Name}.{item.Texture}");
            Log.Success("I've done it mooom!");
            return obj;
        }
        
    }
}

    
    