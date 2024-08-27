using Concurrency;
using Concurrency.Chapter1.ConcurrencyGeneralInformation;
using Concurrency.Chapter2.AsyncBasics;
using Concurrency.Chapter3.AsyncThreads;
using Concurrency.Chapter4.ParallelBasics;
using Concurrency.Chapter5.DataFlowBasics;

IChapter chapter = new Chapter5Class();
await chapter.Execute();