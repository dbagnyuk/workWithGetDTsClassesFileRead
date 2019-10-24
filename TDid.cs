using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workWithGetDTsClassesFileRead
{
    public class TDid
    {
        long terminalDevice;
        bool haveLookedSrv = false;
        List<Srv> srvList = new List<Srv>();
        public void processInputData(long inputTD, string srvWhole)
        {
            this.terminalDevice = inputTD;

            string[] srvSplited = srvWhole.Split('|');
            foreach (var service in srvSplited)
            {
                Srv oneSrv = new Srv();
                oneSrv.parseService(service);

                srvList.Add(oneSrv);
            }
        }
        public void writeTDtoConsole()
        {
            Console.WriteLine("\t" + this.terminalDevice);
            this.srvList.ForEach(i => i.writeSrvtoConsole());
        }
        public void writeTDtoFile(ref StreamWriter sw)
        {
            StreamWriter swTD = sw;
            swTD.WriteLine("\t" + this.terminalDevice);
            this.srvList.ForEach(i => i.writeSrvtoFile(ref swTD));
        }
        public long lookupSrv(string lookedSrv)
        {
            foreach (Srv i in srvList)
            {
                if (i.lookupSrv(lookedSrv) && i.serviceValidity())
                    haveLookedSrv = true;
            }

            if (this.haveLookedSrv)
                return this.terminalDevice;
            else return 0;
        }
        public void sortSrvList(string sortSrvsType)
        {
            if (sortSrvsType == "asc")
                srvList = srvList.OrderBy(p => p.returnSrv()).ToList();
            else if (sortSrvsType == "desc")
                srvList = srvList.OrderByDescending(p => p.returnSrv()).ToList();
        }
        public long returnTDid()
        {
            return this.terminalDevice;
        }
    }
}
