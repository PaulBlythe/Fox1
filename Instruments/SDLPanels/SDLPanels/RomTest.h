#pragma once

#include "MiniComputer.h"
class RomTest :
	public MiniProg
{

public:
	RomTest(float time);
	~RomTest();

	int Update(float dt);

private:
	float executionTime;

};




