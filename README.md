# csharp-learning


������� �� CLR: 
1) ����� � ������, ���� ����� ��������� ��������� ���� ������ (manager, provider)
2) ��� ����� ����� ������, ���, ��, xunit
3) builder ��� � ef ��� ������� ������
4) StringComparer
5) ��������� ������� �������� �� ������������ ��������� EmailAddressAttribute ASP.NET Core https://github.com/dotnet/runtime/issues/27592
6) ���������� � ������� �������, ������������, ������������ ����� C# (http, db api)
7) int[] ��� key � dict
8) ������ ���� �� ������ �� �����, � �� �� ����� � ������
1) ��� ����� ������������ ���������� �����
2) �������� ����� ������� �����������
3) ��� ������� � runtime �� heap � ������ ����������, ������ ������ ������
4) ������������� ����� ������������ ������ 
5) call - ������������ ��� ����������� ����� 
callvirt - ������������� ��� �����, ����������� ��� �����
6) ��� sealed ����� �������������� ����� callvirt, ���� �� ��������� ������ �� ��������� ����������� �����, � callvirt ����� ����������, ������ ��� ���� �������� � �������, � �� � ��������
7) ������� ����� C#: ������ .net � C#, .net framework ����� �������� net core
8) ����� ����� ������������ �����
9) BeforeFieldInit
10) ������ ���������� �� ������������ ����� �� �������� ������� �������������� ������, ����� � ���� ���� ������ ����� �������������� (�� � ����� ��������� �� ��������) ���� ���������
11) ������ ���� �� ������������ ������� ����������, ������� ����������, ���������� ����������
12) �������� ������ ��������� �������
13) ++n, n++
14) C# ����������
15) DefaultParameterValue
16) typeof
17) ����� ������� ����� ������������ ref � out
18) for(; fs != null; ContinueProcessingFiles(ref fs)) - ��� ���
1) ����� ����� ����������� �����������
2) ������� ������������� �����, � ����� ������
3) ������� ����� ����������, ����� ����������
4) ����� CLR ������, ��� ����� ��������� ������
5) ��������� ������� �� ������ 10 (IList<string> � List<Object> ��������)
6) �����������
7) ��� ������ ��� �������� ���� ������ ���������� ��������������, � ��� ���� ����� �� ������������ ������
1) ������������� ������
2) ��� � ����� ����� � ������� ����� �� ����������� ����� ��������� ������ ����������� �����
�� ����: ���������� IEnumerator � �� ������ ������������ foreach ��� �������
3) ������ EIMI ������ ������ �������� ��� public
4) ������ ������ ����� ����� ������ �� ��, ��� ���� ���������������� ��� �� ���� ������������������ �� ��� ������, 
����� ����� ���������� ��������, ����� ��� �� ����� ����� �����������������
5) ����������� ���������� ����������
1) ����� ������ ������� ������������� (��������� ������������ ����� �������� �������� ������������� ������� ��� ������ �� ��������?)
2) Ordinal, cultural comparison
3) ����������� � ��� ��� ������ �������� � ���� ����� � 9 ������
4) �������������� �����
5) ��� ����� ������������� ������� ������� � C# ��������: NetworkStream ��������

---------------------------------------------------
��� ����� ����� ����� ����������:
1) IEnumerator
2) ��������: Invoke, Invoker
1) System.IO.Stream, FileStream, MemoryStream, System.Net.Sockets.Network.Stream
2) Windows Forms: Button, CheckBox, ListBox ����������� �� System.Windows.Forms.Control
3) System.Collections.Generic: IEnumerable<T>, ICollection, IList, IDictionary
1) System.Enviroment
2) ����� �������� �������������
3) StringBuilder ����������, ��� ���������� ����� ������ ToString, �������� ��� ���������� � ��� ��� ��� 
������ ����� ��� SB
4) System.Globalization.NumberStyles
5) IO �������� � ��������� UTF: BinaryWrite/Reader, StreamWriter/Reader, Encoding, UTF8/32/7Encoding
6) System.Security.Cryptography.CspParameters;
System.Security.Cryptography.X509Certificates.X509Certificate � System.Security.Cryptography.X509Certificates.X509Certificate2
System.Diagnostics.Process � System.Diagnostics.ProcessStartinfo
System.Diagnostics.Eventing.Reader.EventlogSession
SecurePassword
7) Marshal, ���������, unsafe ���
8) delegates: action, func
9) typeof, gettype ...
10) RuntimeHelpers
11) ������� ��� ��� ����������


-----------------------------------------
�������� � C#:
1) 322 ��������: ���������� ������������� �������������� ��������� ��� ������������ �������� ����� ������.
��������� �� ����� ��������� ���������� �� ����� ��� ���������� �����: � .NET 6 �������� ��������� INumber

---------------------------------------------------------
��������� ��� � C#, ����� ������� �����:
1) $""

---------------------------------------
���������� ���������� ���� �� �����:
1) ���� ���� ������ String ���������� ������� � ���������� �������
GC, CLR ����� �� ����� ������������� ����� �������� ���� �������� ������ (�������� ���� ���� ���� �� �������� String ���������� ������), �������� �������
������� � ������, ��� ��� ����� ���� �����������������. 
2) ��� �������� ����� ������� ���
	��� �������� ���� ������������� ������, ������� �������� ������ ��������. ������� ��c��� ������ �� ����� �� ���� ������������� ������

������������ ��������:
������ �����: 
378 �������� - StringBuilder.AppendFormat
388 �������� - Decoder

�����:
424 - ���������� ����������� IEnumberable, ICollection � IList
433 - ������������ ������ � �������� � ������� �������������� �������
451 - 463 - ��������
475 - 483 - ��������
515 - 523 - ����� ����� ����������� ��� ���������� � ������������� �����
538 - 540 - ������� ����������, �� ����� � ���� ����� ������ �������������� ����� � ���������� � �������
543 - ������� ������������� ��������
546 - 553 - ��������� �����
564 - ���������, ����� ���������� ������ ������
574 - PerfView, PerfMon, SOS - �������, ����� �������� �� �������������� GC
582-583 - SafeHandle ��� ������ � ������������� �����
594 - 597 - ����������� ���������� �����������


----------------------------------------------------------
��� �������� ������ �����������:
1) ��� ����� �������:
	1) �������� ����� ------- � �������� �������, � ������ �������� ������ �� ��� �����
	2) ������ ��� �������, ������, ���������� ����� � ������� ��� ��� ������ �����������
	3) ������� �����, ���� �������� ��� ������� ����� � �� ��� �������� ������� � ����������

2) �������� ��� �������� � ����:
	1) Best Practices ��� ����������� ����������: �����, ���������, ������������, ������������������, �����
	2) Concurrency
	3) IO-��������
	4) ������ � �������� ������������, ����������
	5) ������� �������� �����
	6) ������� � ������������� ���������, �������� ���������. ��� �������� CLR 
	�������� ������ � ������, ������� � ��