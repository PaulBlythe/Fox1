#pragma once
#include "SDL.h"
#include "UDPComms.h"

class Panel
{
public:
	Panel() {}

	virtual ~Panel() {}

	virtual void Load() = 0;
	virtual void Update(SDL_Event* e) = 0;
	virtual void Draw(SDL_Surface* screen) = 0;
	virtual void Register(UDPComms * comms) = 0;

};