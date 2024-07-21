using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace csharp_learning.Part4.KeyMechanisms.Chapter24_Serialization;

#pragma warning disable SYSLIB0011
public class Chapter24Class
{
    public void Execute()
    {
        // создание графа объектов для последующей сериализации в поток
        var objectGraph = new List<String> { "Jeff", "Kristin", "Aidan", "Grant" };

        Stream stream1 = SerializeToMemory(objectGraph);

        // Обнуляем все для данного примера
        stream1.Position = 0;
        objectGraph = null;

        objectGraph = (List<string>)DeserializeFromMemory(stream1);
        foreach (var s in objectGraph)
        {
            Console.WriteLine(s + " ");
        }

        using (var stream = new MemoryStream())
        {
            // 1. Создание желаемого модуля форматирования
            IFormatter formatter = new BinaryFormatter();

            // 2. Создание объекта SurrogateSelector
            SurrogateSelector ss = new SurrogateSelector();

            // 3. Селектор выбирает наш суррогат для объекта DateTime
            ss.AddSurrogate(typeof(DateTime), formatter.Context,
                new UniversalToLocalTimeSerializationSurrogate());

            // ПРИМЕЧАНИЕ. AddSurrogate можно вызывать более одного раза
            // для регистрации нескольких суррогатов

            // 4. Модуль форматирования использует наш селектор
            formatter.SurrogateSelector = ss;

            // Создание объекта DateTime с локальным временем машины
            // и его сериализация
            DateTime localTimeBeforeSerialize = DateTime.Now;
            formatter.Serialize(stream, localTimeBeforeSerialize);

            // Поток выводит универсальное время в виде строки,
            // проверяя, что все работает
            stream.Position = 0;
            Console.WriteLine(new StreamReader(stream).ReadToEnd());

            // Десериализация универсального времени и преобразование
            // объекта DateTime в локальное время
            stream.Position = 0;
            DateTime localTimeAfterDeserialize = (DateTime)formatter.Deserialize(stream);
            // Проверка корректности работы
            Console.WriteLine("LocalTimeBeforeSerialize ={0}", localTimeBeforeSerialize);

            Console.WriteLine("LocalTimeAfterDeserialize={0}", localTimeAfterDeserialize);
        }
    }

    private MemoryStream SerializeToMemory(Object objectGraph)
    {
        // конструирование потока, который будет содержать 
        // сериализованные объекты
        MemoryStream stream = new MemoryStream();

        // задание форматирования при сериализации
        BinaryFormatter formatter = new BinaryFormatter();

        // заставляем модуль форматирования сериализовать объекты в поток
        formatter.Serialize(stream, objectGraph);



        return stream;
    }

    private Object DeserializeFromMemory(Stream stream)
    {
        // задание форматирование при сериализации
        BinaryFormatter formatter = new BinaryFormatter();

        // заставляем модуль форматирования десериализовать объекты из потока
        return formatter.Deserialize(stream);
    }
}
#pragma warning restore SYSLIB0011

internal sealed class UniversalToLocalTimeSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(Object obj, SerializationInfo info, StreamingContext context)
    {
        // Переход от локального к мировому времени
        info.AddValue("Date", ((DateTime)obj).ToUniversalTime().ToString("u"));
    }

    public Object SetObjectData(Object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector)
    {
        // Переход от мирового времени к локальному
        return DateTime.ParseExact(info.GetString("Date"), "u", null).ToLocalTime();
    }
}