using System;
using AutoMoqCore.Unity;
using Unity;

namespace AutoMoqCore
{
    public interface IIoC
    {
        T Resolve<T>();
        object Resolve(Type type);
        void RegisterInstance<T>(T instance);
        void RegisterInstance(object instance, Type type);
        object Container { get; }
        void Setup(AutoMoqer autoMoqer, Config config, IMocking mocking);
    }

    public class UnityIoC : IIoC
    {
        private readonly IUnityContainer _container;

        public UnityIoC()
        {
            this._container = new UnityContainer();
        }

        public UnityIoC(IUnityContainer container)
        {
            this._container = container;
        }

        public T Resolve<T>()
        {
            return this._container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public void RegisterInstance<T>(T instance)
        {
            _container.RegisterInstance<T>(instance);
        }

        public void RegisterInstance(object instance, Type type)
        {
            _container.RegisterInstance(type, instance);
        }

        public object Container { get { return _container;  } }

        public void Setup(AutoMoqer autoMoqer, Config config, IMocking mocking)
        {
            AddTheAutoMockingContainerExtensionToTheContainer(autoMoqer, config, mocking);
            RegisterInstance(this);
        }

        private void AddTheAutoMockingContainerExtensionToTheContainer(AutoMoqer automoqer, Config config, IMocking mocking)
        {
            var container = config.Container;
            container.RegisterInstance(config);
            container.RegisterInstance(automoqer);
            container.RegisterInstance<IIoC>(this);
            container.RegisterInstance<IMocking>(mocking);
            container.AddNewExtension<AutoMockingContainerExtension>();
        }
    }
}