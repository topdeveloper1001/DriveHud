
using Ninject;

namespace Model
{
    /// <summary>
    /// Access for IOC Container
    /// </summary>
    public static class Core
    {
        static Core()
        {
            Kernel = new StandardKernel();
        }

        public static IKernel Kernel { get; private set; }
    }
}