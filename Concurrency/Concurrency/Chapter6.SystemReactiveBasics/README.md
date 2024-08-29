����������� �� 110 ��������

# ����� 6. ������ System.Reactive

��� ����� ���������� ���������� LINQ - to Objects (�� ���� IEnumerable<T>) � to Entities (�� ���� 
IQueryable<T>).

������� �������������� � **���������� ������ (lazily)**, � ������������������ ���������� �������� �� ����
�������������. �� �������������� ������ ������������ ������ � *������������*; ��� ��������� ��������-��������
����������� �� ������� �� ������.

System.Reactive (Rx) �������������� ������� ��� ������������������ ������, ����������� � �������� �������.
�������������� Rx ����� ������������� ��� Linq to Events (�� ���� IObservable<T>). 

������� ������������� ������������ ��������� � ������� ������������ LINQ ����������� � ���, ��� Rx
���������� ������ �������������, �.�. ������ ����������, ��� ��������� ��������� �� ����������� �������. 
Rx �������� �� ���� LINQ � ��������� ����� ������ ��������� ��� ������ ����������.

��� ��������� LINQ ���� �������� � Rx. � ����� ��������������� ����� �����������, ������� Rx ��������� �
LINQ, �������� ��������������� ��� ������ �� *��������*.

����� ������������ System.Reactive, ���� ���������� NuGet-����� ��� System.Reactive.


# 6.1. �������������� ������� .NET

## ������

������� �������, ������� ������ ������������������ ��� ������� 
����� System.Reactive, ������������ ������ ����� OnNext ��� ������ 
������������� �������.

## �������

FromEventPattern ����� ����� �������� � ����� �������� ������� EventHandler<T>. ��� ��� 
�������� ������� ������������ �� ������ ����� ����� ����������. ��������, Progress<T> ����������
������� ProgressChanged � ����� EventHandler<T>, ��� ��������� ����� ��������� ��� � 
FromEventPattern:

```
var progress = new Progress<int>();

IObservable<EventPattern<int>> progressReports =
	Observable.FromEventPattern<int>(
		handler => progress.ProgressChanged += handler,
		handler => progress.ProgressChanged -= handler);
progressReports.Subscribe(data => Trace.WriteLine("OnNext: " + data.EventArgs));
```

������, ��� data.EventArgs ������ ��������� � ����� int. ��������-��� FromEventPattern 
(int � ����������� �������) ��������� � ����� T
� EventHandler<T>. ��� ������-��������� FromEventPattern ��������� 
System.Reactive ������������� � �������� �������� �� �������

����� ����� ���������� ����������������� ���������� ���������� 
EventHandler<T>, ��� ��������� ����� ������������ �� �� FromEventPattern, 
�� ����� ������ ���� ����� ���������� ���������� ��� �������� ��� 
������� �������. ��� ����� ����� �������������� � FromEventPattern, �� 
��� ��������� ��������� ������� ������. ��������, ��� System.Timers.
Timer ���������� ������� Elapsed, ����������� � ���� ElapsedEventHandler. 
�������� ������ ������� ����� ��������� � FromEventPattern:

```
var timer = new System.Timers.Timer(interval: 1000) { Enabled = true };
IObservable<EventPattern<ElapsedEventArgs>> ticks =
	Observable.FromEventPattern<ElapsedEventHandler, ElapsedEventArgs>(
		handler => (s, a) => handler(s, a),
		handler => timer.Elapsed += handler,
		handler => timer.Elapsed -= handler);

ticks.Subscribe(data => Trace.WriteLine("OnNext: " + data.EventArgs.SignalTime));
```

��������� ����������� ���������� ���������. ���� �������� ������ 
�������, ������������ *��������� (reflection)*:

```
var timer = new System.Timers.Timer(interval: 1000) { Enabled = true };
IObservable<EventPattern<object>> ticks =
	Observable.FromEventPattern(timer, nameof(Timer.Elapsed));

ticks.Subscribe(data => Trace.WriteLine("OnNext: " + ((ElapsedEventArgs)data.EventArgs).SignalTime));
```

��� ����� ������� ����� FromEventPattern �������� ������� �����. 
��� ���� � ���� ���� ���� ����������: ����������� �� �������� ������ 
� ������� ����������. ��� ��� data.EventArgs ��������� � ���� object, 
��� �������� ������������� ��� � ElapsedEventArgs ��������������.

## ���������

# 6.2. �������� ����������� ���������

## ������

## �������

## ���������

# 6.3. ����������� ������ ������� � �������������� Window � Buffer

## ������

## �������

## ���������

# 6.4. �������� ������� ������� ����������� ����������� � �������

## ������

## �������

## ���������

# 6.5. ����-����

## ������

## �������

## ���������