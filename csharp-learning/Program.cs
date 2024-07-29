using csharp_learning.Part2.TypeDesign.Chapter4_BasicsOfTypes;
using csharp_learning.Part2.TypeDesign.Chapter5_ValAndRefTypes;
using csharp_learning.Part2.TypeDesign.Chapter6_MembersAndTypesBasicInformation;
using csharp_learning.Part2.TypeDesign.Chapter7_ConstsAndFields;
using csharp_learning.Part2.TypeDesign.Chapter8_Methods;
using csharp_learning.Part2.TypeDesign.Chapter9_Params;
using csharp_learning.Part2.TypeDesign.Chapter10_Properties;
using csharp_learning.Part2.TypeDesign.Chapter11_Events;
using csharp_learning.Part2.TypeDesign.Chapter12_Generalization;
using csharp_learning.Part2.TypeDesign.Chapter13_Interfaces;
using csharp_learning.Part3.BasicDataTypes.Chapter14_Symbols_Strings_TextProcessing;
using csharp_learning.Part3.BasicDataTypes.Chapter15_EnumTypesAndBitFlags;
using csharp_learning.Part3.BasicDataTypes.Chapter16_Arrays;
using csharp_learning.Part3.BasicDataTypes.Chapter17_Delegates;
using csharp_learning.Part3.BasicDataTypes.Chapter18_CustomizedAttributes;
using csharp_learning.Part3.BasicDataTypes.Chapter19_NullCompatibleTypes;
using csharp_learning.Part4.KeyMechanisms.Chapter20_ExceptionsAndStateControl;
using csharp_learning.Part4.KeyMechanisms.Chapter21_GarbageCollection;
using csharp_learning.Part4.KeyMechanisms.Chapter22_CLRHostingAndAppDomains;
using csharp_learning.Part4.KeyMechanisms.Chapter23_AssemblyLoadingAndReflection;
using csharp_learning.Part4.KeyMechanisms.Chapter24_Serialization;
using csharp_learning.Part4.KeyMechanisms.Chapter25_InteractionWithWinRTComponents;
using csharp_learning.Part5.Multithreading.Chapter26_ExecutionThreads;
using csharp_learning.Part5.Multithreading.Chapter27_AsynchronousCalculationOperations;

namespace csharp_learning;

public class Program
{
    static void Main(string[] args)
    {
        var chapter = new Chapter27Class();
        chapter.Execute();
    }
}