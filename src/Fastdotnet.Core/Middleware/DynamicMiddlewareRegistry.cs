using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Fastdotnet.Core.Middleware
{
    /// <summary>
    /// A thread-safe registry for storing and retrieving types of dynamically loaded middleware.
    /// This service is registered as a singleton and acts as the central point of truth for the DynamicMiddlewareDispatcher.
    /// </summary>
    public class DynamicMiddlewareRegistry
    {
        private readonly ConcurrentDictionary<Type, bool> _middlewareTypes = new ConcurrentDictionary<Type, bool>();

        /// <summary>
        /// Registers a middleware type.
        /// </summary>
        /// <param name="middlewareType">The type of the middleware to register. Must implement IDynamicMiddleware.</param>
        public void Register(Type middlewareType)
        {
            _middlewareTypes.TryAdd(middlewareType, true);
        }

        /// <summary>
        /// Unregisters a middleware type.
        /// </summary>
        /// <param name="middlewareType">The type of the middleware to unregister.</param>
        public void Unregister(Type middlewareType)
        {
            _middlewareTypes.TryRemove(middlewareType, out _);
        }

        /// <summary>
        /// Gets a snapshot of the currently registered middleware types.
        /// </summary>
        /// <returns>An enumerable of middleware types.</returns>
        public IEnumerable<Type> GetMiddlewareTypes()
        {
            return _middlewareTypes.Keys;
        }
    }
}
