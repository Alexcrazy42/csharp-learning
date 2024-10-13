# ����� 12. �������������

������������� �������������: �������� ������ � ������ ������.

������������� ������ �������������� ��� ������ ����� ������ ��� 
���������� ���� ���� ������� �� ���������� ������:

1. ��������� ������ ���� ����������� ������������

2. ��� ����� ��� ���������� (������ ��� ����������) ���� � �� �� ������

3. �� ������� ���� ���� ����� ���� ��������� (��� ����������) ������

���� ���� ����� ���� ��������� ������, �� ������ ��������������� ��������������, ����� ����������� ��������������
��� ��������� ��� ��������� ����������� �������.

���� �� ������, ��� ����� ���������� �� GUI-��������� ��� ��������� ASP.NET (��� ������ ������� ���������, �������
��������� ����������� ������ ������ ��������� ���� � ����� ������ �������), ������������� ����� ��������.

��������� ��� ��������� ����� �������� � �������������� �������� ������������:

```
// ������ ���!!
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

�������� � ���������� ������������� ����, ���������� ������, � ������� ������������ ��� Parallel:

```
void IndependentParallelism(IEnumerable<int> values)
{
	Parallel.ForEach(values, item => Trace.WriteLine(item));
}
```

���������� ������ �������������, ������� �� ��������� � ������� 4.2:

```
// ������ ���!!
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

���� �� � �������� ��� ������������ ���������, ��� ���������� ���, ����� �������������, �� ������ ����� ��������
�������� ������:

```
// ������ ���!!
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

� ����������������� ����������� ��� �����, ��� ����� ����������, �� ��� ������ ������������� ��� �������� � ���:

```
async Task<int> ThreadsafeCollectionsAsync()
{
	var dictionary = new ConcurrentDictionary<int, int>();
	Task task1 = Task.Run(() => { dictionary.TryAdd(2, 3); });
	Task task2 = Task.Run(() => { dictionary.TryAdd(3, 5); });
	Task task3 = Task.Run(() => { dictionary.TryAdd(5, 7); });
	await Task.WhenAll(task1, task2, task3);
	return dictionary.Count; // ������ ���������� 3.
}
```

# 12.1. ���������� � ������� lock

## ������

������� ����� ������. ��������� ���������� ���������� ������ � ������ ���� ������ �� ���������� �������.

## �������

lock, ���� ����� ������ � ���� lock, �� ��� ��������� ������ �� ������ ����� � ���� ����, ���� ���������� ��
����� �����:

```
class MyClass
{
	// ���������� �������� ���� _value.
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

## ���������

���������� ��������� ���������� ����������: Monitor, Spin, Lock � ReaderWriterLockSlim. � ����������� ����������
��� ���� ���������� ����������� ������� �� ������ �������������� ��������.

� ���������, ��� ������������� ����� ������������� �� ReaderWriterLockSlim, ����� ���� ��������� �� ��������
�����������.

��� ������������� ���������� �������� ����������������� �������� ������� ��������������:

1. ��������� ��������� ����������

2. ��������������, ��� ������ �������� ����������

3. ��������� �� �������� ����� ����, ����������� �����������

4. ������� �� ���������� ������������ ��� ��� ��������� ����������

������� �� ������� lock(this) ��� lock � ����� ����������� Type ��� string, ��� ����� �������� � �����������������, 
��������� �� ������� ����.

# 12.2. ���������� async

## ������

������� ����� ������. ��������� ���������� ���������� ������ � ������ �� ������ ����������� ������, ������� �����
������������ await.

## �������

��� SemaphoreSlim ��� �������� � .NET 4.5 ��� ����������� ������������� � async. ������ �������������:

```
class MyClass
{
	// ���������� �������� ���� _value.
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

����� ����� ��������������� ���� AsyncLock �� ���������� Nito.AsyncEx, ������� �������� ���� ����� ���������� API:

class MyClass
{
	// ���������� �������� ���� _value.
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

## ���������

� ���� �������� ��������� ������������ �� ������� 12.1.

���������� ���������� ������ ���� ����������; ��� �� ������ ���� 
���������� �� ��������� ������. ����������� ����� �������������� 
(� ��������� ������������), ��� ������ �������� ��������� ����������. 
������� � �������� ����� ����, ������������ ��� ��������� 
����������. � ���������, �� ��������� ������������ ���, ������� ������ 
�������, ����� ����������� ������� � ����� ���������.

# 12.3. ����������� �������

## ������

��������� ��������� ����������� �� ������ ������ �������.

## �������

����� ��������������� ������������ ������ - ������� � ������ ������� ManualResetEventSlim.
������� � ������ ������� ����� ���������� � ����� �� ���� ���������: ������������� ��� ����������. 
����� ����� ����� ��������� ����� � ������������� ��������� ��� �������� ��� �����. ����� ����� ����� 
������� �������� ������� � ������������� ���������.

� ������� ���� ��� ������, ������� ���������� ������� ��������; ���� ����� ������� ������� �� �������

## ���������

ManualResetEventSlim - �������� ������������� ������, ������������ ����� ������� �������, ������ ������������
��� ������� ������ �����, ����� ��� �������.

���� ������ ������������ ����� ���������, ������������ ��������� ������ ����� ��������, ����������� �����������
������������� ������� "�������������/�����������". � ������ �������, ���� ������� ������������ ������ ��� 
����������� ������� � ����� ������, ����� ������������ lock.

ManualResetEventSlim �������� ���������� ��������, ������� WaitForInitialization 
��������� ���������� ����� �� �������� �������. 
���� �� ������ ������� ������� ��� ������������ ������, ����������� 
����������� ������ ���, ��� ������� � ������� 12.4.

# 12.4. ����������� �������

## ������

��������� ��������� ����������� �� ������ ������ �������, ��� ���� ���������� ���������� ������ ������� ���
����������.

## �������

TaskCompletionSource<T> ��� ����, ����� ��������� ����������� ����������, ���� ����������� ������ ���� ����������
������ ���� ���.

���������� Nito.AsyncEx �������� ��� AsyncManualResetEvent � ��������������� ������ ManualResetEvent 
��� ������������ ����. ��������� ������ �������� �������������, �� ����������, ��� ��������� 
������������ ��� AsyncManualResetEvent:

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


## ���������

������� ������������ ����� �������� ����������� ������ ����������. 
�� ���� ���� ������� ������������ ����� ���������, ������������ ��� 
�������� ������ �� ����� ����� ���� � ������, ����������� ����������� ������������� 
������� ��������������/������������. �� ����� 
������������ � ������� ������ ���������� ��� ������� ����������� 
������� � ����� ������; � ����� ��������� ������� ��������� ����������� ����������

# 12.5. �����������

## ������

������� ��� � ������� �������� ����������� - ���� ������� �������. ��������� ����� ������ ��������������� 
�������������.

�������������� ����������� ����������, ���� ����� ���������� �� �������� ���� �� ������, �������� ������ �������������
� �������� ������. � ���� �������� ����������� ������ ���� ����� ������������� �������� � �������.

## �������

��� �������������� ������� ������������ �������������� ���������� 
���������. � Reactive Extensions ������������� ����� ������������� 
����������� � ��������, ���������� ��������� ����; ����������� �� 
����������� �������� System.Reactive ����� �������� ��������������� 
� ������� 6.4.

� Dataflow � � ������������ ���� ���������� ���������� ��������� 
��� ����������� ������� ��������������:

```
IPropagatorBlock<int, int> DataflowMultiplyBy2()
{
	var options = new ExecutionDataflowBlockOptions
	{
		MaxDegreeOfParallelism = 10
	};

	return new TransformBlock<int, int>(data => data * 2, options);
}

// ������������� Parallel LINQ (PLINQ)
IEnumerable<int> ParallelMultiplyBy2(IEnumerable<int> values)
{
	return values.AsParallel()
		.WithDegreeOfParallelism(10)
		.Select(item => item * 2);
}

// ������������� ������ Parallel
void ParallelRotateMatrices(IEnumerable<Matrix> matrices, float degrees)
{
	var options = new ParallelOptions
	{
		MaxDegreeOfParallelism = 10
	};

	Parallel.ForEach(matrices, options, matrix => matrix.Rotate(degrees));
}
```

������������ ����������� ��� ����� �������������� � ������� 
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


## ���������

����������� ����� ���� ����������� �����, ����� �� �������������, 
��� ��� ����������� ������� ����� �������� (��������, ������������� 
������� ��� ������� �����������). ������, ��� �������� ������������ 
������ �������� �� �������, ����� ������, ��� � �������������, ������� 
���������� ����������� ������ ����� �������������.


lock, ManualResetEventSlim, AutoResetEvent, CountdownEvent, Barrier, Nito.AsyncEx.AsyncLock, TaskCompletionSource,

Nito.Asyncex.AsyncManualResetEvent
