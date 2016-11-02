﻿using Farmhand.Attributes;
using Farmhand.Events.Arguments.MenuEvents;
using StardewValley.Menus;
using System;

namespace Farmhand.Events
{
    /// <summary>
    /// Contains events relating to menus
    /// </summary>
    public static class MenuEvents
    {
        public static event EventHandler<EventArgsOnMenuChanged> OnMenuChanged = delegate { };
        
        [PendingHook]
        internal static void InvokeMenuChanged(IClickableMenu priorMenu, IClickableMenu newMenu)
        {
            EventCommon.SafeInvoke(OnMenuChanged, null, new EventArgsOnMenuChanged(priorMenu, newMenu));
        }
    }
}
