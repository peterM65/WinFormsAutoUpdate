using LoopbackRecorderWinForms.Helper;
using LoopbackRecorderWinForms.Interface;
using Microsoft.Extensions.DependencyInjection;


namespace LoopbackRecorderWinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ISpeechAiRep, SpeechAiRep>();

            services.AddSingleton<MainForm>();

            var provider = services.BuildServiceProvider();

            ApplicationConfiguration.Initialize();
            Application.Run(provider.GetRequiredService<MainForm>());
        }
    }
}
