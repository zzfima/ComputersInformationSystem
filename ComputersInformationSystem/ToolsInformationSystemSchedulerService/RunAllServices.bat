cd..
cd ConfigurationCRUDService
start dotnet run

ping 127.0.0.1 -n 6 > nul

cd..
cd ToolsInformationCRUDService
start dotnet run

cd..
cd RemoteMachineVersionInfoService
start dotnet run

cd..
cd DiscoverAliveRemoteMachinesService
start dotnet run

cd..
cd ToolsInformationSystemScheduler
start dotnet run