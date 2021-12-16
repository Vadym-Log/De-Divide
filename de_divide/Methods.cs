using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace de_divide
{
    class Methods
    {
        static string[] dictFile;
        static string[] wordsFile;

        public static List<string> LoadFiles(out string contextDict)
        {
            DirectoryInfo directory = new DirectoryInfo(@".");
            string fileNameDict = directory.FullName + "\\de-dictionary.txt";
            //Console.WriteLine("Введите полное имя директории и файла для загрузки словаря:");
            //string fileNameDict = Console.ReadLine();
            try
            {
                dictFile = File.ReadAllLines(fileNameDict);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Файл загрузить не удалось.");
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                Environment.Exit(0);
            }
            List<string> listDict = new List<string>();
            foreach (string d in dictFile)
                listDict.Add(d);
            listDict.Sort();
            StringBuilder stringBuilderDict = new StringBuilder(string.Empty);
            foreach (string d in listDict)
                stringBuilderDict = stringBuilderDict.AppendLine(d);
            contextDict = stringBuilderDict.ToString();
            string pathDict = directory.FullName + "\\result_dict.txt";
            File.AppendAllText(pathDict, contextDict);

            string fileNameWords = directory.FullName + "\\de-test-words.txt";
            //Console.WriteLine("Введите полное имя директории и файла для загрузки проверяемых слов:");
            //string fileNameWords = Console.ReadLine();
            try
            {
                wordsFile = File.ReadAllLines(fileNameWords);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Файл загрузить не удалось.");
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                Environment.Exit(0);
            }
            List<string> listWords = new List<string>();
            foreach (string w in wordsFile)
                listWords.Add(w);
            listWords.Sort();
            StringBuilder stringBuilderWords = new StringBuilder(string.Empty);
            foreach (string w in listWords)
                stringBuilderWords = stringBuilderWords.AppendLine(w);
            string contextWords = stringBuilderWords.ToString();
            string pathWords = directory.FullName + "\\result_words.txt";
            File.AppendAllText(pathWords, contextWords);

            return listWords;
        }

        static StringBuilder stringBuilderResult = new StringBuilder(string.Empty);
        public static void SaveResult(string word, List<string> listResult, string pathResult)
        { 
            stringBuilderResult.Clear();
            stringBuilderResult.Append("(in) " + word + " ---> " + "(out) ");
            foreach (string res in listResult)
                stringBuilderResult.Append(res + ", ");
            stringBuilderResult.AppendLine();

            try
            {
                File.AppendAllText(pathResult, stringBuilderResult.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Файл сохранить не удалось.");
                Console.WriteLine(ex.Message);
                Console.ReadKey();
                Environment.Exit(0);
            }
            listResult.Clear();
        }

        static Regex rg;
        static MatchCollection matchedTemp;
        static List<string> listMatched;
        static List<string> matchedList;
        static IEnumerable<string> words;
        static string firstChar;
        static bool stop;

        public static void Extraction(string contextDict, string pattern, string currentWord, out string choosenWord, out string nextWord, out bool finish)
        {
            rg = new Regex(pattern);
            matchedTemp = rg.Matches(contextDict);
            listMatched = new List<string>();
            foreach (Match item in matchedTemp)
                listMatched.Add(item.Value);
            words = from i in listMatched
                    where i.Length > 2 && i.Length < currentWord.Length
                    select i;
            matchedList = new List<string>();
            foreach (var item in words)
                matchedList.Add(item);
            matchedList.Sort((x, y) => x.Length.CompareTo(y.Length));
            matchedList.Reverse();
            choosenWord = currentWord;
            nextWord = "";
            finish = false;
            stop = false;
            if (matchedList != null && matchedList.Count != 0)
            {
                foreach (string item in matchedList)
                    if (currentWord.Contains(item))
                    {
                        choosenWord = item;
                        nextWord = currentWord.Remove(0, item.Length);
                        if (nextWord.Length < 3)
                            choosenWord += nextWord;
                        else
                        {
                            firstChar = nextWord[0].ToString().ToUpper();
                            nextWord = nextWord.Remove(0, 1);
                            nextWord = nextWord.Insert(0, firstChar);
                            stop = true;
                        }                        
                        break;
                    }
                if (!stop)
                    finish = true;
            }
            else
                finish = true;
        }

        static string lastWord;
        static string wordLast;
        static string combineWord;
        static string currentWord;
        static string pattern;
        static int index;
        static int count;
        static List<string> resultList;

        public static List<string> CheckLastWord(string contextDict, List<string> listResult, out bool endCheck)
        {
            endCheck = false;
            lastWord = listResult.Last<string>().ToString();

            wordLast = lastWord;
            firstChar = wordLast[0].ToString().ToLower();
            wordLast = wordLast.Remove(0, 1);
            wordLast = wordLast.Insert(0, firstChar);

            pattern = "(\\b()\\b)";
            index = pattern.IndexOf(')');
            pattern = pattern.Insert(index, lastWord);
            rg = new Regex(pattern);
            matchedTemp = rg.Matches(contextDict);
            if ((matchedTemp == null || matchedTemp.Count == 0) && listResult.Count > 1)
            {
                count = listResult.Count;
                combineWord = listResult[count - 2] + wordLast;
                listResult.RemoveAt(count - 2);
                listResult.Insert(count - 2, combineWord);
                listResult.RemoveAt(count - 1);
            }
            else
                endCheck = true;
            resultList = new List<string>();
            foreach (string item in listResult)
            {
                currentWord = item;
                firstChar = item[0].ToString().ToLower();
                currentWord = currentWord.Remove(0, 1);
                currentWord = currentWord.Insert(0, firstChar);
                resultList.Add(currentWord);
            }
            return resultList;
        }
    }
}
