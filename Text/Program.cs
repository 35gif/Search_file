using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace Text
{
    internal class Program : Form
    {
        private static List<string> _result = new List<string>();
        public static int Errors { get; private set; } = 0;
        [STAThread]
        private static void Main(string[] args)
        {
            Console.Write("Введите путь: ");
            string startPath = Console.ReadLine();
            Console.Write("Введите название и расширение файла: ");
            string pattern = Console.ReadLine();
            Console.Clear();

            _result = SearchFiles(startPath, pattern);
            Console.WriteLine("Поиск завершён.\n\tПоказать файлы?(y/n)\n");
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                Console.Clear();
                int i = 0;
                foreach (string file in _result)
                {
                    Console.WriteLine("{0}: {1}", i, file);
                    i++;
                }
                Console.WriteLine("\n\n\tFiles [{0}]", _result.Count);
                Console.WriteLine("\nБольше?(y/n)");
                if (Console.ReadKey(true).Key == ConsoleKey.Y)
                {
                    List<string> withoutDirectories = new List<string>();
                    string includeWord = null;
                    bool whileTrue = true;

                    while (whileTrue)
                    {
                        Console.WriteLine("\n1 - Without directories\n2 - include word\n3 - next\n4 - delete\n5 - Info file");

                        ConsoleKey key = Console.ReadKey(true).Key;

                        switch (key)
                        {
                            case ConsoleKey.D1:
                                Console.Write("Directory: ");
                                withoutDirectories.Add(Console.ReadLine());
                                break;

                            case ConsoleKey.D2:
                                Console.Write("Word: ");
                                includeWord = Console.ReadLine();
                                break;

                            case ConsoleKey.D3:
                                whileTrue = false;
                                break;

                            case ConsoleKey.D4:
                                Console.Write("File Id: ");
                                int num = int.Parse(Console.ReadLine() ?? string.Empty);
                                File.Delete(_result[num]);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("File \"{0}\" was deleted!", _result[num]);
                                Console.ResetColor();
                                break;
                            case ConsoleKey.D5:
                                Info.Information();
                                break;
                                foreach (string file in _result)
                                {
                                    Console.WriteLine("{0}: {1}", i, file);
                                    i++;
                                }
                        }
                    }
                    Console.Clear();

                    _result = SortFiles(_result, includeWord, withoutDirectories);
                    OpenFiles();
                }
            }

            Console.ReadKey(true);
        }

        private static List<string> SortFiles(List<string> files, string includeWord = null, List<string> directories = null)
        {
            List<string> newFiles = new List<string>();

            foreach (string file in files)
            {
                if (directories != null)
                {
                    foreach (string directory in directories)
                    {
                        if (file.ToLower().Contains(directory.ToLower()))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(file);
                            Console.ResetColor();
                        }
                        else
                        {
                            if (includeWord != null)
                            {
                                if (file.ToLower().Contains(includeWord.ToLower())) newFiles.Add(file);
                            }
                            else
                            {
                                newFiles.Add(file);
                            }
                        }
                    }
                }
                else
                {
                    if (includeWord != null)
                    {
                        if (file.ToLower().Split('\\').Last().Contains(includeWord.ToLower())) newFiles.Add(file);
                    }
                    else
                    {
                        newFiles.Add(file);
                    }
                }
            }

            Console.WriteLine();

            foreach (string file in newFiles)
            {
                Console.WriteLine(file);
            }

            return newFiles;
        }

        private static List<string> SearchFiles(string path, string pattern)
        {
            var files = new List<string>();

            try
            {
                files.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
                foreach (var directory in Directory.GetDirectories(path))
                {
                    files.AddRange(SearchFiles(directory, pattern));
                }
            }
            catch (UnauthorizedAccessException) { }

            return files;
        }

        private static void OpenFiles()
        {
            Console.WriteLine("\nEnded. [{0}] \n\tOpen?(y/n)", _result.Count);
            if (Console.ReadKey(true).Key == ConsoleKey.Y)
            {
                Console.Clear();
                foreach (var file in _result)
                {
                    try
                    {
                        Thread.Sleep(400);
                        Console.Write(file);
                        System.Diagnostics.Process.Start(file);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\t\t[open]");
                        Console.ResetColor();
                    }
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\t\t[error]");
                        Console.ResetColor();
                        Errors++;
                    }
                }

                Console.WriteLine("\nEnded.\n\tErrors: [{0}]", Errors);
            }
            else Console.Clear();

        }
    }
    public class Info : Form
    {
        [STAThread]
        public static void Information()
        {
            //Вывести информацию о файле
            var file = new OpenFileDialog // создаём диалоговое окно для выбора файла
            {
                //открыть много файлов
                Multiselect = false,
                //фильтр по расширению
                Filter = "Text files (*.*)|*.*|All files (*.*)|*.*"

            };

            if (file.ShowDialog() != DialogResult.OK) //закрываем программу если диалоговое окно с выбором базы было закрыто
                return;
            //Вывести информацию о файле
            var fileInfo = new FileInfo(file.FileName);
            Console.WriteLine("\n");
            Console.WriteLine("Имя файла: {0}", fileInfo.Name);
            Console.WriteLine("Расширение: {0}", fileInfo.Extension);
            Console.WriteLine("Полный путь: {0}", fileInfo.FullName);
            Console.WriteLine("Размер: {0}", fileInfo.Length);
            Console.WriteLine("Дата создания: {0}", fileInfo.CreationTime);
            Console.WriteLine("Дата последнего изменения: {0}", fileInfo.LastWriteTime);
            Console.WriteLine("Дата последнего доступа: {0}", fileInfo.LastAccessTime);
            Console.WriteLine("Доступен для записи: {0}", fileInfo.IsReadOnly);
            Console.WriteLine("Атрибуты: {0}", fileInfo.Attributes);
            Console.WriteLine("\n");
            Console.ReadKey();
        }
    }
}
