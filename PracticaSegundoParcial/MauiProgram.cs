using Microsoft.Extensions.Logging;
using System.IO;
using PracticaSegundoParcial.Data;

namespace PracticaSegundoParcial;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Ruta a la base de datos SQLite
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "clientes.db3");
        builder.Services.AddSingleton(new ClienteDatabase(dbPath));

        return builder.Build();
    }
}
