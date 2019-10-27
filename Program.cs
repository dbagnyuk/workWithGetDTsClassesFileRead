using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ServiceModel;
using workWithGetDTsClassesFileRead.ISca;

namespace workWithGetDTsClassesFileRead
{
    class Program
    {
        static void Main(string[] args)
        {
            List<long> inputedPAsList = new List<long>();
            List<PAid> inputedPAsObjList = new List<PAid>();

            string lookedSrv = null;
            string sortTDsType = null;
            string sortSrvsType = null;

            bool lookSrv = false;
            bool sortIdx = false;
            bool writeFile = false;
            bool writeConsole = true;

            string pathToFileIn = @"c:\in.txt";
            string pathToFileOut = null;
            FileStream fileStreamIn = null;
            FileStream fileStreamOut = null;
            StreamReader streamReaderIn = null;
            StreamWriter streamWriterOut = null;
            Console.BufferHeight = 1000;

            if (args.Length == 1 && args[0] == "/?")
                writeAppHelp();

            for (int i = 0; i < args.Length; i++)
            {
                if (!string.Equals(args[i].ToLower(), "/c")
                    && !string.Equals(args[i].ToLower(), "/st") && !string.Equals(args[i].ToLower(), "/ss")
                    && !string.Equals(args[i].ToLower(), "asc") && !string.Equals(args[i].ToLower(), "desc")
                    && !string.Equals(args[i].ToLower(), "/fi") && !string.Equals(args[i].ToLower(), "/fo")
                    && !args[i].Contains(":\\"))
                {
                    foreach (char c in args[i])
                        if (!char.IsLetterOrDigit(c))
                            exitError();
                    lookedSrv = Convert.ToString(args[i]).ToUpper();
                    lookSrv = true;
                }

                if (string.Equals(args[i].ToLower(), "/st"))
                {
                    sortTDsType = "asc";
                    sortIdx = true;
                }

                if (i > 0 && string.Equals(args[i - 1].ToLower(), "/st") && (string.Equals(args[i].ToLower(), "asc") || string.Equals(args[i].ToLower(), "desc")))
                {
                    sortTDsType = Convert.ToString(args[i].ToLower());
                    sortIdx = true;
                }

                if (string.Equals(args[i].ToLower(), "/ss"))
                {
                    sortSrvsType = "asc";
                    sortIdx = true;
                }

                if (i > 0 && string.Equals(args[i - 1].ToLower(), "/ss") && (string.Equals(args[i].ToLower(), "asc") || string.Equals(args[i].ToLower(), "desc")))
                {
                    sortSrvsType = Convert.ToString(args[i].ToLower());
                    sortIdx = true;
                }

                if (string.Equals(args[i].ToLower(), "/fi"))
                    pathToFileIn = @"c:\in.txt";

                if (i > 0 && string.Equals(args[i - 1].ToLower(), "/fi") && args[i].Contains(":\\"))
                    pathToFileIn = Convert.ToString(args[i].ToLower());

                if (string.Equals(args[i].ToLower(), "/fo"))
                {
                    pathToFileOut = @"c:\out.txt";
                    writeFile = true;
                }

                if (i > 0 && string.Equals(args[i - 1].ToLower(), "/fo") && args[i].Contains(":\\"))
                {
                    pathToFileOut = Convert.ToString(args[i].ToLower());
                    writeFile = true;
                }

                if (string.Equals(args[i].ToLower(), "/c"))
                    writeConsole = false;
            }
            Console.Clear();

            try
            {
                fileStreamIn = new FileStream(pathToFileIn, FileMode.Open);
                streamReaderIn = new StreamReader(fileStreamIn);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                System.Environment.Exit(1);
            }

            string line;
            while ((line = streamReaderIn.ReadLine()) != null)
                inputedPAsList.Add(long.Parse(line));
            inputedPAsList.Sort();
            streamReaderIn.Close();
            fileStreamIn.Close();

            foreach (long inputedPA in inputedPAsList)
            {
                Console.Write(".");

                GetTDsOutput scaOutput = null;
                try
                {
                    ScaClient scaClient = new ScaClient("ConfigurationService_ISca", new EndpointAddress("http://msk-dev-foris:8106/SCA"));
                    scaOutput = scaClient.GetTDs(new GetTDsInput() { PANumber = inputedPA });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    System.Environment.Exit(1);
                }

                PAid searchedPA = new PAid(inputedPA, scaOutput);
                inputedPAsObjList.Add(searchedPA);
            }
            Console.Clear();

            if (writeFile)
            {
                try
                {
                    if (File.Exists(pathToFileOut))
                        fileStreamOut = new FileStream(pathToFileOut, FileMode.Truncate);
                    else
                        fileStreamOut = new FileStream(pathToFileOut, FileMode.CreateNew);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    writeFile = false;
                    if (!writeConsole)
                        System.Environment.Exit(1);
                }

                if (writeFile)
                    streamWriterOut = new StreamWriter(fileStreamOut);
            }

            foreach (PAid searchedPA in inputedPAsObjList)
            {
                if (lookSrv)
                    searchedPA.lookupSrv(lookedSrv);
                if (sortIdx)
                    searchedPA.sortTDsList(sortTDsType, sortSrvsType);
                if (writeConsole)
                {
                    searchedPA.writePAtoConsole();
                    searchedPA.writeTDwithSrvtoConsole(lookedSrv);
                    Console.WriteLine("\n");
                }
                if (writeFile)
                {
                    searchedPA.writePAtoFile(ref streamWriterOut);
                    searchedPA.writeTDwithSrvtoFile(lookedSrv, ref streamWriterOut);
                    streamWriterOut.WriteLine("\n");
                }
            }

            if (writeFile)
            {
                streamWriterOut.Close();
                fileStreamOut.Close();
                fileReadPath(pathToFileIn);
                fileWritePath(pathToFileOut);
            }

            Console.Write("\nPress Enter for exit...");
            Console.Read();
        }

        static void exitError()
        {
            Console.WriteLine("\nWrong input, please use 'tworkWithGetDTsClassesFile.exe /?' for help.");
            System.Environment.Exit(1);
        }
        static void fileReadPath(string fp)
        {
            Console.WriteLine($"\nInput from \"{fp}\"");
        }
        static void fileWritePath(string fp)
        {
            Console.WriteLine($"\nOutput saved as \"{fp}\"");
        }
        static void writeAppHelp()
        {
            Console.WriteLine("\nUsage:");
            Console.WriteLine("\tworkWithGetDTsClassesFile.exe [Srv] [/fi {Path}] [/fo {Path}] [/c] " +
                            "\n\t\t\t\t      [/st {asc|desc}] [/ss {asc|desc}]");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("\tSrv  - Service code which you looking for" +
                            "\n\t       (must not contain special symbols).");
            Console.WriteLine("\t/fi  - {Path} path and name to file fith PAs for process" +
                            "\n\t       the file must contain one PA per line" +
                            "\n\t       (by deafault will be used \"c:\\in.txt\").");
            Console.WriteLine("\t/fo  - {Path} path and name to file in which result will be save" +
                            "\n\t       (by deafault will be used \"c:\\out.txt\").");
            Console.WriteLine("\t/st  - {asc|desc} sorting for Terminal Devices" +
                            "\n\t       (default \"asc\").");
            Console.WriteLine("\t/ss  - {asc|desc} sorting for Service codes" +
                            "\n\t       (default \"asc\").");
            Console.WriteLine("\t/c   - decline write ouput to console.");
            Console.WriteLine("\n\t/?    - for this help.");
            Console.WriteLine("\nExample:");
            Console.WriteLine("\tworkWithGetDTsClassesFileRead.exe TEST666 /fi c:\\in.txt /fo c:\\out.txt /st desc /ss desc");
            System.Environment.Exit(0);
        }
    }
}