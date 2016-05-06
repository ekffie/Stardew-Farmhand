﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Farmhand.Events.Arguments;
using Farmhand.Logging;

namespace Farmhand.Events
{
    //TODO This can have it's performance improved by quite a bit. Important considering the GlobalRouteInvoke will be called upwards of 100 times PER FRAME on heavily used
    //methods like draw. (Which modders should never, ever do. Modders if you're reading this - please ask me! Always happy to add an event. This class' sole purpose is to 
    //provide an alternative until I get around to implementing your requests.)
    public static class GlobalRouteManager
    {
        public static int ListenedMethods = 0;
                
        //IsEnabled could be a property which returns Listeners.Any or Listeners.Count > 0 but it's being accessed potentailly thousands of times per frame.
        public static bool IsEnabled = false;

        private static List<Action<EventArgsGlobalRouteManager>>[] _listeners;        
        private static List<Action<EventArgsGlobalRouteManager>>[] Listeners
        {
            get
            {
                if (_listeners == null)
                {
                    _listeners = new List<Action<EventArgsGlobalRouteManager>>[ListenedMethods];
                }
                return _listeners;
            }
        }

        private static readonly Dictionary<string, int> MapIndexes = new Dictionary<string, int>();

        public static void MapIndex(string type, string method, int index)
        {
            var key = $"{type}.{method}";
            if (MapIndexes.ContainsKey(key))
                return;

            MapIndexes[key] = index;
        }

        public static void InitialiseMappings()
        {
        }


        //public static void GlobalRouteInvoke(string type, string method, out object output, params object[] @parans) //TODO Add once implemented
        public static void GlobalRouteInvoke(int index, string type, string method)
        {
            if (!IsEnabled)
                return;
                        

            if (Listeners[index] != null)
            {
                var evtArgs = new EventArgsGlobalRouteManager(type, method, null, null);
                foreach (var evt in Listeners[index])
                {
                    evt.Invoke(evtArgs);
                }
            }
        }

        //public static bool IsBeingListenedTo(string type, string method)
        public static bool IsBeingListenedTo(int method)
        {
           // Logging.Log.Success($"BLAH {method}");
            return Listeners[method] != null;
        }

        /// <summary>
        /// Attach a listener and enable the global route table
        /// </summary>
        /// <param name="type">The type containing the method to listen for</param>
        /// <param name="method">The method to listen for</param>
        /// <param name="callback">The delegate to add</param>
        public static void Listen(string type, string method, Action<EventArgsGlobalRouteManager> callback)
        {
            var key = $"{type}.{method}";
            int index;
            if (MapIndexes.TryGetValue(key, out index))
            {
                if (Listeners[index] == null)
                    Listeners[index] = new List<Action<EventArgsGlobalRouteManager>>();

                Listeners[index].Add(callback);
                IsEnabled = true;
            }
            else
            {
                throw new Exception("The method ({key}) is not available for listening");
            }
            
        }

        /// <summary>
        /// Remove an attached listener and disable the global route table if no listeners are attached
        /// </summary>
        /// <param name="type">The type containing the method to listen for</param>
        /// <param name="method">The method to listen for</param>
        /// <param name="callback">The delegate to remove. This must be the same instance used when first registering the listener</param>
        [Obsolete("Something wrong with this")]
        public static void Remove(string type, string method, Action<EventArgsGlobalRouteManager> callback)
        {
            //var key = $"{type}.{method}";
            //if (Listeners.ContainsKey(key))
            //{
            //    if (Listeners[key] != null)
            //    {
            //        Listeners[key].Remove(callback);
            //        if (Listeners[key].Count <= 0)
            //        {
            //            Listeners[key] = null;
            //        }
            //    }

            //    if (Listeners[key] == null)
            //    {
            //        Listeners.Remove(key);
            //    }   
            //}

            //if (Listeners.Count <= 0)
            //{
            //    IsEnabled = false;
            //}
        }
    }
}
