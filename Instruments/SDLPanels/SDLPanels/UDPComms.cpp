#include "UDPComms.h"

#define DEFAULT_PORT            5150

int threadFunction(void* data);

UDPComms::UDPComms()
	: ShouldStop(false),
	  status(0)
{
}


UDPComms::~UDPComms()
{
	ShouldStop = true;
	SDL_WaitThread(threadID, NULL);
}

void UDPComms::Start()
{
	threadID = SDL_CreateThread(threadFunction, "LazyThread", this);

}

void UDPComms::Stop()
{
	ShouldStop = true;
	SDL_WaitThread(threadID, NULL);
	closesocket(s);
	WSACleanup();
}

void UDPComms::SendString(const char * str)
{
	int ret = send(s, str, strlen(str), 0);
	if (ret == SOCKET_ERROR)
	{
		WSACleanup();
		status = -5;
	}
}


//*************************************************************************************
//* Connect the panel with the host game in a background thread
//*************************************************************************************
int threadFunction(void* data)
{
	const char* szRecipient = "127.0.0.1";
	char* sendbuf = NULL;

	UDPComms * host = (UDPComms *)data;
	if (WSAStartup(MAKEWORD(2, 2), &host->wsd) != 0)
	{
		host->status = -1;
		return 1;
	}
	host->s = socket(AF_INET, SOCK_DGRAM, 0);
	if (host->s == INVALID_SOCKET)
	{
		host->status = -2;
		return 1;
	}
	int iPort = DEFAULT_PORT;

	host->recipient.sin_family = AF_INET;
	host->recipient.sin_port = htons((short)iPort);
	if ((host->recipient.sin_addr.s_addr = inet_addr(szRecipient)) == INADDR_NONE)
	{
		struct hostent *hoste = NULL;

		hoste = gethostbyname(szRecipient);
		if (hoste)
			CopyMemory(&host->recipient.sin_addr, hoste->h_addr_list[0],hoste->h_length);
		else
		{
			WSACleanup();
			host->status = -3;
			return 1;
		}
	}

	if (connect(host->s, (SOCKADDR *)&host->recipient, sizeof(host->recipient)) == SOCKET_ERROR)
	{
		WSACleanup();
		host->status = -4;
		return 1;
	}
	host->status = 10;
	return 0;
}