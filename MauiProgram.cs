using Camera.MAUI;
using CommunityToolkit.Maui;
using Maui.FixesAndWorkarounds;
using Microsoft.Extensions.Logging;

namespace CameraView;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureMauiWorkarounds()
            .UseMauiCameraView()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });



#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
