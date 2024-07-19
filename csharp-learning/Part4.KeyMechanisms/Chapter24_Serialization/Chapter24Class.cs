using System.Runtime.Serialization.Formatters.Binary;

namespace csharp_learning.Part4.KeyMechanisms.Chapter24_Serialization;

public class Chapter24Class
{
    public void Execute()
    {
        // создание графа объектов для последующей сериализации в поток
        var objectGraph = new List<String> { "Jeff", "Kristin", "Aidan", "Grant" };

        Stream stream = SerializeToMemory(objectGraph);

        // Обнуляем все для данного примера
        stream.Position = 0;
        objectGraph = null;

        objectGraph = (List<string>) DeserializeFromMemory(stream);
        foreach (var s in objectGraph)
        {
            Console.Write(s + " ");
        }
    }

    private MemoryStream SerializeToMemory(Object objectGraph)
    {
        // конструирование потока, который будет содержать 
        // сериализованные объекты
        MemoryStream stream = new MemoryStream();

        // задание форматирования при сериализации
        BinaryFormatter formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011
        // заставляем модуль форматирования сериализовать объекты в поток
        formatter.Serialize(stream, objectGraph);

#pragma warning restore SYSLIB0011

        return stream;
    }

    private Object DeserializeFromMemory(Stream stream)
    {
        // задание форматирование при сериализации
        BinaryFormatter formatter = new BinaryFormatter();

        // заставляем модуль форматирования десериализовать объекты из потока
#pragma warning disable SYSLIB0011
        return formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
    }
}
