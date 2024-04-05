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

namespace csharp_learning;

public class Program
{
    static void Main(string[] args)
    {
        Chapter15Class chapter = new Chapter15Class();
        chapter.Execute();
    }
}