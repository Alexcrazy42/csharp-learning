namespace cscharp_learning.Chapter9;

public class Chapter9Class
{
    private static Int32 n = 0;
    private static void M(Int32 x = 9, String s = "A", DateTime dt = default(DateTime),
        Guid guid = new Guid())
    {
        Console.WriteLine($"x = {x}, s = {s}, dt = {dt}, guid = {guid}");
    }

    public void Execute()
    {
        M();

        M(8, "X");

        M(5, guid: Guid.NewGuid(), dt: DateTime.Now);

        int x = 10;
        Console.WriteLine($"x: {++x} {x++}"); // x: 11, 11

        M(++n, (n++).ToString());

        M(s: n++.ToString(), x: n++); // сначала обработается параметр s, хоть он и стоит в сигнатуре метода позже x


        Console.WriteLine("---------------------------------------");
        Int32 x1 = 5;
        GetVal(out x1); // инициализация x1 не обязательная, функция получает ее адрес из стека
                        // функции Execute
                        // использование out со значимыми типами увеличивает производительность
                        // т.к. CLR не надо копировать значения параметров
        Console.WriteLine(x1); // 10

        AddVal(ref x1); // x1 требуется иницилизировать для этого вызоыва с ref
                        // иначе ошибка csc: использование переменной, которой не присвоено значение
        Console.WriteLine(x1); // 20

        //FileStream fs; // объект fs не инициализирован
        //StartProcessingFiles(out fs);
        //// продолжаем пока, остаются файлы для обработки
        //for(; fs != null; ContinueProcessingFiles(ref fs)) 
        //{
        //    fs.Read(new Span<byte>());
        //}

        Console.WriteLine("--------------------------------------------");
        Console.WriteLine(Add(new [] { 1, 2, 3, 4, 5 }));
        Console.WriteLine(Add(1, 2, 3, 4, 5));
        Console.WriteLine(Add()); // передает новый элемент Int32[0] методу Add
        Console.WriteLine(Add(null)); // передает методу Add Значение null, что более эффективно
    }

    private static void GetVal(out Int32 v)
    {
        v = 10; // этот метод должен инициализировать переменную v
                  // если этого не сделать компилятор выкенет ошибку, что до передачи управления из текущего метода,
                  // параметру, которые отметили как out, должно быть присвоено значение
    }

    private static void AddVal(ref Int32 v)
    {
        v += 10; // этот метод может использовать иницилизированный параметр v
                 // в out так нельзя, csc не позволит: использование выходного параметра, которому
                 // не присвоено значение
    }

    private static void StartProcessingFiles(out FileStream fs)
    {
        fs = new FileStream("", FileMode.Open); // в этом методе объект fs должен быть инициализирован
    }

    private static void ContinueProcessingFiles(ref FileStream fs)
    {
        fs.Close(); // Закрытие последнего обрабатываемого файла
        // открыть следующий файл или вернуть Null, если файлов больше нет
        if (true) fs = null;
        else fs = new FileStream("", FileMode.Open);
        
    }

    // ключевое слово params заставляет компилятор рассматривать параметр как 
    // экземпляр настраиваемого аттрибута System.ParamArrayAttribute
    // обнаружив такой вызов, csc проверяет все методы с заданным именем, у которыъ 
    // ни один из параметров не помечен аттрибутом ParamArray
    static Int32 Add(params Int32[] values)
    {
        Int32 sum = 0;
        if (values != null)
        {
            for(Int32 x = 0; x < values.Length; x++)
            {
                sum += values[x];
            }
        }

        return sum;
    }
}