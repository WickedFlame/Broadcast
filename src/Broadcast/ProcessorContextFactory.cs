using System;

namespace Broadcast
{
    public static class ProcessorContextFactory
    {
        /// <summary>
        /// Factory delegate that returns the default mode that the <see cref="IProcessorContext"/> runs in
        /// </summary>
        public static Func<ProcessorMode> ModeFactory;

        /// <summary>
        /// Factory delegate that returns the default <see cref="IProcessorContext"/>
        /// </summary>
        public static Func<IProcessorContext> ContextFactory = () => new ProcessorContext();

        internal static IProcessorContext GetContext()
        {
            var context = ContextFactory != null ? ContextFactory() : new ProcessorContext();
            if (ModeFactory != null)
            {
                context.Mode = ModeFactory();
            }

            return context;
        }

        internal static ProcessorMode GetMode()
        {
            return ModeFactory != null ? ModeFactory() : ProcessorMode.Default;
        }
    }
}
