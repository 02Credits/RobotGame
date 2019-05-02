using Autofac;
using System.Reflection;

namespace RobotGameShared {
    public static class Factory {
        private static IContainer container;
        private static ContainerBuilder builder;

        public static void BeginRegistration() {
            builder = new ContainerBuilder();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsImplementedInterfaces()
                .AsSelf()
                .InstancePerLifetimeScope();
        }

        public static void RegisterSingleton<T>(T singleton) where T : class {
            builder.RegisterInstance(singleton).AsSelf();
        }

        public static void EndRegistration() {
            container = builder.Build();
            builder = null;
        }

        public static T Resolve<T>() {
            return container.Resolve<T>();
        }
    }
}
