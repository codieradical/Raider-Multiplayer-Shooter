#pragma once

#include <string>
#include <teamspeak/clientlib.h>

//Since both of these are the same, 
//there's no point in doing this, right?

//It looks nice so it's staying...

#define EXPORT __declspec(dllexport)

extern "C"
{
	EXPORT void SetupLogging(void(*_logCallback)(std::string message));
	//VoIPClient bool StartClient(char* username, char* ipAddr, void(*disconnectCallback)(int exitCode, char* exitDetails));
	EXPORT bool StartClient(std::string username, std::string ipAddr, int port, ClientUIFunctions callbacks, std::string path);
	EXPORT bool StopClient();
	EXPORT bool StartServer();
}

class SharedAPI
{
public:
	SharedAPI();
	~SharedAPI();
	static void(*SharedAPI::logCallback)(std::string message);
};

