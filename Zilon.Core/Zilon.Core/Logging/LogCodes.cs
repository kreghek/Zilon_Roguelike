namespace Zilon.Logic.Logging
{
    //The first digit can detail the general class: 1xxx can be used for 'Start' operations, 2xxx for normal behaviour, 
    //3xxx for activity tracing, 4xxx for warnings, 5xxx for errors, 8xxx for 'Stop' operations, 9xxx for fatal errors, etc.

    //The second digit can detail the area, e.g. 21xx for database information (41xx for database warnings, 51xx for database errors), 
    //22xx for calculation mode (42xx for calculation warnings, etc), 23xx for another module, etc.

    public enum LogCodes
    {
        StartCommon = 1000,
        TraceCommon = 3000,
        TraceCommands = 3100,
        TraceInitialization = 3010,
        WarnCommon = 4000,
        WarnInitialization = 4010,
        ErrorCommon = 5000,
        ErrorCommonCommunication = 5010,
        ErrorCommands = 5100,
        ErrorCraft = 5200,
        ErrorInitialization = 5010,
        StopCommon = 8000,
        FatalCommon = 9000,
    }
}
