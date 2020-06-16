#pragma once

#include <SDL_thread.h>
#include <winsock2.h>

class UDPComms
{
public:
	UDPComms();
	~UDPComms();

	void Start();
	void Stop();
	void SendString(const char * str);

	bool ShouldStop;

	SOCKET			s;
	WSADATA			wsd;
	SOCKADDR_IN		recipient;
	int				status;

private:
	SDL_Thread *	threadID;


};

