#pragma once
#include "MiniComputer.h"
class CommTest :
	public MiniProg
{
public:
	CommTest();
	~CommTest();

	int Update(float dt);

private:
	float executionTime = 0;
	bool done = false;
};