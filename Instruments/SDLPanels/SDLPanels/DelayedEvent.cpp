#include "DelayedEvent.h"


DelayedEvent::DelayedEvent(float time, int code)
{
	exitcode = code;
	delay = time;
}

DelayedEvent::~DelayedEvent()
{
}

int DelayedEvent::Update(float dt)
{
	delay -= dt;
	if (delay > 0)
		return 0;

	return exitcode;
}




