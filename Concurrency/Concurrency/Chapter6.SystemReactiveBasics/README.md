Остановился на 110 странице

# Глава 6. Основы System.Reactive

Два самый популярных провайдера LINQ - to Objects (на базе IEnumerable<T>) и to Entities (на базе 
IQueryable<T>).

Запросы обрабатываются в **отложенном режиме (lazily)**, а последовательности генерируют значения по мере
необходимости. На концептуальном уровне используется модель с *вытягиванием*; при обработке элементы-значения
извлекаются из очереди по одному.

System.Reactive (Rx) интерпретирует события как последовательности данных, поступающих с течением времени.
Соответственно Rx можно рассматривать как Linq to Events (на базе IObservable<T>). 

Главное различиемежду наблюдаемыми объектами и другими провайдерами LINQ заключается в том, что Rx
использует модель проталкивания, т.е. запрос определяет, как программа реагирует на поступление событий. 
Rx строится на базе LINQ и добавляет новые мощные операторы как методы расширения.

Все операторы LINQ тоже доступны в Rx. В главе рассматриваются новые возможности, которые Rx добавляет к
LINQ, особенно предназначенным для работы со *временем*.

Чтобы использовать System.Reactive, надо установить NuGet-пакет для System.Reactive.


# 6.1. Преобразование событий .NET

## Задача

Имеется событие, которое должно интерпретироваться как входной 
поток System.Reactive, генерирующий данные через OnNext при каждом 
инициировании события.

## Решение

FromEventPattern лучше всего работате с типом делегата события EventHandler<T>. Это тип 
делегата события используется во многих более новых фрейморках. Например, Progress<T> определяет
событие ProgressChanged с типом EventHandler<T>, что позволяет легко упаковать его в 
FromEventPattern:

```
var progress = new Progress<int>();

IObservable<EventPattern<int>> progressReports =
	Observable.FromEventPattern<int>(
		handler => progress.ProgressChanged += handler,
		handler => progress.ProgressChanged -= handler);
progressReports.Subscribe(data => Trace.WriteLine("OnNext: " + data.EventArgs));
```

Отмечу, что data.EventArgs сильно типизован с типом int. Аргумент-тип FromEventPattern 
(int в приведенном примере) совпадает с типом T
в EventHandler<T>. Два лямбда-аргумента FromEventPattern позволяют 
System.Reactive подписываться и отменять подписку на событие

Более новые фреймворки пользовательского интерфейса используют 
EventHandler<T>, что позволяет легко использовать их из FromEventPattern, 
но более старые типы часто определяют уникальный тип делегата для 
каждого события. Они также могут использоваться с FromEventPattern, но 
это потребует несколько большей работы. Например, тип System.Timers.
Timer определяет событие Elapsed, относящееся к типу ElapsedEventHandler. 
Подобные старые события можно упаковать в FromEventPattern:

```
var timer = new System.Timers.Timer(interval: 1000) { Enabled = true };
IObservable<EventPattern<ElapsedEventArgs>> ticks =
	Observable.FromEventPattern<ElapsedEventHandler, ElapsedEventArgs>(
		handler => (s, a) => handler(s, a),
		handler => timer.Elapsed += handler,
		handler => timer.Elapsed -= handler);

ticks.Subscribe(data => Trace.WriteLine("OnNext: " + data.EventArgs.SignalTime));
```

Синтаксис определенно становится неудобным. Ниже приведен другой 
вариант, использующий *отражение (reflection)*:

```
var timer = new System.Timers.Timer(interval: 1000) { Enabled = true };
IObservable<EventPattern<object>> ticks =
	Observable.FromEventPattern(timer, nameof(Timer.Elapsed));

ticks.Subscribe(data => Trace.WriteLine("OnNext: " + ((ElapsedEventArgs)data.EventArgs).SignalTime));
```

При таком подходе вызов FromEventPattern выглядит намного проще. 
При этом у него есть один недостаток: потребитель не получает данные 
с сильной типизацией. Так как data.EventArgs относится к типу object, 
вам придется преобразовать его в ElapsedEventArgs самостоятельно.

## Пояснение

# 6.2. Отправка уведомлений контексту

## Задача

## Решение

## Пояснение

# 6.3. Группировка данных событий с использованием Window и Buffer

## Задача

## Решение

## Пояснение

# 6.4. Контроль потоков событий посредством регулировки и выборки

## Задача

## Решение

## Пояснение

# 6.5. Тайм-ауты

## Задача

## Решение

## Пояснение