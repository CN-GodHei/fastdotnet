using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace Fastdotnet.Core.Middleware
{
    /// <summary>
    /// A thread-safe registry for storing and retrieving types of dynamically loaded middleware.
    /// This service is registered as a singleton and acts as the central point of truth for the DynamicMiddlewareDispatcher.
    /// It uses WeakReference to hold middleware types, preventing it from blocking plugin assembly unloading.
    /// </summary>
    public class DynamicMiddlewareRegistry
    {
        private readonly ConcurrentDictionary<string, WeakReference<Type>> _middlewareTypes = new ConcurrentDictionary<string, WeakReference<Type>>();

        /// <summary>
        /// Registers a middleware type.
        /// </summary>
        /// <param name="middlewareType">The type of the middleware to register. Must implement IDynamicMiddleware.</param>
        public void Register(Type middlewareType)
        {
            if (middlewareType?.FullName == null) return;
            _middlewareTypes[middlewareType.FullName] = new WeakReference<Type>(middlewareType);
        }

        /// <summary>
        /// Unregisters a middleware type.
        /// </summary>
        /// <param name="middlewareType">The type of the middleware to unregister.</param>
        public void Unregister(Type middlewareType)
        {
            if (middlewareType?.FullName == null) return;
            _middlewareTypes.TryRemove(middlewareType.FullName, out _);
        }

        /// <summary>
        /// Gets a snapshot of the currently registered and active middleware types.
        /// Dead references to garbage-collected types are cleaned up during this process.
        /// </summary>
        /// <returns>An enumerable of middleware types.</returns>
        public IEnumerable<Type> GetMiddlewareTypes()
        {
            var activeTypes = new List<Type>();
            foreach (var pair in _middlewareTypes)
            {
                if (pair.Value.TryGetTarget(out Type targetType))
                {
                    activeTypes.Add(targetType);
                }
                else
                {
                    // Proactively remove dead references from the dictionary
                    _middlewareTypes.TryRemove(pair.Key, out _);
                }
            }
            return activeTypes;
        }
    }
}
