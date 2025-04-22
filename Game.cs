using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgProdAvanz_Examen1
{
    internal class Game
    {
        private char[,] mapa;
        private int filas;
        private int columnas;
        private Player player = null!;
        private List<Enemy> enemies;
        private Random random;
        private bool gameActive;
        private bool endPlayerTurn;
        private MessageQueue messageQueue;
        private MovementHistory movementHistory;

        public Game(int filas = 6, int columnas = 10, int enemyCount = 2)
        {
            this.filas = filas;
            this.columnas = columnas;
            this.mapa = new char[filas, columnas];
            this.enemies = new List<Enemy>();
            this.random = new Random();
            this.gameActive = true;
            this.endPlayerTurn = false;
            this.messageQueue = new MessageQueue();
            this.movementHistory = new MovementHistory();

            InitializeMap();
            CreatePlayer();
            CreateEnemies(enemyCount);
        }

        private void InitializeMap()
        {
            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    mapa[i, j] = '.'; //Espacios vacios
                }
            }
        }

        private void CreatePlayer()
        {
            Console.WriteLine("¡Bienvenido a Water Emblem, muevete de forma estrategica y acaba uno a uno con todos los enemigos!");
            Console.Write("Ingresa el nombre de tu personaje: ");
            string playerName = string.Empty;

            try
            {
                playerName = Console.ReadLine() ?? string.Empty;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error al leer la entrada de nombre del jugador: {ex.Message}");
                Console.WriteLine("Se usará un nombre por defecto (Héroe).");
            }

            if (string.IsNullOrWhiteSpace(playerName))
                playerName = "Héroe";

            //Posicionar al jugador en el centro abajo, lo más alejado del jefe
            int playerRow = filas - 1;
            int playerCol = columnas / 2;

            player = new Player(playerName);
            player.Row = playerRow;
            player.Column = playerCol;

            //Marcar en el mapa
            mapa[playerRow, playerCol] = 'P';
        }

        private void CreateEnemies(int count)
        {
            //Crear jefe en el centro arriba, lo más alejado del jugador
            int bossRow = 0;
            int bossCol = columnas / 2;

            Boss boss = new Boss("Señor Oscuro", 150, 15, 25, bossRow, bossCol);
            enemies.Add(boss);
            mapa[bossRow, bossCol] = 'B';

            //Generar enemigos aleatorios
            string[] enemyNames = { "Caballero", "Arquero", "Lancero", "Mago", "Berserker" };

            for (int i = 0; i < count; i++)
            {
                int row, col;
                do
                {
                    row = random.Next(filas);
                    col = random.Next(columnas);
                } while (mapa[row, col] != '.'); //Busca un espacio vacío

                string name = $"{enemyNames[random.Next(enemyNames.Length)]} Enemigo";
                int health = random.Next(40, 81); //Vida para los enemigos
                int minAttack = random.Next(3, 7); //Ataque minimo para enemigos
                int maxAttack = random.Next(minAttack + 2, minAttack + 6); //Ataque maximo para enemigos

                Enemy enemy = new Enemy(name, health, minAttack, maxAttack, row, col);
                enemies.Add(enemy);
                mapa[row, col] = 'E';
            }
        }

        public void Run()
        {
            while (gameActive)
            {
                //Turno del jugador
                PlayerTurn();

                //Verificar si el juego terminó después del turno del jugador
                if (!gameActive) break;

                //Turno de los enemigos
                EnemyTurn();

                //Verificar victoria o derrota
                CheckGameState();
            }

            Console.WriteLine("¡Gracias por jugar!");
        }

        private void PlayerTurn()
        {
            endPlayerTurn = false;
            player.ResetMovementPoints();

            messageQueue.ClearMessages();

            while (!endPlayerTurn)
            {
                Console.Clear();
                DrawMap();
                DisplayStats();

                DisplayTurnOptions();

                Console.Write("\nElije una opción: ");
                string input = Console.ReadLine() ?? string.Empty;

                if (input.ToLower() == "q")
                {
                    gameActive = false;
                    return;
                }

                if (!int.TryParse(input, out int option))
                {
                    Console.WriteLine("Entrada inválida. Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                    continue;
                }

                switch (option)
                {
                    case 1: //Mover
                        HandlePlayerMovement();
                        break;
                    case 2: //Atacar (si hay enemigos al lado)
                        if (HasAdjacentEnemies())
                        {
                            HandlePlayerAttack();
                        }
                        else
                        {
                            Console.WriteLine("No hay enemigos adyacentes para atacar.");
                            Console.WriteLine("Presiona cualquier tecla para continuar...");
                            Console.ReadKey();
                        }
                        break;
                    case 3: //Descansar
                        player.Rest();
                        Console.WriteLine($"{player.Name} descansa y recupera parte de su salud.");
                        Console.WriteLine("Presiona cualquier tecla para continuar...");
                        Console.ReadKey();
                        endPlayerTurn = true;
                        break;
                    case 4: //Terminar turno
                        endPlayerTurn = true;
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Presiona cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void HandlePlayerMovement()
        {
            bool movementComplete = false;

            movementHistory.ClearHistory();
            movementHistory.RecordMovement(player.Row, player.Column, player.MovementPoints);

            while (!movementComplete)
            {
                Console.Clear();
                DrawMap();

                Console.WriteLine($"PASOS RESTANTES: {player.MovementPoints}");
                Console.WriteLine();

                if (player.CanMove())
                {
                    DisplayMovementOptions();
                }
                else
                {
                    Console.WriteLine("Te has quedado sin pasos para moverte.");
                }

                if (movementHistory.CanUndo())
                {
                    Console.WriteLine("Z. Deshacer último movimiento");
                }

                Console.WriteLine("0. Volver a las opciones de turno");

                Console.Write("\nElije una opción: ");
                string input = Console.ReadLine() ?? string.Empty;

                if (input == "0")
                {
                    movementComplete = true;
                    continue;
                }

                //Opción para deshacer movimiento
                if (input.ToLower() == "z" && movementHistory.CanUndo())
                {
                    try
                    {
                        //Obtener el estado anterior
                        var (prevRow, prevCol, prevMoves) = movementHistory.UndoMovement();

                        //Actualizar el mapa
                        mapa[player.Row, player.Column] = '.';

                        //Restaurar el estado del jugador
                        player.Row = prevRow;
                        player.Column = prevCol;

                        //Restaurar puntos de movimiento
                        while (player.MovementPoints < prevMoves)
                        {
                            player.ReceiveMovementPoint();
                        }

                        //Colocar al jugador en el mapa
                        mapa[player.Row, player.Column] = 'P';

                        messageQueue.AddMessage("Has deshecho tu último movimiento.");
                        continue;
                    }
                    catch (Exception ex)
                    {
                        messageQueue.AddMessage($"Error al deshacer: {ex.Message}");
                        continue;
                    }
                }

                if (!player.CanMove() && input.ToLower() != "z" && input != "0")
                {
                    Console.WriteLine("No puedes moverte más. Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                    continue;
                }

                if (!int.TryParse(input, out int direccion))
                {
                    Console.WriteLine("Entrada inválida. Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                    continue;
                }

                int nuevaFila = player.Row;
                int nuevaColumna = player.Column;

                switch (direccion)
                {
                    case 1: //Arriba
                        if (player.Row > 0) nuevaFila--;
                        break;
                    case 2: //Abajo
                        if (player.Row < filas - 1) nuevaFila++;
                        break;
                    case 3: //Izquierda
                        if (player.Column > 0) nuevaColumna--;
                        break;
                    case 4: //Derecha
                        if (player.Column < columnas - 1) nuevaColumna++;
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Presiona cualquier tecla para continuar...");
                        Console.ReadKey();
                        continue;
                }

                //Verifica si el movimiento es válido
                if (mapa[nuevaFila, nuevaColumna] != '.')
                {
                    Console.WriteLine("¡No puedes moverte a una casilla ocupada!");
                    Console.WriteLine("Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                    continue;
                }

                //Guardar el estado actual antes de mover
                movementHistory.RecordMovement(player.Row, player.Column, player.MovementPoints);

                //Actualizar posición del jugador en el mapa visual
                mapa[player.Row, player.Column] = '.';
                player.Row = nuevaFila;
                player.Column = nuevaColumna;
                mapa[player.Row, player.Column] = 'P';

                //Reducir pasos
                player.Move();
            }
        }
        private void HandlePlayerAttack()
        {
            List<Enemy> adjacentEnemies = GetAdjacentEnemies();

            if (adjacentEnemies.Count == 0)
            {
                messageQueue.AddMessage("No hay enemigos adyacentes para atacar.");
                Console.WriteLine("Presiona cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nSelecciona un enemigo para atacar:");
            for (int i = 0; i < adjacentEnemies.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {adjacentEnemies[i].Name}");
            }
            Console.WriteLine("0. Volver a las opciones de turno");

            Console.Write("\nElije una opción: ");
            string input = Console.ReadLine() ?? string.Empty;

            if (input == "0")
                return;

            if (!int.TryParse(input, out int option) || option < 1 || option > adjacentEnemies.Count)
            {
                messageQueue.AddMessage("Entrada inválida.");
                Console.WriteLine("Presiona cualquier tecla para continuar...");
                Console.ReadKey();
                return;
            }

            Enemy targetEnemy = adjacentEnemies[option - 1];
            int damage = player.Attack();
            targetEnemy.TakeDamage(damage);

            messageQueue.AddMessage($"{player.Name} ataca a {targetEnemy.Name} y le causa {damage} puntos de daño!");

            //Verificar si el enemigo murió
            if (!targetEnemy.IsAlive)
            {
                messageQueue.AddMessage($"¡{targetEnemy.Name} ha sido derrotado!");
                player.AddKill();

                //Limpiar enemigo del mapa
                mapa[targetEnemy.Row, targetEnemy.Column] = '.';
                enemies.Remove(targetEnemy);
            }

            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();

            //Terminar el turno automáticamente después de atacar
            endPlayerTurn = true;
        }

        private void EnemyTurn()
        {
            Console.Clear();
            DrawMap();
            Console.WriteLine("\n--- TURNO DE LOS ENEMIGOS ---\n");

            // Limpiar mensajes anteriores al inicio del turno enemigo
            messageQueue.ClearMessages();
            messageQueue.AddMessage("⚔️ Comienza el turno enemigo");

            foreach (var enemy in enemies)
            {
                enemy.ResetMovementPoints();
            }

            foreach (var enemy in enemies.ToList())
            {
                while (enemy.CanMove())
                {
                    var (newRow, newCol) = enemy.DecideNextMove(mapa, player.Row, player.Column);

                    // Si la posición no cambió, el enemigo no pudo o no quiso moverse
                    if (newRow == enemy.Row && newCol == enemy.Column)
                        break;

                    // Verifica si la nueva posición es válida
                    if (mapa[newRow, newCol] != '.')
                        break;

                    // Actualizar posición del enemigo en el mapa visual
                    mapa[enemy.Row, enemy.Column] = '.';
                    enemy.Row = newRow;
                    enemy.Column = newCol;
                    mapa[enemy.Row, enemy.Column] = enemy is Boss ? 'B' : 'E';

                    messageQueue.AddMessage($"{enemy.Name} se mueve a una nueva posición.");

                    // Redibujar el mapa y mensajes para mostrar el movimiento
                    Console.Clear();
                    DrawMap();
                    System.Threading.Thread.Sleep(500); // Pausa momentánea para visualizar el movimiento
                }

                // Verifica si está adyacente al jugador para atacar
                if (IsAdjacentToPlayer(enemy))
                {
                    int damage = enemy.Attack();
                    player.TakeDamage(damage);

                    messageQueue.AddMessage($"⚡ {enemy.Name} ataca a {player.Name} y le causa {damage} puntos de daño!");

                    // Redibujar para mostrar el ataque
                    Console.Clear();
                    DrawMap();
                    System.Threading.Thread.Sleep(800); // Pausa un poco más larga para ataques

                    // Verifica si el jugador murió
                    if (!player.IsAlive)
                    {
                        messageQueue.ShowImportantMessage($"¡{player.Name} ha muerto! Fin del juego.");
                        gameActive = false;
                        Console.WriteLine("Presiona cualquier tecla para continuar...");
                        Console.ReadKey();
                        return;
                    }
                }
            }

            messageQueue.AddMessage("🏁 Fin del turno enemigo");
            Console.WriteLine("Presiona cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private void CheckGameState()
        {
            //Verificaa si todos los enemigos han muerto
            if (enemies.Count == 0)
            {
                Console.Clear();
                DrawMap();
                Console.WriteLine("\n¡VICTORIA! Has derrotado a todos los enemigos.");
                gameActive = false;
                Console.WriteLine("Presiona cualquier tecla para continuar...");
                Console.ReadKey();
            }

            //Verifica si el jugador ha sido derrotado (también se verifica en EnemyTurn)
            if (!player.IsAlive)
            {
                Console.Clear();
                DrawMap();
                Console.WriteLine("\n¡HAS MUERTO! Tu personaje ha caído en batalla.");
                gameActive = false;
                Console.WriteLine("Presiona cualquier tecla para continuar...");
                Console.ReadKey();
            }
        }

        private void DrawMap()
        {
            Console.WriteLine("Mapa del juego:");
            Console.WriteLine();

            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    ConsoleColor originalColor = Console.ForegroundColor;

                    //Establecer color según el tipo de entidad
                    switch (mapa[i, j])
                    {
                        case 'P': //Jugador
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write('P');
                            break;
                        case 'E': //Enemigo normal
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write('E');
                            break;
                        case 'B': //Jefe
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.Write('B');
                            break;
                        case '.': //Espacio vacío
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write('.');
                            break;
                        default:
                            Console.Write(mapa[i, j]);
                            break;
                    }

                    Console.ForegroundColor = originalColor;
                }
                Console.WriteLine();
            }
            Console.WriteLine();

            //Mostrar mensajes en cola
            messageQueue.DisplayMessages();
        }

        private void DisplayStats()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("--- ESTADÍSTICAS DEL JUGADOR ---");
            Console.WriteLine(player.GetDescription());
            Console.ResetColor();
            Console.WriteLine();

            // Mostrar estadísticas de los enemigos
            Console.WriteLine("--- ENEMIGOS RESTANTES ---");
            foreach (var enemy in enemies)
            {
                if (enemy is Boss)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(enemy.GetDescription());
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(enemy.GetDescription());
                    Console.ResetColor();
                }
            }
            Console.WriteLine();
        }

        private void DisplayTurnOptions()
        {
            Console.WriteLine("OPCIONES DE TURNO:");
            Console.WriteLine("1. Moverse");

            //Solo muestra opción de atacar si hay enemigos adyacentes
            if (HasAdjacentEnemies())
            {
                Console.WriteLine("2. Atacar");
            }

            Console.WriteLine("3. Descansar (recupera vida y termina tu turno)");
            Console.WriteLine("4. Terminar turno");
            Console.WriteLine("q. Salir del juego");
        }

        private void DisplayMovementOptions()
        {
            Console.WriteLine("Opciones de movimiento:");

            if (player.Row > 0 && mapa[player.Row - 1, player.Column] == '.')
                Console.WriteLine("1. Arriba");
            if (player.Row < filas - 1 && mapa[player.Row + 1, player.Column] == '.')
                Console.WriteLine("2. Abajo");
            if (player.Column > 0 && mapa[player.Row, player.Column - 1] == '.')
                Console.WriteLine("3. Izquierda");
            if (player.Column < columnas - 1 && mapa[player.Row, player.Column + 1] == '.')
                Console.WriteLine("4. Derecha");

            Console.WriteLine("0. Volver a las opciones de turno");
        }

        private bool HasAdjacentEnemies()
        {
            return GetAdjacentEnemies().Count > 0;
        }

        private List<Enemy> GetAdjacentEnemies()
        {
            List<Enemy> adjacentEnemies = new List<Enemy>();

            //Verificar celdas adyacentes (arriba, abajo, izquierda, derecha)
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int newRow = player.Row + dx[i];
                int newCol = player.Column + dy[i];

                if (newRow >= 0 && newRow < filas && newCol >= 0 && newCol < columnas)
                {
                    if (mapa[newRow, newCol] == 'E' || mapa[newRow, newCol] == 'B')
                    {
                        //Encuentra a el enemigo en esta posición
                        Enemy? adjacent = enemies.FirstOrDefault(e => e.Row == newRow && e.Column == newCol);
                        if (adjacent != null)
                        {
                            adjacentEnemies.Add(adjacent);
                        }
                    }
                }
            }

            return adjacentEnemies;
        }

        private bool IsAdjacentToPlayer(Enemy enemy)
        {
            //Verificar si el enemigo está junto al jugador
            return (Math.Abs(enemy.Row - player.Row) == 1 && enemy.Column == player.Column) ||
                   (Math.Abs(enemy.Column - player.Column) == 1 && enemy.Row == player.Row);
        }
    }
}
