#include "RamTest.h"

RamTest::RamTest(float time)
{
	executionTime = time;
}

RamTest::~RamTest()
{

}

int RamTest::Update(float dt)
{
	executionTime -= dt;
	if (executionTime > 0)
		return 0;

	return prRamTestPass;
}