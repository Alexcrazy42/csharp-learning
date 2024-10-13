# Глава 12. Синхронизация

Разновидности синхронизации: передача данных и защита данных.

Синхронизация должна использоваться для защиты общих данных при 
выполнении всех трех условий из следующего списка:

1. Несколько частей кода выполняются одновременно

2. Эти части код обращаются (читают или записывают) одни и те же данные

3. По крайней мере одна часть кода обновляет (или записывает) данные

Если одна часть кода обновляет данные, вы можете воспользоваться синхронизацией, чтобы обноавления воспринимались
как атомарные для остальных компонентов системы.

Если вы знаете, что метод вызывается из GUI-контекста или контекста ASP.NET (или любого другого контекста, который
позволяет выполняться только одному фрагменту кода в любой момент времени), синхронизация будет излишней.

Следующий код обновляет общее значение с использованием простого параллелизма:

```
// ПЛОХОЙ КОД!!
async Task<int> SimpleParallelismAsync()
{
	int value = 0;
	Task task1 = Task.Run(() => { value = value + 1; });
	Task task2 = Task.Run(() => { value = value + 1; });
	Task task3 = Task.Run(() => { value = value + 1; });
	await Task.WhenAll(task1, task2, task3);
	return value;
}
```

Переходя к настоящему параллельному коду, рассмотрим пример, в котором используется тип Parallel:

```
void IndependentParallelism(IEnumerable<int> values)
{
	Parallel.ForEach(values, item => Trace.WriteLine(item));
}
```

Рассмотрим пример агрегирования, похожий на описанный в рецепте 4.2:

```
// ПЛОХОЙ КОД!!
int ParallelSum(IEnumerable<int> values)
{
	int result = 0;
		Parallel.ForEach(source: values,
		localInit: () => 0,
		body: (item, state, localValue) => localValue + item,
		localFinally: localValue => { result += localValue; });
	return result;
}
```

Хоть мы и говорили про неизменяемые коллекции, для следующего код, нужна синхронизация, тк каждый поток пытается
изменить ссылку:

```
// ПЛОХОЙ КОД!!
async Task<bool> PlayWithStackAsync()
{
	ImmutableStack<int> stack = ImmutableStack<int>.Empty;
	Task task1 = Task.Run(() => { stack = stack.Push(3); });
	Task task2 = Task.Run(() => { stack = stack.Push(5); });
	Task task3 = Task.Run(() => { stack = stack.Push(7); });
	await Task.WhenAll(task1, task2, task3);
	return stack.IsEmpty;
}
```

С потокобезопасными коллекциями все проще, они могут изменяться, но вся логика синхронизации уже встроена в них:

```
async Task<int> ThreadsafeCollectionsAsync()
{
	var dictionary = new ConcurrentDictionary<int, int>();
	Task task1 = Task.Run(() => { dictionary.TryAdd(2, 3); });
	Task task2 = Task.Run(() => { dictionary.TryAdd(3, 5); });
	Task task3 = Task.Run(() => { dictionary.TryAdd(5, 7); });
	await Task.WhenAll(task1, task2, task3);
	return dictionary.Count; // Всегда возвращает 3.
}
```

# 12.1. Блокировки и команда lock

## Задача

Имеются общие данные. Требуется обеспечить безопасное чтения и запись этих данных из нескольких потоков.

## Решение

lock, если поток входит в блок lock, то все остальные потоки не смогут войти в этот блок, пока блокировка не
будет снята:

```
class MyClass
{
	// Блокировка защищает поле _value.
	private readonly object _mutex = new object();
	private int _value;
	public void Increment()
	{
		lock (_mutex)
		{
			_value = _value + 1;
		}
	}
}
```

## Пояснение

Существует несколько механизмов блокировки: Monitor, Spin, Lock и ReaderWriterLockSlim. В большинстве приложений
эти типы блокировок практически никогда не должны использоваться напрямую.

В частности, для разработчиков проще переключиться на ReaderWriterLockSlim, когда акая сложность не является
необходимой.

При использовании блокировок следуюет руководствоваться четырьмя важными рекомендациями:

1. Ограничье видимость блокировки

2. Документируйте, что именно защищает блокировка

3. Сократите до минимума объем кода, защищенного блокировкой

4. Никогда не выполняйте произвольный код при удержании блокировки

Никогда не делайте lock(this) или lock с любым экземпляром Type или string, это может привести к взаимоблокировкам, 
доступным из другого кода.

# 12.2. Блокировки async

## Задача

Имеются общие данные. Требуется обеспечить безопасное чтение и запись из разных программных блоков, которые могут
использовать await.

## Решение

Тип SemaphoreSlim был обновлен в .NET 4.5 для обеспечения совместимости с async. Пример использования:

```
class MyClass
{
	// Блокировка защищает поле _value.
	private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1);
	private int _value;
	public async Task DelayAndIncrementAsync()
	{
		await _mutex.WaitAsync();
		try
		{
			int oldValue = _value;
			await Task.Delay(TimeSpan.FromSeconds(oldValue));
			_value = oldValue + 1;
		}
		finally
		{
			_mutex.Release();
		}
	}
}
```

Также можно воспользоваться типо AsyncLock из библиотеки Nito.AsyncEx, который обладает чуть более элегантным API:

class MyClass
{
	// Блокировка защищает поле _value.
	private readonly AsyncLock _mutex = new AsyncLock();
	private int _value;
	public async Task DelayAndIncrementAsync()
	{
		using (await _mutex.LockAsync())
		{
			int oldValue = _value;
			await Task.Delay(TimeSpan.FromSeconds(oldValue));
			_value = oldValue + 1;
		}
	}
}

## Пояснение

В этой ситуации действуют рекомендации из рецепта 12.1.

Экземпляры блокировок должны быть приватными; они не должны быть 
доступными за пределами класса. Обязательно четко документируйте 
(и тщательно продумывайте), что именно защищает экземпляр блокировки. 
Сведите к минимуму объем кода, выполняемого при удержании 
блокировки. В частности, не вызывайте произвольный код, включая выдачу 
событий, вызов виртуальных методов и вызов делегатов.

# 12.3. Блокирующие сигналы

## Задача

Требуется отправить уведомление от одного потоку другому.

## Решение

Самый распространнный межпотоковый сигнал - событие с ручным сбросом ManualResetEventSlim.
Событие с ручным сбросом может находиться в одном из двух состояний: установленном или сброшенном. 
Любой поток может перевести поток в установленное состояние или провести его сброс. Поток также может 
ожидать перехода события в установленное состояние.

В примере есть два метода, которые вызываются разными потоками; один поток ожидает сигнала от другого

## Пояснение

ManualResetEventSlim - отличный универсальный сигнал, передаваемый одним потоком другому, однако использовать
его следует только тогда, когда это уместно.

Если сигнал представляет собой сообщение, отправляющее некоторые данные между потоками, рассмотрите возможность
использования очереди "производитель/потребитель". С другой стороны, если сигналы используются только для 
координации доступа к общим данным, лучше использовать lock.

ManualResetEventSlim является синхронным сигналом, поэтому WaitForInitialization 
блокирует вызывающий поток до отправки сигнала. 
Если вы хотите ожидать сигнала без приостановки потока, используйте 
асинхронный сигнал так, как описано в рецепте 12.4.

# 12.4. Асинхронные сигналы

## Задача

Требуется отправить уведомление от одного потока другому, при этом получатель оповещения должен ожидать его
асинхронно.

## Решение

TaskCompletionSource<T> для того, чтобы отправить уведомление асинхронно, если уведомление должно быть отправлено
только один раз.

Библиотека Nito.AsyncEx содержит тип AsyncManualResetEvent — приблизительный аналог ManualResetEvent 
для асинхронного кода. Следующий пример является искусственным, но показывает, как правильно 
использовать тип AsyncManualResetEvent:

```
class MyClass
{
	private readonly AsyncManualResetEvent _connected = new AsyncManualResetEvent();
	
	public async Task WaitForConnectedAsync()
	{
		await _connected.WaitAsync();
	}

	public void ConnectedChanged(bool connected)
	{
		if (connected)
			_connected.Set();
		else
			_connected.Reset();
	}
}
```


## Пояснение

Сигналы представляют собой механизм уведомлений общего назначения. 
Но если этот «сигнал» представляет собой сообщение, используемое для 
отправки данных от одной части кода в другую, рассмотрите возможность использования 
очереди «производитель/потребитель». Не стоит 
использовать и сигналы общего назначения для простой координации 
доступа к общим данным; в таких ситуациях следует применять асинхронную блокировку

# 12.5. Регулировка

## Задача

Имеется код с высокой степенью конкуренции - даже слишком высокой. Требуется найти способ скорректировать 
конкуретность.

Конкурентность оказывается чрезмерной, если части приложения не успевают друг за другом, элементы данных накапливаются
и занимают память. В этом сценарии регулировка частей кода может предотвратить проблемы с памятью.

## Решение

Все представленные решения ограничивают конкурентность конкретным 
значением. В Reactive Extensions предусмотрены более разнообразные 
возможности — например, скользящие временные окна; регулировка по 
наблюдаемым объектам System.Reactive более подробно рассматривается 
в рецепте 6.4.

В Dataflow и в параллельном коде существуют встроенные параметры 
для регулировки степени конкурентности:

```
IPropagatorBlock<int, int> DataflowMultiplyBy2()
{
	var options = new ExecutionDataflowBlockOptions
	{
		MaxDegreeOfParallelism = 10
	};

	return new TransformBlock<int, int>(data => data * 2, options);
}

// Использование Parallel LINQ (PLINQ)
IEnumerable<int> ParallelMultiplyBy2(IEnumerable<int> values)
{
	return values.AsParallel()
		.WithDegreeOfParallelism(10)
		.Select(item => item * 2);
}

// Использование класса Parallel
void ParallelRotateMatrices(IEnumerable<Matrix> matrices, float degrees)
{
	var options = new ParallelOptions
	{
		MaxDegreeOfParallelism = 10
	};

	Parallel.ForEach(matrices, options, matrix => matrix.Rotate(degrees));
}
```

Конкурентный асинхронный код может регулироваться с помощью 
SemaphoreSlim:

```
async Task<string[]> DownloadUrlsAsync(HttpClient client,
 IEnumerable<string> urls)
{
	using var semaphore = new SemaphoreSlim(10);
	
	Task<string>[] tasks = urls.Select(async url =>
	{
		await semaphore.WaitAsync();
		try
		{
			return await client.GetStringAsync(url);
		}
		finally
		{
			semaphore.Release();
		}
	}).ToArray();
	return await Task.WhenAll(tasks);
}
```


## Пояснение

Регулировка может быть необходимой тогда, когда вы обнаруживаете, 
что код задействует слишком много ресурсов (например, процессорного 
времени или сетевых подключений). Учтите, что конечные пользователи 
обычно работают на машинах, менее мощных, чем у разработчиков, поэтому 
чрезмерная регулировка обычно лучше недостаточной.


lock, ManualResetEventSlim, AutoResetEvent, CountdownEvent, Barrier, Nito.AsyncEx.AsyncLock, TaskCompletionSource,

Nito.Asyncex.AsyncManualResetEvent
