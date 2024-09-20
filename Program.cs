Console.WriteLine("***********************************************");
Console.WriteLine("You are O, Catch X!");
Console.WriteLine("use A, W, S, D for movement");
Console.WriteLine("For now if you don't catch it before");
Console.WriteLine("it reaches an edge, the program will crash.");
Console.WriteLine("Press ANY key to start.");
Console.WriteLine("            GoodLuck!");
Console.WriteLine("************************************************");
Loop();


static string InsertPlayer(Tuple<int, int> Coordinates, in string Field, string player) 
{
    try
    {
        var split =  Field.Split("\r\n");
        var height = split[Coordinates.Item2];
        var result = height.Insert(Coordinates.Item1, player);
        result = result.Remove(Coordinates.Item1 + 1, 1);
        split[Coordinates.Item2] = result;
    
        result = string.Empty;
    foreach(var i in split)
    {
        result += i + "\r\n";
    }
    return result;
    } catch(Exception e)
    {
        Console.WriteLine("THREW AT INSERTPLAYER: ITEM {0}", player);
        Console.ReadLine();
    }
    return Field;
}
static int Sanitized(string widthOrHeight, int current)
{
    var Transformer =  new Func<int, int, int>((max, current) => {
        if (current <= 0) return 0;
        if (current >= max) return max;
        return current;
    });

    var SwitchConstructor = new Func<string, Func<int>>(

        (type) =>{

            var a = new Func<int, int>((wOrH) =>
            {
                    int parsed = (int.TryParse(type[1].ToString(), out parsed)) ? (parsed > 0) ? parsed : 1 : 1;
                    return Transformer.Invoke(wOrH * parsed, current);
            });

            var res = type[0];
            return res switch
            {
                'h' => new Func<int>(() =>
                {
                    return a.Invoke(9);
                }),
                'w' => new Func<int>(() =>
                {
                    return a.Invoke(19);
                }),

            };
        }
    );

    return SwitchConstructor(widthOrHeight).Invoke();
}
static void Loop()
{
    int counter = 0;
    int width = 5;
    bool quit = true;
    int stage = 0;
    int height = 5;
    int TreasureHeight = new Random().Next(10 * stage +1);
    int TreasureWidth = new Random().Next(10 * stage +1);

    Action Sanitizer = () =>
    {
        Action<int> Selection = new Action<int>((stage) => {

                width = Sanitized($"w{stage}", width);
                height = Sanitized($"h{stage}", height);
                TreasureHeight = Sanitized($"w{stage}", TreasureHeight);
                TreasureWidth = Sanitized($"h{stage}", TreasureWidth);
        });
        switch (stage)
        {
            case 0:
                Selection.Invoke(1);
            break;
            case 1:
                Selection.Invoke(2);
            break;
            case 2:
                Selection.Invoke(3);
            break;
            default:
                Console.WriteLine("Reloading");
            break;
        }

    };
    Action Logic = () =>
    {

        if (width == TreasureWidth && height == TreasureHeight)
        {
            Console.BackgroundColor = ConsoleColor.Green;
           if(stage == 2)
           {
                stage = -1;
                height = new Random().Next(10);
                width = new Random().Next(20);
           }
           stage++;
            TreasureHeight = new Random().Next((int)(10 * (stage +1)));
            TreasureWidth = new Random().Next((int)(20 * (stage +1)));
        }
    };
    string Rooms = string.Empty;
    while (quit)
    {
        var readKey = Console.ReadKey();
        if (readKey.KeyChar == 'x')
        {
            stage = (stage == 3) ? stage = 0 : stage = stage +1;
        }
        if(readKey.KeyChar == 'a')
        {
            width--;
        }
        if(readKey.KeyChar == 'd')
        {
            width++;
        }
        if(readKey.KeyChar == 's')
        {
            height++;
        }
        if(readKey.KeyChar == 'w')
        {
            height--;
        }
        if(readKey.KeyChar == 'q')
        {
            quit = false;
        }
        counter++;
        if((counter%3) ==0)
        {
            TreasureWidth++;
        }
        if((counter%3) == 1)
        {
            TreasureHeight++;
        }
        Sanitizer.Invoke();
        Logic.Invoke();
        Console.Clear();
        Thread.Sleep(100);
        Console.BackgroundColor = ConsoleColor.Black;
        Rooms = CreateSeveralRooms(stage);
        Rooms = InsertPlayer(new Tuple<int, int>(TreasureWidth, TreasureHeight),Rooms, "X");
        Rooms = InsertPlayer(new Tuple<int, int>(width, height), Rooms, "O");
        Console.Write(Rooms);
        Console.WriteLine(stage);
        Console.WriteLine();
    }
}
static string CreateRoomAdjoinable(int columns, bool addTop)
{
    string RoomComposite = string.Empty;
    int width = 20;
    int height = 10;
    const int wallwidth = 1;
    string[] rowInRoom = new string[10];
    int AddStar = 0;
    for (int k = columns; k > 0; k--)
    {
        for (int i = rowInRoom.Length; i >= 0; i--)
        {
            if ((i == 0 && addTop) || i == 9)
            {
                rowInRoom[i] += CreatingOuterWall(new string(""), width);
                rowInRoom[i] += (k == 1) ? "\r\n" : "";
            }
            else if (i > 0 && i < 9)
            {
                string buffer = string.Empty;
                rowInRoom[i] += CreateInnerWall(buffer, width, wallwidth);
                if(k == 1)
                {
                    rowInRoom[i] += "\r\n";
                }
            }
        }
    }
    foreach (var i in rowInRoom)
    {
        RoomComposite += i;
    }
    return RoomComposite;
}
static string CreateRoom()
{
    string RoomComposite = string.Empty;
    const int width =  20;
    const int wallwidth = 1;
    string[] Room = new string[10];
 
    for (int i = Room.Length - 1; i >= 0; i--)
    {
        if (i == 0 || i == 9)
        {
            Room[i] += CreatingOuterWall(new string(""), width);
            Room[i] += "\r\n";
        }
        else if (i > 0 && i < 9)
        {

            Room[i] += CreateInnerWall(new string(""), width, wallwidth);
            Room[i] += "\r\n";
        }
    }
    foreach (var i in Room)
    {
        RoomComposite += i;
    }
    return RoomComposite;
}
static string CreateSeveralRooms(int stage)
{
    return stage switch
    {
        0 => CreateRoom(),
        1 => CreateSecondStage(),
        2 => CreateThirdStage(),
        _ => CreateRoom()
    };
}
static string CreateSecondStage()
{
    return CreateMultistage(2);
}
static string CreateThirdStage()
{
    return CreateMultistage(3);
}
static string CreateMultistage(int stage)
{
    string result = string.Empty;
    return CreateRoomRow(stage);
}
static string CreateRoomRow(int columns)
{
    string result = string.Empty;
    string[] Rooms = new string[columns];
    for(int i = 0; i < Rooms.Length; i++)
    {
        Rooms[i] += CreateRoomAdjoinable(columns, (i==0));
    }
    foreach(var i in Rooms)
    {
        result += i;
    }
    return result;
}
static string CreatingOuterWall(string arr, int width)
{
    for (int i = width; i > 0; i--)
    {
       arr += '*';
    }
    return arr;
}
static string CreateInnerWall(string arr, int width, int wallwidth)
{
    for (int i = width; i > 0; i--)
    {
        if (i < wallwidth +1)
        {
            arr += '*';
        }
        if (i > wallwidth && i <= width - wallwidth)
        {
            arr += " ";
        }
        if (i > (width - wallwidth))
        {
            arr +='*';
        }
    }
    return arr;
}
