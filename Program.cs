using ProgProdAvanz_Examen1;

internal class Program
{
    private static void Main(string[] args)
    {
        MostrarBanner();
        Console.WriteLine("\nPresiona cualquier tecla para comenzar...");
        Console.ReadKey();

        //Configurar reglas del juego
        int filas = 6;
        int columnas = 10;
        int enemyCount = 2; //NO contando al jefe

        Game game = new Game(filas, columnas, enemyCount);
        game.Run();

        Console.WriteLine("Desarrollado por Rafael Portocarrero");
        Console.WriteLine("\nPresiona cualquier tecla para salir...");
        Console.ReadKey();



    }

    private static void MostrarBanner()
    {
        ConsoleColor colorOriginal = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Cyan; // Color azul claro para el banner

        Console.WriteLine(@" _    _       _              _____          _     _                
| |  | |     | |            |  ___|        | |   | |               
| |  | | __ _| |_ ___ _ __  | |__ _ __ ___ | |__ | | ___ _ __ ___  
| |/\| |/ _` | __/ _ \ '__| |  __| '_ ` _ \| '_ \| |/ _ \ '_ ` _ \ 
\  /\  / (_| | ||  __/ |    | |__| | | | | | |_) | |  __/ | | | | |
 \/  \/ \__,_|\__\___|_|    \____/_| |_| |_|_.__/|_|\___|_| |_| |_|
                                                                   
                                                                   ");

        Console.WriteLine("====================================");
        Console.WriteLine("   WATER EMBLEM v1.0   ");
        Console.WriteLine("====================================");
        Console.ForegroundColor = colorOriginal; // Restaurar color original
    }

}
