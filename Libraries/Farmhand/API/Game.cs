﻿namespace Farmhand.API
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Farmhand.Extensibility;

    using Microsoft.Xna.Framework.Graphics;

    using StardewValley;
    using StardewValley.Menus;

    /// <summary>
    ///     Game-related API Functionality
    /// </summary>
    public static class Game
    {
        private static RenderTarget2D screen;

        /// <summary>
        ///     Gets or sets the current active clickable menu.
        /// </summary>
        public static IClickableMenu ActiveClickableMenu
        {
            get
            {
                return Game1.activeClickableMenu;
            }

            set
            {
                Game1.activeClickableMenu = value;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether has loaded game.
        /// </summary>
        public static bool HasLoadedGame => Game1.hasLoadedGame;

        /// <summary>
        ///     Gets the current event.
        /// </summary>
        public static Event CurrentEvent => Game1.CurrentEvent;

        /// <summary>
        ///     Gets the current player.
        /// </summary>
        public static Farmer Player => Game1.player;

        /// <summary>
        ///     Gets the graphics device.
        /// </summary>
        public static GraphicsDevice GraphicsDevice => Game1.graphics?.GraphicsDevice;

        /// <summary>
        ///     Gets a saves unique identifier
        /// </summary>
        public static ulong GameUniqueId => Game1.uniqueIDForThisGame;

        internal static RenderTarget2D Screen
        {
            get
            {
                if (screen == null)
                {
                    screen =
                        (RenderTarget2D)
                        typeof(Game1).GetField("screen", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(Game1.game1);
                }

                return screen;
            }
        }

        internal static Game1 CreateGameInstance()
        {
            var overridingExtension = ExtensibilityManager.Extensions.FirstOrDefault(e => e.GameOverrideClass != null);
            if (overridingExtension != null)
            {
                return (Game1)Activator.CreateInstance(overridingExtension.GameOverrideClass);
            }

            return GetFarmhandOverrideInstance();
        }

        internal static Game1 GetFarmhandOverrideInstance()
        {
            return new Game1();
        }
    }
}