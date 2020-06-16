#pragma once
#include "MiniComputer.h"
class CPUTest :
	public MiniProg
{
public:
	CPUTest(float time);
	~CPUTest();

	int Update(float dt);

private:
	float executionTime;

};
