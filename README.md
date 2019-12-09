# workWithGetDTsClassesFileRead

Usage:
        workWithGetDTsClassesFile.exe [Srv] [/fi {Path}] [/fo {Path}] [/c]
                                      [/st {asc|desc}] [/ss {asc|desc}]

Options:
        Srv  - Service code which you looking for
               (must not contain special symbols).
        /fi  - {Path} path and name to file fith PAs for process
               the file must contain one PA per line
               (by deafault will be used "c:\in.txt").
        /fo  - {Path} path and name to file in which result will be save
               (by deafault will be used "c:\out.txt").
        /st  - {asc|desc} sorting for Terminal Devices
               (default "asc").
        /ss  - {asc|desc} sorting for Service codes
               (default "asc").
        /c   - decline write ouput to console.

        /?    - for this help.

Example:
        workWithGetDTsClassesFileRead.exe TEST666 /fi c:\in.txt /fo c:\out.txt /st desc /ss desc
