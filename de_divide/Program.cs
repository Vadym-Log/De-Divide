using System;
using System.Collections.Generic;
using System.IO;

namespace de_divide
{
    class Program
    {
        static void Main(string[] args)
        {
            string contextDict;
            List<string> listWords;
            listWords = Methods.LoadFiles(out contextDict);

            string pattern;
            string firstChar;
            int index;

            string currentWord;
            string choosenWord;
            string nextWord;
            string lastWord;
            bool finish;
            bool endCheck;
            bool startCheck = true;
            List<string> listResult = new List<string>();

            DirectoryInfo directory = new DirectoryInfo(@".");
            string pathResult = directory.FullName + "\\result.txt";
            //Console.WriteLine("Введите полное имя директории и файла для сохранения результата:");
            //string pathResult = Console.ReadLine();

            foreach (string word in listWords)
            {
                currentWord = word;
                firstChar = currentWord[0].ToString().ToUpper();
                currentWord = currentWord.Remove(0, 1);
                currentWord = currentWord.Insert(0, firstChar);
                do
                {
                    pattern = "(\\w*)";
                    index = pattern.IndexOf('(');
                    pattern = pattern.Insert(index, firstChar);
                    Methods.Extraction(contextDict, pattern, currentWord, out choosenWord, out nextWord, out finish);
                    listResult.Add(choosenWord);
                    if (nextWord != "")
                    {
                        currentWord = nextWord;
                        firstChar = nextWord[0].ToString();
                    }
                } while (!finish);
                do
                {
                    if (!startCheck)
                    {
                        lastWord = listResult[listResult.Count - 1];
                        firstChar = lastWord[0].ToString().ToUpper();
                        lastWord = lastWord.Remove(0, 1);
                        lastWord = lastWord.Insert(0, firstChar);
                        listResult.RemoveAt(listResult.Count - 1);
                        listResult.Add(lastWord);
                    }
                    listResult = Methods.CheckLastWord(contextDict, listResult, out endCheck);
                    startCheck = false;
                }
                while (!endCheck);

                Methods.SaveResult(word, listResult, pathResult);
            }
        }
    }
}
