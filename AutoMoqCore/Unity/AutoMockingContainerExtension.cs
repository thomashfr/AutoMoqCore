using Unity.Builder;
using Unity.Extension;

namespace AutoMoqCore.Unity
{
    internal class AutoMockingContainerExtension : UnityContainerExtension
    {
        private readonly IIoC ioc;
        private readonly IMocking mocking;

        public AutoMockingContainerExtension(IIoC ioc, IMocking mocking)
        {
            this.ioc = ioc;
            this.mocking = mocking;
        }

        protected override void Initialize()
        {
            SetBuildingStrategyForBuildingUnregisteredTypes();
        }

        private void SetBuildingStrategyForBuildingUnregisteredTypes()
        {
            var strategy = new AutoMockingBuilderStrategy(mocking, ioc);
            Context.Strategies.Add(strategy, UnityBuildStage.PreCreation);
        }
    }
}