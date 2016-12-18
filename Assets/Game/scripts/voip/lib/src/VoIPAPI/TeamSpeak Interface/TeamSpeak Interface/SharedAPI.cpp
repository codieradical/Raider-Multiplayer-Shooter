#include <string.h>
#include "SharedAPI.h"
#include "VoIPClient.h"



SharedAPI::SharedAPI()
{
}


SharedAPI::~SharedAPI()
{
}

EXPORT bool StartClient(std::string username, std::string ipAddr, int port, ClientUIFunctions callbacks, std::string path)
{
	return VoIPClient::StartClient(username, ipAddr, port, callbacks, path);
}

EXPORT bool StopClient()
{
	return VoIPClient::StopClient();
}

void(*SharedAPI::logCallback)(std::string message);
EXPORT void SetupLogging(void(*_logCallback)(std::string message))
{
	SharedAPI::logCallback = _logCallback;
}
