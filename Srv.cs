using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workWithGetDTsClassesFileRead
{
    public class Srv
    {
        string srvName;
        DateTime dateFrom;
        DateTime dateTo;
        bool srvValidity;
        bool searchedSrv;
        public void parseService(string strService)
        {
            if (!strService.Contains(","))
            {
                srvName = strService;
                dateTo = Convert.ToDateTime("2100-01-01 00:00:00");
                dateFrom = Convert.ToDateTime("1900-01-01 00:00:00");
            }
            else if (strService.Contains(","))
            {
                string[] srvCol = strService.Split(',');
                int idx = 0;
                foreach (var oneField in srvCol)
                {
                    ++idx;

                    if (!String.IsNullOrEmpty(oneField) && idx % 3 == 1)
                        srvName = oneField.ToUpper();

                    if (!String.IsNullOrEmpty(oneField) && idx % 3 == 2)
                        dateTo = Convert.ToDateTime(oneField);
                    else if (String.IsNullOrEmpty(oneField) && idx % 3 == 2)
                        dateTo = Convert.ToDateTime("2100-01-01 00:00:00");

                    if (!String.IsNullOrEmpty(oneField) && idx % 3 == 0)
                        dateFrom = Convert.ToDateTime(oneField);
                    else if (String.IsNullOrEmpty(oneField) && idx % 3 == 0)
                        dateFrom = Convert.ToDateTime("1900-01-01 00:00:00");

                    if (idx % 3 == 0)
                        break;
                }
            }
            if (dateFrom <= DateTime.Now && dateTo >= DateTime.Now)
                srvValidity = true;
            else
                srvValidity = false;
        }
        public void writeSrvtoConsole()
        {
            if (searchedSrv)
                markSearchedSrv();
            else
                Console.Write("\t\t" + srvName + ", ".PadRight(12 - srvName.Length));
            Console.Write(dateFrom + ", ".PadRight(3));
            Console.Write(dateTo + ", ".PadRight(3));
            if (srvValidity)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Valid".PadRight(3), Console.ForegroundColor);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("NoValid".PadRight(3), Console.ForegroundColor);
            }
            Console.ResetColor();
        }
        public void writeSrvtoFile(ref StreamWriter sw)
        {
            StreamWriter swSrv = sw;
            swSrv.Write("\t\t" + srvName + ", ".PadRight(12 - srvName.Length));
            swSrv.Write(dateFrom + ", ".PadRight(3));
            swSrv.Write(dateTo + ", ".PadRight(3));
            if (srvValidity)
                swSrv.WriteLine("Valid".PadRight(3));
            else
                swSrv.WriteLine("NoValid".PadRight(3));
        }
        public void markSearchedSrv()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("\t\t" + srvName + ", ".PadRight(12 - srvName.Length), Console.ForegroundColor);
            Console.ResetColor();
        }
        public bool lookupSrv(string lookedSrv)
        {
            if (srvName == lookedSrv /*&& dateFrom <= DateTime.Now && dateTo >= DateTime.Now*/)
                searchedSrv = true;

            return searchedSrv;
        }
        public bool serviceValidity()
        {
            return this.srvValidity;
        }
        public string returnSrv()
        {
            return this.srvName;
        }
    }
}