using Concurrency;
using Concurrency.Chapter1.ConcurrencyGeneralInformation;
using Concurrency.Chapter2.AsyncBasics;
using Concurrency.Chapter3.AsyncThreads;
using Concurrency.Chapter4.ParallelBasics;
using Concurrency.Chapter5.DataFlowBasics;
using Concurrency.Chapter7.Testing;
using Concurrency.Chapter8.Interaction;
using Concurrency.Chapter9.Collections;

IChapter chapter = new Chapter9Class();
await chapter.Execute();