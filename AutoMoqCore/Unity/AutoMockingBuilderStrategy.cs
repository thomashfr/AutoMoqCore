using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Builder;
using BuilderStrategy = Unity.Strategies.BuilderStrategy;

namespace AutoMoqCore.Unity
{
    public class AutoMockingBuilderStrategy : BuilderStrategy
    {
        private readonly IIoC ioc;
        private readonly IMocking mocking;

        public AutoMockingBuilderStrategy(IMocking mocking, IIoC ioc)
        {
            this.mocking = mocking;
            this.ioc = ioc;
        }

        public override void PreBuildUp(ref BuilderContext context)
        {
            var type = GetTheTypeFromTheBuilderContext(context);
            if (AMockObjectShouldBeCreatedForThisType(type))
            {
                var mock = CreateAMockTrackedByAutoMoq(type);
                context.Existing = mock.ActualObject;
            }

            LoadAnyAbstractDependenciesOf(type);
        }
        

        private void LoadAnyAbstractDependenciesOf(Type type)
        {
            foreach (var dependency in AbstractDependenciesOf(type))
                BuildThisByAskingTheContainerForIt(dependency);
        }

        private void BuildThisByAskingTheContainerForIt(Type type)
        {
            try
            {
                ioc.Resolve(type);
            }
            catch
            {
                // ignored
            }
        }


        private static IEnumerable<Type> AbstractDependenciesOf(Type type)
        {
            return type.GetConstructors()
                .SelectMany(x => x.GetParameters())
                .Distinct()
                .Where(x => x.ParameterType.IsAbstract)
                .Where(x => x.ParameterType.IsInterface == false)
                .Select(x => x.ParameterType);
        }

        private MockCreationResult CreateAMockTrackedByAutoMoq(Type type)
        {
            return mocking.CreateANewMockObjectAndRegisterIt(type);
        }

        private bool AMockObjectShouldBeCreatedForThisType(Type type)
        {
            return ThisTypeIsNotAFunction(type) &&
                   ThisTypeIsNotRegistered(type) &&
                   ThisIsNotTheTypeThatIsBeingResolvedForTesting(type);
        }

        private bool ThisIsNotTheTypeThatIsBeingResolvedForTesting(Type type)
        {
            var mocker = ioc.Resolve<AutoMoqer>();
            return (mocker.ResolveType == null || mocker.ResolveType != type);
        }

        private static bool ThisTypeIsNotAFunction(Type type)
        {
            return type.Name != "Func`1";
        }

        private static Type GetTheTypeFromTheBuilderContext(BuilderContext context)
        {
            return (context.Type);
        }

        private bool ThisTypeIsNotRegistered(Type type)
        {
            return mocking.AMockHasNotBeenRegisteredFor(type);
        }
    }
}