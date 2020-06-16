#pragma once

#include "MiniComputer.h"

class RamTest :
	public MiniProg
{

public:
	RamTest(float time);
	~RamTest();

	int Update(float dt);

private:
	float executionTime;

};

