using Moq;
using Unity;

namespace AutoMoqCore
{
    public class Config
    {
        public Config()
        {
            MockBehavior = MockBehavior.Loose;
            Container = new UnityContainer();
        }

        public MockBehavior MockBehavior { get; set; }
        public IUnityContainer Container { get; set; }
    }
}