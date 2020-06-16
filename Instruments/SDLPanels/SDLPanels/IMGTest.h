#pragma once
#include "MiniComputer.h"
class IMGTest :
	public MiniProg
{
public:
	IMGTest(float time);
	~IMGTest();

	int Update(float dt);

private:
	float executionTime;

};

