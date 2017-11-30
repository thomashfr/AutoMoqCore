using Moq;

namespace AutoMoqCore.Helpers
{
    public class WithAutomoqer
    {
        public static AutoMoqer Mocker;

        public WithAutomoqer()
        {
            Mocker = new AutoMoqer();
        }

        public WithAutomoqer(Config config)
        {
            Mocker = new AutoMoqer(config);
        }

        public static Mock<T> GetMock<T>() where T : class
        {
            return Mocker.GetMock<T>();
        }

        public static T Create<T>() where T : class
        {
            return Mocker.Create<T>();
        }

        public static void SetInstance<T>(T instance) where T : class
        {
            Mocker.SetInstance(instance);
        }
    }
}