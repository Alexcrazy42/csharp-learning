﻿# Посмотреть позже:

1. Глава 6 про System.Reactive

2. Глава 7: рецепты начиная с 4

3. Глава 8: рецепты начиная с 5, + подразобраться с репептами 1-3

4. Глава 10: 10.6, 10.7

# Структура книги

1. В главе 1 содержится введение в различные виды конкурентности, 
описанные в книге: параллелизм, асинхронное и реактивное программирование, потоки данных.

2. В главах 2–6 представлено более подробное введение в разновидности 
конкурентности.

3. В каждой из оставшихся глав рассматривается конкретный аспект конкурентности; они также могут 
рассматриваться как сборник решений типичных проблем

# Главные моменты

1. Каждый поток имеет собственный независимый 
стек, но он совместно использует память со всеми остальными потоками 
процесса


2. Пул потоков содержит набор рабочих потоков, готовых к выполнению любой работы, которая им 
будет назначена. Пул потоков отвечает за определение количества потоков, 
находящихся в пуле потоков в любой момент времени.

3. Поток - низкоуровневая концепция. Работа с пулом потоков напрямую - чуть выше.
Параллельная обрабботка и потоки данных - еще выше. На нем мы и находимся.

4. Терминальный оператор и асинхронный делегат

5. Неизменяемые коллекции: (9 глава)

    Экземпляр никогда не изменяется
    Так как экземпляр никогда не изменяется, он потокобезопасен по своей природе
    При вызове изменяющего метода для неизменяемой коллекции возвращается новая измененная версия
    Неизменяемые коллекции являются потокобезопасными, но ссылки на них потокобезопасными не являются


6. (171 страница)
Одно важное примечание по поводу отсортированных множеств: индексирование 
для них выполняется за время O(log N), а не O(1), как у 
ImmutableList<T> (см. рецепт 9.2). Это означает, что в данной ситуации 
действует та же рекомендация: используйте foreach вместо for там, где 
это возможно, с ImmutableSortedSet<T>.

# Вопросы

1. В чем прикол потокобезопасных коллекций, если автор пишет, что надо чтобы операции были
достаточно редки

# Рассмотреть позже

1. Рецепт 5.4. BufferBlock, 9.9

# Concurrency

1. Основы:
    Как происходит исполнение кода
        Регистры
        Стэк
    Единицы исполнения кода
        Процесс
        Поток
        Корутина
    Многозадачность
        Коорперативная
        Вытесняющая

2. ОС
    Прерывания
    Context Switching
    Виртуальная память
    Адресное пространство процесса
    Сегменты процесса
        stack
        heap
        text
        data/bss

3. Основы с практикой
    Конкуретное исполнение
    Паралелльное исполнение

## УПРАЖНЕНИЕ

Написать приложение, которое будет конкурентно/параллельно печатать что-то на экран, либо обрабатывать клиентские соединения (без разделяемых данных)

4. Базовые примитивы синхронизации
    Мьютексы
    Семафоры
    Условные переменные

## УПРАЖНЕНИЕ

Написать приложение, которое будет параллельно работать с какой-нибудь структурой данных

Написать приложение, которое будет ограничивать количество одновременных соединений для сервера

Написать приложение под кодовых названием consumer-producer, его еще называют reader-writer (вся информация в инете)

5. Базовая архитектура компьютера
    Иерархия памяти
    Разрядность процессора
    Разрядность шины

6. Атомики
    Load
    Store
    CompareAndSwap
    Атомики с указателями

## УПРАЖНЕНИЕ

Написать приложение, которое будет считать RPS-сервера

Написать spin-lock

7. Проблемы параллельного программирования
    Data race
    Dead lock
    Live lock
    Starvation

# УПРАЖНЕНИЕ

Решить задачу об обедающих философах

Решить задачу о спящем парикмахере

Решить задачу о курильщиках

8. Продвинутые примитивы синхронизации
    Каналы
    Мьютексы
        read-write
        recursive
        timed
        hierarchical
    Futures
    Promises

# УПРАЖНЕНИЕ

Написать read-write, recursive, timed, hierarchical mutex

9. Паттерны
    Worker pool
    Scheduler
    Batcher

# УПРАЖНЕНИЕ

Написать poll воркеров, которые будут параллельно выполнять какие-то задачи

Написать планировщик задач, например, как setTimeout и setInterval из JS

Написать batcher

10. Ввод-вывод
    Синхронный ввод-вывод
    Мультиплексированный ввод-вывод
    Асихронный ввод-вывод

# УПРАЖНЕНИЕ

Написать TCP-сервер на epoll-ах

# ТУТ ЗАКРЫТО УЖЕ 90% ЗАДАЧ ПАРАЛЛЕЛЬНОГО ПРОГРАММИРОВАНИЯ, НО ОСТАЮТСЯ НЮАНСЫ (НАПРИМЕР, ЧТО КОМПИЛЯТОР МЕНЯЕТ ИНСТРУКЦИИ)

11. Барьеры памяти
    Write-барьер
    Read-барьер
    Acquire-барьер
    Release-барьер

12. Различные внутренности
    Кэши процессора
        Когерентность кэша (MESI)
        Store buffer
        Invalidation queue
        False sharing
    Pipeline процессора
    Hyper-threading
    Алгоритмы планирования
        First Come First Serve
        Shortest Job Next
        Round Robin
        Приоритетное планирование
    Инверсия приоритетов
    Закон Амдала

13. Алгоритмы синхронизации
    Грубая синхронизация
    Тонкая синхронизация
    Оптимистичная синхронизация
    Ленивая синхронизация
    Партиционирование

# УПРАЖНЕНИЕ

Написать несколько реализаций связного списка, используя грубую, тонкую, оптимистическую и ленивую синхронизацию

Написать партиционирование для хэш-таблицы

14. lock-free структуры данных
    ABA проблема
    Стек Трайбера
    Очередь Майкла-Скотта

# УПРАЖНЕНИЕ

Реализовать стек Трайбера

Реализовать очередь Майкла-Скотта

15. wait-free структура данных

# ОБЪЕМНАЯ ПРАКТИКА

1. Примитивная in-memory key-value база данных

2. writer-had lock

3. Примитивная репликация