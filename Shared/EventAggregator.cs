using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RobotGameShared {
    public interface IHandle<T> {
        void Handle(T e);
    }

    public class EventAggregator {
        readonly Dictionary<Type, List<object>> handlers = new Dictionary<Type, List<object>>();
        
        public void Subscribe<T>(IHandle<T> handler) {
            var interfaces = handler.GetType().GetTypeInfo().ImplementedInterfaces
                .Where(x => x.GetTypeInfo().IsGenericType && x.GetGenericTypeDefinition() == typeof(IHandle<>));

            foreach (var interfaceType in interfaces) {
                var eventType = interfaceType.GetTypeInfo().GenericTypeArguments[0];

                List<object> handlerList;
                if (!handlers.TryGetValue(eventType, out handlerList)) {
                    handlerList = new List<object>();
                    handlers[eventType] = handlerList;
                }

                handlerList.Add(handler);
            }
        }

        public void Publish<T>(T eventObject) {
            foreach (IHandle<T> handler in handlers[typeof(T)]) {
                handler.Handle(eventObject);
            }
        }
    }
}
