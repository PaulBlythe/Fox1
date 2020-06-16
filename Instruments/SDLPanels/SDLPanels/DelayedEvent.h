#pragma once
#include "MiniComputer.h"
class DelayedEvent :
	public MiniProg
{
public:
	DelayedEvent(float time, int code);
	~DelayedEvent();

	int Update(float dt);

private:
	int exitcode;
	float delay;

};



