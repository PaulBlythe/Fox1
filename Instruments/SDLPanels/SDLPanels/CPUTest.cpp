#include "CPUTest.h"


CPUTest::CPUTest(float time)
{
	executionTime = time;
}

CPUTest::~CPUTest()
{

}

int CPUTest::Update(float dt)
{
	executionTime -= dt;
	if (executionTime > 0)
		return 0;

	return prCPUTestPass;
}