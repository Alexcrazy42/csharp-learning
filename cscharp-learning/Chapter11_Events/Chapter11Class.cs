namespace csharp_learning.Chapter11_Events;

public class Chapter11Class
{
    public void Execute()
    {
        MailManager mailManager = new MailManager();
        Fax fax = new Fax(mailManager);
        mailManager.SimulateNewMail("from", "to", "subject");

        TypeWithLotsOfEvents twle = new TypeWithLotsOfEvents();

        twle.Foo += HandleFooEvent;

        twle.SimulateFoo();
    }

    private static void HandleFooEvent(object sender, FooEventArgs e)
    {
        Console.WriteLine("Handling Foo Event Here......");
    }
}

// Этап 1. Определение типа для хранения информации, которая 
// передается получателем уведомления о событии
public class NewMailEventArgs : EventArgs
{
    private readonly String from, subject, to;

    public NewMailEventArgs(String from, String to, String subject)
    {
        this.from = from;
        this.to = to;
        this.subject = subject;
    }

    public String From { get { return from; } }

    public String To { get { return to; } }

    public String Subject { get { return subject; } }
}


public class MailManager
{
    // Этап 2. Определение члена события
    // Получатели уведомления о событии должны предоставить метод обратного вызова, 
    // прототип которого совпадает с типом-делегатом EventHandler<NewMailEventArgs>
    // Определение самого члена события
    // При компиляции этой строки компилятор превратит ее в следующие три конструкции:
    // 1) Закрытое поле делегата, инициализированное значение null
    // 2) Открытый метод add_Xxx (где Xxx - это имя события)
    // позволяет объектам регистрироваться для получения уведомлений о событии
    // 3) Открытый метод remove_Xxx (где Xxx - это имя события)
    // позволяет объектам отменять регистрацию в качестве получателей уведомлений о событии
    public event EventHandler<NewMailEventArgs> NewMailEvent;

    // Этап 3. Определение метода, ответственного за уведомление зарегестрированных объектов о 
    // событии. Если этот класс изолированный, нужно сделать метода закрытым или 
    // виртуальным
    protected virtual void OnNewMail(NewMailEventArgs e)
    {
        // Сохранить поле делегата во временном поле
        // для обеспечения безопаности потоков
        EventHandler<NewMailEventArgs> temp = NewMailEvent;

        // Есть вот такой потокобезопасный метод, но я пока с этим на разбирался, поэтому оставлю как есть
        // зная, что JIT компилятор не будет делать оптимизации и обходиться без локальной переменной temp
        //EventHandler<NewMailEventArgs> temp = Interlocked.CompareExchange(ref NewMailEvent, null, null);

        // Если есть объекты, зарегестрированные для получения
        // уведомления о событии, уведомляем их
        if (temp != null)
        {
            temp(this, e);
        }
    }

    // Этап 4. Определение метода, транслирующего входную информацию в желаемое событие
    public void SimulateNewMail(String from, String to, String subject)
    {
        // создать объекты для хранения информации, которую 
        // нужно передать получателям уведомления
        NewMailEventArgs e = new NewMailEventArgs(from, to, subject);

        // Вызвать виртуальный метод, уведомляющий объект о событии
        // Если ни один из производных типов не переопределяет этот метод, 
        // объект уведомит всех зарегистрированных получателей уведомления
        OnNewMail(e);
    }
}

// класс расширения для того, чтобы вынести логику уведомления подписчиков уведомления
public static class EventArgExtensions
{
    public static void Raise<TEventArgs>(this TEventArgs e, Object sender, ref EventHandler<TEventArgs> eventDelegate)
        where TEventArgs : EventArgs
    {
        EventHandler<TEventArgs> temp = Interlocked.CompareExchange(ref eventDelegate, null, null);
        if (temp != null)
        {
            temp(sender, e);
        }
    }
}

public class Fax
{
    // передаем конструктору объект MailManager
    public Fax(MailManager mm)
    {
        mm.NewMailEvent += FaxMsg;
    }

    // MailManager вызывает этот метод для уведомления
    // объекта Fax о прибытии нового почтового сообщения
    private void FaxMsg(Object sender, NewMailEventArgs e)
    {
        // sender можно использовать для взаимодействия с объектом MailManager
        // если нужно вернуть ему какую-то информацию 

        // e указывает доп информацию о событии, 
        // которую пожелает предоставить MailManager

        Console.WriteLine("Faxing mail message: ");
        Console.WriteLine($"From = {e.From}, To = {e.To}, Subject={e.Subject}");
    }

    public void Unregister(MailManager mm)
    {
        mm.NewMailEvent -= FaxMsg;
    }
}

// этот класс нужен для поддержания безопасности типа
// и кода при использовании EventSet
public sealed class EventKey : Object { }

public sealed class EventSet
{
    // закрытый словарь служит для отображения EventKey -> Delegate
    private readonly Dictionary<EventKey, Delegate> events = new();

    // ДОбавление отображения EventKey -> Delegate, если его не существует
    // компоновка делегата с существующим ключом EventKey
    public void Add(EventKey key, Delegate handler)
    {
        Monitor.Enter(events);
        Delegate d;
        events.TryGetValue(key, out d);
        events[key] = Delegate.Combine(d, handler);
        Monitor.Exit(events);
    }

    // удаление делегата из EventKey (если он сущесвует)
    // и ликвидация отображения EventKey -> Delegate,
    // когда удален последний делегат
    public void Remove(EventKey key, Delegate handler)
    {
        Monitor.Enter(events);

        // вызов TryGetValue, чтобы исключение не было вброшено
        // при попытке удалить делегат из неустановленного 
        // ключа EventKey
        Delegate d;
        if(events.TryGetValue(key, out d))
        {
            d = Delegate.Remove(d, handler);

            // Если делегат остается, то установить новый ключ EventKey
            // иначе - удалить EventKey
            if (d != null)
            {
                events[key] = d;
            }
            else
            {
                events.Remove(key);
            }
        }
        Monitor.Exit(events);
    }

    // поднятие события для обозначенного ключа EventKey
    public void Raise(EventKey key, Object sender, EventArgs e)
    {
        Delegate d;
        Monitor.Enter(events);
        events.TryGetValue(key, out d);
        Monitor.Exit(events);

        if (d != null)
        {
            // из-за того что каталог может содержать несколько разных типов 
            // делегатов, невозможно создать вызов защищенного типа делегата 
            // во время компиляции. Вызов метода DymamicInvoke типа System.Delegate
            // передав в метод обратного вызова параметры в виде массива объектов. 
            // DynamicInvoke будет контролировать безопасность типов параметров метода 
            // обратного вызова и вызов этого метода. Если будет найдено 
            // несоответствие типов, вбрасывается исключение
            d.DynamicInvoke(new Object[] { sender, e } );
        }
    }

}

public class FooEventArgs : EventArgs { }

public class TypeWithLotsOfEvents
{
    // определение закрытого экземплярного поля, ссылающегося на коллекцию 
    // Коллекция управляет множеством пар Event/Delegate
    private readonly EventSet eventSet = new();

    // защищенное свойство позволяет унаследовать типа доступа к коллекции
    protected EventSet EventSet { get { return eventSet; } }

    // определение членов необходимо для события Foo
    // Каждый объект имеет свой хэш-код для нахождения связанного списка делегата
    // события в коллекции объекта
    protected static readonly EventKey fooEventKey = new();

    // Определим для события метод-аксессор, которые добавляет 
    // делелат в коллекцию и удаляет его из коллекции
    public event EventHandler<FooEventArgs> Foo
    {
        add { eventSet.Add(fooEventKey, value); }
        remove { eventSet.Remove(fooEventKey, value); }
    }

    // определение защищенного виртуального метода On для этого события
    protected virtual void OnFoo(FooEventArgs e)
    {
        eventSet.Raise(fooEventKey, this, e);
    }

    // Определение метода, осуществляющий ввод этого события
    public void SimulateFoo()
    {
        OnFoo(new FooEventArgs());
    }
}